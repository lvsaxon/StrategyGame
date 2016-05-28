using UnityEngine;
using System.Collections;


public class SpawnBarrier : MonoBehaviour {

    public GameObject[] powerUps;
    public Vector2 spawnSize;
    public float startIn = 5;
    public float spawnTimer;

    Timer timer;
    int countTimer;
    Vector3 spawnPoint, lastSpawnPos;
	PowerUpLocationManager powerUpLocation;


    void Start() {      
        timer = GameObject.FindGameObjectWithTag("Game Manager").GetComponent<Timer>();
        powerUpLocation = GameObject.FindGameObjectWithTag("Game Manager").GetComponent<PowerUpLocationManager>();

        StartCoroutine("AirSpawning");  
    }


    void Update() {
        spawnTimer += Time.deltaTime;
        countTimer = (int) spawnTimer;

        if(countTimer%startIn == 0)
           spawnPoint = new Vector3(transform.position.x + Random.Range(-spawnSize.x/2, spawnSize.x/2), transform.position.y, transform.position.z + Random.Range(-spawnSize.y/2, spawnSize.y/2));        
    }


    /* Air Spawn PowerUps */
    IEnumerator AirSpawning(){
        yield return new WaitForSeconds(startIn);

        while(true && (lastSpawnPos != spawnPoint)){
            int randIndx = Random.Range(0, powerUps.Length);
            GameObject power = Instantiate(powerUps[randIndx], spawnPoint, Quaternion.identity) as GameObject;
            lastSpawnPos = power.transform.position;
            powerUpLocation.AddObject(power);
            
            if(timer.TimeLeft <= 0) {
               yield break;
            }

            yield return new WaitForSeconds(startIn);
        }
    }

    
    void OnDrawGizmos(){
        Gizmos.DrawWireCube(transform.position, new Vector3(spawnSize.x, 1f, spawnSize.y));
    }
}
