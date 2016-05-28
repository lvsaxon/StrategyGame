using UnityEngine;
using System.Collections;

public class CameraSwap : MonoBehaviour {
	public Camera Camera1;
	public Camera Camera2;
	public Camera Camera3;
	void Start() {
		Camera1.enabled = true;
		Camera2.enabled = false;
		Camera3.enabled = false;
	}
	void Update() {
		if (Input.GetKeyDown ("f1")) {	
			Camera1.enabled = true;
			Camera2.enabled = false;
			Camera3.enabled = false;
		}
		if (Input.GetKeyDown("f2")) {
			Camera1.enabled = false;
			Camera2.enabled = true;
			Camera3.enabled = false;
		}
		else if (Input.GetKeyDown("f3")) {
			Camera1.enabled = false;
			Camera2.enabled = false;
			Camera3.enabled = true;
		}
	}
}