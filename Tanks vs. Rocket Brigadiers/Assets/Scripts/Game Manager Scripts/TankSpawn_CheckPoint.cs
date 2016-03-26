using UnityEngine;
using System.Collections;

public class TankSpawn_CheckPoint : MonoBehaviour {

    public bool respawn;
    public float respawnDuration;
    Vector3 spawnPoint;

    void Start() {
        respawn = true;
        spawnPoint = new Vector3();
    }


    void Update() {
        
        if(respawn)
           spawnPoint = transform.position;
        else{ 
           spawnPoint = Vector3.zero;
        }
    }


    /* Disable & Enable Respawn after Few Seconds */
    IEnumerator RespawnDuration(float duration) {
        
        respawn = false;
        yield return new WaitForSeconds(duration);
        respawn = true;
    }


    /* Spawn Tank when respawn is True */
    public Vector3 RespawnTank() {
        
        return spawnPoint;
    }
    

    /* Check for Any Targets or Shots within the Collider */
    void OnTriggerEnter(Collider collid) {

        if(collid.tag == "Rocket" || collid.tag == "Rocket Brigadier"){
           StopCoroutine("RespawnDuration");
           StartCoroutine("RespawnDuration", respawnDuration);
        }
    }
}
