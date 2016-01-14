using UnityEngine;
using System.Collections;

public class SpawnChecking : MonoBehaviour {

    bool respawn = true;

    /* Spawn Tank when respawn is True */
    public Vector3 RespawnTank() {
        Vector3 spawnPoint = new Vector3();

        if(respawn)
           spawnPoint = transform.position;
        
        return spawnPoint;
    }
    
    /* Check for Any Targets or Shots within the Collider */
    void OnTriggerStay(Collider collid) {
        
        if(collid.tag == "Shot" || collid.tag == "Rocket Brigadier")
           respawn = false;
        else
           respawn = true;
    }
}
