
using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class TankCoordinator: MonoBehaviour {

    public FormationList formationsList;

    Units unit;
    int squadPosSupply;   
    GameObject[] tanks;
    int delta_PosCount, V_PosCount, followerCount = 0;
    bool prepareToMove, isGroupFormed, callingFollowers;
       
    Tank_Follower follower;
    TankFormationManager formationManager;
    

    void Awake() {
        unit = GetComponent<Units>();
        formationManager = new TankFormationManager();       
        tanks = GameObject.FindGameObjectsWithTag("Tank");

        if(tanks != null){ 
            foreach(GameObject tank in tanks)
                 if(tank.name != "tank leader")
                    follower = tank.GetComponent<Tank_Follower>();
        }else
            return;
        
        DisableFormationColliders();
    }


    void Start() {
        prepareToMove = false;
        V_PosCount = formationsList.VFormation.Length;
        delta_PosCount = formationsList.deltaFormation.Length;
            
        //90% Issue A Formation to Followers
        if(Random.Range(0, 100) <= 90)
           ChooseRandomFormation();

        else {
           setCallingFollowers(false); 
           setSquadronFormed(false);
           StartCoroutine("WaitToMove");
        }
    }


    void Update() {

        if(tanks != null){
           if(prepareToMove){
              setSquadronFormed(true);
              StartCoroutine("WaitToMove");
           }
           
           if(GetComponent<TankHealth>().currentHealth <= 0)
              setSquadronFormed(false);
        }else
            return;
    }


    /* Choose a random formation */
    void ChooseRandomFormation() {
        setCallingFollowers(true);
        int randomIndex = Random.Range(0, 2);
        print("Formation#: "+randomIndex);

        //Store 5 formations
        switch(randomIndex) {

            //Delta Formation
            case 0:
                squadPosSupply = delta_PosCount;
                formationManager.StoreFormations("Delta", formationsList.deltaFormation);
                    
                //Enable DeltaFormation Colliders
                for(int i=0; i<formationsList.VFormation.Length; i++)
                    formationsList.deltaFormation[i].GetComponent<Collider>().enabled = true;
            break;

            //'V' Formation
            case 1:
                squadPosSupply = V_PosCount;
                formationManager.StoreFormations("V", formationsList.VFormation);

                //Enable VFormation Colliders
                for(int i=0; i<formationsList.VFormation.Length; i++)
                    formationsList.VFormation[i].GetComponent<Collider>().enabled = true;
            break;
        }
    }


    /* Wait a Few Seconds to Move Squadron */
    IEnumerator WaitToMove(){
        yield return new WaitForSeconds(0.5f);
        MoveTeamFormation();
    }


    /*  Move Entire Team */
    void MoveTeamFormation() {
        
        if(tanks != null)
           unit.enabled = true;
        else
           return;    
    }


    /* Disable Squad Formation Colliders */
    void DisableFormationColliders() {

        //Disable Formation Colliders
        for(int i=0; i<formationsList.VFormation.Length; i++){
            formationsList.VFormation[i].GetComponent<Collider>().enabled = false;
            formationsList.deltaFormation[i].GetComponent<Collider>().enabled = false;
        }
    }


    /* Return Delta Formation List */
    public Transform[] DeltaFormation() {

        return formationsList.deltaFormation;
    }


    /* Return 'V' Formation List */
    public Transform[] VFormation() {

        return formationsList.VFormation;
    }


    /* Set if Squadron is Formed */
    public void setSquadronFormed(bool _isFormed) {

        isGroupFormed = _isFormed;
    }


    /* Return if Squadron isFormed */
    public bool isSquadronFormed() {

        if(isGroupFormed && GetComponent<TankHealth>().currentHealth > 0)
           return true;
        else
           return false;
    }


    /* Set Condition of Calling for Followers */
    public void setCallingFollowers(bool _called) {

        callingFollowers = _called;
    }


    /* Leader Calling Followers */
    public bool isCallingFollowers() {

        return callingFollowers;
    }
    

    /* Count # of Tanks Inside Collider */  
    void OnTriggerEnter(Collider collid) {
        
        if(collid.tag == "Tank") { 
           followerCount++;
           
           if(followerCount == squadPosSupply)
              prepareToMove = true;
        }        
    }
}
