
using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class PathRequestManager : MonoBehaviour {

    bool isProcessingPath;
    PathRequest pathRequest;
    PathFinding targetFinder;

    static PathRequestManager managerContext;
    Queue<PathRequest> pathRequestQueue = new Queue<PathRequest>();
    

    /* Give out Path Request from Start to EndPos */
    struct PathRequest {
        public Vector3 startPath, endPath;
        public Action<Vector3[], bool> callback;

        public PathRequest(Vector3 _start, Vector3 _end, Action<Vector3[], bool> _call){
            startPath = _start;
            endPath = _end;
            callback = _call;
        }
    }


    void Start() {
        managerContext = this;
        targetFinder = GetComponent<PathFinding>();
    }


    /* Requested Path from Agents; Action<> store completePaths & call them back hen asked for */
    public static void RequestPath(Vector3 startPath, Vector3 endPath, Action<Vector3[], bool> callback) {
        PathRequest newPathRequest = new PathRequest(startPath, endPath, callback); 

        //Store Path in Queue class context
        managerContext.pathRequestQueue.Enqueue(newPathRequest);
        managerContext.TryNextProcess();
    }


    /* Process another Path available */
    void TryNextProcess() {
        
        //When not processing anything, get the 1st path in the queue
        if(!isProcessingPath && pathRequestQueue.Count > 0) { 
           pathRequest = pathRequestQueue.Dequeue();
           isProcessingPath = true;
           targetFinder.StartFindPath(pathRequest.startPath, pathRequest.endPath);
        }
    }


    /* Successfully find the path thats specified */
    public void FinishedProcessingThePath(Vector3[] path, bool success) {

        pathRequest.callback(path, success);
        isProcessingPath = false;
        TryNextProcess();
    }
}
