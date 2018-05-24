using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class MapEdit : MonoBehaviour {
    public TileBase a;
    public Tilemap tileMap;
    // Use this for initialization
    void Start () {
        tileMap.SetTile(new Vector3Int(1,1,1), a);
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
