using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class RespawnManager: MonoBehaviour {

    public float startTime, spawnTime;
    public Transform[] tankRespawnPoints, soldierRespawnPoints;

    Vector3 spawnPoint = new Vector3();
    TankSpawn_CheckPoint tankSpawnCheck;
    SoldierSpawn_CheckPoint soldierSpawnCheck;
    

    /* Respawn Tank Object Within a given Duration */
    public Vector3 RespawnTank() {

        StartCoroutine("SafeTank_RespawnPoints");
        return spawnPoint;
    }


    /* Respawn Soldier at This Position */
    public Vector3 RespawnSoldier() {
        
        StartCoroutine("SafeSoldier_RespawnPoints");
        return spawnPoint;
    }


    /* Loop through Soldier SpawnPoints Until its Safe */
    IEnumerator SafeSoldier_RespawnPoints() {
        float[] offsetX = {-5f, -4.5f, -3.5f, -2, 2.2f, 3.7f, 4.6f, 5.3f};
        float[] offsetZ = offsetX;

        while(true){
            int index = Random.Range(0, 3);
            soldierSpawnCheck = soldierRespawnPoints[index].GetComponent<SoldierSpawn_CheckPoint>();
            spawnPoint = soldierSpawnCheck.RespawnSoldier();
            
            //Exit Loop if != Vector.Zero
            if(spawnPoint != Vector3.zero){
               index = Random.Range(0, 8);
               spawnPoint = new Vector3(spawnPoint.x +  offsetX[index], spawnPoint.y, spawnPoint.z + offsetZ[index]);
               yield break;
            }
        }
    }


    /* Loop through Tank SpawnPoints Until its Safe */
    IEnumerator SafeTank_RespawnPoints() {
        float[] offsetX = {-5f, -4.5f, -3.5f, -2, 2.2f, 3.7f, 4.6f, 5.3f};
        float[] offsetZ = offsetX;

        while(true){
            int index = Random.Range(0, 3);
            tankSpawnCheck = tankRespawnPoints[index].GetComponent<TankSpawn_CheckPoint>();
            spawnPoint = tankSpawnCheck.RespawnTank();
            
            //Exit Loop if != Vector.Zero
            if(spawnPoint != Vector3.zero){
               index = Random.Range(0, 8);
               spawnPoint = new Vector3(spawnPoint.x +  offsetX[index], spawnPoint.y, spawnPoint.z + offsetZ[index]);
               yield break;
            }
        }
    }
}
