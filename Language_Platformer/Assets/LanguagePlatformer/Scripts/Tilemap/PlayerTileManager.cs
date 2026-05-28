using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class PlayerTileManager : MonoBehaviour
{
    
    public BasicTileData[] tileDatas;
    public GameObject plr;
    private Tilemap physicalMap;
    private TileController tileController;
    private Dictionary<ScriptableTile, BasicTileData> baseTileDatas;



    void Awake()
    {
        baseTileDatas = new Dictionary<ScriptableTile, BasicTileData>();

        foreach (var tileData in tileDatas)
        {
            baseTileDatas.Add(tileData.tile, tileData);
        }

    }

    void Start()
    {
        tileController = GetComponent<TileController>();
        physicalMap = transform.Find("Physical").GetComponent<Tilemap>();
    }


    void Update() //check nearby tiles to player
    {
        List<Vector3Int> closeTilePositions = new List<Vector3Int>();
        BoundsInt bounds = new BoundsInt();
        bounds.SetMinMax(new Vector3Int((int)plr.transform.position.x - 3, (int)plr.transform.position.y - 3, (int)plr.transform.position.z),
        new Vector3Int((int)plr.transform.position.x + 3, (int)plr.transform.position.y + 3, (int)plr.transform.position.z + 1));
        
        foreach (var pt in bounds.allPositionsWithin)
        {
            closeTilePositions.Add(pt); //list of Vector3Int
        }

        if (closeTilePositions != null)
            baseTileCheck(closeTilePositions);
    }

    private void baseTileCheck(List<Vector3Int> positions)
    {
        List<Vector3Int> nearbyBaseTiles = new List<Vector3Int>();
        List<Vector3Int> glowingTiles = new List<Vector3Int>();

        for (int i = 0; i < positions.Count; i++)
        {
            if (physicalMap.GetTile(positions[i]) != null)
            {
                ScriptableTile tile = (ScriptableTile)physicalMap.GetTile(positions[i]);
                if (baseTileDatas.ContainsKey(tile))
                    nearbyBaseTiles.Add(positions[i]);
            }
        }

        foreach (Vector3Int tilepos in nearbyBaseTiles)
        {
            print("Four");
            ScriptableTile tile  = (ScriptableTile)physicalMap.GetTile(tilepos);
            if (baseTileDatas[tile].glowable && plr.GetComponent<PlayerVariables>().getGlow())
            {
                tile.SetSprite("glow", physicalMap, tilepos);
                glowingTiles.Add(tilepos);
            }
        }
 
        tileController.AddToGlow(glowingTiles);
    }

}
