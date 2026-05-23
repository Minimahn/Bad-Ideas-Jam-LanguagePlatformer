using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TileManager : MonoBehaviour
{
    
    public BasicTileData[] tileDatas;
    public GameObject plr;
    public Tilemap physicalMap;
    private Dictionary<ScriptableTile, BasicTileData> baseTileDatas;



    void Awake()
    {
        baseTileDatas = new Dictionary<ScriptableTile, BasicTileData>();

        foreach (var tileData in tileDatas)
        {
            baseTileDatas.Add(tileData.tile, tileData);
        }

    }


    void Update() //get player position & block they are standing on for this test
    {
        Vector3Int cellPos = physicalMap.WorldToCell(new Vector3(plr.transform.position.x, plr.transform.position.y - 1.5f, plr.transform.position.z));
        TileBase tile = physicalMap.GetTile(cellPos);
        baseTileCheck(cellPos, (ScriptableTile)tile);
    }

    private void baseTileCheck(Vector3Int pos, ScriptableTile tile)
    {
        
        if (baseTileDatas.ContainsKey(tile)) //easiest to change tile with another
        {
            tile.SetSprite("glow", physicalMap, pos); //temporary test
        }   
    }

}
