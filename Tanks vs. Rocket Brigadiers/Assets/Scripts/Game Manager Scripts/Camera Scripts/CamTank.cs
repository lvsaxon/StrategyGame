using UnityEngine;
using System.Collections;

public class CamTank : MonoBehaviour {
	public GameObject target;
	public float xOffset = 0;
	public float yOffset = 5;
	public float zOffset = -10;

	void LateUpdate() {
		this.transform.position = new Vector3(target.transform.position.x + xOffset,
			target.transform.position.y + yOffset,
			target.transform.position.z + zOffset);
	}
}