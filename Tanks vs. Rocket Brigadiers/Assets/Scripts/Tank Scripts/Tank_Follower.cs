
using UnityEngine;
using System;
using System.Collections.Generic;
using System.Collections;

public class Tank_Follower: MonoBehaviour {

    public Transform rememberPos;
    public bool ready, isAssignedPos;

    GameObject leader;
    String squadFormationType;
    List<GameObject> followers;
    Transform[] squadPositionList;
    
    bool occupied;
    GameObject[] tanks;
    TankCoordinator coordinator;
    TankFormationManager formationsManager;


    void Awake() {
        followers = new List<GameObject>();
        squadPositionList = new Transform[2];
        tanks = GameObject.FindGameObjectsWithTag("Tank");

        foreach(GameObject tank in tanks)
            if(tank.name != "tank leader")
               followers.Add(tank);
            else{ 
               leader = tank;
               coordinator = leader.GetComponent<TankCoordinator>();
            }

        formationsManager = new TankFormationManager();
    }


    void Update() {

        if(coordinator){
           if(coordinator.isCallingFollowers() && coordinator.gameObject.GetComponent<TankHealth>().currentHealth > 0){
              if(isAssignedPos){
                 AssignedPosition();

              }else if(!isAssignedPos){
                 MoveToFormation();
              }

           }else 
              StartCoroutine("FindTarget");
        }


        if(ready){
           //Search for Target if Leader is Dead
           if(leader.GetComponent<TankHealth>().currentHealth <= 0){
              StartCoroutine("FindTarget");
           }
        }
    }


    /* Recieve Position Request from Tank_Formation */
    public void PositionRequest_Received(String formationType, Transform[] pos) {
        squadFormationType = formationType;

        //Followers were Not Assigned a Position
        if(rememberPos == null){ 
           isAssignedPos = false;
           squadPositionList = pos;
        }

        //Followers were Assigned a Position
        if(rememberPos != null) 
           isAssignedPos = true;
    }


    /* Move to Requested Formation */
    void MoveToFormation() {
        Transform tank;
        Vector3 squadPos;

        if(formationsManager != null){
           //For Every Follower move to Assigned Position
           for(int i=0; i<followers.Count; i++){
                tank = followers[i].transform;
                squadPos = squadPositionList[i].position;

                if(squadPos != Vector3.zero)
                   tank.position = Vector3.MoveTowards(tank.position, squadPos, 4f*Time.deltaTime);

                if((int) Vector3.Distance(tank.transform.position, squadPos) == 0 && !occupied){
                   ready = true;
                   formationsManager.ReserveSquadPosition(squadFormationType, transform.position);
                   rememberPos = formationsManager.RememberPosition();

                }else{
                    //Move to anotherPos
                }
            } 
        } else return;
    }
    
    
    /* LookAt Assigned Position */
    void TurnToPosition(Vector3 direction) {
        Quaternion targetRotation = new Quaternion();

        Vector3 targetPosition = direction;
        if((targetPosition - transform.position) != Vector3.zero)
           targetRotation = Quaternion.LookRotation(targetPosition - transform.position, Vector3.up);

        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 0.5f);
    }


    /* Move to Assigned Position */
    void AssignedPosition() {

        switch(squadFormationType){

            case "Delta":
                transform.position = Vector3.MoveTowards(transform.position, rememberPos.position, Time.deltaTime * 10f);            
            break;

            case "V":
                transform.position = Vector3.MoveTowards(transform.position, rememberPos.position, Time.deltaTime * 10f);               
            break;
        }
    }


    /* Search for Target */
    IEnumerator FindTarget() {

        yield return new WaitForSeconds(0.5f);
        GetComponent<Units>().enabled = true;
    }


    /* SquadPos is Occupied */
    public void PositionOccupied(bool _occupied) {

        occupied = _occupied;
    }
}