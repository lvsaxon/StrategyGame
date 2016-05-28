using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class SoldierFollower: MonoBehaviour {

    public Transform remember;
    public bool isAssignedPos;

    string squadType;
    bool turn, retreating;
    GameObject[] soldiers;
    Vector3 turnToPosition;
    Transform squadPosition;
         
    string[] flanking;
    GameObject leader; 
    List<Transform> followers;
    SoldierCoordinator coordinator;
    Dictionary<string, Transform> maneuverPositionList;

    TDEnemy units;


    void Awake() {
        followers = new List<Transform>();
        maneuverPositionList = new Dictionary<string, Transform>();

        soldiers = GameObject.FindGameObjectsWithTag("Rocket Brigadier");        
        foreach(GameObject soldier in soldiers){ 
            if(soldier.name.Contains("leader")){ 
               leader = soldier;
               coordinator = leader.GetComponent<SoldierCoordinator>();
            }else
               followers.Add(soldier.transform);
        }
    }


    void Start() {
        units = GetComponent<TDEnemy>();
    }


    void Update() {
        int currHealth = GetComponent<SoldierHealth>().currentHealth;

        if(transform != null){
           if(currHealth > 0){
              if(coordinator && coordinator.isCallingFollowers() && coordinator.gameObject.GetComponent<SoldierHealth>().currentHealth > 0){
                 
                 //Assigned A Position
                 if(isAssignedPos){ 
                    StartCoroutine("AssignedPosition");
                 }

              }else
                 retreating = true;
           }

           //Locate Target if NotCalled By Leader || Leader is Dead
           //if(retreating)
              //StartCoroutine("LocateLeader");
        }
    }


    /* Recieve Position Request from Leader */
    public void PositionRequest_Received(string formationType, Transform pos) {

        squadPosition = pos;
        squadType = formationType;
        maneuverPositionList[formationType] = squadPosition;
    }


    /* Move to Ordered Position */
    public void ManeuverCommand(string formationType) {

        squadType = formationType;
        remember = maneuverPositionList[formationType];  
        isAssignedPos = true;
    }


    /* Move to Assigned Position */
    IEnumerator AssignedPosition() {
        yield return new WaitForSeconds(0f);
       
        Transform reservedPos;
        if(maneuverPositionList.TryGetValue(squadType, out reservedPos)){
            float distance = Vector3.Distance(transform.position, reservedPos.position);
           
            Vector3 assignedPos = new Vector3(reservedPos.position.x, 2.7f, reservedPos.position.z);
            units.enabled = true;

            if(units.movementSpeed <= 45f)
               units.movementSpeed += 10f * Time.deltaTime;
            units.setAssignedPath(assignedPos);

            if(distance <= 15f){
               foreach(Transform f in followers){
                 if(f.GetComponent<TDEnemy>().movementSpeed >= 0f) 
                    f.GetComponent<TDEnemy>().movementSpeed -= 15f * Time.deltaTime;                                     
                 f.position = Vector3.MoveTowards(f.position, assignedPos, Time.deltaTime * 10f);
               }
           }
        }
    }


    /* Search for Target */
    IEnumerator LocateLeader() {
        yield return new WaitForSeconds(0.5f);

        //Fall back to home side
        if(leader)
           GetComponent<TDEnemy>().enabled = true;
    }
}
