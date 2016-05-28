using UnityEngine;


public class TankSquadronControl: MonoBehaviour {

    GameObject[] tanks;
    Transform tank, leader;
    
    void Start() {
        tanks = GameObject.FindGameObjectsWithTag("Tank");
        
        foreach(GameObject tank in tanks)
             if(tank.name == "CyberTank Leader")
                leader = tank.transform;             
    }
    

    void Update() {
        
        //Clamp Colliders to Ground if Leader Looks Up
        if(leader != null){ 
           if(leader.rotation.eulerAngles.x != 0){
              float x = Mathf.Clamp01(transform.rotation.eulerAngles.x); 
              float z = Mathf.Clamp01(transform.rotation.eulerAngles.z); 
              transform.rotation = Quaternion.Euler(x, leader.rotation.eulerAngles.y, z);
           }
        }
    }
}
