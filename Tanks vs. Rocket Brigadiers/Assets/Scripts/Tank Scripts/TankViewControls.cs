using UnityEngine;
using System.Linq;
using System.Collections.Generic;
using System.Collections;


public class TankViewControls: MonoBehaviour {

    public float hoverLift = 0.17f;

    Timer timer;
    int currHealth;
    bool locateTarget, gettingDamaged;
    Transform enemyFound, currTarget;

    TDEnemy units;
    GameObject[] brigadiers;
    Queue<Transform> nextTarget;       
    TankTurretControl turretCtrl;
    PowerUpLocationManager powerUpLocationManager; /* Health, Incres Power, Shield */


    void Awake() {
        brigadiers = GameObject.FindGameObjectsWithTag("Rocket Brigadier");
    }


    void Start() {
        units = GetComponent<TDEnemy>();
        nextTarget = new Queue<Transform>();
        turretCtrl = GetComponentInChildren<TankTurretControl>();

        GameObject gameManager = GameObject.FindGameObjectWithTag("Game Manager");
        timer = gameManager.GetComponent<Timer>();
        powerUpLocationManager = gameManager.GetComponent<PowerUpLocationManager>();
    }


    void Update() {
        currHealth = GetComponent<TankHealth>().currentHealth;
        transform.rotation = Quaternion.Euler(0, transform.eulerAngles.y, 0);
        
        if(timer.TimeLeft > 0){ 

           if(currHealth > 0){
              GetTargetDistance();

              StartCoroutine("Hover");
              if(nextTarget.Count == 0)
                 gettingDamaged = false;

              //Getting Damaged
              if(gettingDamaged){
                 if(!turretCtrl.isShooting){
                    currTarget = nextTarget.Peek();
                    turretCtrl.ShootLongRange(currTarget);
                    nextTarget.Dequeue();

                 }else if(turretCtrl.CurrentTarget){ 
                       if(turretCtrl.CurrentTarget.GetComponent<SoldierHealth>().currentHealth <= 0){
                          turretCtrl.setCurrentTarget(nextTarget.Peek());
                          nextTarget.Dequeue();
                    }else
                       return;
                 }
                    /*  1. Finish off current Target
                        2. Search Queue if Empty
                          --Empty: find ClosestTarget()
                          --Else Locate Target & Start Shooting
                          GettingDamaged(enemyFirePos);*/
               }else{
                  //turretCtrl.setChosenTarget(ClosestTarget());
               }

            }else{
               gettingDamaged = false;
            }
        }
    }
    
    
    /* Target Has been Located */
    IEnumerator LocateTarget(Vector3 _targetPos) {
        yield return new WaitForSeconds(1.2f);

        Vector3 targetPosition = _targetPos;
        Quaternion targetRotation = Quaternion.LookRotation(targetPosition - transform.position, Vector3.back);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 5f);
        transform.rotation = Quaternion.Euler(0f, transform.eulerAngles.y, 0f);
    }


    /* Follow Path if Target is Out of Range; Else Stop following*/
    void GetTargetDistance() {
        float distance = 0;
        Vector3 target = new Vector3();

        if(ClosestTarget() != Vector3.zero){ 
           target = ClosestTarget();
           distance = Vector3.Distance(transform.position, target);
        }
        
        //Rotate Base After Turret Locates Target
        if(distance < turretCtrl.lookDistance){
           if(turretCtrl.isShooting) 
              units.setStopPath(true);

           if(turretCtrl.isLocatingTarget)
              StartCoroutine("LocateTarget", target);
           else
              units.setStopPath(true);

        }else{
           units.setStopPath(false);
        }
    }


    /* Hover When Idle */
    IEnumerator Hover() {
        yield return new WaitForSeconds(0.5f);

        if(GetComponent<Rigidbody>().IsSleeping())
           transform.position = new Vector3(transform.position.x, Mathf.Sin(Time.time) * hoverLift+transform.position.y, transform.position.z);
    }


    /* Look At Target Nearest to You */
    Vector3 ClosestTarget() {
        float shortestDistance = Mathf.Infinity;
        Vector3 closestTarget = new Vector3();

        foreach(GameObject soldier in brigadiers){
            if(soldier.GetComponent<SoldierHealth>() && soldier.GetComponent<SoldierHealth>().currentHealth > 0){
               float distance = Vector3.Distance(transform.position, soldier.transform.position);

               if(shortestDistance > distance){
                  shortestDistance = distance;
                  closestTarget = soldier.transform.position;
               }
            }
        }

        return closestTarget;
    }


    /* Shoot @ Target Hitting You */
    void OnCollisionEnter(Collision collision) {
        
        if(collision.collider.tag == "Rocket"){ 
           gettingDamaged = true;
           enemyFound = collision.collider.GetComponent<Soldier_ShootingDamage>().getParentObject();
           
           //Add to Queue if Empty
           if(nextTarget.Count == 0){ 
              nextTarget.Enqueue(enemyFound);

           //Add to Queue if it Contains No Duplicate Objects
           }else if(!nextTarget.Contains(enemyFound)){
              nextTarget.Enqueue(enemyFound);
           }
        }

        /* Negate Collision of Like Agents */
        if(collision.collider.tag == "Tank")
           collision.collider.GetComponent<Rigidbody>().velocity = Vector3.zero;
    }
}