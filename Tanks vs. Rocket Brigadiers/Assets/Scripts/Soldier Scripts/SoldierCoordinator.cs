using UnityEngine;
using System.Collections;


public class SoldierCoordinator: MonoBehaviour {

    public bool inFormation;
    public FormationList formationsList;

    Units unit;
    int squadPosSupply;
    GameObject[] soldiers;
    int delta_PosCount, V_PosCount, followerCount = 0;
    bool prepareToMove, isGroupFormed, callingFollowers;        

    SoldierFollower follower;
    SoldierFormationManager formationManager;
    

    void Awake() {
        unit = GetComponent<Units>();
        formationManager = new SoldierFormationManager();       
        soldiers = GameObject.FindGameObjectsWithTag("Rocket Brigadier");

        if(soldiers != null){ 
            foreach(GameObject soldier in soldiers)
                 if(soldier.name != "soldier leader")
                    follower = soldier.GetComponent<SoldierFollower>();
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
           isGroupFormed = false;
           callingFollowers = false; 
           StartCoroutine("WaitToMove");
        }
    }


    void Update() {

        if(soldiers != null){
           if(prepareToMove){
              inFormation = true;
              StartCoroutine("WaitToMove");
           }

           if(GetComponent<SoldierHealth>().currentHealth <= 0)
              isGroupFormed = false;
        }else
            return;
    }


    /* Choose a random formation */
    void ChooseRandomFormation() {
        callingFollowers = true;
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
        
        if(soldiers != null){ 
           unit.enabled = true;
        }else
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


    /* Return if Squadron isFormed */
    public bool isSquadronFormed() {

        if(isGroupFormed && GetComponent<TankHealth>().currentHealth > 0)
           return true;
        else
           return false;
    }


    /* Leader Calling Followers */
    public bool isCallingFollowers() {

        return callingFollowers;
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

