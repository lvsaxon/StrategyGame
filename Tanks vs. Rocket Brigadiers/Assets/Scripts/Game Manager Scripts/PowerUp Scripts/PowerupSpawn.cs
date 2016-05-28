using UnityEngine;
using System.Collections;

public class PowerupSpawn : MonoBehaviour {

    public GameObject[] powerUps;
    public float startIn = 5;
    public float spawnTimer;
    
    bool spawn;
    Timer timer;
    int countTimer;
    Transform powerObj;
    PowerUpLocationManager powerUpLocation;
    

    void Start() {
        spawn = true;
        StartCoroutine("Spawning");

        timer = GameObject.FindGameObjectWithTag("Game Manager").GetComponent<Timer>();
        powerUpLocation = GameObject.FindGameObjectWithTag("Game Manager").GetComponent<PowerUpLocationManager>();
    }


    void Update() {
        spawnTimer += Time.deltaTime;
        countTimer = (int) spawnTimer;

        if(!spawn && !powerObj){
           spawn = true;
           StartCoroutine("Spawning");
        }
    }


    /* Respawn PowerUp onPickUp */
	IEnumerator RespawnPowerUp(){
        float respawnDuration = Random.Range(7.5f, 15f);
        yield return new WaitForSeconds(respawnDuration);

        StartCoroutine("Spawning");
    }


    /* Spawn PowerUps */
    IEnumerator Spawning(){
        yield return new WaitForSeconds(startIn);

        if(spawn){
            int randIndx = Random.Range(0, powerUps.Length);
            GameObject power = Instantiate(powerUps[randIndx], transform.position, Quaternion.identity) as GameObject;
            powerUpLocation.AddObject(power);
            spawn = false;
        }

        if(timer.TimeLeft <= 0) {
           yield break;
        }
    }


    /* Detect Spawned PowerUp Objects */
    void OnTriggerStay(Collider collid) {

        if(collid.tag == "Health" || collid.tag == "Incres Power" || collid.tag == "Shield")
           powerObj = collid.transform; 
    }
}

