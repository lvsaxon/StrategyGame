using UnityEngine;
using System.Collections;

public class DestroyObjects : MonoBehaviour {

    
    void OnTriggerExit(Collider collid) {
        Destroy(collid.gameObject);
    }
}
