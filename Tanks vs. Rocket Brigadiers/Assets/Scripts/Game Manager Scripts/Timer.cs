using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Timer : MonoBehaviour {

    public float TimeLeft;
	public Text TimerTxt;
    float volume = 1;
    AudioSource[] source;
    AudioSource audioSource;

    void Start(){
        source = GetComponents<AudioSource>();
		TimerTxt.GetComponent<Text>();
    }


	void Update () {
		TimeLeft -= Time.deltaTime;
		TimerTxt.text = TimeLeft.ToString ("f2");
		
        //30 Secs Remain
        if(TimeLeft <= 30){
           audioSource = source[0];
           audioSource.enabled = true;
        }

        //10 Secs Remain
        if(TimeLeft <= 10) {
           audioSource = source[1];
           audioSource.enabled = true;
           TimerTxt.color = Color.red; 
        }

        if(TimeLeft <= 0){
           audioSource = source[2];
           audioSource.enabled = true;
           TimerTxt.text = "0:00";
            
           if(!audioSource.isPlaying)
              source[3].enabled = true;  
           
           if(source[3].isPlaying && volume > 0.1f) 
              FadeOut();                             
        }
    }


    /* FadeOut Last Audio  */
    void FadeOut() {
        volume -= 0.1f * Time.deltaTime;
        source[3].volume = volume;
    }
}
