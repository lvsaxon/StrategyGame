using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class TankTurretControl: MonoBehaviour {

    [Range(0, 200)]
    public float lookDistance;
    [Range(90, 180)]
    public float fullAngleView = 150f;
    public float lookingSpeed;
    public LayerMask targetMask;
    public TankShooting tankShooting;
    public bool showRaycast;
    
    float nextFire, angle;
    int currHealth, damageIncres;   
    float damageIncres_MaxTime = 7f;
    bool locateTarget, startShooting, gettingDamaged;    
    bool lockedOn;
       
    Timer timer;
    Transform leader, currTarget;
    List<Transform> followers;
    GameObject[] brigadiers, tanks;
    

    void Awake() {
        brigadiers = GameObject.FindGameObjectsWithTag("Rocket Brigadier");
        followers = new List<Transform>();

        tanks = GameObject.FindGameObjectsWithTag("Tank");
        foreach(GameObject tank in tanks)
             if(!tank.name.Contains("Leader"))
                followers.Add(tank.transform);
             else
                leader = tank.transform;
    }


    void Start() {       
        GameObject gameManager = GameObject.FindGameObjectWithTag("Game Manager");
        timer = gameManager.GetComponent<Timer>();
    }


    void Update() {
        RaycastHit hit;
        currHealth = GetComponentInParent<TankHealth>().currentHealth;
        Ray ray = new Ray(transform.position, -transform.up);

        if(showRaycast)
           Debug.DrawRay(ray.origin, -transform.up*lookDistance, Color.red);

        if(timer.TimeLeft > 0){ 
           //Locate Target
           if(locateTarget)
              LocateTarget();
        
           //Start Shooting
           if(startShooting)
              StartShooting();        

           if(currHealth > 0){
              for(int i=0; i<brigadiers.Length; i++){
                  
                  if(brigadiers != null){
                     Vector3 direction = ClosestTarget() - transform.position;
                     angle = Vector3.Angle(direction, -transform.up);

                     //Check if target is <= to 1/2 of Field View Angle
                     if(angle <= (fullAngleView/2f) && brigadiers[i] && isTargetOnMap(ClosestTarget())){
                           
                        //Shooting Range
                        if(Physics.Raycast(ray.origin, direction.normalized, out hit, lookDistance, targetMask)){
                           
                           if(hit.collider.tag != "Rocket Brigadier"){
                              locateTarget = false;
                              startShooting = false;
                           }

                           //Target Found && Health > 0
                           if(hit.collider.tag == "Rocket Brigadier"){
                              if(hit.collider.GetComponent<SoldierHealth>().currentHealth > 0){
                                 locateTarget = true;
                               
                                 //Start shooting when < 1/4 Field View & Come to a complete Stop
                                 float distance = Vector3.Distance(transform.position, hit.collider.transform.position);
                                 if((distance < lookDistance) && (angle < fullAngleView/5.5f)){ 
                                    startShooting = true;

                                    if(angle <= 1.5f)
                                       lockedOn = true;
                                 }                                   
                              }
                           }
                        }else{
                           lockedOn = false; 
                           locateTarget = false;   
                           startShooting = false;
                        }
                     }                                             
                  }else return;
              }
           
           //Stop Every Behavior
           }else{
              lockedOn = false;
              startShooting = false;
              locateTarget = false;
           }
        }
    }


    /* Target Has been Located */
    void LocateTarget() {
        
        Vector3 targetPosition = ClosestTarget();
        Quaternion targetRotation = Quaternion.LookRotation(targetPosition - transform.position, Vector3.up);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * lookingSpeed);
        transform.rotation = Quaternion.Euler(270f, transform.eulerAngles.y, 0f);

        if(lockedOn)
           StartCoroutine("LockedOn", targetPosition);        
    }


    /* Start Shooting at Target */
    void StartShooting() {
        
        if(Time.time > nextFire){
           foreach(Transform shotSpawn in tankShooting.shotSpawns){  
               nextFire = Time.time + tankShooting.rateOfFire;
               GameObject rayCannon = Instantiate(tankShooting.shotObject, shotSpawn.position, shotSpawn.rotation) as GameObject;
               rayCannon.GetComponent<TankRayCannon>().setParentObject(transform);
               
               if(damageIncres > 0 && Time.time <= damageIncres_MaxTime)
                  rayCannon.GetComponent<TankRayCannon>().IncreaseDamage(damageIncres);
               else{ 
                  damageIncres = 0;
                  rayCannon.GetComponent<TankRayCannon>().DefaultDamage();
               }
           }
        }
    }


    /* Increase Damage PowerUp; Temporary Damage Incres */
    public void DamageIncrease(int _dmgIncres) {

        damageIncres = _dmgIncres;
    }


    /* Look At Target Nearest to You */
    Vector3 ClosestTarget() {
        float shortestDistance = Mathf.Infinity;
        Vector3 closestTarget = new Vector3();

        if(currTarget)
           closestTarget = currTarget.position;

        else{ 
           foreach(GameObject soldier in brigadiers){
                if(soldier){
                  float distance = Vector3.Distance(transform.position, soldier.transform.position);

                  if(shortestDistance > distance){
                     shortestDistance = distance;
                     closestTarget = soldier.transform.position;
                  }
               }
           }
        }
    
        return closestTarget;
    }


    /* Chech if Target is on the Map */
    bool isTargetOnMap(Vector3 target) {

        //if((target.x < 97 && target.x >= 1) && (target.z < 97 && target.z >= 1))
        if((target.x <=223 && target.x >= -333) && (target.y < 20 && target.y >= -4) && (target.z <=315 && target.z >= -396))
            return true;
        else
            return false;
    }


    /* Locked On Target */
    IEnumerator LockedOn(Vector3 targetPosition) {
        yield return new WaitForSeconds(3f);

        Quaternion targetRotation = Quaternion.LookRotation(targetPosition - transform.position, Vector3.up);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * lookingSpeed);
        transform.rotation = Quaternion.Euler(270f, transform.eulerAngles.y, 0f);
    }


    /* Set Current Target */
    public void setCurrentTarget(Transform _currTarget) {

        currTarget = _currTarget;       
    }


    /* Shoot @ FarAway Targets if Getting Damaged */
    public void ShootLongRange(Transform target) {
        currTarget = target;

        if(Vector3.Distance(transform.position, currTarget.position) >= lookDistance){ 
           locateTarget = true;
           startShooting = true;
        }
    }


    /* Return Current Target */
    public Transform CurrentTarget {
        
        get{
            if(currTarget)
               return currTarget;
            else
               return null;
        }
    }





    /*---------------------- TURRET STATES -------------------------------------*/
    public bool isShooting {

        get{ return startShooting; }
    }

    public bool isLocatingTarget {

        get{ return locateTarget; }
    }
}
