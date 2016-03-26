using UnityEngine;

public class Tank_AmmunitionRounds : MonoBehaviour {

    public int shootingDamage;
    
    Vector3 originPos;
    float soundClipLength;
    SoldierHealth soldierHealth;
    
    void Start() {
        originPos = transform.position;
    }


    /* Return the OriginPos That it Was Fired From */
    public Vector3 getOriginPosition() {

        return originPos;
    }


    /* Shooting Damage */
    void OnCollisionEnter(Collision collision) {

        if(collision.gameObject.tag == "Rocket Brigadier"){
           soldierHealth = collision.gameObject.GetComponent<SoldierHealth>();

           if(soldierHealth != null) { 
              soldierHealth.TakingDamage(shootingDamage);
            }
        }

        Destroy(gameObject, 2);
    }
}