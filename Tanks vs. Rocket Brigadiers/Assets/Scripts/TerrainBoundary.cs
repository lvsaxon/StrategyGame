using UnityEngine;

public class TerrainBoundary : MonoBehaviour {

    void OnTriggerExit(Collider collid) {
        Destroy(collid.gameObject);
    }
}
