using UnityEngine;
using System.Collections;

public class Look : MonoBehaviour {

    SphereCollider r;
    int fullView = 90, count = 0;
    Vector3 targetPos;
    bool look, grow, rotate;

    void Start() {
        targetPos = new Vector3();
        grow = true;
    }

	void Update () {
        
	    RaycastHit hit;
        Ray ray = new Ray(transform.position, transform.forward);
        Debug.DrawRay(ray.origin, transform.forward*20, Color.red);

        //Grow Collider
        if(grow)
           Grow();

        //Rotate Obj
        if(rotate && targetPos != new Vector3()) { 
            Vector3 targetPosition = targetPos;
            Quaternion targetRotation = Quaternion.LookRotation(targetPosition - transform.position, Vector3.up);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 3f);
        }


        float angle = Vector3.Angle(transform.position, targetPos);
        if(angle <= fullView/2){
            if(Physics.Raycast(ray.origin, ray.direction, out hit)) {
           
                if(hit.collider.tag == "Tank"){ 
                   rotate = false;
                
                }
            }
        }
	}


    void OnCollisionEnter(Collision col) {

        if(col.collider.tag == "Tank") {
           //look = true;
        }
    }


    void OnTriggerStay(Collider col) {
        
        if(col.tag == "Tank") {
           targetPos = col.transform.position;
           rotate = true;
           print("Total: "+count);
        }
        
    }

    void OnTriggerEnter(Collider col) {
        
        if(col.tag == "Tank"){
            Physics.IgnoreCollision(GetComponent<SphereCollider>(), col);
            grow = false;
           
        }
    }


    void Grow() {

            r = GetComponent<SphereCollider>();
            r.radius *= Time.deltaTime+1f;
        
        
    }
}
