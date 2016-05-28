using UnityEngine;
using UnityEngine.UI;

public class SoldierScore : MonoBehaviour {

    public static int SScore;
    public Image scorebarUI;

	Text txt;
    float maxScore = 5000;

    void Start () {
		txt = GetComponent <Text> ();
		SScore = 0;//reset ot zero
        scorebarUI.fillAmount = 0;
	}

    void Update () {
		txt.text = ""+SScore;
        scorebarUI.fillAmount = SScore / maxScore;		
	}
}
