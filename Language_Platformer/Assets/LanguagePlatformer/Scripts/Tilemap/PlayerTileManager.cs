using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using Unity.Mathematics;
using NUnit.Framework;

public class PlayerTileManager : MonoBehaviour
{
    
    public BasicTileData[] tileDatas;
    private GameObject plr;
    private Tilemap physical;
    private Tilemap special;
    private Tilemap lighting;
    private TileController tileController;
    private PlayerMovement movement;
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
        movement = GameObject.Find("Player").GetComponent<PlayerMovement>();
        tileController = GetComponent<TileController>();
        physical = transform.Find("Physical").GetComponent<Tilemap>();
        special = transform.Find("Special").GetComponent<Tilemap>();
        lighting = transform.Find("Lighting").GetComponent<Tilemap>();
        plr = GameObject.Find("Player");
    }


    void Update() //check nearby tiles to player
    {

        List<Vector3Int> closeTilePositions = new List<Vector3Int>();
        BoundsInt bounds = new BoundsInt();
        bounds.SetMinMax(new Vector3Int((int)plr.transform.position.x - 3, (int)plr.transform.position.y - 5, (int)plr.transform.position.z),
        new Vector3Int((int)plr.transform.position.x + 3, (int)plr.transform.position.y + 2, (int)plr.transform.position.z + 1));
        
        foreach (var pt in bounds.allPositionsWithin)
        {
            Vector3Int closestPt = new Vector3Int(pt.x, pt.y, pt.z);
            if (closestPt.x < plr.transform.position.x)
                closestPt.x += 1;
            if (closestPt.y < plr.transform.position.y)
                closestPt.y += 1;
            float distToPlr = math.abs((closestPt - plr.transform.position).magnitude);
            if (distToPlr < 8.1f)
                closeTilePositions.Add(pt); //list of Vector3Int
        }

        baseTileCheck(closeTilePositions);
        WaterCheck(TileController.getColumnTilesWithinRadius(plr.transform.position, 0.9f));
    }


    private void WaterCheck(List<Vector3Int> positions)
    {
        bool foundWater = true;
        foreach (var pos in positions)
        {
            ScriptableTile tile = (ScriptableTile)special.GetTile(pos);
            if (tile == null) //if its not null from special, I know its water
            {
                foundWater = false;
            }
        }
        if (positions.Count > 0)
            movement.SetSub(foundWater);
    }

    private void baseTileCheck(List<Vector3Int> positions)
    {
        List<Vector3Int> nearbyTiles = new List<Vector3Int>();
        List<Vector3Int> glowingTiles = new List<Vector3Int>();
        ScriptableTile tile = ScriptableObject.CreateInstance<ScriptableTile>();
        bool priorNear = false;
        bool priorGlow = false;

        for (int i = 0; i < positions.Count; i++)
        {
            if (physical.GetTile(positions[i]) != null)
            {
                tile = (ScriptableTile)physical.GetTile(positions[i]);
                if (baseTileDatas.ContainsKey(tile)) //might be removable if all tiles fall into place
                {    
                    priorNear = true;
                    nearbyTiles.Add(positions[i]);
                    if (baseTileDatas[tile].glowable && plr.GetComponent<PlayerVariables>().getGlow())
                    {
                        priorGlow = true;
                        glowingTiles.Add(positions[i]);
                    }
                }
            }

            if (special.GetTile(positions[i]) != null)
            {
                tile = (ScriptableTile)special.GetTile(positions[i]);
                if (baseTileDatas.ContainsKey(tile))
                {
                    if (!priorNear)
                    {
                        priorNear = true;
                        nearbyTiles.Add(positions[i]);
                    }
                    if (baseTileDatas[tile].glowable && plr.GetComponent<PlayerVariables> ().getGlow() && !priorGlow)
                    {
                        priorGlow = true;
                        glowingTiles.Add(positions[i]);
                    }
                }
            }

            if (lighting.GetTile(positions[i]) != null)
            {
                tile = (ScriptableTile)lighting.GetTile(positions[i]);
                if (baseTileDatas.ContainsKey(tile))
                {
                    if (!priorNear)
                    {
                        priorNear = true;
                        nearbyTiles.Add(positions[i]);
                    }
                    if (baseTileDatas[tile].glowable && plr.GetComponent<PlayerVariables> ().getGlow() && !priorGlow)
                    {
                        priorGlow = true;
                        glowingTiles.Add(positions[i]);
                    }
                }
            }

            priorNear = false;
            priorGlow = false;
        }
 
        
        tileController.AddToGlow(glowingTiles);
    }

}
