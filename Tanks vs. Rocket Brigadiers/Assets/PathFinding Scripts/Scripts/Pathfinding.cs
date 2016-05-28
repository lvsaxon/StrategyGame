using UnityEngine;
using System.Collections.Generic;


public class Pathfinding: MonoBehaviour 
{
    [HideInInspector]
    public List<Vector3> Path = new List<Vector3>();
    bool JS = false;

    public void FindPath(Vector3 startPosition, Vector3 endPosition){

        Pathfinder.Instance.InsertInQueue(startPosition, endPosition, SetList);
    }
	 

	public void FindJSPath(Vector3[] arr){

        if(arr.Length > 1){	
		   Pathfinder.Instance.InsertInQueue(arr[0], arr[1], SetList);
	    }
    }


    protected virtual void SetList(List<Vector3> path){

        if(path == null){
            return;
        }
		
		if(!JS){
	        Path.Clear();
	        Path = path;

        }else{
			Vector3[] arr = new Vector3[path.Count];
			for(int i = 0; i < path.Count; i++){
				arr[i] = path[i];
			}

            if(arr.Length > 0){
               gameObject.SendMessage("GetJSPath", arr);
            }
		}
    }
}
	
