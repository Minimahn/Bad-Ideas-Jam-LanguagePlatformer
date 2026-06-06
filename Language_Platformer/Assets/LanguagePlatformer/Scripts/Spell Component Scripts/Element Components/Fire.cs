using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections.Generic;
using Unity.Mathematics;

public class Fire : MonoBehaviour
{

    private Collider2D coll;
    private GameObject grid;
    private Tilemap physical;
    private TileController tileController;
    private Dictionary<ScriptableTile, BasicTileData> baseTileDatas;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        coll = GetComponent<Collider2D>();
        grid = GameObject.Find("Grid").gameObject;
        physical = grid.transform.Find("Physical").GetComponent<Tilemap>();
        baseTileDatas = new Dictionary<ScriptableTile, BasicTileData>();
        tileController = grid.GetComponent<TileController>();

        foreach (var tileData in grid.GetComponent<PlayerTileManager>().tileDatas)
        {
            baseTileDatas.Add(tileData.tile, tileData);
        }
    }

    // copying playertilemanager rn
    void Update()
    {
        List<Vector3Int> closeTilePositions = new List<Vector3Int>();
        List<Vector3Int> burningTiles = new List<Vector3Int>();

        BoundsInt bounds = new BoundsInt();
        bounds.SetMinMax(new Vector3Int((int)(transform.position.x - 1), (int)(transform.position.y - 1), (int)transform.position.z-1),
        new Vector3Int((int)(transform.position.x+1), (int)(transform.position.y+1), (int)transform.position.z + 1));

        foreach (var pt in bounds.allPositionsWithin)
        {
            ScriptableTile tile = (ScriptableTile)physical.GetTile(pt);
            if (tile != null) 
            {
                if (baseTileDatas[tile].flammable)
                    burningTiles.Add(pt);
            }
        }
        tileController.AddToBurn(burningTiles);
    }

}
