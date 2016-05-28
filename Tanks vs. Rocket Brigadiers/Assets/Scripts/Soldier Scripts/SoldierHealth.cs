using UnityEngine;
using UnityEngine.UI;
using System.Collections;


public class SoldierHealth : MonoBehaviour {

    public int startingHealth = 100;
    public int currentHealth;
    public int scoreValue;
    
    [Range(0f, 20f)]
    public float sinkingRate;
    public HealthUI healthUI;

    Animator animator;
    GameObject gameManager;
    new Rigidbody rigidbody;
    bool isDead, startSinking;       
    RespawnManager respawnManager;
    
    Timer timer;
    Camera mainCamera;
    GameObject[] soldiers;
    Quaternion originRotation;
    
    bool fillShieldUI;
    Canvas healthCanvas;
    Image healthImage, shieldImage;
    float startingShield, currShield;


    void Start() {
        currentHealth = startingHealth;
        originRotation = transform.rotation;
        animator = GetComponent<Animator>();
        rigidbody = GetComponent<Rigidbody>();

        //Shield Amount
        startingShield = startingHealth * 0.75f;
        currShield = startingShield;

        //Health UI Components
        healthImage = healthUI.healthUI;
        shieldImage = healthUI.shieldUI;
        healthCanvas = healthUI.healthCanvas;
        mainCamera = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();

        //GameManager Components
        gameManager = GameObject.FindGameObjectWithTag("Game Manager");
        timer = gameManager.GetComponent<Timer>();
        respawnManager = gameManager.GetComponent<RespawnManager>();        
    }


    void Update() {
        healthCanvas.transform.LookAt(mainCamera.transform.position); 

        //Gradually Fill Shield UI
        if(fillShieldUI && shieldImage.fillAmount != 1){
           shieldImage.fillAmount = Mathf.MoveTowards(shieldImage.fillAmount, 1, Time.deltaTime*1.5f);
           
           if(shieldImage.fillAmount == 1)
              fillShieldUI = false;
        }

        /*  Health Conditions
            curHeath < 30
            *find Powerup
            *call Help
            *dodge
            
            currHeath < 10
            *run away
            *call Help   */

        if(timer.TimeLeft <= 0)
           StopCoroutine("Restore");

        if(startSinking){
           Physics.gravity = -Vector3.up * sinkingRate*Time.deltaTime;           
        }
    }


    /* Damage Taken from Enemies */
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
           StartCoroutine("Restore");   //Reset Object back to Defaults: Health, Collider, etc..
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
            healthImage.fillAmount = currentHealth / (float)startingHealth;

            if(healthImage.fillAmount <= 0.5f){
                healthImage.color = Color.Lerp(Color.green , Color.yellow, currentHealth);
            
                if(healthImage.fillAmount <= 0.25f)
                    healthImage.color = Color.Lerp(Color.yellow , Color.red, currentHealth);
            }

        //Shield Is Enabled
        }else{
            shieldImage.fillAmount = currShield / startingShield;
            if(shieldImage.fillAmount == 0)
               shieldImage.enabled = false;
        }                 
    }


    /* Soldier Is Dead */
    void Dead() {
        isDead = true;
        healthCanvas.enabled = false;
        rigidbody.isKinematic = false;
        GetComponent<TDEnemy>().setStopPath(true);
        GetComponent<Collider>().isTrigger = true;
        GetComponentInChildren<SphereCollider>().isTrigger = true;

        if(timer.TimeLeft > 0)       
           TankScore.TScore += scoreValue;

        rigidbody.velocity = Vector3.zero;
        rigidbody.constraints = ~RigidbodyConstraints.FreezePositionY;

        int random = Random.Range(0, 100);
        if(random < 50)
           animator.SetTrigger("IsDeadFront");
        else 
           animator.SetTrigger("IsDeadBack");
        
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
        healthImage.fillAmount = 1;
        healthImage.color = Color.green;
        animator.SetTrigger("IsRevived");

        currentHealth = startingHealth;
        GetComponent<TDEnemy>().setStopPath(false);
        GetComponent<Collider>().isTrigger = false;
        transform.rotation = originRotation;
        transform.position = respawnManager.RespawnSoldier();
        
        healthCanvas.enabled = true;
        shieldImage.fillAmount = 0;
        rigidbody.isKinematic = true;
        GetComponent<TDEnemy>().enabled = true;        
        GetComponentInChildren<SphereCollider>().isTrigger = false;

        rigidbody.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;

        yield return new WaitForSeconds(5);
    }
}
