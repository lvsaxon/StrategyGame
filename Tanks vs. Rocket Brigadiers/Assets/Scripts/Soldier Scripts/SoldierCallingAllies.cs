using UnityEngine;
using System.Collections;

public class SoldierCallingAllies : MonoBehaviour {

    
	void OnTriggerEnter(Collider collid) {

        if(collid.tag == "Rocket Brigadier"){
           //GetComponentInParent<SoldierCoordinator>().ChooseSquadronFormation();
        }
    }


    void OnTriggerStay(Collider collid) {

        if(collid.tag == "Rocket Brigadier"){
           
        }  
    }
}
