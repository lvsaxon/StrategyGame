using UnityEngine;
using System.Collections;

public class RespawnHandler : MonoBehaviour {

    public float startTime, spawnTime;
    public GameObject tank;
    public GameObject soldier;
    public Transform[] tankRespawns;
    public Transform[] soldierRespawns;

    GameObject spawnPoints;
    SpawnChecking spawnCheck;

    void Start() {
        
        spawnPoints = GameObject.FindGameObjectWithTag("Spawn");
        spawnCheck = spawnPoints.GetComponentInChildren<SpawnChecking>();

        StartCoroutine(RespawnDuration());
    }


    /*What gameObj to Spawn & how many*/


    /* Respawn Object Within a given Duration */
    IEnumerator RespawnDuration() {
        yield return new WaitForSeconds(startTime);

        if (soldier == null) { 
            //for(int i=0; i<3; i++) {
                if(spawnCheck.RespawnTank() == tankRespawns[0].position) {
                    Instantiate(soldier, tankRespawns[0].position, Quaternion.identity);
                }
            //}
        yield return new WaitForSeconds(spawnTime);
        }
    }
}
