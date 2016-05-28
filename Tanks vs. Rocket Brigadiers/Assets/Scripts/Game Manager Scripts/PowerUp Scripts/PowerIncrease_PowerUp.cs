using UnityEngine;

public class PowerIncrease_PowerUp: MonoBehaviour {

    bool taken = false;

    /* Disable Gravoty when touching the Ground */
    void OnCollisionEnter(Collision collision) {

        GetComponent<Rigidbody>().isKinematic = true;
        if(collision.collider.tag == "Obstacle")
           Destroy(gameObject);
    }


    /* Destroy PowerUp On Collision */
    void OnTriggerEnter(Collider collid) {
        Transform agent;
        
        if(collid.tag == "Rocket Brigadier" && !taken){ 
           taken = true;
           agent = collid.transform;
           agent.GetComponent<Soldier_FieldOfView>().DamageIncrease(Random.Range(5, 10), 10f);
           Destroy(gameObject, 0.2f);
        }

        if(collid.tag == "Tank" && !taken){
           taken = true;
           agent = collid.transform;
           agent.GetComponentInChildren<TankTurretControl>().DamageIncrease(Random.Range(5, 10));
           Destroy(gameObject, 0.2f);
       }
    }
}
