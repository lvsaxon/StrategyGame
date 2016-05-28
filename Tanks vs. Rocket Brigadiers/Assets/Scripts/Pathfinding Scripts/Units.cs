using UnityEngine;
using System.Collections;


public class Units: MonoBehaviour {

    public bool haltPath;
    public bool showPathGizmo;

    [Range(0, 10)]
    public float movementSpeed;
    [Range(1, 5)]
    public float rotationSpeed;
    
    Vector3[] path;   
    int targetIndx, health;
    Vector3 closestTarget, currWaypoint;
    GameObject[] soldiers, tanks, targets;
    
    float dist;
    Vector3 previous;
    Animator animator;
    

    void Start() {
        
        if(gameObject.tag == "Tank"){
           soldiers = GameObject.FindGameObjectsWithTag("Rocket Brigadier");
           targets = soldiers;
        }

        if(gameObject.tag == "Rocket Brigadier"){
           animator = GetComponent<Animator>();
           tanks = GameObject.FindGameObjectsWithTag("Tank");
           animator.SetBool("IsSprinting", true);
           targets = tanks;
        }
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


    /* Begin Strafing Coroutine */
    public void StartStrafing(int count) {

        StartCoroutine("Strafing", count);        
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
