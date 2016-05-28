using UnityEngine;
using System.Collections;

public class TDEnemy: Pathfinding{

    [Range(0, 100)]
    public float movementSpeed;
    [Range(0, 10)]
    public float rotationSpeed;
    public bool haltPath;

    int health;
    Animator animator;
    bool newPath = true;
    Transform chosenTarget;
    GameObject[] targets;
	
    Vector3 assignedPath;


    void Awake() {
        
        if(gameObject.tag == "Tank") {
           targets = GameObject.FindGameObjectsWithTag("Rocket Brigadier");
        }

        if(gameObject.tag == "Rocket Brigadier"){ 
           animator = GetComponent<Animator>();
           targets = GameObject.FindGameObjectsWithTag("Tank");
        }
    }


	void Update (){
        
        if(gameObject.tag == "Rocket Brigadier"){ 
           health = GetComponent<SoldierHealth>().currentHealth;
        }

        if(gameObject.tag == "Tank"){
           health = GetComponent<TankHealth>().currentHealth;
        }

        if(ClosestTarget() != Vector3.zero && newPath){
           StartCoroutine(PathTimer());
        }

        if(!haltPath && health > 0)
           FollowPath();
    }


    /* Find the Shortest Path */
    IEnumerator PathTimer(){
        newPath = false;
        
        if(assignedPath != Vector3.zero)
           FindPath(transform.position, assignedPath);
        else      
           FindPath(transform.position, ClosestTarget());
        
        yield return new WaitForSeconds(0);
        newPath = true;
    }


    /* Move Agent Along the Path */
    private void FollowPath(){

         if(Path.Count > 0){
            //Remove Next Waypoint When < 5f units Away
            if(Vector3.Distance(transform.position, new Vector3(Path[0].x, transform.position.y, Path[0].z)) < 5f){
               Path.RemoveAt(0);
            }
                
            if(Path.Count > 0){            
               Vector3 targetPosition = new Vector3(Path[0].x, transform.position.y, Path[0].z);
               Quaternion targetRotation = Quaternion.LookRotation(targetPosition - transform.position, Vector3.up);
               transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * rotationSpeed);   
               transform.position = Vector3.MoveTowards(transform.position, targetPosition, Time.deltaTime * movementSpeed);                
            }
        }
    }


    IEnumerator PathRemoval(float speed){
        yield return new WaitForSeconds((1 * Pathfinder.Instance.Tilesize) / speed);
        if(Path.Count > 0) {
           Path.RemoveAt(0);
        }
    }


    /* Look At Target Nearest to You */
    Vector3 ClosestTarget() {
        float shortestDistance = Mathf.Infinity;
        Vector3 closestTarget = new Vector3();

        foreach(GameObject target in targets){
            if(target){
               float distance = Vector3.Distance(transform.position, target.transform.position);

               if(shortestDistance > distance){
                  shortestDistance = distance;
                  closestTarget = target.transform.position;
               }
            }
        }

        return closestTarget;
    }


    /* Set New Assigned Path */
    public void setAssignedPath(Vector3 _newPath) {
        StopCoroutine("PathTimer");
        assignedPath = _newPath;
    }


    /* Set the State of Stopping the Path */
    public void setStopPath(bool _stop) {

        haltPath = _stop;
    }
}
