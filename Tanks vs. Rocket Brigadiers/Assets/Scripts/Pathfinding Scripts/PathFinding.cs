using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;


public class PathFinding: MonoBehaviour {
    
    NavigationGridMap navigationGrid;
    PathRequestManager requestManager;

    void Awake() {
        navigationGrid = GetComponent<NavigationGridMap>();
        requestManager = GetComponent<PathRequestManager>();
    }


    /* Find Path to Target's Position */
    IEnumerator FindTargetPath(Vector3 initialPosition, Vector3 finalPosition) {
        bool pathSucess = false;
        Vector3[] waypoints = new Vector3[0];

        Node startingPosition = navigationGrid.GetAgent_NodePosition(initialPosition);
        Node targetPosition = navigationGrid.GetAgent_NodePosition(finalPosition); 

        //Check if the Nodes are Walkable before finding the Path
        if(startingPosition.isNodeWalkable && targetPosition.isNodeWalkable){ 
            HeapNodeSort<Node> openPath = new HeapNodeSort<Node>(navigationGrid.MaxSize());
            HashSet<Node> closedPath = new HashSet<Node>();
            openPath.Add(startingPosition);

            //Loop through each Node to find Shortest Path
            while(openPath.Count() > 0) {
                Node currentNode = openPath.RemoveFirstItem();
                closedPath.Add(currentNode);

                //Check if Agent has met its Destination
                if(currentNode == targetPosition) {
                   pathSucess = true;
                   break;
                }

                //Check for Neighboring Nodes surrounding CurrentNode
                foreach(Node neighbors in navigationGrid.GetNeighboringNodes(currentNode)){
                    if (!neighbors.isNodeWalkable || closedPath.Contains(neighbors)) 
                        continue;
                
                    int neighborMovementCost = currentNode.G + GetNeighboring_NodeDistance(currentNode, neighbors) + neighbors.movementPenalty;
                    if(neighborMovementCost < neighbors.G || !openPath.Contains(neighbors)){
                       neighbors.G = neighborMovementCost;
                       neighbors.H = GetNeighboring_NodeDistance(neighbors, targetPosition);
                       neighbors.parentNode = currentNode; 

                       //Add neighbor Node if OpenPath doesNot Have it
                       if(!openPath.Contains(neighbors)) 
                          openPath.Add(neighbors);
                       else
                          openPath.UpdateNodeItem(neighbors);
                    }
                }
            }
        }

        yield return null;

        //Path was a Sucess; Retrace the Entire Path
        if(pathSucess)
           waypoints = RetracePath(startingPosition, targetPosition); 
        
        requestManager.FinishedProcessingThePath(waypoints, pathSucess);        
    }


    /* Get Distance from CurrentNode to NeighboringNode */
    int GetNeighboring_NodeDistance(Node curr, Node neighbor) {
        int distanceX = Mathf.Abs(neighbor.gridX - curr.gridX);
        int distanceY = Mathf.Abs(neighbor.gridY - curr.gridY);

        int distanceSum = (int)Mathf.Sqrt(Mathf.Pow(distanceX, 2f) + Mathf.Pow(distanceY, 2f));

        return distanceSum;
    }


    /* Show Complete Path from StartPos to TargetPos */
    Vector3[] RetracePath(Node startNode, Node endNode){
        Node currentPath = endNode;
        List<Node> completePath = new List<Node>();

        while(currentPath != startNode){
              completePath.Add(currentPath);
              currentPath = currentPath.parentNode;
        }

        Vector3[] waypoints = SimplifyPath(completePath);
        //Display Path in Reverse
        Array.Reverse(waypoints);

        return waypoints;
    }

    
    /* Simplify Path whenever Path changes direction */
    Vector3[] SimplifyPath(List<Node> path) {
        List<Vector3> waypoints = new List<Vector3>();
        Vector2 currDirection = Vector2.zero;

        for(int i=1; i<path.Count; i++) {
            Vector2 newDirection = new Vector2(path[i-1].gridX - path[i].gridX, path[i-1].gridY - path[i].gridY);

            if(newDirection != currDirection) {
               waypoints.Add(path[i].worldPosition);
            }

            currDirection = newDirection;
        }

        return waypoints.ToArray();
    }


    /* Star findind the Path */
    public void StartFindPath(Vector3 startPos, Vector3 targetPos) {

        StartCoroutine(FindTargetPath(startPos, targetPos));
    }
}