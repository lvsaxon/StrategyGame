using UnityEngine;


public class SoldierSquadronControl: MonoBehaviour {

    GameObject[] soldiers;
    Transform soldier, leader;

    void Start() {
        soldiers = GameObject.FindGameObjectsWithTag("Rocket Brigadier");
        
        foreach(GameObject soldier in soldiers)
             if(soldier.name == "soldier leader")
                leader = soldier.transform;  
    }
    

    void Update() {
        
        //Clamp Collider's to Ground if Leader Looks Up
        if(leader){ 
           if(leader.rotation.eulerAngles.x != 0){ 
              float x = Mathf.Clamp01(transform.rotation.eulerAngles.x); 
              float z = Mathf.Clamp01(transform.rotation.eulerAngles.z);
              transform.rotation = Quaternion.Euler(x, leader.rotation.eulerAngles.y, z);
           }
        }       
    }
}