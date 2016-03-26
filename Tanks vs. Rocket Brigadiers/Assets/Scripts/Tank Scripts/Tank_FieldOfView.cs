using UnityEngine;
using System.Linq;
using System.Collections.Generic;
using System.Collections;


public class Tank_FieldOfView : MonoBehaviour {

    [Range(0, 100)]
    public float lookDistance;
    [Range(90, 180)]
    public float fullAngleView = 150f;

    public float lookingSpeed;
    public LayerMask targetMask;
    public TankShooting tankShooting;
    public bool showRaycast;
    
    int currHealth;
    float nextFire, angle;
    Vector3 targetPos, enemyFirePos;
    bool locateTarget, startShooting, gettingDamaged;    
    bool lockedOn;
    
    Units units;
    Transform leader;
    List<Transform> followers;
    GameObject[] brigadiers, tanks;
    

    void Awake() {
        brigadiers = GameObject.FindGameObjectsWithTag("Rocket Brigadier");
        followers = new List<Transform>();

        tanks = GameObject.FindGameObjectsWithTag("Tank");
        foreach(GameObject tank in tanks)
             if(tank.name != "tank leader")
                followers.Add(tank.transform);
             else
                leader = tank.transform;
    }


    void Start() {
        enemyFirePos = new Vector3();
        units = GetComponent<Units>();
    }


    void Update() {
        RaycastHit hit;
        setChosenTarget(ClosestTarget());
        currHealth = GetComponent<TankHealth>().currentHealth;
        Ray ray = new Ray(transform.position, transform.forward);

        if(showRaycast)
           Debug.DrawRay(ray.origin, transform.forward*lookDistance, Color.red);

        if(locateTarget)
           LocateTarget();
        
        if(startShooting)
           StartShooting();

        if(gettingDamaged && enemyFirePos != new Vector3() && !startShooting){
           GettingDamaged(enemyFirePos);
        }
        
        if(currHealth > 0){ 
            foreach(GameObject soldier in brigadiers){
          
                if(brigadiers != null){ 
                    Vector3 direction = targetPos - transform.position;
                    angle = Vector3.Angle(direction, transform.forward);

                    //Check if target is <= to 1/2 of Field View Angle
                    if(angle <= (fullAngleView/2f) && soldier && isTargetOnMap(ClosestTarget())){
                    
                        //Shooting Range
                        if(Physics.Raycast(ray.origin, direction.normalized, out hit, lookDistance, targetMask)){
                        
                            //Obstacle is hit; Maneuvour around it
                            if(hit.collider.tag == "Obstacle"){
                                locateTarget = false;
                                startShooting = false;
                            }
                
                            //Target is Found & Health > 0
                            if(hit.collider.tag == "Rocket Brigadier"){
                                if(hit.collider.GetComponent<SoldierHealth>().currentHealth > 0){
                                   locateTarget = true;
                                                             
                                   //Start shooting when < Randomize (1/4 || 1/8 || 1/5) Field View & Come to a complete Stop
                                   if(angle < (fullAngleView/4f) && Vector3.Distance(transform.position, soldier.transform.position) < 40){
                                      startShooting = true;
                                      gettingDamaged = false;
                                      units.StopPath(true);
                                
                                      if(angle <= 1.5f){
                                         lockedOn = true;
                                      }

                                   }else{ 
                                      lockedOn = false;
                                      startShooting = false;
                                      units.StopPath(false);
                                   }

                                }else if(hit.collider.GetComponent<SoldierHealth>().currentHealth <= 0){
                                    lockedOn = false;
                                    startShooting = false;
                                    units.StopPath(false);
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

         //Stop Every Behavior
         }else{
            lockedOn = false;
            startShooting = false;
            locateTarget = false;
        }
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
           nextFire = Time.time + tankShooting.rateOfFire;
           GameObject shot = Instantiate(tankShooting.shotObject, tankShooting.shotSpawn.position, tankShooting.shotSpawn.rotation) as GameObject;
           shot.GetComponent<Rigidbody>().velocity = transform.forward * tankShooting.shootingSpeed;
        }
    }


    /* Look At Target Nearest to You */
    Vector3 ClosestTarget() {
        float shortestDistance = Mathf.Infinity;
        Vector3 closestTarget = new Vector3();

        foreach(GameObject soldier in brigadiers){
            if(soldier){
               float distance = Vector3.Distance(transform.position, soldier.transform.position);

               if(shortestDistance > distance){
                  shortestDistance = distance;
                  closestTarget = soldier.transform.position;
               }
            }
        }

        return closestTarget;
    }


    /* Locate Enemy Hitting You */
    void GettingDamaged(Vector3 target) {
        
        Vector3 targetPosition = target;
        Quaternion targetRotation = Quaternion.LookRotation(targetPosition - transform.position, Vector3.up);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 7f);

        if(Vector3.Distance(transform.position, target) <= lookDistance){
           if(angle <= 35f)
              startShooting = true;

           if(angle <= 0.5f)
              transform.LookAt(target);
        }
    }
   

    /* Chech if Target is on the Map */
    bool isTargetOnMap(Vector3 target) {

        if((target.x < 97 && target.x >= 1) && (target.z < 97 && target.z >= 1))
            return true;
        else
            return false;
    }


    /* Set Chosen Target */
    void setChosenTarget(Vector3 _target) {

        targetPos = _target;
    }


    /* Shoot @ Target Hitting You */
    void OnCollisionEnter(Collision collision) {
        
        if(collision.collider.tag == "Rocket"){ 
           gettingDamaged = true;
           enemyFirePos = collision.collider.GetComponent<Soldier_ShootingDamage>().getOriginPosition();
        }
    }
}