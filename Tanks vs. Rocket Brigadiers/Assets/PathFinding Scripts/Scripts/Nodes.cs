

public class Nodes {
    public int x            = 0;
    public int y            = 0;
    public float yCoord     = 0;
    public float xCoord     = 0;
    public float zCoord     = 0;
    public int ID           = 0;
    public bool walkable    = true;
    public Nodes parent      = null;

    public int F            = 0;
    public int H            = 0;
    public int G            = 0;

    //Use for faster look ups
    public int sortedIndex = -1;

    public Nodes(int indexX, int indexY, float height, int idValue, float xcoord, float zcoord, bool w, Nodes p = null)
    {
        x = indexX;
        y = indexY;
        yCoord = height;
        ID = idValue;
        xCoord = xcoord;
        zCoord = zcoord;
        walkable = w;
        parent = p;
        F = 0;
        G = 0;
        H = 0;
    }
}

