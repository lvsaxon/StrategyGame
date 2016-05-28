
using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class TankFollower: MonoBehaviour {

    public Transform rememberPos;
    public bool isAssignedPos;

    string squadType;
    GameObject[] tanks;
    Transform squadPosition;
    bool turn, locateTarget;

    GameObject leader;
    TankCoordinator coordinator;
    Dictionary<string, Transform> maneuverPositionList;


    void Awake() {
        maneuverPositionList = new Dictionary<string, Transform>();
        tanks = GameObject.FindGameObjectsWithTag("Tank");

        foreach(GameObject tank in tanks){ 
            if(tank.name == "CyberTank Leader"){ 
               leader = tank;
               coordinator = leader.GetComponent<TankCoordinator>();
            }
        }
    }


    void Update() {
        int currHealth = GetComponent<TankHealth>().currentHealth;

        if(transform != null){
           if(currHealth > 0){ 

               if(coordinator && coordinator.isCallingFollowers() && coordinator.gameObject.GetComponent<TankHealth>().currentHealth > 0){

                     //Assigned A Position
                     if(isAssignedPos){
                        StartCoroutine("AssignedPosition");
                     }

               }else
                  locateTarget = true;
           }

           //Search for Target if Leader is Dead
           if(locateTarget)
              StartCoroutine("LocateTarget");
        }
    }


    /* Recieve Position Request from Tank_Formation */
    public void PositionRequest_Received(string formationType, Transform pos) {
        squadType = formationType;
        bool hasPos = maneuverPositionList.ContainsKey(formationType);

        //Followers were Not Assigned a Position
        if(!hasPos){ 
           squadPosition = pos;
           maneuverPositionList[squadType] = squadPosition;
           rememberPos = maneuverPositionList[squadType];
           hasPos = true;
        }

        //Followers were Assigned a Position
        if(rememberPos != null) 
           isAssignedPos = true;
    }


    /* Move to Assigned Position */
    IEnumerator AssignedPosition() {
        yield return new WaitForSeconds(1.5f);

        Transform reservedPos;
        if(maneuverPositionList.TryGetValue(squadType, out reservedPos)){
           float distance = Vector3.Distance(transform.position, reservedPos.position);

           if(distance >= 1f){
              Vector3 assignedPos = new Vector3(reservedPos.position.x, transform.position.y, reservedPos.position.z);
              transform.position = Vector3.MoveTowards(transform.position, assignedPos, 10f*Time.deltaTime);
           }
        }
    }


    /* Search for Target */
    IEnumerator LocateTarget() {
        yield return new WaitForSeconds(0.5f);

        GetComponent<TDEnemy>().enabled = true;
    }


    /* Form Horizontal Formation if Leader has stopped */
    IEnumerator LinearFormation() {
        yield return new WaitForSeconds(3f);

        Vector3 direction = new Vector3(leader.transform.position.x+2, transform.position.y, transform.position.z);
        if(Vector3.Distance(transform.position, direction) < 0.5f)
           yield break;
        else
           transform.position = Vector3.MoveTowards(transform.position, direction, Time.deltaTime*4f);
    }
}