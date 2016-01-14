using UnityEngine;
using System.Collections;

public class DetectObstacle : MonoBehaviour {

    Rigidbody rigidbody;

    void Start() {
        rigidbody = GetComponent<Rigidbody>();
    }

    void Update() {
        
        if(rigidbody.IsSleeping())
           rigidbody.WakeUp();
    }
}
