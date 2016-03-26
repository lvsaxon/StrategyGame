using UnityEngine;
using System;
using System.Collections.Generic;


public class TankFormationManager {

    String formationType;
    Transform reservedPos;
    List<Transform> openPos;

    GameObject[] followers;
    Transform[] deltaPos, VPos;
    TankCoordinator tankCoordinator;
    

    /* Create A Formation Object; Issued by Leader */
    public TankFormationManager() {
        openPos = new List<Transform>();
        followers = GameObject.FindGameObjectsWithTag("Tank");
        
        //Initialize Leader Tank 
        foreach(GameObject tank in followers) {
            if(tank.name == "tank leader") {
               tankCoordinator = tank.GetComponent<TankCoordinator>();
               deltaPos = tankCoordinator.DeltaFormation();
               VPos = tankCoordinator.VFormation();
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
            foreach(GameObject tanks in followers){
                foreach(Tank_Follower tankFollowers in tanks.GetComponents<Tank_Follower>()) { 
                    tankFollowers.PositionRequest_Received(formationType, openPos.ToArray());
                }
            }
        }else
            return;
    }


    /* Reserve Assigned Squad Position */
    public void ReserveSquadPosition(String keyType, Vector3 pos) {
        
        //Formation Checking
        if(tankCoordinator != null){ 
            switch(keyType) {

                case "Delta":
                    for(int i=0; i<tankCoordinator.DeltaFormation().Length; i++)
                        if((int) Vector3.Distance(pos, deltaPos[i].position) == 0) 
                           reservedPos = deltaPos[i];
                break;

                case "V":
                    for(int i=0; i<tankCoordinator.VFormation().Length; i++)
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
