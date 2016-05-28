using UnityEngine;

public class HealthPowerUp: MonoBehaviour {

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
           agent.GetComponent<SoldierHealth>().ReplenishHealth(Random.Range(0.2f, 0.8f));
           Destroy(gameObject, 0.2f);
        }

        if(collid.tag == "Tank" && !taken) {
           taken = true;
           agent = collid.transform;
           agent.GetComponent<TankHealth>().ReplenishHealth(Random.Range(0.2f, 0.8f));
           Destroy(gameObject, 0.2f);
        }
    }
}
