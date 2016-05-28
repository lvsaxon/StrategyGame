using UnityEngine;
using System.Collections;

public class BeamCollision : MonoBehaviour {
	
    public bool Reflect = false;
	public GameObject HitEffect = null;

	private bool bHit = false;
    private BeamParam BP;

	// Use this for initialization
	void Start () {
		BP = transform.root.gameObject.GetComponent<BeamParam>();
	}
	
	// Update is called once per frame
	void Update () {
		
		RaycastHit hit;
        Quaternion Angle;

        if (HitEffect != null && !bHit && Physics.Raycast(transform.position, transform.forward, out hit, Mathf.Infinity)){
            
            if(hit.collider.tag == "Rocket Brigadier"){
                bHit = true;
                
                //Reflect to Normal
                Angle = Quaternion.AngleAxis(180.0f, transform.up) * transform.rotation;

                GameObject obj = (GameObject)Instantiate(HitEffect, transform.position + transform.forward*hit.distance, Angle);
			    obj.GetComponent<BeamParam>().SetBeamParam(BP);
			    obj.transform.localScale = transform.localScale;
            }
		}
	}
}
