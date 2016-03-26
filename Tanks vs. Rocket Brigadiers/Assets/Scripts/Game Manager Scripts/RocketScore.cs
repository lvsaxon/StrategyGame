using UnityEngine;
using UnityEngine.UI;

public class RocketScore : MonoBehaviour {

    public static int SScore;
	Text txt;

    void Start () {
		txt = GetComponent <Text> ();
		SScore = 0;//reset ot zero
	}


    void Update () {
		txt.text = ""+SScore;		
	}
}
