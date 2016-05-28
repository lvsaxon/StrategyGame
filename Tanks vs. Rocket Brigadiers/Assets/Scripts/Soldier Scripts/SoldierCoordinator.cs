using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class SoldierCoordinator: MonoBehaviour {

    public bool isGroupFormed;
    public FormationList formationsList;
    public Maneuvers[] maneuverList;
    public float soundSpeed, timer;
    
    List<Transform> followers;
    GameObject[] soldiers, tanks;
    bool prepareToMove, callingFollowers;
    int delta_PosCount, V_PosCount, followerCount = 0;
    int squadPosSupply, maneuverCountTimer;  
       
    TDEnemy units;
    bool lowHealth;
    int rand, indx = 0;
    float startingHealth;
    SoldierFollower soldierFollower;

    Transform tankLeader;
    string[] squadForm, flanking;
    

    void Awake() {
        units = GetComponent<TDEnemy>();
        followers = new List<Transform>();
        tanks = GameObject.FindGameObjectsWithTag("Tank");
        soldiers = GameObject.FindGameObjectsWithTag("Rocket Brigadier");

        foreach(GameObject soldier in soldiers){
             if(!soldier.name.Contains("leader")){ 
                followers.Add(soldier.transform);
            }
        }

        foreach(GameObject tank in tanks){
             if(tank.name.Contains("leader")){ 
                tankLeader = tank.transform;
            }
        }
    }


    void Start() {
        V_PosCount = formationsList.VFormation.Length;
        delta_PosCount = formationsList.deltaFormation.Length;
        
        squadForm = new string[] {"Delta", "V"};
        flanking = new string[] {"Flank 1", "Flank 2", "Flank 3"};

        SendFormationList();
        SendManeuverList();
        ChooseSquadronFormation();
       
        StartCoroutine("RandomFlankNum");
    }


    void Update() {        
        timer+=Time.deltaTime;
        maneuverCountTimer = (int)timer;
        int currHealth = GetComponent<SoldierHealth>().currentHealth;

        if(soldiers != null){
           if(prepareToMove){
              isGroupFormed = true;
              StartCoroutine("CommandSquadron");
           }

           //30% Health
           if(currHealth <= GetComponent<SoldierHealth>().startingHealth * 0.3f){ 
              //Call Followers Back
           }

           if(currHealth <= 0){ 
              isGroupFormed = false;
              StopAllCoroutines();
           }

        }else
            return;
    }


    /* Send Formation List to Followers */
    void SendFormationList() {
        
        for(int i=0; i<followers.Count; i++){
            for(int j=0; j<squadForm.Length; j++){
                if(followers[i].GetComponent<SoldierFollower>()){ 
                    if(squadForm[j] == "Delta")
                       followers[i].GetComponent<SoldierFollower>().PositionRequest_Received(squadForm[j], formationsList.deltaFormation[i]);
                    else
                       followers[i].GetComponent<SoldierFollower>().PositionRequest_Received(squadForm[j], formationsList.VFormation[i]);
                }
            }
        }
    }


    /* Send Maneuver List to Followers */
    void SendManeuverList() {
        
        for(int i=0; i<followers.Count; i++){
            for(int j=0; j<flanking.Length; j++){ 
                if(followers[i].GetComponent<SoldierFollower>()){

                    if(flanking[j] == "Flank 1"){ 
                       followers[i].GetComponent<SoldierFollower>().PositionRequest_Received(flanking[j], maneuverList[j].flankPositions[i]);

                    }else if(flanking[j] == "Flank 2"){
                       followers[i].GetComponent<SoldierFollower>().PositionRequest_Received(flanking[j], maneuverList[j].flankPositions[i]);

                    }else{ 
                       followers[i].GetComponent<SoldierFollower>().PositionRequest_Received(flanking[j], maneuverList[j].flankPositions[i]);
                    }
                }
            }
        }
    }


    /* Choose a Random Squad Formation; Start of Game; Enemy Destroyed; Leader Almost Dead*/
    void ChooseSquadronFormation() {
        callingFollowers = true;
        int indx = Random.Range(0, 2);
        print("Formation: "+squadForm[indx]);

        switch(1) {

            //Delta Formation
            case 0:
                squadPosSupply = delta_PosCount;
                for(int i=0; i<followers.Count; i++)
                    if(followers[i].GetComponent<SoldierFollower>())
                       followers[i].GetComponent<SoldierFollower>().ManeuverCommand("Delta");
                
             break;

            //'V' Formation
            case 1:
                squadPosSupply = V_PosCount;
                for(int i=0; i<followers.Count; i++) 
                    if(followers[i].GetComponent<SoldierFollower>())
                       followers[i].GetComponent<SoldierFollower>().ManeuverCommand("V");
                
            break;
        }
    }


    /* Wait a Few Seconds to Move Squadron */
    IEnumerator CommandSquadron(){
        units.enabled = true;
        
        //Path has Stopped; Get followers in Position
        if(units.haltPath){
           StartCoroutine("EnemySpotted");  
           StartCoroutine("FlankingManeuvers");
           yield return new WaitForSeconds(5f);
        }
    }

    
    /* Form V Squadron When Enemy is Spotted */
    IEnumerator EnemySpotted() {
        yield return new WaitForSeconds(1f);

        if(followers != null)
           for(int i=0; i<followers.Count; i++){
               followers[i].GetComponent<SoldierFollower>().ManeuverCommand("V");
           }

        yield return null;
    }


    /* Call Flanking Maneuvers Every few Seconds */
    IEnumerator FlankingManeuvers() {
        yield return new WaitForSeconds(5f);
        
        //Begin Flanking Maneuvers when Leader is Alive
        if(tankLeader){ 
            if(tankLeader.GetComponent<TankHealth>().currentHealth > 0){ 
               for(int i=0; i<followers.Count; i++){ 
                   followers[i].GetComponent<SoldierFollower>().ManeuverCommand(flanking[rand]);
               }

            //Call Followers Back to Squad Formation when Leader is Dead
            }else{
               foreach(Transform follower in followers){ 
                  int rand = Random.Range(0, 2);
                  if(rand > 0)
                     follower.GetComponent<SoldierFollower>().ManeuverCommand("V");
                  else
                     follower.GetComponent<SoldierFollower>().ManeuverCommand("Delta");
               }

               yield return new WaitForSeconds(5f);
            }
        }
    }


    /* Produce RandomNum Every 5 Secs */
    IEnumerator RandomFlankNum() {
        yield return new WaitForSeconds(5f);

        while(true) {
            yield return rand = Random.Range(0, 3);
            yield return new WaitForSeconds(5f);
        }
    }

    






















    /* Return if Squadron isFormed */
    public bool isSquadronFormed() {

        if(isGroupFormed && GetComponent<SoldierHealth>().currentHealth > 0)
           return true;
        else
           return false;
    }


    /* Leader Calling Followers */
    public bool isCallingFollowers() {

        return callingFollowers;
    }


    void CallingFollowers() {
        /*yield return new WaitForSeconds(1f);
        
        callingCollider.radius *= Time.deltaTime+soundSpeed;
        if(soundCount < 3 && callingCollider.radius >= 10){
           callingCollider.radius = 0.5f;
           soundCount++;

        }else if(soundCount >= 3){
           callingSound = false;
           callingCollider.radius = 0.5f;
        }*/
    }
    
    
    /* Count # of Soldiers Inside Collider */  
    void OnTriggerEnter(Collider collid) {
        
        if(collid.tag == "Rocket Brigadier") { 
           followerCount++;
           
           if(followerCount == squadPosSupply)
              prepareToMove = true;
        }     
    }
}

