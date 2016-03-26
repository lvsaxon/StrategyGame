using UnityEngine;
using System.Collections.Generic;

public class Node: IHeapItem<Node> {

    public int G, H, gridX, gridY;
    public int movementPenalty;
    public Node parentNode;
    public bool isNodeWalkable;
    public Vector3 worldPosition;

    int heapIndex;


    /* Create Characteristics for Node Object */
    public Node(bool _walkable, Vector3 _worldPosition, int _x, int _y, int _movementPenalty) {
        isNodeWalkable = _walkable;
        worldPosition = _worldPosition;
        movementPenalty = _movementPenalty;
        gridX = _x;
        gridY = _y;
    }


    public int F() {
        return G+H;
    }


    public int HeapIndex {

        get{ return heapIndex;  }

        set{ heapIndex = value; }
    }


    /* Compare other Node's by F-Cost */
    public int CompareTo(Node compareNode) {
        int compare = F().CompareTo(compareNode.F());

        //if F-cost is Equal use H-Cost
        if(compare == 0) {
           compare = H.CompareTo(compareNode.H);
        }

        return -compare;
    }
}
