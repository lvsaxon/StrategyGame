using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;


public class Soldier_FieldOfView: MonoBehaviour {

    [Range(0, 200)]
    public float lookDistance;
    [Range(90, 180)]
    public float fullAngleView = 150f;

    public float lookingSpeed, rayHeight;
    public LayerMask targetMask;
    public SoldierShooting soldierShooting;
    public bool showRaycast;

    int currHealth;     
    Vector3 targetPos, enemyFirePos;
    float nextFire, angle, maxRotationSpd;
    float soundLength, accelerationLookingSpd;
    bool locateTarget, startShooting, gettingDamaged;
    
    new AudioSource audio;
    new Rigidbody rigidbody;
    Queue<Transform> nextTarget;
    GameObject[] tanks, soldiers, tturrets;
    bool inRange, strafe;
    
    Timer timer;
    TDEnemy units;
    Transform leader;
    Animator animator;
    List<Transform> followers;
    
    int damageIncres = 0;
    float damageIncres_MaxTime = 7f;
    PowerUpLocationManager powerUpLocationManager; /* Health, Incres Power, Shield */


    void Awake() {
        followers = new List<Transform>();
        nextTarget = new Queue<Transform>();
        tanks = GameObject.FindGameObjectsWithTag("Tank");
        tturrets = GameObject.FindGameObjectsWithTag("TTurret");

        soldiers = GameObject.FindGameObjectsWithTag("Rocket Brigadier");
        foreach(GameObject soldier in soldiers){ 
             if(soldier.name != "soldier leader")
                followers.Add(soldier.transform);
             else
                leader = soldier.transform;
        }
    }


    void Start() {
        maxRotationSpd = 6f;
        accelerationLookingSpd = 2.5f;
        enemyFirePos = new Vector3();
        units = GetComponent<TDEnemy>();
        animator = GetComponent<Animator>();

        audio = GetComponent<AudioSource>();        
        rigidbody = GetComponent<Rigidbody>();
        timer = GameObject.FindGameObjectWithTag("Game Manager").GetComponent<Timer>();
        powerUpLocationManager = GameObject.FindGameObjectWithTag("Game Manager").GetComponent<PowerUpLocationManager>();
        
        //Destroy Invisible Objects
        Debug.Log("Soldiers: "+soldiers.Length);
        /*for(int i=0; i<soldiers.Length; i++){
             Destroy(soldiers[i]);
        } Debug.Log(soldiers.Length);  */   
    }


    void FixedUpdate() {
        RaycastHit hit;
        targetPos = ClosestTarget();
        currHealth = GetComponent<SoldierHealth>().currentHealth;
        Ray ray = new Ray(transform.position+Vector3.up*rayHeight, transform.forward);
        
        if(showRaycast)
           Debug.DrawRay(soldierShooting.shotSpawn.position, ray.direction*lookDistance, Color.magenta);

        if(timer.TimeLeft > 0){
           
            if(locateTarget)
               LocateTarget();

            /*if(strafe)
               units.StartStrafing(3);*/

            if(units.enabled){ 
               animator.SetBool("IsSprinting", true);
               if(inRange){
                  setAnimationState("ReadyToFire");
                  if(startShooting)
                     StartCoroutine("StartShooting");
               }
            }else
               setAnimationState("Idle");
            
            if(gettingDamaged && enemyFirePos != new Vector3() && !startShooting){
                GettingDamaged(enemyFirePos);
            }


            if(currHealth > 0){
                foreach(GameObject tank in tanks){ 
                    Vector3 direction = targetPos - transform.position;
                    angle = Vector3.Angle(direction, transform.forward);

                    if(Physics.Raycast(ray.origin, direction.normalized, out hit, lookDistance, targetMask)){
                        
                        //Obstacle is hit; Maneuvour around it
                        if(hit.collider.tag != "Tank"){
                           locateTarget = false;
                           inRange = false;
                        }

                        if(hit.collider.tag == "TTurret") {
                           if(hit.collider.GetComponent<Tank_AutoTurret>() && hit.collider.GetComponent<Tank_AutoTurret>().currHealth > 0) {
                              locateTarget = true;

                              if(angle < (fullAngleView * 0.2f))
                                 inRange = true;                             
                            }
                        }
                        
                        //Target is Found & Health > 0
                        if(hit.collider.tag == "Tank"){
                           if(tank.GetComponent<TankHealth>().currentHealth > 0){
                              locateTarget = true;
                               
                               //Start shooting when < Randomize (1/4 || 1/8 || 1/5) Field View & Come to a complete Stop
                               float distance = Vector3.Distance(transform.position, targetPos);
                               if(angle < (fullAngleView * 0.2f) && (distance < lookDistance)){
                                  inRange = true;                             
                               }
                           }                            
                       }
                    }else{
                       inRange = false;
                       locateTarget = false;
                    }
                }

            //Stop Every Behavior when Health <= 0
            }else
               SuspendAllBehaviors();
        }else
            GameOver();
    }


    /* Target Has been Located */
    void LocateTarget() {
        
        Vector3 targetPosition = targetPos;
        Quaternion targetRotation = Quaternion.LookRotation(targetPosition - transform.position, Vector3.up);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, Time.deltaTime * lookingSpeed);
        transform.rotation = Quaternion.Euler(0, transform.eulerAngles.y, 0f);

        if(angle <= 3f)
           transform.LookAt(new Vector3(targetPosition.x, transform.position.y, targetPosition.z));
    }


    /* Start Shooting at Target */
    IEnumerator StartShooting() {
        
        while(Time.time > nextFire){ 
              nextFire = Time.time + Random.Range(soldierShooting.rateOfFire-0.2f, soldierShooting.rateOfFire);
              audio.PlayOneShot(soldierShooting.rifleAudio, 0.25f);
              Instantiate(soldierShooting.shotFlare, soldierShooting.shotSpawn.position, Quaternion.identity);

              //Bullet Object
              GameObject bullet = Instantiate(soldierShooting.shotObject, soldierShooting.shotSpawn.position, soldierShooting.shotSpawn.rotation) as GameObject;
              bullet.GetComponent<Soldier_ShootingDamage>().setParentObject(transform);
              bullet.GetComponent<Rigidbody>().velocity = transform.forward * soldierShooting.shootingSpeed;

              //Temporary Damage Incres PowerUp
              if(damageIncres > 0 && Time.deltaTime <= damageIncres_MaxTime)
                 bullet.GetComponent<Soldier_ShootingDamage>().IncreaseDamage(damageIncres);
              else{ 
                 damageIncres = 0;
                 bullet.GetComponent<Soldier_ShootingDamage>().DefaultDamage();
              }
           
              yield return null;
        }
    }


    /* Increase Damage PowerUp; Temporary Damage Incres */
    public void DamageIncrease(int _dmgIncres, float duration) {

        damageIncres = _dmgIncres;
    }


    /* Find Target Closest to You */
    public Vector3 ClosestTarget() {
        float shortestDistance = Mathf.Infinity;
        Vector3 closestTank = new Vector3(), closestTurret = new Vector3(), closestTarget = new Vector3();

        //Find Closest Tank
        foreach(GameObject tank in tanks){
            TankHealth tankHealth = tank.GetComponent<TankHealth>(); 

            if(tankHealth && tank.GetComponent<TankHealth>().currentHealth > 0){
               float distance = (transform.position - tank.transform.position).sqrMagnitude;

               if(shortestDistance > distance){
                  shortestDistance = distance;
                  closestTank = tank.transform.position;
               }
            }
        }

        //Find Closest TTurret
        foreach(GameObject turret in tturrets) {
            float distance = (transform.position - turret.transform.position).sqrMagnitude;

            if(shortestDistance > distance){
               shortestDistance = distance;
               closestTurret = turret.transform.position;
            }
        }


        //Compare Between Closest Targets
        float tankDist = (transform.position - closestTank).sqrMagnitude;
        float turretDist = (transform.position - closestTurret).sqrMagnitude;
        if(tankDist < turretDist)
           closestTarget = closestTank;
        else
           closestTarget = closestTurret;


        return closestTarget;
    }


    /* Locate Enemy Hitting You */
    void GettingDamaged(Vector3 target) {
        targetPos = target;

        Vector3 targetPosition = target;
        Quaternion targetRotation = Quaternion.LookRotation(targetPosition - transform.position, Vector3.up);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 7f);

        if(Vector3.Distance(transform.position, target) <= lookDistance){
           if(angle <= 35f)
              startShooting = true;
        }
    }


    /* Soldier Animation States */
    void setAnimationState(string animState) {
        
        switch(animState) {

            case "Idle":
                animator.SetBool("IsReadyToFire", false);
                animator.SetBool("IsSprinting", false);
            break;

            case "ReadyToFire":
                animator.SetBool("IsReadyToFire", true);
                animator.SetBool("IsSprinting", false);
            break;

            case "Walk":
                animator.SetBool("IsShooting", true);
            break;

            case "Sprint":
                animator.SetBool("IsSprinting", true);
            break;
        }
    }


    /* Check if Target is on the Map */
    bool isTargetOnMap(Vector3 target) {
        
        if((target.x <=223 && target.x >= -333) && (target.y < 20 && target.y >= -4) && (target.z <=315 && target.z >= -396))
           return true;
        else
           return false;
    }


    /* Terminate All Behaviors */
    void GameOver() {
        StopAllCoroutines();
        setAnimationState("Idle");
    }

    
    /* LookAt What's Firing at You */
    void OnTriggerEnter(Collider collid) {

        if(collid.tag == "Shot" && !startShooting) {
           gettingDamaged = true;
        }
    }


    /* Negate Collision of Like Agents */
    void OnCollisionEnter(Collision collision) {

        if(collision.collider.tag == "Rocket Brigadier")
           collision.collider.GetComponent<Rigidbody>().velocity = Vector3.zero;
    }










    //---------------------------------------------------- ANIMATION STATES ------------------------------------------------------------------//
    #region  Animation State Commands

    /* Shoot Target */
    public void ShootTarget() {
        startShooting = true;
        units.setStopPath(true);               
    }

    /* Stop Shooting */
    public void StandBy() {
        startShooting = false;
    }

    /* Search Target */
    public void SearchTarget() {
        startShooting = false;
        units.setStopPath(false);
    }

    /* Stop All Behaviors */
    public void SuspendAllBehaviors() {
        inRange = false;
        startShooting = false;
        locateTarget = false;
        units.enabled = false;
        animator.SetBool("IsReadyToFire", false);
    }

    #endregion
}
