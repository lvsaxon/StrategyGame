using UnityEngine;
using UnityEngine.UI;

public class TankScore : MonoBehaviour {

    public static int TScore;
    public Image scorebarUI;

	Text txt;
    float maxScore = 5000;

	void Start () {
		txt = GetComponent<Text>();
		TScore = 0; //reset ot zero
        scorebarUI.fillAmount = 0;
	}
	
	void Update () {
		txt.text = ""+TScore;
        scorebarUI.fillAmount = TScore / maxScore;
	}
}
