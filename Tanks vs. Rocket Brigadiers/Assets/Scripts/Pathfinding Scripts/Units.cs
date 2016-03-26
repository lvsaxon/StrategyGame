using UnityEngine;
using System.Linq;
using System.Collections;
using System.Collections.Generic;


public class Units: MonoBehaviour {

    public bool haltPath;
    public bool showPathGizmo;

    [Range(0, 10)]
    public float movementSpeed;
    [Range(1, 5)]
    public float rotationSpeed;

    Vector3[] path;   
    Vector3 closestTarget;    
    int targetIndx, health;
    GameObject[] soldiers, tanks, targets;
    GameObject gameManager;

    Animator animator;
    NavigationGridMap gridMap;
    

    void Start() {

        if(gameObject.tag == "Tank"){
           soldiers = GameObject.FindGameObjectsWithTag("Rocket Brigadier");
           targets = soldiers;
        }

        if(gameObject.tag == "Rocket Brigadier"){
           tanks = GameObject.FindGameObjectsWithTag("Tank");
           targets = tanks;
        }

        gameManager = GameObject.FindGameObjectWithTag("Game Manager");
        gridMap = gameManager.GetComponent<NavigationGridMap>();
        animator = GetComponent<Animator>();
    }


    void Update() {
        
        if(targets != null){ 
           PathRequestManager.RequestPath(transform.position, ClosestTarget(), WhenPathIsFound);
        }
    }


    /* When Path has been successfully Found */
    public void WhenPathIsFound(Vector3[] newPath, bool pathSuccessful) {

        if(pathSuccessful){
           path = newPath;
           StopCoroutine("FollowPath");
           StartCoroutine("FollowPath");
        }
    }


    /* Follow the Path thats Specified */
    IEnumerator FollowPath() {
        Vector3 currWaypoint = path[0];

        if(gameObject.tag == "Tank"){
           health = GetComponent<TankHealth>().currentHealth;
        }

        if(gameObject.tag == "Rocket Brigadier"){
           health = GetComponent<SoldierHealth>().currentHealth;
        }

        while(true && !haltPath && health > 0){
            if(transform.position == currWaypoint){
               targetIndx++;  //Advance to next wayPoint
               
               if(targetIndx >= path.Length) 
                  yield break;

               currWaypoint = path[targetIndx];
            }

            if(animator)
               animator.SetBool("IsSprinting", true);

            Vector3 targetPosition = currWaypoint;
            Quaternion targetRotation = Quaternion.LookRotation(targetPosition - transform.position, Vector3.up);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);            
            transform.position = Vector3.MoveTowards(transform.position, currWaypoint, movementSpeed * Time.deltaTime);
                      
            yield return null;
        }
    }


    /* Look At Soldier Nearest to You */
    Vector3 ClosestTarget() {
        float shortestDist = Mathf.Infinity;
        Vector3 closestTarget = new Vector3();

        foreach(GameObject target in targets){
            if(target){
               float distance = Vector3.Distance(transform.position, target.transform.position);

               if(shortestDist > distance){
                  shortestDist = distance;
                  closestTarget = target.transform.position;
               }
            }
        }

        return closestTarget;
    }


    /* Set the State of Stopping the Path */
    public void StopPath(bool _stop) {
        haltPath = _stop;
    }


    /* Determine if Target is inBounds */
    bool isTargetInBounds(Transform soldier) {

        if((soldier.position.x >= 1 && soldier.position.x < gridMap.gridSize.x) && 
           (soldier.position.z >= 1 && soldier.position.z < gridMap.gridSize.y))
            return true;
        else
            return false;
    }


    /* Display Complete Path */
    public void OnDrawGizmos() {

        if(path != null && showPathGizmo) {
            for(int i=targetIndx; i<path.Length; i++) {
                Gizmos.color = Color.black;
                Gizmos.DrawCube(path[i], Vector3.one);

                if(i == targetIndx) 
                    Gizmos.DrawLine(transform.position, path[i]);
                else
                    Gizmos.DrawLine(path[i-1], path[i]);
            }
        }
    }
}
