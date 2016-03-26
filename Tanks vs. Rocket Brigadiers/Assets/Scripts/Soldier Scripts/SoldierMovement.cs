using UnityEngine;
using System.Collections;

public class SoldierMovement : MonoBehaviour {

    public float speed = 5;
   
    void Update () {
        //transform.position = Vector3.Slerp(transform.position, v[i], 0.05f* Time.deltaTime);
        //Vector3.MoveTowards(transform.position, new Vector3(transform.position.x, 5f, 480f), speed*Time.deltaTime);

        transform.Translate(Mathf.Cos(Time.time/2) * speed * Time.deltaTime, 0, 0);
	}
}
