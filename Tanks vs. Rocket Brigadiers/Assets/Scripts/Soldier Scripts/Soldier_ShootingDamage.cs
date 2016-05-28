using UnityEngine;

public class Soldier_ShootingDamage : MonoBehaviour {

	public int shootingDamage;

    int defaultDmg;
    Transform parentObj;
    float soundClipLength;
    TankHealth tankHealth;

    void Awake() {
        defaultDmg = shootingDamage;
    }


    /* Set Owner/Parent for This Object */
    public void setParentObject(Transform _parent) {

        parentObj = _parent;
    }

    
    /* Return the OriginPos That it Was Fired From */
    public Transform getParentObject() {

        return parentObj;
    }


    /* Increase Damage Output */
    public void IncreaseDamage(int _incresDmg) {

        shootingDamage += _incresDmg;
    }


    /* Set Shooting Damage back to Regular Defaults */
    public void DefaultDamage() {

        shootingDamage = defaultDmg;
    }


    /* Shooting Damage */
    void OnCollisionEnter(Collision collision) {

        //Tank
        if(collision.gameObject.tag == "Tank"){
           tankHealth = collision.gameObject.GetComponent<TankHealth>();

           if(tankHealth != null)
              tankHealth.TakingDamage(shootingDamage);
        }

        //TTurret
        if(collision.collider.tag == "TTurret") {
           Transform turret = collision.collider.transform;

           if(turret)
              turret.GetComponent<Tank_AutoTurret>().TakingDamage(shootingDamage);
        }

        Destroy(gameObject);
    }
}
