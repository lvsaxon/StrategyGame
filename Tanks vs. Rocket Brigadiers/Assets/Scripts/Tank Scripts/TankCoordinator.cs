using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class TankCoordinator: MonoBehaviour {

    public bool squadronFormed;
    public FormationList formationsList;
    public Maneuvers[] maneuverList;
    public float soundSpeed, timer;

    List<Transform> followers;
    GameObject[] tanks, soldiers;
    bool prepareToMove, callingFollowers;
    int delta_PosCount, V_PosCount, followerCount = 0;    
    int squadPosSupply, maneuverCountTimer; 
       
    TDEnemy units;
    float startingHealth;
    TankFollower tankFollower;
    
    
    void Awake() {
        units = GetComponent<TDEnemy>();
        followers = new List<Transform>();
        tanks = GameObject.FindGameObjectsWithTag("Tank");
        soldiers = GameObject.FindGameObjectsWithTag("Rocket Brigadier");

        foreach(GameObject tank in tanks){ 
            if(tank.name != "CyberTank Leader"){ 
               followers.Add(tank.transform);
               followers.Reverse();
               tankFollower = tank.GetComponent<TankFollower>();
            }
        }
    }


    void Start() {
        V_PosCount = formationsList.VFormation.Length;
        delta_PosCount = formationsList.deltaFormation.Length;
            
        ChooseRandomFormation();
        //callingSound = true;
    }


    void Update() {       
        timer+=Time.deltaTime;
		maneuverCountTimer = (int)timer;
        int currHealth = GetComponent<TankHealth>().currentHealth;

        if(tanks != null){
           if(prepareToMove){
              squadronFormed = true;
              StartCoroutine("CommandSquadron");
           }
           
           //30% health left
           if(currHealth <= GetComponent<TankHealth>().startingHealth * 0.3f){
              //Call Followers Back
           }

           if(currHealth <= 0) {
              squadronFormed = false;
              StopAllCoroutines();
           }

        }else
            return;
    }


    /* Choose a random formation */
    void ChooseRandomFormation() {
        callingFollowers = true;
        int indx = Random.Range(0, 2);
        string[] squadForm = {"Delta", "V"};
        print("Formation#: "+squadForm[indx]);

        switch(indx) {

            //Delta Formation
            case 0:
                squadPosSupply = delta_PosCount;
                for(int i=0; i<followers.Count; i++)
                    followers[i].GetComponent<TankFollower>().PositionRequest_Received("Delta", formationsList.deltaFormation[i]);
                    
            break;

            //'V' Formation
            case 1:
                squadPosSupply = V_PosCount;
                for(int i=0; i<followers.Count; i++)
                    followers[i].GetComponent<TankFollower>().PositionRequest_Received("V", formationsList.VFormation[i]);

            break;
        }
    }


    /* Wait a Few Seconds to Move Squadron */
    IEnumerator CommandSquadron(){
        yield return new WaitForSeconds(0.5f);
        
        units.enabled = true;
        int randomTime = Random.Range(6, 12);
        //Path has Stopped; Get followers in Position
        if(units.haltPath){
           yield return new WaitForSeconds(5f);

            if(maneuverCountTimer % randomTime == 0){
               StartCoroutine("FlankingManeuvers", maneuverCountTimer);
               yield return new WaitForSeconds(maneuverCountTimer);
            }
        }
    }


    /* Call Flanking Maneuvers Every few Seconds */
    IEnumerator FlankingManeuvers(int waitForCommand) {
        yield return new WaitForSeconds(waitForCommand);
        
        int indx;
        int flankNum = Random.Range(0, 100);
        //25% Flank1 chance
        if(flankNum <= 25f){
           for(int i=0; i<followers.Count; i++){ 
               indx = Random.Range(0, 3);
               //followers[i].GetComponent<Tank_Follower>().PositionRequest_Received("Flank 1", maneuverList[indx].flankPositions[i]);
            }
        
        //52% Flank2 chance
        }else if(flankNum > 25 && flankNum <= 77f){
            for(int i=0; i<followers.Count; i++){ 
                indx = Random.Range(0, 3);
                //followers[i].GetComponent<Tank_Follower>().PositionRequest_Received("Flank 2", maneuverList[indx].flankPositions[i]);
            }

        //23% Flank3 chance
        }else{
            for(int i=0; i<followers.Count; i++){ 
                indx = Random.Range(0, 3);
                //followers[i].GetComponent<Tank_Follower>().PositionRequest_Received("Flank 3", maneuverList[indx].flankPositions[i]);
            }
        }
    }


    /* Return if Squadron isFormed */
    public bool isSquadronFormed() {

        if(squadronFormed && GetComponent<TankHealth>().currentHealth > 0)
           return true;
        else
           return false;
    }


    /* Leader Calling Followers */
    public bool isCallingFollowers() {

        return callingFollowers;
    }
    

    /* Count # of Tanks Inside Collider */  
    void OnTriggerEnter(Collider collid) {
        
        if(collid.tag == "Tank"){ 
           followerCount++;
           
           if(followerCount == squadPosSupply)
              prepareToMove = true;
        }        
    }
}
