using UnityEngine;

[System.Serializable]
public class SoldierShooting {

    public float rateOfFire;
    public Transform shotSpawn;
    public GameObject shotObject;
    public AudioClip rifleAudio;
    public GameObject shotFlare;

    
    [Range(0, 100)]
    public int shootingSpeed;

}
