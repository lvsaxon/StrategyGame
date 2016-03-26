using UnityEngine;

public class Soldier_ShootingDamage : MonoBehaviour {

	public int shootingDamage;

    Vector3 originPos;
    float soundClipLength;
    TankHealth tankHealth;
    
    void Start() {
        originPos = transform.position;
    }


    /* Return the OriginPos That it Was Fired From */
    public Vector3 getOriginPosition() {

        return originPos;
    }


    /* Shooting Damage */
    void OnCollisionEnter(Collision collision) {
        
        if(collision.gameObject.tag == "Tank"){
           tankHealth = collision.gameObject.GetComponent<TankHealth>();

           if(tankHealth != null)
              tankHealth.TakingDamage(shootingDamage);
        }

        Destroy(gameObject);
    }
}
