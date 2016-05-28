using UnityEngine;
using UnityEngine.SceneManagement;

public class JumpScene : MonoBehaviour {

	public int scene_index = 0;
	
	public void ChangeScene(){
		print("SceneChange:"+scene_index);
        
        SceneManager.LoadScene(scene_index);
	}
}
