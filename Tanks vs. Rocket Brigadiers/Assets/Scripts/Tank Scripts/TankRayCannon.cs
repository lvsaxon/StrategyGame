using UnityEngine;

public class TankRayCannon: MonoBehaviour {

    public int shootingDamage;
    public GameObject explosion;
    public AudioClip explosionAudio;

    int defaultDmg;
    float beamLength;
    Transform parentObj;
    new AudioSource audio;
    SoldierHealth soldierHealth;


    void Awake() {
        defaultDmg = shootingDamage;
        beamLength = GetComponent<BeamParam>().MaxLength;
        GetComponent<CapsuleCollider>().height = beamLength;
    }


    /* Set Owner/Parent for This Object */
    public void setParentObject(Transform _parent) {

        parentObj = _parent;
    }


    /* Return the OriginPos That it Was Fired From */
    public Transform getParentObject() {
        
        return parentObj;
    }


    /* Increase Damage Output */
    public void IncreaseDamage(int _incresDmg) {

        shootingDamage += _incresDmg;
    }


    /* Set Shooting Damage back to Regular Defaults */
    public void DefaultDamage() {

        shootingDamage = defaultDmg;
    }


    /* Shooting Damage */
    void OnTriggerEnter(Collider collid) {
        
        if(collid.tag == "Rocket Brigadier"){
           soldierHealth = collid.GetComponent<SoldierHealth>();
           GameObject explosive = Instantiate(explosion, collid.transform.position, Quaternion.identity) as GameObject;
           audio = explosive.GetComponent<AudioSource>();   
           audio.PlayOneShot(explosionAudio, 0.5f);

           if(soldierHealth != null) { 
              soldierHealth.TakingDamage(shootingDamage);
           }
        }

        Destroy(gameObject, 1f);
    }
}