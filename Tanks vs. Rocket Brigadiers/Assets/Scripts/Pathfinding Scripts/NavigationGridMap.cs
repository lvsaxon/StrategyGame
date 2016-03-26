using UnityEngine;
using System.Collections.Generic;


[System.Serializable]
public class TerrainType {
    public LayerMask terrainLayer;
    public int terrainPenalty;
}


public class NavigationGridMap : MonoBehaviour {

    public bool displayGrid;
    public float nodeRadius;
    public LayerMask unwalkableLayer;
    public Vector2 gridSize;
    public TerrainType[] walkableTerrainTypes;

    Node[,] nodes;
    float nodeDiameter; 
    int gridSizeX, gridSizeY;
    LayerMask walkableLayer;
    Dictionary<int, int> walkableLayer_Dictionary = new Dictionary<int, int>();


    void Awake() {
        nodeDiameter = nodeRadius * 2;
        
        //# of Nodes from Grid's length & width
        gridSizeX = Mathf.RoundToInt(gridSize.x / nodeDiameter);
        gridSizeY = Mathf.RoundToInt(gridSize.y / nodeDiameter);

        foreach(TerrainType region in walkableTerrainTypes) {
            walkableLayer.value = walkableLayer | region.terrainLayer.value;
            
            //Using Dictionary for faster Terrain Layer Searching 
            walkableLayer_Dictionary.Add((int) Mathf.Log(region.terrainLayer.value, 2), region.terrainPenalty);
        }

        CreateNodeGrid();
    }


    /* MaxSize of Map */
    public int MaxSize() {
        
        return gridSizeX * gridSizeY;
    }


    /* Create Node GridMap */
    void CreateNodeGrid() {
        int movementPenalty = 0;
        nodes = new Node[gridSizeX, gridSizeY];
        Vector3 gridOrigin = transform.position - Vector3.right * gridSize.x/2 - Vector3.forward * gridSize.y/2;//Vector3.zero;

        //Create Node Positions
        for(int x=0; x<gridSizeX; x++){
            for(int y=0; y<gridSizeY; y++){
                Vector3 nodePosition = gridOrigin + Vector3.right*(x * nodeDiameter + nodeRadius) + Vector3.forward*(y * nodeDiameter + nodeRadius);
                bool isNodeWalkable = !(Physics.CheckSphere(nodePosition, nodeRadius+2, unwalkableLayer));

                if(isNodeWalkable) {
                    RaycastHit hit;
                    Ray ray = new Ray(nodePosition + Vector3.up * 35, Vector3.down);
                    if(Physics.Raycast(ray, out hit, 50, walkableLayer)) {
                       walkableLayer_Dictionary.TryGetValue(hit.collider.gameObject.layer, out movementPenalty);
                    }
                }

                nodes[x,y] = new Node(isNodeWalkable, nodePosition, x, y, movementPenalty);
            }
        }
    }


    /* Get Agent's Node GridPos from World Position */
    public Node GetAgent_NodePosition(Vector3 nodePosition) {
        float percentX = (nodePosition.x / gridSize.x); 
        float percentY = (nodePosition.z / gridSize.y); 

        percentX = Mathf.Clamp01(percentX);
        percentY = Mathf.Clamp01(percentY);

        int x = Mathf.RoundToInt((gridSizeX-1) * percentX);
        int y = Mathf.RoundToInt((gridSizeY-1) * percentY);

        return nodes[x,y];
    }


    /* Get Neighboring Nodes Surrounding CurrentNode */
    public List<Node> GetNeighboringNodes(Node node){
        List<Node> neighboringNodes = new List<Node>();

        for(int x=-1; x<=1; x++){
            for(int y=-1; y<=1; y++){
                if(x==0 && y==0)
                   continue;

                int checkingXPos = node.gridX + x;
                int checkingYPos = node.gridY + y;

                if(checkingXPos >= 0 && checkingXPos < gridSizeX && checkingYPos >= 0 && checkingYPos < gridSizeY) {
                    neighboringNodes.Add(nodes[checkingXPos, checkingYPos]);
                }
            }
        }

        return neighboringNodes;
    }


    /* Draw visual Representation of the Node Grid */
    void OnDrawGizmos() {
        Node[,] gridNodes = nodes;
        Gizmos.DrawWireCube(transform.position, new Vector3(gridSize.x, 1, gridSize.y));
        
        if(gridNodes != null && displayGrid){
            foreach(Node node in nodes) {
                Gizmos.color = (node.isNodeWalkable)? Color.white: Color.red;
                Vector3 nodeSize = Vector3.one/2 * (nodeDiameter  - 0.5f);
                Gizmos.DrawCube(node.worldPosition, nodeSize);
            }
        }
    }
}
