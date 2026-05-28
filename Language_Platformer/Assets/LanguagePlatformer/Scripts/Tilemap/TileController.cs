using System;
using System.Collections.Generic;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UIElements;

public class TileController : MonoBehaviour
{
    private List<Vector3Int> glowingTilePositions = new List<Vector3Int>(); //save positions of tiles to remember
    public GameObject player;
    private Tilemap physical;

    void Start()
    {
        physical = transform.Find("Physical").GetComponent<Tilemap>();
    }

    void Update()
    {
        //go through all checks -- can make this nicer later

        if (glowingTilePositions.Count > 0)
            checkGlowing();

    }


    private void checkGlowing()
    {
        List<int> spots = new List<int>();
        for (int i = 0; i < glowingTilePositions.Count; i++)
        {
            // if out of player render dist, remove from list
            // for now... for every player and monster and its spells hierarchy  (spell, to do)
            float distToPlr = math.abs((glowingTilePositions[i] - player.transform.position).magnitude);
            if (distToPlr > 4f)
            {
                ScriptableTile tile = (ScriptableTile)physical.GetTile(glowingTilePositions[i]);
                // I know its glowing if its here
                tile.SetSprite("base", physical, glowingTilePositions[i]);
                spots.Add(i);
            }
        }
        for (int i = spots.Count - 1; i >= 0; i--)
            glowingTilePositions.RemoveAt(spots[i]);
    }

    public void AddToGlow(List<Vector3Int> tiles)
    {
        foreach (Vector3Int tilepos in tiles)
        {
            if (!glowingTilePositions.Contains(tilepos)) //tile could already be there, if not add
                glowingTilePositions.Add(tilepos);
        }
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
        bounds.SetMinMax(new Vector3Int((int)(pos.x - dist), (int)(pos.y - 1), (int)pos.z),
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
