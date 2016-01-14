using UnityEngine;
using System.Collections;

public class SoldierHealth : MonoBehaviour {

    public int health;
    
    /* Damage Taken from Tanks */
    public int DamageTaken(int damage) {
        int remainingHealth = health;

        remainingHealth -= damage;
        health = remainingHealth;

        if (remainingHealth <= 0) {
            Destroy(gameObject, 1);
        }

        return health;
    }
}
