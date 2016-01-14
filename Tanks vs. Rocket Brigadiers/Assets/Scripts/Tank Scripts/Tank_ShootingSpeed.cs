using UnityEngine;
using System.Collections;

public class Tank_ShootingSpeed : MonoBehaviour {

	public float shootingSpeed;
    public int shootingDamage;
    Rigidbody rigidbody;

    SoldierHealth soldierHealth;

    void Start() {
        rigidbody = GetComponent<Rigidbody>();
        rigidbody.velocity = transform.forward * shootingSpeed;

        GameObject soldier = GameObject.FindGameObjectWithTag("Rocket Brigadier");
        soldierHealth = soldier.GetComponent<SoldierHealth>();
    }


    /* Shooting Damage */
    void OnCollisionEnter(Collision collision) {

        Destroy(gameObject, 2);

        if (collision.gameObject.tag == "Rocket Brigadier") 
            soldierHealth.DamageTaken(shootingDamage);
    }
}
