using UnityEngine;
using System.Collections;

public class Look : MonoBehaviour {

    public float speed;

    int fullView = 90;
    Vector3 targetPos;
    bool look, rotate;

    void Start() {
        targetPos = new Vector3();
    }

	void Update () {
        
	    RaycastHit hit;
        Ray ray = new Ray(transform.position, transform.forward);
        Debug.DrawRay(ray.origin, transform.forward*20, Color.red);

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
           
        }
    }


    void OnTriggerStay(Collider col) {
        
        if(col.tag == "Tank") {
           
        }
    }

    void OnTriggerEnter(Collider col) {
        
        if(col.tag == "Tank"){
           
        }
    }
}




/*--------------------------------------------------SOUND CREATION && EXTRA CODE -------------------------------------------------------------*/

#region Agents/Leader Calling Followers or Help Sound Code
/*public float speed;

SphereCollider r;
public int soundCount;
float originalRad;
bool grow;

void Start() {
    r = GetComponent<SphereCollider>();
    grow = true;
    originalRad = r.radius;
    soundCount = 0;
}

void Update () {

    //Grow Collider
    if(grow)
       Grow();
    else 
        StartCoroutine("SoundWaves");

}


IEnumerator SoundWaves() {
    yield return null;

    r.radius = originalRad;
    grow = true;
}


void OnTriggerEnter(Collider col) {

    if(col.tag == "TagName"){
       grow = false;
       soundCount++;
    }
}


void Grow() {

    r.radius *= Time.deltaTime+speed;
}*/

#endregion


#region PowerUp Sound
    /*new AudioSource audio;

    void Start() {
        audio = GetComponent<AudioSource>();
        StartCoroutine("PlayClip", 4);
    }

    IEnumerator PlayClip(int count) {
       

        for(int i=0; i<count; i++) {
            
            audio.PlayOneShot(audio.clip, 1);
            yield return new WaitForSeconds(audio.clip.length);
        }
    }*/
#endregion
