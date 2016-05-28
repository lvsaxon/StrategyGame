using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class SeekTarget : MonoBehaviour {

	public Transform target;
    public LayerMask targetMask;
    public int speed;

    List<Transform> path;
    Stack<Transform> s;

    void Start() {
        path = new List<Transform>();
        s = new Stack<Transform>();
        s.Push(target);
    }

    void Update() {
        Ray ray = new Ray(transform.position, transform.forward);
        Debug.DrawRay(ray.origin, transform.forward*20, Color.red);
        Vector3 closest = Vector3.one;

        /*if(Physics.Raycast(ray.origin, ray.direction*20, out hit, targetMask)) {

            if(hit.collider.tag == "Obstacle") {
               Transform o = hit.collider.transform;
               path = o.GetComponent<Pos>().pos.ToList();
               path.Add(target);
               ShortestPath(path.ToArray());
            }

        }
        closest = s.Peek().position;
        print(closest);
        if((int) Vector3.Distance(transform.position, closest) < 2)
           s.Pop();*/

        transform.position = Vector3.MoveTowards(transform.position, closest, speed*Time.deltaTime);
    }


    void ShortestPath(Transform[] path) {
        float shortestDistance = Mathf.Infinity;
        
        foreach(Transform p in path){
            float distance = Vector3.Distance(transform.position, p.position);

            if(shortestDistance > distance){
               shortestDistance = distance;
               s.Push(p);
            }
        }
    }
}
