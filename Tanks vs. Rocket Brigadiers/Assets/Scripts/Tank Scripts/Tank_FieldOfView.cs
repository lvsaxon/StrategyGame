using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Tank_FieldOfView : MonoBehaviour {

    public float distance;
    GameObject soldier;
    NavMeshAgent navAgent;

    float nextFire;
    float fullAngleView = 150f;
    Stack<Vector3> stack;
    
    TankShooting tankShooting;
    TankFollow tankFollow;

    void Awake() {
        tankShooting = GetComponent<TankShooting>();

        stack = new Stack<Vector3>();
        soldier = GameObject.FindGameObjectWithTag("Rocket Brigadier");
        tankFollow = GetComponent<TankFollow>();

        navAgent = GetComponent<NavMeshAgent>();
    }
    

    void Update() {
        RaycastHit hit;
        Ray ray = new Ray(transform.position, transform.forward);
        Debug.DrawRay(ray.origin, transform.forward*distance, Color.cyan);

        tankFollow.FollowTarget(true);
    
        /*--Error Check if Obj Exists--*/
        if(soldier != null) { 
            Vector3 direction = soldier.transform.position - transform.position;
            float angle = Vector3.Angle(direction, transform.forward);

            //Check if target is <= to 1/2 of Field View Angle
            if (angle <= (fullAngleView/2f)) {
            
                if (Physics.Raycast(ray.origin, direction.normalized, out hit, distance)) {
                
                    //Check if Obstacle is hit
                    if (hit.collider.tag == "Obstacle") {
                        TargetLocated(false);
                        StartShooting(false);

                        if (stack.Count != 0) {  
                            tankFollow.FollowTarget(true);
                        }
                    }
                
                    //Check if target is Found
                    if (hit.collider.tag == "Rocket Brigadier") {
                        TargetLocated(true);
                        tankFollow.FollowTarget(false);
                        UpdateLocation(hit.collider.gameObject.transform.position);

                        //Start shooting when < 1/6 Field View
                        if(angle < (fullAngleView/4f) && navAgent.velocity == Vector3.zero)
                           StartShooting(true);
                    }
                }
            }
        }else
            print("Enemy Terminated");
    }

    
    /* Target Has been Located */
    void TargetLocated(bool isTargetLocated) {

        //Look At Target
        if (isTargetLocated) {
            Vector3 targetPosition = soldier.transform.position;
            Quaternion targetRotation = Quaternion.LookRotation(targetPosition - transform.position, Vector3.up);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 5f);
        }
    }


    /* Start Shooting at Target */
    void StartShooting(bool startShooting) {

        //Shoot at Target
        if (startShooting) {
            if (Time.time > nextFire) { 
                nextFire = Time.time + tankShooting.rateOfFire;
                Instantiate(tankShooting.shotObject, tankShooting.shotSpawn.position, tankShooting.shotSpawn.rotation);
            }
        }
    }


    /* Get Target's most Recent Location */
    void UpdateLocation(Vector3 position) {
        
         stack.Push(position);
    }
}
