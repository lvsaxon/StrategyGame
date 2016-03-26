using UnityEngine;
using System;
using System.Collections.Generic;


public class SoldierFormationManager {

    String formationType;
    Transform reservedPos;
    List<Transform> openPos;

    GameObject[] followers;
    Transform[] deltaPos, VPos;
    SoldierCoordinator soldierCoordinator;
    

    /* Create A Formation Object; Issued by Leader */
    public SoldierFormationManager() {
        openPos = new List<Transform>();
        followers = GameObject.FindGameObjectsWithTag("Rocket Brigadier");
        
        //Initialize Leader Tank 
        foreach(GameObject soldier in followers) {
            if(soldier.name == "soldier leader") {
               soldierCoordinator = soldier.GetComponent<SoldierCoordinator>();
               deltaPos = soldierCoordinator.DeltaFormation();
               VPos = soldierCoordinator.VFormation();
            }
        }
    }


    /* Create a Random Formation, Issued By the Leader; */
    public void StoreFormations(String keyName, Transform[] formationPos) {
        formationType = keyName;

        for(int i=0; i<formationPos.Length; i++)
            openPos.Add(formationPos[i]);

        SendFormationRequests();
    }


    /* Send Formation Request to Followers */
    void SendFormationRequests() {
        
        //Get Every TankFollower Object's Component
        if(followers != null){ 
            foreach(GameObject soldiers in followers){
                foreach(SoldierFollower soldierFollowers in soldiers.GetComponents<SoldierFollower>()) { 
                    soldierFollowers.PositionRequest_Received(formationType, openPos.ToArray());
                }
            }
        }else
            return;
    }


    /* Reserve Assigned Squad Position */
    public void ReserveSoldierPosition(String keyType, Vector3 pos) {
        
        //Formation Checking
        if(soldierCoordinator != null){ 
            switch(keyType) {

                case "Delta":
                    for(int i=0; i<soldierCoordinator.DeltaFormation().Length; i++)
                        if((int) Vector3.Distance(pos, deltaPos[i].position) == 0) 
                           reservedPos = deltaPos[i];
                break;

                case "V":
                    for(int i=0; i<soldierCoordinator.VFormation().Length; i++)
                        if((int) Vector3.Distance(pos, VPos[i].position) == 0) 
                           reservedPos = VPos[i];
                break;
            }
        }
    }


    /* Send Reserved Position to Followers if Not Empty */
    public Transform RememberPosition() {

        return reservedPos;
    }
}

