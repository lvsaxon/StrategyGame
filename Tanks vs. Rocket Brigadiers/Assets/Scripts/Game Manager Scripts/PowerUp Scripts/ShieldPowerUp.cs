using UnityEngine;

public class ShieldPowerUp: MonoBehaviour {

    bool taken = false;

    /* Disable Gravoty when touching the Ground */
    void OnCollisionEnter(Collision collision) {

        GetComponent<Rigidbody>().isKinematic = true;
        if(collision.collider.tag == "Obstacle")
           Destroy(gameObject);
    }


    /* Destroy PowerUp On Collision */
    void OnTriggerEnter(Collider collision) {
        Transform agent;
        
        if(collision.tag == "Rocket Brigadier" && !taken){ 
           taken = true;
           agent = collision.transform;
           agent.GetComponent<SoldierHealth>().EnableShield(true);
           Destroy(gameObject, 0.2f);
        }

        if(collision.tag == "Tank" && !taken) {
           taken = true;
           agent = collision.transform;
           agent.GetComponent<TankHealth>().EnableShield(true);
           Destroy(gameObject, 0.2f);
       }
    }
}
