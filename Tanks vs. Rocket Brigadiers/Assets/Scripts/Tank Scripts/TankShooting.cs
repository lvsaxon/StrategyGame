using UnityEngine;

[System.Serializable]
public class TankShooting {

	public float rateOfFire;
    public Transform shotSpawn;
    public GameObject shotObject;

    [Range(0, 20)]
    public int shootingSpeed;
}
