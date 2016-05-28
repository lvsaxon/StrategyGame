using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;


public class Pathfinder : MonoBehaviour 
{
    //Singleton
    private static Pathfinder context;
    public static Pathfinder Instance {

        get{ return context; }
        private set{ } 
    }

    //Variables
    private Nodes[,] nodes = null;
    public float Tilesize = 5;
    public float MaxFalldownHeight;
    public float ClimbLimit;
    public int HeuristicAggression;
    public float HighestPoint;
    public float LowestPoint;

    public Vector2 MapStartPosition;
    public Vector2 MapEndPosition;

    public List<string> DisallowedTags;
    public List<string> IgnoreTags;
    public bool MoveDiagonal = true;

    public bool DrawMapInEditor = false;
    public bool CheckFullTileSize = false;
    public bool showGridGizmos;

    int maxSearchRounds = 0;

    //Queue path finding to not bottleneck it
    private List<QueuePath> queue = new List<QueuePath>();

    //Set singleton!
    void Awake(){
        context = this;
    }

	
	void Start (){      
        Instance.CreateMap();
	}


    void Update(){
        
        while(queue.Count > 0){
              StartCoroutine(PathHandler(queue[0].startPos, queue[0].endPos, queue[0].storeRef));            
              queue.RemoveAt(0);
        }

        DrawMapLines();
	}

    //-------------------------------------------------INSTANIATE MAP-----------------------------------------------//
    #region map
    
    private void CreateMap(){
        //Find positions for start and end of map
        int startX  = (int)MapStartPosition.x;
        int startZ  = (int)MapStartPosition.y;
        
        int endX    = (int)MapEndPosition.x;
        int endZ    = (int)MapEndPosition.y;

        //Find tile width and height
        int width = (int)((endX - startX) / Tilesize);
        int height = (int)((endZ - startZ) / Tilesize);

        //Set map up
        nodes = new Nodes[width, height];
        int size = width * height;
        SetListsSize(size);

        //Fill up Map
        for(int i = 0; i < height; i++){
            for(int j = 0; j < width; j++){
                
                float x = startX + (j * Tilesize) + (Tilesize / 2); //Position from where we raycast - X
                float y = startZ + (i * Tilesize) + (Tilesize / 2); //Position from where we raycast - Z
                int ID = (i * width) + j; //ID we give to our Node!
                
                float dist = Mathf.Abs(HighestPoint) + Mathf.Abs(LowestPoint);
                RaycastHit[] raycastHits; 

                if (CheckFullTileSize)
                {
                    raycastHits = Physics.SphereCastAll(new Vector3(x, HighestPoint, y), Tilesize / 2, Vector3.down, dist);
                }
                else
                {
                    raycastHits = Physics.SphereCastAll(new Vector3(x, HighestPoint, y), Tilesize / 16, Vector3.down, dist);
                }
                bool free = true;
                float maxY = -Mathf.Infinity;
                
                foreach(RaycastHit hit in raycastHits)
                {
                    if (DisallowedTags.Contains(hit.transform.tag))
                    {
                        if (hit.point.y > maxY)
                        {
                            //It is a disallowed walking tile, make it false
                            nodes[j, i] = new Nodes(j, i, 0, ID, x, y, false); //Non walkable tile!
                            free = false;
                            maxY = hit.point.y;
                        }
                    }
                    else if(IgnoreTags.Contains(hit.transform.tag))
                    {
                        //Do nothing we ignore these tags
                    }
                    else
                    {
                        if (hit.point.y > maxY)
                        {
                            //It is allowed to walk on this tile, make it walkable!
                            nodes[j, i] = new Nodes(j, i, hit.point.y, ID, x, y, true); //walkable tile!
                            free = false;
                            maxY = hit.point.y;
                        }
                    }
                }
                //We hit nothing set tile to false
                if (free == true)
                {
                nodes[j, i] = new Nodes(j, i, 0 ,ID, x, y, false);//Non walkable tile! 
                }        
            }
        }
    }

    #endregion //End map
    
    //---------------------------------------SETUP PATH QUEUE---------------------------------------//
    //Pathfinding Class Only
    public void InsertInQueue(Vector3 startPos, Vector3 endPos, Action<List<Vector3>> listMethod){
        QueuePath q = new QueuePath(startPos, endPos, listMethod);
        queue.Add(q);
    }

    
    //---------------------------------------FIND PATH: A*------------------------------------------//
     #region A* Star

    private Nodes[] openList;
    private Nodes[] closedList;
    private Nodes startNode;
    private Nodes endNode;
    private Nodes currentNode;
    
    //Use it with KEY: F-value, VALUE: ID. ID's might be looked up in open and closed list then
    private List<NodeSearch> sortedOpenList = new List<NodeSearch>();

    private void SetListsSize(int size){
        openList   = new Nodes[size];
        closedList = new Nodes[size];
    }

    IEnumerator PathHandler(Vector3 startPos, Vector3 endPos, Action<List<Vector3>> listMethod){
        yield return StartCoroutine(SinglePath(startPos, endPos, listMethod));
    }

    IEnumerator SinglePath(Vector3 startPos, Vector3 endPos, Action<List<Vector3>> listMethod){
        FindPath(startPos, endPos, listMethod);
        yield return null;
    }

    public void FindPath(Vector3 startPos, Vector3 endPos, Action<List<Vector3>> listMethod){
        //The list we returns when path is found
        List<Vector3> returnPath = new List<Vector3>();
        bool endPosValid = true;


        //Find start and end nodes, if we cant return null and stop!
        startNode = FindClosestNode(startPos);
        endNode = FindClosestNode(endPos);

        if (startNode != null){
            if (endNode == null){
                endPosValid = false;
                FindEndNode(endPos);
                
                //End Node is still NULL
                if(endNode == null){
                    //still no end node - we leave and sends an empty list
                    maxSearchRounds = 0;
                    listMethod.Invoke(new List<Vector3>());
                    return;
                }
            }

            //Clear lists if they are filled
            Array.Clear(openList, 0, openList.Length); 
            Array.Clear(closedList, 0, openList.Length);

            if(sortedOpenList.Count > 0){
               sortedOpenList.Clear();
            }
            
            //Insert start node
            openList[startNode.ID] = startNode;

            //sortedOpenList.Add(new NodeSearch(startNode.ID, startNode.F));
            BHInsertNode(new NodeSearch(startNode.ID, startNode.F));

            bool endLoop = false;

            while (!endLoop){
                //If we have no nodes on the open list AND we are not at the end, then we got stucked! return empty list then.
                if (sortedOpenList.Count == 0){
                    print("Empty Openlist & closedList");
                    listMethod.Invoke(new List<Vector3>());
                    return;
                }

                //Get lowest node and insert it into the closed list
                int id = BHGetLowest();
                currentNode = openList[id];
                closedList[currentNode.ID] = currentNode;
                openList[id] = null;
                
                if (currentNode.ID == endNode.ID){
                    endLoop = true;
                    continue;
                }
                //Now look at neighbours, check for unwalkable tiles, bounderies, open and closed listed nodes.

                if (MoveDiagonal)
                {
                    NeighbourCheck();
                }
                else
                {
                    NonDiagonalNeighborCheck();
                }
            }


            while (currentNode.parent != null)
            {
                returnPath.Add(new Vector3(currentNode.xCoord, currentNode.yCoord, currentNode.zCoord));
                currentNode = currentNode.parent;
            }

            returnPath.Reverse();

            if (returnPath.Count > 2 && endPosValid){
                //Now make sure we do not go backwards or go to long
                if (Vector3.Distance(returnPath[returnPath.Count - 1], returnPath[returnPath.Count - 3]) < Vector3.Distance(returnPath[returnPath.Count - 3], returnPath[returnPath.Count - 2]))
                {
                    returnPath.RemoveAt(returnPath.Count - 2);
                }
                if (Vector3.Distance(returnPath[1], startPos) < Vector3.Distance(returnPath[0], returnPath[1])) {
                    returnPath.RemoveAt(0);
                }
            }
            maxSearchRounds = 0;
            listMethod.Invoke(returnPath);

        }
        else
        {
            maxSearchRounds = 0;
            listMethod.Invoke(new List<Vector3>());
        }
    }

    // Find start and end Node
    private void SetStartAndEndNode(Vector3 start, Vector3 end){
        startNode = FindClosestNode(start);
        endNode = FindClosestNode(end);
    }


    /* See if The Next Node is Walkable */
    public bool IsTheClosestNodeWalkable(Vector3 pos){
        int x = (MapStartPosition.x < 0F) ? Mathf.FloorToInt(((pos.x + Mathf.Abs(MapStartPosition.x)) / Tilesize)) : Mathf.FloorToInt((pos.x - MapStartPosition.x) / Tilesize);
        int z = (MapStartPosition.y < 0F) ? Mathf.FloorToInt(((pos.z + Mathf.Abs(MapStartPosition.y)) / Tilesize)) : Mathf.FloorToInt((pos.z - MapStartPosition.y) / Tilesize);

        if(x < 0 || z < 0 || x > nodes.GetLength(0) || z > nodes.GetLength(1))
           return false;

        Nodes n = nodes[x, z];
        return n.walkable;
    }


    /* Find the closet Node next to target's position */
    private Nodes FindClosestNode(Vector3 pos){      
        int x = (MapStartPosition.x < 0F) ? Mathf.FloorToInt(((pos.x + Mathf.Abs(MapStartPosition.x)) / Tilesize)) :  Mathf.FloorToInt((pos.x - MapStartPosition.x) / Tilesize);
        int z = (MapStartPosition.y < 0F) ? Mathf.FloorToInt(((pos.z + Mathf.Abs(MapStartPosition.y)) / Tilesize)) : Mathf.FloorToInt((pos.z - MapStartPosition.y) / Tilesize);

        if(x < 0 || z < 0 || x > nodes.GetLength(0) || z > nodes.GetLength(1))
           return null;

        Nodes n = nodes[x, z];

        if(n.walkable){
           return new Nodes(x, z, n.yCoord, n.ID, n.xCoord, n.zCoord, n.walkable);
        }
        else
        {
            //If we get a non walkable tile, then look around its neightbours
            for (int i = z - 1; i < z + 2; i++)
            {
                for (int j = x - 1; j < x + 2; j++)
                {
                    //Check they are within bounderies
                    if (i > -1 && i < nodes.GetLength(1) && j > -1 && j < nodes.GetLength(0))
                    {
                        if (nodes[j, i].walkable)
                        {
                            return new Nodes(j, i, nodes[j, i].yCoord, nodes[j, i].ID, nodes[j, i].xCoord, nodes[j, i].zCoord, nodes[j, i].walkable);
                        }
                    }
                }
            }
            return null;
        }
    }


    /* Look for the endNode */
    private void FindEndNode(Vector3 pos){       
        int x = (MapStartPosition.x < 0F) ? Mathf.FloorToInt(((pos.x + Mathf.Abs(MapStartPosition.x)) / Tilesize)) : Mathf.FloorToInt((pos.x - MapStartPosition.x) / Tilesize);
        int z = (MapStartPosition.y < 0F) ? Mathf.FloorToInt(((pos.z + Mathf.Abs(MapStartPosition.y)) / Tilesize)) : Mathf.FloorToInt((pos.z - MapStartPosition.y) / Tilesize);

        Nodes closestNode = nodes[x, z];
        List<Nodes> walkableNodes = new List<Nodes>();

        int turns = 1;
        while(walkableNodes.Count < 1 && maxSearchRounds < 10/Tilesize){
            walkableNodes = EndNodeNeighbourCheck(x, z, turns);
            turns++;
            maxSearchRounds++;
        }

        //If we found some walkable tiles we will then return the nearest
        if(walkableNodes.Count > 0){
            int lowestDist = 99999999;
            Nodes n = null;

            foreach(Nodes node in walkableNodes){
                int i = GetHeuristics(closestNode, node);
                if(i < lowestDist){
                    lowestDist = i;
                    n = node;
                }
            }

            endNode = new Nodes(n.x, n.y, n.yCoord, n.ID, n.xCoord, n.zCoord, n.walkable);
        }
    }

    private List<Nodes> EndNodeNeighbourCheck(int x, int z, int r)
    {      
        List<Nodes> nodes = new List<Nodes>();
        
        for (int i = z - r; i < z + r + 1; i++)
        {
            for (int j = x - r; j < x + r + 1; j++)
            {
                //Check that we are within bounderis, and goes in ring around our end pos
                if (i> -1 && j > -1 && i < this.nodes.GetLength(0) && j < this.nodes.GetLength(1) && ((i< z - r + 1 || i > z + r - 1) || (j< x - r + 1 || j > x + r - 1)))
                {
                    //if it is walkable put it on the right list
                    if (this.nodes[j, i].walkable)
                    {
                        nodes.Add(this.nodes[j, i]);
                    }
                }
            }
        }

        return nodes;
    }


    /* Check Neighboring Nodes from Current Node */
    private void NeighbourCheck(){
        int x = currentNode.x;
        int y = currentNode.y;

        for(int i = y-1; i < y+2; i++){
            for(int j = x-1; j < x+2; j++){

                //Check it is within the bounderies
                if(i > -1 && i < nodes.GetLength(1) && j > -1 && j < nodes.GetLength(0)){
                  
                    //Dont check for the current node.
                    if(i != y || j != x){

                        //Check the node is walkable
                        if(nodes[j, i].walkable){

                            //We do not recheck anything on the closed list
                            if(closedList[nodes[j, i].ID] == null){

                                //Check if we can move up or jump down!
                                if((nodes[j, i].yCoord - currentNode.yCoord < ClimbLimit && nodes[j, i].yCoord - currentNode.yCoord >= 0) || 
                                    (currentNode.yCoord - nodes[j, i].yCoord < MaxFalldownHeight && currentNode.yCoord >= nodes[j, i].yCoord)){

                                    //If it is not on the open list then add it to
                                    if(openList[nodes[j, i].ID] == null){
                                        Nodes addNode = new Nodes(nodes[j, i].x, nodes[j, i].y, nodes[j, i].yCoord, nodes[j, i].ID, nodes[j, i].xCoord, nodes[j, i].zCoord, nodes[j, i].walkable, currentNode);
                                        addNode.H = GetHeuristics(nodes[j, i].x, nodes[j, i].y);
                                        addNode.G = GetMovementCost(x, y, j, i) + currentNode.G;
                                        addNode.F = addNode.H + addNode.G;
                                        
                                        //Insert in Open & Sorted list
                                        openList[addNode.ID] = addNode;
                                        BHInsertNode(new NodeSearch(addNode.ID, addNode.F));

                                    }else{
                                        //If it is on openlist then check if the new paths movement cost is lower
                                        Nodes node = GetNodeFromOpenList(nodes[j, i].ID);
                                        if(currentNode.G + GetMovementCost(x, y, j, i) < openList[nodes[j, i].ID].G){
                                            node.parent = currentNode;
                                            node.G = currentNode.G + GetMovementCost(x, y, j, i);
                                            node.F = node.G + node.H;
                                            BHSortNode(node.ID, node.F);
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
    }


    private void NonDiagonalNeighborCheck()
    {            
        int x = currentNode.x;
        int y = currentNode.y;

        for (int i = y - 1; i < y + 2; i++)
        {
            for (int j = x - 1; j < x + 2; j++)
            {
                //Check it is within the bounderies
                if (i > -1 && i < nodes.GetLength(1) && j > -1 && j < nodes.GetLength(0))
                {
                    //Dont check for the current node.
                    if (i != y || j != x)
                    {
                        //Check that we are not moving diagonal
                        if(GetMovementCost(x, y, j, i) < 14)
                        {
                            //Check the node is walkable
                            if (nodes[j, i].walkable)
                            {
                                //We do not recheck anything on the closed list
                                if (closedList[nodes[j, i].ID] == null)
                                {
                                    //Check if we can move up or jump down!
                                    if ((nodes[j, i].yCoord - currentNode.yCoord < ClimbLimit && nodes[j, i].yCoord - currentNode.yCoord >= 0) || (currentNode.yCoord - nodes[j, i].yCoord < MaxFalldownHeight && currentNode.yCoord >= nodes[j, i].yCoord))
                                    {
                                        //If it is not on the open list then add it to
                                        if (openList[nodes[j, i].ID] == null)
                                        {
                                            Nodes addNode = new Nodes(nodes[j, i].x, nodes[j, i].y, nodes[j, i].yCoord, nodes[j, i].ID, nodes[j, i].xCoord, nodes[j, i].zCoord, nodes[j, i].walkable, currentNode);
                                            addNode.H = GetHeuristics(nodes[j, i].x, nodes[j, i].y);
                                            addNode.G = GetMovementCost(x, y, j, i) + currentNode.G;
                                            addNode.F = addNode.H + addNode.G;
                                            //Insert on open list
                                            openList[addNode.ID] = addNode;
                                            //Insert on sorted list
                                            BHInsertNode(new NodeSearch(addNode.ID, addNode.F));
                                            //sortedOpenList.Add(new NodeSearch(addNode.ID, addNode.F));
                                        }
                                        else
                                        {
                                            ///If it is on openlist then check if the new paths movement cost is lower
                                            Nodes n = GetNodeFromOpenList(nodes[j, i].ID);
                                            if (currentNode.G + GetMovementCost(x, y, j, i) < openList[nodes[j, i].ID].G)
                                            {
                                                n.parent = currentNode;
                                                n.G = currentNode.G + GetMovementCost(x, y, j, i);
                                                n.F = n.G + n.H;
                                                BHSortNode(n.ID, n.F);
                                                //ChangeFValue(n.ID, n.F);
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
    }


    private void ChangeFValue(int id, int F){

        foreach (NodeSearch ns in sortedOpenList){
            if (ns.ID == id){
                ns.F = F;
            }
        }
    }


    private int GetHeuristics(int x, int y){
        //Make sure heuristic aggression is not less then 0!
        int HA = (HeuristicAggression < 0) ? 0 : HeuristicAggression;

        return (int)(Mathf.Abs(x - endNode.x) * (10F + (10F * HA))) + (int)(Mathf.Abs(y - endNode.y) * (10F + (10F * HA)));
    }


    private int GetHeuristics(Nodes a, Nodes b){
        //Make sure heuristic aggression is not less then 0!
        int HA = (HeuristicAggression < 0)? 0 : HeuristicAggression;

        return (int)(Mathf.Abs(a.x - b.x) * (10F + (10F * HA))) + (int)(Mathf.Abs(a.y - b.y) * (10F + (10F * HA)));
    }


    /* Movement Cost: Straight or Diagonal */
    private int GetMovementCost(int x, int y, int j, int i){

        return (x != j && y != i) ? 14 : 10;
    }


    private Nodes GetNodeFromOpenList(int id){

        return (openList[id] != null) ? openList[id] : null;
    }

    #region Binary_Heap (min)

    private void BHInsertNode(NodeSearch ns){
        int index;
        bool continueNodeInsertion = true;
        
        //Insert Node if there is nothing in the BinaryTree
        if (sortedOpenList.Count == 0) {
            sortedOpenList.Add(ns);
            openList[ns.ID].sortedIndex = 0;
            return;
        }

        sortedOpenList.Add(ns);
        index = sortedOpenList.Count - 1;
        openList[ns.ID].sortedIndex = index;

        while(continueNodeInsertion){
            int parent = Mathf.FloorToInt((index - 1) / 2);
            
            //Root BinaryTree Node; Stop search
            if (index == 0){
                continueNodeInsertion = false;
                openList[sortedOpenList[index].ID].sortedIndex = 0;
            
            }else{
                
                if (sortedOpenList[index].F < sortedOpenList[parent].F) {
                    NodeSearch s = sortedOpenList[parent];
                    sortedOpenList[parent] = sortedOpenList[index];
                    sortedOpenList[index] = s;

                    //Save sortedlist index's for faster look up
                    openList[sortedOpenList[index].ID].sortedIndex = index;
                    openList[sortedOpenList[parent].ID].sortedIndex = parent;

                    //Reset index to parent ID
                    index = parent;
                
                }else{
                    continueNodeInsertion = false;
                }
            }
        }
    }

    private void BHSortNode(int id, int F)
    {
        bool canMoveFurther = true;
        int index = openList[id].sortedIndex;
        sortedOpenList[index].F = F;

        while (canMoveFurther)
        {
            int parent = Mathf.FloorToInt((index - 1) / 2);

            if (index == 0) //We are the root
            {
                canMoveFurther = false;
                openList[sortedOpenList[index].ID].sortedIndex = 0;
            }
            else
            {
                if (sortedOpenList[index].F < sortedOpenList[parent].F)
                {
                    NodeSearch s = sortedOpenList[parent];
                    sortedOpenList[parent] = sortedOpenList[index];
                    sortedOpenList[index] = s;

                    //Save sortedlist index's for faster look up
                    openList[sortedOpenList[index].ID].sortedIndex = index;
                    openList[sortedOpenList[parent].ID].sortedIndex = parent;

                    //Reset index to parent ID
                    index = parent;
                }
                else
                {
                    canMoveFurther = false;
                }
            }
        }
    }

    private int BHGetLowest()
    {
       
        if (sortedOpenList.Count == 1) //Remember 0 is our root
        {
            int ID = sortedOpenList[0].ID;
            sortedOpenList.RemoveAt(0);           
            return ID;
        }
        else if (sortedOpenList.Count > 1)
        {
            //save lowest not, take our leaf as root, and remove it! Then switch through children to find right place.
            int ID = sortedOpenList[0].ID;
            sortedOpenList[0] = sortedOpenList[sortedOpenList.Count - 1];
            sortedOpenList.RemoveAt(sortedOpenList.Count - 1);
            openList[sortedOpenList[0].ID].sortedIndex = 0;

            int index = 0;
            bool canMoveFurther = true;
            //Sort the list before returning the ID
            while (canMoveFurther)
            {
                int child1 = (index * 2) + 1;
                int child2 = (index * 2) + 2;
                int switchIndex = -1;

                if (child1 < sortedOpenList.Count)
                {
                    switchIndex = child1;
                }
                else
                {
                    break;
                }
                if (child2 < sortedOpenList.Count)
                {
                    if (sortedOpenList[child2].F < sortedOpenList[child1].F)
                    {
                        switchIndex = child2;
                    }
                }
                if (sortedOpenList[index].F > sortedOpenList[switchIndex].F)
                {
                    NodeSearch ns = sortedOpenList[index];
                    sortedOpenList[index] = sortedOpenList[switchIndex];
                    sortedOpenList[switchIndex] = ns;

                    //Save sortedlist index's for faster look up
                    openList[sortedOpenList[index].ID].sortedIndex = index;
                    openList[sortedOpenList[switchIndex].ID].sortedIndex = switchIndex;

                    //Switch around idnex
                    index = switchIndex;
                }
                else
                {
                    break;
                }    
            }
            return ID;
          
        }
        else
        {
            return -1;
        }    
    }

    #endregion

    #endregion //End astar region!

    //---------------------------------------DRAW MAP IN EDITOR-------------------------------------//

    void DrawMapLines()
    {
        if (DrawMapInEditor == true && nodes != null)
        {
            for (int i = 0; i < nodes.GetLength(1); i++)
            {
                for (int j = 0; j < nodes.GetLength(0); j++)
                {
                    if (!nodes[j, i].walkable)
                        continue;
                    
                    for (int y = i - 1; y < i + 2; y++)
                    {
                        for (int x = j - 1; x < j + 2; x++)
                        {
                            if (y < 0 || x < 0 || y >= nodes.GetLength(1) || x >= nodes.GetLength(0))
                                continue;

                            if(!nodes[x, y].walkable)
                                continue;

                            if (nodes[j, i].yCoord > nodes[x, y].yCoord && Mathf.Abs(nodes[j, i].yCoord - nodes[x, y].yCoord) > MaxFalldownHeight)
                                continue;

                            if (nodes[j, i].yCoord < nodes[x, y].yCoord && Mathf.Abs(nodes[x, y].yCoord - nodes[j, i].yCoord) > ClimbLimit)
                                continue;
                          
                            Vector3 start = new Vector3(nodes[j, i].xCoord, nodes[j, i].yCoord + 0.1f, nodes[j, i].zCoord);
                            Vector3 end = new Vector3(nodes[x, y].xCoord, nodes[x, y].yCoord + 0.1f, nodes[x, y].zCoord);

                            UnityEngine.Debug.DrawLine(start, end, Color.green);
                        }
                    }
                }
            }
        }
    }

    #region DynamicSupport

    public void DynamicMapEdit(List<Vector3> checkList, Action<List<Vector2>> listMethod)
    {
        listMethod.Invoke(DynamicFindClosestNodes(checkList));
    }

    public void DynamicRedoMapEdit(List<Vector2> ids)
    {
        foreach (Vector2 v in ids)
        {
            nodes[(int)v.x, (int)v.y].walkable = true;
        }
    }

    public void DynamicRaycastUpdate(Bounds b)
    {
        Vector3 startPos = new Vector3(b.min.x, 0, b.min.z);
        UnityEngine.Debug.Log("Startpos dyn: " + startPos);
        Nodes startNode = FindClosestNode(startPos);

        if (startNode == null)
            return;

        int xIterations = Mathf.CeilToInt(Mathf.Abs((b.max.x - b.min.x) / Tilesize)) + 2;
        int zIterations = Mathf.CeilToInt(Mathf.Abs((b.max.z - b.min.z) / Tilesize)) + 2;

        for (int i = startNode.x - 2; i < startNode.x + xIterations; i++)
        {
            for (int j = startNode.y - 2; j < startNode.y + zIterations; j++)
            {
                if (i >= 0 &&  j >= 0 && i < nodes.GetLength(0) && j < nodes.GetLength(1))
                {

                    float dist = Mathf.Abs(HighestPoint) + Mathf.Abs(LowestPoint);
                    RaycastHit[] hit;

                    if (CheckFullTileSize)
                    {
                        hit = Physics.SphereCastAll(new Vector3(nodes[i, j].xCoord, HighestPoint, nodes[i, j].zCoord), Tilesize / 2, Vector3.down, dist);
                    }
                    else
                    {
                        hit = Physics.SphereCastAll(new Vector3(nodes[i, j].xCoord, HighestPoint, nodes[i, j].zCoord), Tilesize / 16, Vector3.down, dist);
                    }
                    
                    float maxY = -Mathf.Infinity;

                    foreach (RaycastHit h in hit)
                    {
                        if (DisallowedTags.Contains(h.transform.tag))
                        {
                            if (h.point.y > maxY)
                            {
                                //It is a disallowed walking tile, make it false
                                nodes[i, j].walkable = false;
                                nodes[i, j].yCoord = h.point.y;
                                maxY = h.point.y;
                            }
                        }
                        else if (IgnoreTags.Contains(h.transform.tag))
                        {
                            //Do nothing we ignore these tags
                        }
                        else
                        {
                            if (h.point.y > maxY)
                            {
                                //It is allowed to walk on this tile, make it walkable!
                                nodes[i, j].walkable = true;
                                nodes[i, j].yCoord = h.point.y;
                                maxY = h.point.y;
                            }
                        }
                    }
                }
            }
        }
    }

    private List<Vector2> DynamicFindClosestNodes(List<Vector3> vList)
    {      
        List<Vector2> returnList = new List<Vector2>();
        foreach (Vector3 pos in vList)
        {
            int x = (MapStartPosition.x < 0F) ? Mathf.FloorToInt(((pos.x + Mathf.Abs(MapStartPosition.x)) / Tilesize)) : Mathf.FloorToInt((pos.x - MapStartPosition.x) / Tilesize);
            int z = (MapStartPosition.y < 0F) ? Mathf.FloorToInt(((pos.z + Mathf.Abs(MapStartPosition.y)) / Tilesize)) : Mathf.FloorToInt((pos.z - MapStartPosition.y) / Tilesize);

            if (x >= 0 && x < nodes.GetLength(0) && z >= 0 && z < nodes.GetLength(1))
            {
                if (nodes[x, z].walkable)
                {
                    nodes[x, z].walkable = false;
                    returnList.Add(new Vector2(x, z));
                }
            }
        }

        return returnList;
    }

    #endregion


    void OnDrawGizmos() {
        Vector3 startPos = new Vector3(MapStartPosition.x, 0, MapStartPosition.y);
        Vector3 endPos = new Vector3(MapEndPosition.x, 0, MapEndPosition.y);

        if(showGridGizmos)
           Gizmos.DrawWireCube(startPos, endPos);
    }
}
