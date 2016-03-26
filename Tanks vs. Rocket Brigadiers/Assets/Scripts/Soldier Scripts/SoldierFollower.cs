using UnityEngine;
using System;
using System.Collections.Generic;
using System.Collections;

public class SoldierFollower: MonoBehaviour {

    public Transform rememberPos;
    public bool ready, isAssignedPos;

    bool occupied;
    GameObject[] soldiers;
    String squadFormationType;
    List<GameObject> followers;
    Transform[] squadPositionList;   

    GameObject leader;
    SoldierCoordinator coordinator;
    SoldierFormationManager formationManager;
    

    void Awake() {
        followers = new List<GameObject>();
        squadPositionList = new Transform[2];        
        soldiers = GameObject.FindGameObjectsWithTag("Rocket Brigadier");

        foreach(GameObject soldier in soldiers)
            if(soldier.name != "soldier leader"){ 
               followers.Add(soldier);
               followers.Reverse();
            }else {
               leader = soldier;
               coordinator = leader.GetComponent<SoldierCoordinator>();
            }

        formationManager = new SoldierFormationManager();
    }


    void Update() {

        if(coordinator){
           if(coordinator.isCallingFollowers() && coordinator.gameObject.GetComponent<SoldierHealth>().currentHealth > 0){
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
           if(leader.GetComponent<SoldierHealth>().currentHealth <= 0){
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
        GameObject soldier;
        Vector3 squadPos = new Vector3();

        if(formationManager != null){

           //foreach Soldier move to an Assigned Position
           for(int i=0; i<followers.Count; i++){
                soldier = followers[i];
                squadPos = squadPositionList[i].position;

                if(squadPos != new Vector3())
                   soldier.transform.position = Vector3.MoveTowards(soldier.transform.position, squadPos, 3f*Time.deltaTime);
                
                if((int) Vector3.Distance(soldier.transform.position, squadPos) == 0){
                   ready = true;
                   formationManager.ReserveSoldierPosition(squadFormationType, transform.position);
                   rememberPos = formationManager.RememberPosition();

                }else{
                    //Move to anotherPos
                }
            }            
        } else return;
    }
    
    
    /* LookAt the Closest Pos to You */
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
                print("Go to: "+rememberPos);
            break;

            case "V":
                print("Go to: "+rememberPos);
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
