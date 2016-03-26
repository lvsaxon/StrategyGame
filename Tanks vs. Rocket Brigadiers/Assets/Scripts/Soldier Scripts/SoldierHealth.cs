using UnityEngine;
using System.Collections;


public class SoldierHealth : MonoBehaviour {

    public int startingHealth = 100;
    public int currentHealth;
    public int scoreValue;

    [Range(0f, 20f)]
    public float sinkingRate;

    Animator animator;
    GameObject gameManager;
    new Rigidbody rigidbody;
    bool isDead, startSinking;       
    RespawnManager respawnManager;
    

    void Start() {
        currentHealth = startingHealth;
        animator = GetComponent<Animator>();
        rigidbody = GetComponent<Rigidbody>();
        gameManager = GameObject.FindGameObjectWithTag("Game Manager");
        respawnManager = gameManager.GetComponent<RespawnManager>();
    }


    void Update() {

        /*  Health Conditions
            curHeath < 30
            *find Powerup
            *call Help
            *dodge
            
            currHeath < 10
            *run away
            *call Help 
        */

        if(startSinking){
           Physics.gravity = -Vector3.up * sinkingRate*Time.deltaTime;           
        }
    }


    /* Damage Taken from Tanks */
    public void TakingDamage(int damage) {

        if(isDead)
           return;
              
        currentHealth -= damage;       
        if(currentHealth <= 0){
           Dead();
           StartCoroutine("Restore");   //Reset Object back to Defaults: Health, Collider, etc..
        }
    }


    /* Soldier Is Dead */
    void Dead() {
        isDead = true;       
        TankScore.TScore += scoreValue;
        GetComponent<Collider>().isTrigger = true;
        rigidbody.constraints = ~RigidbodyConstraints.FreezePositionY;
        
        int random = Random.Range(0, 100);
        if(random < 50){ 
           animator.SetTrigger("IsDeadFront");
        }else{ 
           animator.SetTrigger("IsDeadBack");
        }

        startSinking = true;
    }


    /* Duration to Start Sinking */
    IEnumerator StartSinking() {
        yield return null;
        startSinking = true;
    }


    /* Restore Soldier back To Regular Defaults */
    IEnumerator Restore() {
        yield return new WaitForSeconds(5);
        
        isDead = false;
        startSinking = false;
        animator.SetTrigger("IsRevived");
        currentHealth = startingHealth;
        transform.position = respawnManager.RespawnSoldier();

        GetComponent<Units>().StopPath(false);
        GetComponent<Collider>().isTrigger = false;
        rigidbody.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;

        yield return new WaitForSeconds(5);
    }
}
