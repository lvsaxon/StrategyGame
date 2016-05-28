using UnityEngine;
using System.Linq;
using System.Collections.Generic;

public class PowerUpLocationManager: MonoBehaviour {

    public List<Transform> powerUpList;


    void Start() {
        powerUpList = new List<Transform>();
    }


    /* Add PowerUp Locations on Map */
    public void AddObject(GameObject powerObj) {
       
        powerUpList.Add(powerObj.transform);
        CheckForEmptyElements();
    }


    /* Move Through List to Remove Empty Elements */
    void CheckForEmptyElements() {

        for(int i=0; i<powerUpList.Count; i++){
            if(powerUpList[i] == null){
               powerUpList.RemoveAt(i);
            }
        }
    }


    /* Return List of PowerUps */
    public Vector3 SearchPowerUp(string name, Transform agentLocation) {
        Vector3 powerPos = new Vector3();
        List<Transform> powerObjects = new List<Transform>();

        //Add Requested PowerUp to List
        for(int i=0; i<powerUpList.Count; i++) {
            if(name == powerUpList[i].tag)
               powerObjects.Add(powerUpList[i]);
        }
        
        //Check if List is Not Empty
        if(powerObjects.Count != 0)
           powerPos = LocateClosestPowerUp(powerObjects, agentLocation).position;  
             
        return powerPos;
    }


    /* Locate the Closest PowerUp for the Agent */
    Transform LocateClosestPowerUp(List<Transform> _powerObjects, Transform _agentLocation) {

        Transform closestPowerUp = _powerObjects.OrderBy(powerUp => Vector3.Distance(powerUp.transform.position, _agentLocation.position)).FirstOrDefault().transform;        
        return closestPowerUp;
    }
}
