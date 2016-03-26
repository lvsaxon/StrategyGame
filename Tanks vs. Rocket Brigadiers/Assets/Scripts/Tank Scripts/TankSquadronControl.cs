using UnityEngine;


public class TankSquadronControl: MonoBehaviour {

    public bool occupied;
    int count = 0, max = 1;
    GameObject[] tanks;
    Transform tank, leader;
    

    void Start() {
        occupied = false;
        tanks = GameObject.FindGameObjectsWithTag("Tank");
        
        foreach(GameObject tank in tanks)
             if(tank.name == "tank leader")
                leader = tank.transform;             
    }
    

    void Update() {
        
        //Clamp Colliders to Ground if Leader Looks Up
        if(leader != null){ 
           if(leader.rotation.eulerAngles.x != 0) { 
              transform.rotation = Quaternion.identity;
              transform.position = new Vector3(transform.position.x, 0f, transform.position.z);
           }
        }

        //Pos Occupied if count is 1
        if(count == max){
           occupied = true;
        }
    }


    /* When TankObj Enters Collider; Take Control Over its Movements */
    void OnTriggerEnter(Collider collid) {
        Tank_Follower follower = collid.GetComponent<Tank_Follower>();

        if(follower){ 
           if(collid.tag == "Tank" && count < max) {
              count++;
              follower.PositionOccupied(true);
           }
        } else return;
    }
}
