using UnityEngine;
using System.Collections;

public class TerrainWidthHeight : MonoBehaviour {

    GameObject terrainObj;

    void Start() {
        terrainObj = GameObject.FindGameObjectWithTag("Landscape");
        Terrain landscape = terrainObj.GetComponent<Terrain>();

        Vector3 terrainSize = landscape.terrainData.size;
        print("Terrain Width: "+terrainSize.x+"\nTerrain Length: "+terrainSize.z);
    }
}
