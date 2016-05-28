using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class Tank_AutoTurret: MonoBehaviour {

    public int startingHealth = 100, currHealth;
    public Transform target;
    public LayerMask targetMask;
    public float turningSpeed;

    [Range(0, 150)]
    public float shootingRange;
    public HealthUI healthUI;
    public GameObject explosion;
    public bool showRaycast;

    Image healthImg;
    Canvas healthCanvas;
    Vector3 lastKnownPos;
    GameObject[] brigadiers;
    Quaternion lookAtRotation;

    bool isDestroyed;
    AutoTurretShoot turretShoot;


    void Start() {
        turretShoot = GetComponentInChildren<AutoTurretShoot>();
        brigadiers = GameObject.FindGameObjectsWithTag("Rocket Brigadier");
        currHealth = startingHealth;

        //Health UI Components
        healthImg = healthUI.healthUI;
        healthCanvas = healthUI.healthCanvas; 
    }
    

    void Update() {
        RaycastHit hit;
        Ray ray = new Ray(transform.position, transform.forward);
        if(showRaycast)
           Debug.DrawRay(ray.origin+Vector3.up*10, ray.direction * shootingRange, Color.red);

        if(currHealth > 0){ 
           StartCoroutine("LocateTarget");

           if(Physics.Raycast(ray.origin, ray.direction.normalized, out hit, targetMask)) {
           
              if(hit.collider.tag == "Rocket Brigadier"){
                 if(turretShoot)
                    turretShoot.StartShooting(true, hit.collider.transform.position);
              }
           }
        }
    }


    /* Locate Target */
    IEnumerator LocateTarget() {
        yield return new WaitForSeconds(0.5f);

        target = ClosestTarget();
        if(target){
            if(lastKnownPos != target.position){
               lastKnownPos = target.position;
               lookAtRotation = Quaternion.LookRotation(lastKnownPos - transform.position);
               transform.rotation = Quaternion.RotateTowards(transform.rotation, lookAtRotation, turningSpeed * Time.deltaTime);
            }
        }
    }


    /* Look At Closest Target */
    Transform ClosestTarget() {
        float shortestDistance = Mathf.Infinity;
        Transform closestTarget = null;

        foreach(GameObject soldier in brigadiers){
            if(soldier){
                float distance = Vector3.Distance(transform.position, soldier.transform.position);

                if(shortestDistance > distance){
                    shortestDistance = distance;
                    closestTarget = soldier.transform;
                }
            }
        }
    
        return closestTarget;
    }   


    /* Health UI */
    void HealthUI(int currentHealth) {

        healthImg.fillAmount = currentHealth / (float)startingHealth;

        if(healthImg.fillAmount == 0) 
           healthCanvas.enabled = false;

        if(healthImg.fillAmount <= 0.5f){
           healthImg.color = Color.Lerp(Color.green , Color.yellow, currentHealth);
            
           if(healthImg.fillAmount <= 0.25f)
              healthImg.color = Color.Lerp(Color.yellow , Color.red, currentHealth);
        }
    }


    /* Getting Damaged */
    public void TakingDamage(int damage) {

        currHealth -= damage;
        HealthUI(currHealth);

        if(currHealth <= 0) {
           isDestroyed = true;
           Instantiate(explosion, transform.position, Quaternion.identity);
           StartCoroutine("RebuildTurret", 30);
        }
    }


    /* Reconstruct Destroyed Turret */
    IEnumerator RebuildTurret(float reforgeTime) {
        yield return new WaitForSeconds(reforgeTime);

        currHealth = startingHealth;
        healthCanvas.enabled = true;
        healthImg.fillAmount = 1;
        healthImg.color = Color.green;

        yield return new WaitForSeconds(2f);
    }
}
