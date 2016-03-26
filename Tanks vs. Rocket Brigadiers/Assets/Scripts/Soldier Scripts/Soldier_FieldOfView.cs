using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;


public class Soldier_FieldOfView : MonoBehaviour {

    [Range(0, 100)]
    public float lookDistance;

    [Range(90, 180)]
    public float fullAngleView = 150f;

    public float lookingSpeed;
    public LayerMask targetMask;
    public SoldierShooting soldierShooting;
    public bool showRaycast;

    int currHealth; 
    float soundLength;
    float nextFire, angle;
    Vector3 targetPos, enemyFirePos;
    bool locateTarget, startShooting, gettingDamaged;

    bool lockedOn, inShootingRange;
    
    Units units;
    Transform leader;
    Animator animator;
    List<Transform> followers;
    GameObject[] tanks, soldiers;


    void Awake() {
        tanks = GameObject.FindGameObjectsWithTag("Tank");
        followers = new List<Transform>();

        soldiers = GameObject.FindGameObjectsWithTag("Rocket Brigadier");
        foreach(GameObject soldier in soldiers){ 
             if(soldier.name != "soldier leader")
                followers.Add(soldier.transform);
             else
                leader = soldier.transform;
        }
    }


    void Start() {
        enemyFirePos = new Vector3();
        units = GetComponent<Units>();
        animator = GetComponent<Animator>();

        //Destroy Invisble Objects
        /*for(int i=1; i<soldiers.Length; i++) {
            Destroy(soldiers[0]);
        }*/
    }


    void Update() {

        RaycastHit hit;
        setChosenTarget(ClosestTarget());
        currHealth = GetComponent<SoldierHealth>().currentHealth;
        Ray ray = new Ray(transform.position, transform.forward);
        
        if(showRaycast)
           Debug.DrawRay(soldierShooting.shotSpawn.position, ray.direction*lookDistance, Color.magenta);

        if(locateTarget)
           LocateTarget();
        
        if(startShooting)
           StartShooting();

        if(gettingDamaged && enemyFirePos != new Vector3() && !startShooting){
           GettingDamaged(enemyFirePos);
        }


        if(currHealth > 0){
            foreach(GameObject tank in tanks){ 
             
                if(tanks != null){
                    Vector3 direction = targetPos - transform.position;
                    angle = Vector3.Angle(direction, transform.forward);
                
                    //Check if target is <= 1/2 of Field View Angle
                    if(angle <= (fullAngleView/2f) && tank && isTargetOnMap(ClosestTarget())){
                        
                        //Looking Distance
                        if(Physics.Raycast(ray.origin, direction.normalized, out hit, lookDistance, targetMask)){
                             
                            //Obstacle is hit; Maneuvour around it
                            if(hit.collider.tag == "Obstacle"){
                               locateTarget = false;
                               startShooting = false;
                            }
                        
                            //Target is Found & Health > 0
                            if(hit.collider.tag == "Tank"){

                                if(tank.GetComponent<TankHealth>().currentHealth > 0){
                                   locateTarget = true;
                               
                                   //Start shooting when < Randomize (1/4 || 1/8 || 1/5) Field View & Come to a complete Stop
                                   Vector3 distance = new Vector3(transform.position.x-targetPos.x, 0, transform.position.z - targetPos.z);
                                   if(angle < (fullAngleView/3.8f) && distance.sqrMagnitude <= Mathf.Pow(lookDistance, 2)){ 
                                      AnimationStates("ReadyToFire");
                                      
                                      //Lock on Target
                                      if(angle <= 0.5f)
                                         lockedOn = true;
                                      else
                                         lockedOn = false;

                                   }else{
                                      AnimationStates("Idle");
                                   }

                                //Stop Shooting when Target is Destroyed
                                }else if(tank.GetComponent<TankHealth>().currentHealth <= 0){
                                   return;
                                }                             
                            }
                        }
                    }

                    //Turn to Target Inside Blind Spot
                    if(angle > (fullAngleView/2)){
                       locateTarget = true;
                    }

                //No Targets Present
                }else return;
            }

        //Stop Every Behavior when Health <= 0
        }else
            SuspendAllBehaviors();
    }

    
    /* Target Has been Located */
    void LocateTarget() {

        Vector3 targetPosition = ClosestTarget();
        Quaternion targetRotation = Quaternion.LookRotation(targetPosition - transform.position, Vector3.up);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * lookingSpeed);

        if(lockedOn){
           transform.eulerAngles = new Vector3(targetRotation.eulerAngles.x, targetRotation.eulerAngles.y, 0);
        }
    }


    /* Start Shooting at Target */
    void StartShooting() {
        
        if(Time.time > nextFire){ 
           nextFire = Time.time + soldierShooting.rateOfFire;
           GameObject shot = Instantiate(soldierShooting.shotObject, soldierShooting.shotSpawn.position, soldierShooting.shotSpawn.rotation) as GameObject;
           shot.GetComponent<Rigidbody>().velocity = transform.forward * soldierShooting.shootingSpeed;
        }
    }

    
    /* Look At Target Nearest to You */
    Vector3 ClosestTarget() {

        Transform target = tanks.OrderBy(tank => Vector3.Distance(tank.transform.position, transform.position)).FirstOrDefault().transform;      
        Vector3 closestTarget = new Vector3(target.position.x, target.position.y-2, target.position.z);

        return closestTarget;
    }


    /* Locate Enemy Hitting You */
    void GettingDamaged(Vector3 target) {
        setChosenTarget(target);

        Vector3 targetPosition = target;
        Quaternion targetRotation = Quaternion.LookRotation(targetPosition - transform.position, Vector3.up);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 7f);

        if(Vector3.Distance(transform.position, target) <= lookDistance){
           if(angle <= 35f)
              startShooting = true;

           if(lockedOn)
              transform.eulerAngles = new Vector3(targetRotation.eulerAngles.x, targetRotation.eulerAngles.y, 0);
        }
    }


    /* Soldier Animation States */
    void AnimationStates(string animState){
        
        switch(animState) {

            case "Idle":
                animator.SetBool("IsReadyToFire", false);
            break;

            case "ReadyToFire":
                animator.SetBool("IsReadyToFire", true);
            break;

            case "Walk":
                animator.SetBool("IsShooting", true);
            break;
        }
    }


    /* Check if Target is on the Map */
    bool isTargetOnMap(Vector3 target) {

        if((target.x < 97 && target.x >= 1) && (target.z < 97 && target.z >= 1))
            return true;
        else
            return false;
    }


    /* Set Chosen Target */
    void setChosenTarget(Vector3 _target) {

        if(_target != Vector3.zero)
           targetPos = _target;
        else
           targetPos = transform.position;
    }


    /* Stop All Behaviors */
    public void SuspendAllBehaviors() {
        startShooting = false;
        locateTarget = false;
        AnimationStates("Dead");
    }

    
    /* LookAt What's Firing at You */
    void OnTriggerEnter(Collider collid) {

        if(collid.tag == "Shot" && !startShooting) {
           //isGettingShot = true;
           targetPos = collid.transform.position;
        }
    }






    //---------------------------------------------------- ANIMATION STATES ------------------------------------------------------------------//
    #region

    /* Shoot Target */
    public void ShootTarget() {
        Vector3 distance = new Vector3(transform.position.x-ClosestTarget().x, 0, transform.position.z - ClosestTarget().z);

        if(distance.sqrMagnitude <= Mathf.Pow(lookDistance, 2)){
           startShooting = true;
           units.StopPath(true);
        }        
    }


    /* Stop Shooting */
    public void StandBy() {
        startShooting = false;
        units.StopPath(false);
    }

    #endregion
}
