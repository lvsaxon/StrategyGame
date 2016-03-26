using UnityEngine;
using UnityEngine.UI;

public class TankScore : MonoBehaviour {

    public static int TScore;
	Text txt;

	void Start () {
		txt = GetComponent <Text> ();
		TScore = 0; //reset ot zero
	}
	
	void Update () {
		txt.text = ""+TScore;
	}
}
