using UnityEngine;
using System.Collections;

public class SoldierMovement : MonoBehaviour {

    public float speed;
    

    void Update () {
	    
        transform.position = Vector3.Lerp(transform.position, new Vector3(transform.position.x, 5f, 480f), speed*Time.deltaTime);
	}
}
