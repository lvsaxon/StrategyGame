using UnityEngine;
using System.Collections;

public class Soldier_FieldOfView : MonoBehaviour {

	public float distance;
    GameObject tank;

    void Start() {

        tank = GameObject.FindGameObjectWithTag("Tank");
    }

    void Update () {
	    Ray ray = new Ray(transform.position, Vector3.forward);
        Debug.DrawRay(ray.origin, transform.forward*distance, Color.white);

        transform.LookAt(tank.transform.position);
	}
}
