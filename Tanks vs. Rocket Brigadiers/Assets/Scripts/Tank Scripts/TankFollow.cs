using UnityEngine;
using System.Collections;

public class TankFollow : MonoBehaviour {

    public float movementSpeed;
	GameObject soldier, waypoints;
    NavMeshPath currentPath;
    
    Vector3 currentVelocity;
    NavMeshAgent navAgent;
 
    void Start () {
	    soldier = GameObject.FindGameObjectWithTag("Rocket Brigadier");
        navAgent = GetComponent<NavMeshAgent>();
    }
	
	
    /* Stop Tank when Target is Visible else Resume following  */
    public void FollowTarget(bool isFollowing) {

        if(soldier != null){ 
            if(isFollowing){
               navAgent.SetDestination(soldier.transform.position);
           
            }else{ 
               navAgent.Stop();
               navAgent.ResetPath();
            }
        
        }else
            isFollowing = false;
    }
}
