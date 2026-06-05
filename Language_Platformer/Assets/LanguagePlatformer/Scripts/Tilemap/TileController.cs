using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Experimental.GlobalIllumination;
using UnityEngine.Tilemaps;
using UnityEngine.UIElements;

public class TileController : MonoBehaviour
{
    private List<Vector3Int> glowingTiles = new List<Vector3Int>(); //save positions of tiles to remember
    // dont need list for burning or freezing, bc they apply and instant decay at certain rate
    private List<Vector3Int> burningTiles = new List<Vector3Int>();
    private List<Vector3Int> freezingTiles = new List<Vector3Int>();
    private float freezeTime = 5f;
    private float burnTime = 3f;

    public GameObject player;
    private Tilemap physical;
    private Tilemap special;
    private Tilemap lighting;
    private Dictionary<ScriptableTile, BasicTileData> baseTileDatas;
    
    void Start()
    {
        physical = transform.Find("Physical").GetComponent<Tilemap>();
        special = transform.Find("Special").GetComponent<Tilemap>();
        lighting = transform.Find("Lighting").GetComponent<Tilemap>();
        baseTileDatas = new Dictionary<ScriptableTile, BasicTileData>();

        foreach (var tileData in GetComponent<PlayerTileManager>().tileDatas)
        {
            baseTileDatas.Add(tileData.tile, tileData);
        }
    }

    void Update()
    {
        //go through all checks -- can make this nicer later

        if (glowingTiles.Count > 0)
            checkGlowing();

    }


    private void checkGlowing() //necessary for glowing bc. range persist based
    {
        List<int> spots = new List<int>();
        ScriptableTile tile = ScriptableObject.CreateInstance<ScriptableTile>();
        for (int i = 0; i < glowingTiles.Count; i++)
        {
            // if out of player render dist, remove from list
            // for now... for every player and monster and its spells hierarchy  (spell, to do)
            float distToPlr = math.abs((glowingTiles[i] - player.transform.position).magnitude);
            if (distToPlr > 3.2f || !player.GetComponent<PlayerVariables>().getGlow())
            {
                tile = (ScriptableTile)physical.GetTile(glowingTiles[i]);
                if (tile != null)
                    tile.SetSprite("base", physical, glowingTiles[i]);
                tile = (ScriptableTile)special.GetTile(glowingTiles[i]);
                if (tile != null)
                    tile.SetSprite("base", special, glowingTiles[i]);
                tile = (ScriptableTile)lighting.GetTile(glowingTiles[i]);
                if (tile != null)
                    tile.SetSprite("base", lighting, glowingTiles[i]);
                spots.Add(i);

            }
        }
        for (int i = spots.Count - 1; i >= 0; i--)
            glowingTiles.RemoveAt(spots[i]);
    }

    public void AddToGlow(List<Vector3Int> tiles)
    {
        ScriptableTile tile = ScriptableObject.CreateInstance<ScriptableTile>();
        foreach (Vector3Int tilepos in tiles)
        {
            
            if (!glowingTiles.Contains(tilepos)) //tile could already be there, if not add
            {
                tile = (ScriptableTile)physical.GetTile(tilepos);
                if (tile != null)
                    tile.SetSprite("glow", physical, tilepos);
                tile = (ScriptableTile)special.GetTile(tilepos);
                if (tile != null)
                    tile.SetSprite("glow", special, tilepos);
                tile = (ScriptableTile)lighting.GetTile(tilepos);
                if (tile != null)
                    tile.SetSprite("glow", lighting, tilepos);
                glowingTiles.Add(tilepos);
            }
        }
    }

    IEnumerator burnTile(Vector3Int pos)
    {
        List<Vector3Int> surrounding = getTilesWithinRadius(pos, 1f);

        float incrementer = 0f; 
        bool checkNearby = false;
        ScriptableTile tile = (ScriptableTile)physical.GetTile(pos);
        tile.SetSprite("burn", physical, pos);
        while (incrementer < burnTime)
        {
            if (incrementer > burnTime / 3f)
                tile.SetSprite("burn2", physical, pos);

            if (incrementer > burnTime * (3f / 4f))
                tile.SetSprite("burn3", physical, pos);
                
            if (incrementer > burnTime * (1f / 2f) && !checkNearby)
            {
                List<Vector3Int> nearbyBurns = new List<Vector3Int>();
                checkNearby = true;
                foreach (Vector3Int spot in surrounding)
                {
                    ScriptableTile near = (ScriptableTile)physical.GetTile(spot);
                    if (near != null && spot != pos)
                    {
                        if (baseTileDatas[near].flammable)
                        nearbyBurns.Add(spot);
                    }

                }
                AddToBurn(nearbyBurns);
            }

            incrementer += Time.deltaTime;
            yield return null;
        }
        burningTiles.Remove(pos);
        physical.SetTile(pos, null); //delete tile
        yield return null;
    }
    public void AddToBurn(List<Vector3Int> tiles)
    {
        ScriptableTile tile = ScriptableObject.CreateInstance<ScriptableTile>();
        foreach (Vector3Int tilepos in tiles)
        {
            if (!burningTiles.Contains(tilepos))
            {
                burningTiles.Add(tilepos);
                StartCoroutine(burnTile(tilepos));
            }
        }
    }

    IEnumerator FreezeTile(Vector3Int pos)
    {
        float incrementer = 0f; 
        ScriptableTile newTile = GetComponent<PlayerTileManager>().tileDatas[8].tile;
        physical.SetTile(pos, newTile);

        while (incrementer < freezeTime)
        {
            incrementer += Time.deltaTime;
            yield return null;
        }
        freezingTiles.Remove(pos);
        physical.SetTile(pos, null); //delete tile
        yield return null;
    }

    public void AddToFreeze(List<Vector3Int> tiles)
    {
        ScriptableTile tile = ScriptableObject.CreateInstance<ScriptableTile>();
        foreach (Vector3Int tilepos in tiles)
        {
            if (!freezingTiles.Contains(tilepos))
            {
                freezingTiles.Add(tilepos);
                StartCoroutine(FreezeTile(tilepos));
            }
        }
    }


    //getters
    public static List<Vector3Int> getTilesWithinRadius(Vector3 pos, float dist) {
        List<Vector3Int> tiles = new List<Vector3Int>();
        BoundsInt bounds = new BoundsInt();
        bounds.SetMinMax(new Vector3Int((int)(pos.x - dist), (int)(pos.y - dist), (int)pos.z),
        new Vector3Int((int)(pos.x + dist+1), (int)(pos.y + dist+1), (int)pos.z + 1));

        foreach (var pt in bounds.allPositionsWithin)
        {
            tiles.Add(pt);
        }
        return tiles;
    }
    public static List<Vector3Int> getRowTilesWithinRadius(Vector3 pos, float dist)
    {
        List<Vector3Int> tiles = new List<Vector3Int>();
        BoundsInt bounds = new BoundsInt();
        bounds.SetMinMax(new Vector3Int((int)(pos.x - dist - 1), (int)(pos.y - 1), (int)pos.z),
        new Vector3Int((int)(pos.x + dist), (int)(pos.y), (int)pos.z + 1));

        foreach (var pt in bounds.allPositionsWithin)
        {
            tiles.Add(pt);
        }
        return tiles;
    }

    public static List<Vector3Int> getColumnTilesWithinRadius(Vector3 pos, float dist)
    {
        List<Vector3Int> tiles = new List<Vector3Int>();
        BoundsInt bounds = new BoundsInt();
        bounds.SetMinMax(new Vector3Int((int)pos.x, (int)(pos.y - dist), (int)pos.z),
        new Vector3Int((int)(pos.x + 1), (int)(pos.y + dist), (int)pos.z + 1));

        foreach (var pt in bounds.allPositionsWithin)
        {
            tiles.Add(pt);
        }
        return tiles;
    }

}
