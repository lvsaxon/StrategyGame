using UnityEngine;
using System.Collections;

public class SeekTarget : MonoBehaviour {

	public Transform target;
    public LayerMask targetMask;
    public int speed;

    Collider s;

    void Start() {
        s = GetComponentInChildren<Collider>();
        print(s.transform);
    }

    void Update() {
        Ray ray = new Ray(transform.position, transform.forward);
        Debug.DrawRay(ray.origin, transform.forward*20, Color.red);

        transform.position = Vector3.MoveTowards(transform.position, transform.forward*5500, speed*Time.deltaTime);
        
    }
}
