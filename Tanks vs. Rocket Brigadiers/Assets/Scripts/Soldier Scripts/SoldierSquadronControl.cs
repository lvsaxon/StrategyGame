using UnityEngine;


public class SoldierSquadronControl: MonoBehaviour {

    int count, max = 1;
    GameObject[] soldiers;
    Transform soldier, leader;

    void Start() {
        count = 0;
        soldiers = GameObject.FindGameObjectsWithTag("Rocket Brigadier");
        
        foreach(GameObject soldier in soldiers)
             if(soldier.name == "soldier leader")
                leader = soldier.transform;              
    }
    

    void Update() {
        
        if(soldier != null && count == 1 && leader.GetComponent<SoldierHealth>().currentHealth > 0){ 
           soldier.position = Vector3.Lerp(soldier.position, transform.position, Time.deltaTime * 10f);
        } else
           count = 0;

        //Clamp Colliders to Ground if Leader Looks Up
        if(leader.rotation.eulerAngles.x != 0){ 
           transform.rotation = Quaternion.identity;
           transform.position = new Vector3(transform.position.x, 0f, transform.position.z);
        }
    }


    /* Reserve Collider for Only 1 Soldier */
    void OnTriggerEnter(Collider collid) {
        SoldierFollower follower = GetComponent<SoldierFollower>();

        if(follower){ 
           if(collid.tag == "Rocket Brigadier" && count < max) {
              count++;
              follower.PositionOccupied(true);
           }
        } else return;
    }
}
