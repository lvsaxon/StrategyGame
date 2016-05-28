using UnityEngine;

public class AutoTurretShoot: MonoBehaviour {

    public Transform shotSpawn;
    public GameObject shotObject;
    public float height, rocketSpeed;
    public bool showRaycast;

    float shootingRange, nextFire, delay = 3f;


    void Update() {

        Ray ray = new Ray(transform.position, -transform.up);
        if(showRaycast)
           Debug.DrawRay(ray.origin, ray.direction * shootingRange, Color.magenta);
    }


    public void StartShooting(bool startShooting, Vector3 shootingDir){

        if(startShooting && (Time.time > nextFire)){
           nextFire = Time.time + delay;
           GameObject rocket = Instantiate(shotObject, shotSpawn.position, shotSpawn.rotation) as GameObject;
           rocket.GetComponent<Rigidbody>().velocity = shotSpawn.right * rocketSpeed;
        }
    }
}
