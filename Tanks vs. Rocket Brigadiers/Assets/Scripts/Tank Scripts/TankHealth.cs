using UnityEngine;
using UnityEngine.UI;
using System.Collections;


public class TankHealth : MonoBehaviour {

    public int startingHealth = 100;
    public int currentHealth;
    public int scoreValue;
    public HealthUI healthUI;
    public GameObject deathExplosion;

    bool isDead;
    Timer timer;
    Camera mainCamera;
    Quaternion originRotation;
    RespawnManager respawnManager;
       
    bool fillShieldUI;
    Canvas healthCanvas;    
    Image healthImg, shieldImage;
    float startingShield, currShield;


    void Start() {
        currentHealth = startingHealth;
        originRotation = transform.rotation;

        //Shield Amount
        startingShield = startingHealth * 0.75f;
        currShield = startingShield;

        //Health UI Components
        healthImg = healthUI.healthUI;
        shieldImage = healthUI.shieldUI;
        healthCanvas = healthUI.healthCanvas;   

        //GameManager Components
        mainCamera = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
        timer = GameObject.FindGameObjectWithTag("Game Manager").GetComponent<Timer>();
        respawnManager = GameObject.FindGameObjectWithTag("Game Manager").GetComponent<RespawnManager>();
    }


    void Update() {
        healthCanvas.transform.LookAt(mainCamera.transform.position); 

        //Gradually Fill Shield UI
        if(fillShieldUI && shieldImage.fillAmount != 1){
           shieldImage.fillAmount = Mathf.MoveTowards(shieldImage.fillAmount, 1, Time.deltaTime*1.5f);
           
           if(shieldImage.fillAmount == 1)
              fillShieldUI = false;
        }  

        if(timer.TimeLeft <= 0)
           StopCoroutine("Restore");
    }


    /* Damage Taken From Enemies */
    public void TakingDamage(int damage) {     

        if(isDead)
           return;

        if(!shieldImage.enabled)      
           currentHealth -= damage;
        else
           currShield -= damage; 

        HealthUI(currentHealth);
        if(currentHealth <= 0){
           Dead();
           StartCoroutine("Restore");   //Reset Object back to Defaults: health, Collider, etc..
        }
    }


    /* Health PowerUp; Replenish Health */
    public void ReplenishHealth(float replenishAmount) {

        if(currentHealth <= 0)
           return;

        currentHealth += (int) (startingHealth * replenishAmount);
        HealthUI(currentHealth);
        if(currentHealth > startingHealth)
           currentHealth = startingHealth;
    }


    /* Shield PowerUp; Enable Shield */
    public void EnableShield(bool enableShield) {

        shieldImage.enabled = enableShield;
        fillShieldUI = true;
        currShield = startingShield;
    }


    /* Health & Shield UI */
    void HealthUI(int currentHealth) {

        //Shield Not Enabled; Affect Health Only
        if(!shieldImage.enabled){ 
            healthImg.fillAmount = currentHealth / (float)startingHealth;
            if(healthImg.fillAmount <= 0.5f){
                healthImg.color = Color.Lerp(Color.green , Color.yellow, currentHealth);
            
                if(healthImg.fillAmount <= 0.25f)
                    healthImg.color = Color.Lerp(Color.yellow , Color.red, currentHealth);
            }

        //Shield Is Enabled
        }else{
            shieldImage.fillAmount = currShield / startingShield;
            if(shieldImage.fillAmount == 0)
               shieldImage.enabled = false;
        } 
    }


    /* Tank Is Dead */
    void Dead() {
        isDead = true;
        healthCanvas.enabled = false;
        GetComponent<TDEnemy>().setStopPath(true);
        GetComponent<Collider>().isTrigger = true;
        Instantiate(deathExplosion, transform.position, Quaternion.identity);

        if(timer.TimeLeft > 0)
           SoldierScore.SScore += scoreValue;
    }


    /* Restore Soldier back To Regular Defaults */
    IEnumerator Restore() {
        yield return new WaitForSeconds(3);

        isDead = false;
        healthImg.fillAmount = 1;
        healthCanvas.enabled = true;
        healthImg.color = Color.green;
        currentHealth = startingHealth;
        healthCanvas.transform.LookAt(mainCamera.transform.position); 

        transform.rotation = originRotation;
        transform.position = respawnManager.RespawnTank();
        GetComponent<TDEnemy>().setStopPath(false);
        GetComponent<Collider>().isTrigger = false; 

        shieldImage.fillAmount = 1;
        shieldImage.color = Color.blue;
        GetComponent<TDEnemy>().enabled = true;

        yield return new WaitForSeconds(5);
    }
}
