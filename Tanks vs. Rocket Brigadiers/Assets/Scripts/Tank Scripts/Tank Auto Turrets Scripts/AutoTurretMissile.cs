using UnityEngine;

public class AutoTurretMissile: MonoBehaviour {

    public int damage;
    public float blastRadius = 20f; 
    public GameObject explosion;

    void OnCollisionEnter(Collision collision) {
        ContactPoint contactPoint = collision.contacts[0];
        Collider[] colliders = Physics.OverlapSphere(contactPoint.point, blastRadius);

        foreach(Collider col in colliders) {
            if(col.tag == "Rocket Brigadier")
               col.GetComponent<SoldierHealth>().TakingDamage(damage);
        }

        Instantiate(explosion, contactPoint.point, Quaternion.identity);
        Destroy(gameObject);
    }


    /*void OnDrawGizmos() {
        Gizmos.color = Color.white;
        Gizmos.DrawWireSphere(transform.position, blastRadius);
    }*/
}
