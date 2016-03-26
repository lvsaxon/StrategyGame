using UnityEngine;
using System.Collections;

public class TankHealth : MonoBehaviour {

    public int startingHealth = 100;
    public int currentHealth;
    public int scoreValue;

    GameObject gameManager;
    RespawnManager respawnManager;

    void Start() {
        currentHealth = startingHealth;

        gameManager = GameObject.FindGameObjectWithTag("Game Manager");
        respawnManager = gameManager.GetComponent<RespawnManager>();
    }


    /* Damage Taken from Tanks */
    public void TakingDamage(int damage) {     
        currentHealth -= damage;
        
        if(currentHealth <= 0){
           RocketScore.SScore += scoreValue;
           GetComponent<Collider>().isTrigger = true;
           
           if(gameObject.name == "tank leader"){             
              GetComponent<TankCoordinator>().setCallingFollowers(false);
              GetComponent<TankCoordinator>().setSquadronFormed(false);
           }

           StartCoroutine("Restore");   //Reset Object back to Defaults: health, Collider, etc..
        }
    }


    /* Restore Soldier back To Regular Defaults */
    IEnumerator Restore() {
        yield return new WaitForSeconds(3);

        currentHealth = startingHealth;   
        transform.position = respawnManager.RespawnTank();
        GetComponent<Units>().StopPath(false);
        GetComponent<Collider>().isTrigger = false; 

        yield return new WaitForSeconds(5);
    }
}
