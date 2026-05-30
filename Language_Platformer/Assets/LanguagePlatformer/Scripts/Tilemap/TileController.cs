using System;
using System.Collections.Generic;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UIElements;

public class TileController : MonoBehaviour
{
    private List<Vector3Int> glowingTiles = new List<Vector3Int>(); //save positions of tiles to remember
    // dont need list for burning or freezing, bc they apply and instant decay at certain rate
    private float freezeTime = 5f;
    private float burnTime = 5f;

    public GameObject player;
    private Tilemap physical;
    private Tilemap special;
    private Tilemap lighting;

    void Start()
    {
        physical = transform.Find("Physical").GetComponent<Tilemap>();
        special = transform.Find("Special").GetComponent<Tilemap>();
        lighting = transform.Find("Lighting").GetComponent<Tilemap>();
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
        ScriptableTile tile = new ScriptableTile();
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
        ScriptableTile tile = new ScriptableTile();
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

    public void AddToBurn(List<Vector3Int> tiles)
    {
        
    }

    public void AddToFreeze(List<Vector3Int> tiles)
    {
        
    }


    //getters
    public static List<Vector3Int> getTilesWithinRadius(Vector3 pos, float dist) {
        List<Vector3Int> tiles = new List<Vector3Int>();
        BoundsInt bounds = new BoundsInt();
        bounds.SetMinMax(new Vector3Int((int)(pos.x - dist), (int)(pos.y - dist), (int)pos.z),
        new Vector3Int((int)(pos.x + dist), (int)(pos.y + dist), (int)pos.z + 1));

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
