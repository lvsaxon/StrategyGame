using UnityEngine;
using System.Collections;

public class TankFollowPath : MonoBehaviour {

    public float movementSpeed;
    public Transform target;
	GameObject gameManager;
    
    /*NavigationGridMap navGrid;
    NavMeshAgent navAgent;
    TargetFinder targetLocated;*/
    new Rigidbody rigidbody;
 
    void Awake(){
        //navGrid = gameManager.GetComponent<NavigationGridMap>();
        //targetLocated = GetComponent<TargetFinder>();

	    //soldier = respawnManager.soldier;
        rigidbody = GetComponent<Rigidbody>();
        //navAgent = GetComponent<NavMeshAgent>();
    }

    
    /* Stop Tank when Target is Visible else Resume following  */
    public void FollowTarget(bool isFollowing) {

        if(target != null){ 
            if(isFollowing){
               //targetLocated.Path();
               rigidbody.velocity = (target.position - transform.position) * Time.deltaTime*movementSpeed;
               //transform.position = Vector3.Slerp(transform.position, target.position, movementSpeed*Time.deltaTime);
               //navAgent.SetDestination(target.position);
               
            }else{
                rigidbody.velocity = Vector3.zero;
               //Slow Down & Stop
               /* navAgent.Stop();
               navAgent.ResetPath();*/
            }
        
        }else
            isFollowing = false;
    }
}
