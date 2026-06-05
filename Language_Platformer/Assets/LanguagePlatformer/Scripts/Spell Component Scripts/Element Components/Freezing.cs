using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections.Generic;

public class Freezing : MonoBehaviour
{
    private Collider2D coll;
    private GameObject grid;
    private Tilemap physical;
    private Tilemap special;
    private TileController tileController;
    private Dictionary<ScriptableTile, BasicTileData> baseTileDatas;
    private Vector2 p1;
    private Vector2 p2;
    private Vector2 p3;
    private Decay decay;

    private GameObject wand;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        wand = GameObject.Find("Wand");
        coll = GetComponent<Collider2D>();
        grid = GameObject.Find("Grid").gameObject;
        physical = grid.transform.Find("Physical").GetComponent<Tilemap>();
        special = grid.transform.Find("Special").GetComponent<Tilemap>();
        baseTileDatas = new Dictionary<ScriptableTile, BasicTileData>();
        tileController = grid.GetComponent<TileController>();
        decay = GetComponent<Decay>();

        foreach (var tileData in grid.GetComponent<PlayerTileManager>().tileDatas)
        {
            baseTileDatas.Add(tileData.tile, tileData);
        }
        

    }

    // copying playertilemanager rn
    void FixedUpdate()
    {
        if (!decay.IsSpellActive())
        {

        transform.rotation = Quaternion.Euler(0, 0, wand.transform.rotation.eulerAngles.z);
        List<Vector3Int> closeTilePositions = new List<Vector3Int>();
        List<Vector3Int> freezingTiles = new List<Vector3Int>();


        float leftAngle = transform.rotation.eulerAngles.z + 180f;
        float coneAngle = 30f; //test
        float distance = 5f;
        float halfAngle = coneAngle / 2f;
        
        float upperAngleRad = (leftAngle + halfAngle) * Mathf.Deg2Rad;
        float lowerAngleRad = (leftAngle - halfAngle) * Mathf.Deg2Rad;

        p1 = new Vector2(transform.position.x, transform.position.y);
        p2 = p1 + new Vector2(Mathf.Cos(upperAngleRad), Mathf.Sin(upperAngleRad)) * distance;
        p3 = p1 + new Vector2(Mathf.Cos(lowerAngleRad), Mathf.Sin(lowerAngleRad)) * distance;

        print(p1 + " " + p2 + " " + p3);    

        BoundsInt bounds = new BoundsInt();
        bounds.SetMinMax(new Vector3Int((int)transform.position.x - 4, (int)transform.position.y - 4, (int)transform.position.z),
        new Vector3Int((int)transform.position.x + 5, (int)transform.position.y + 5, (int)transform.position.z + 1));

        foreach (var pt in bounds.allPositionsWithin)
        {
            //if pt is in the triangle


            ScriptableTile tile = (ScriptableTile)special.GetTile(pt);
            if (tile != null && IsPointInCone(pt)) 
            {
                
                if (baseTileDatas[tile].freezable)
                {
                    freezingTiles.Add(pt);
                }
            }
        }
        tileController.AddToFreeze(freezingTiles);

        }
    }

    private bool IsPointInCone(Vector3Int pt)
    {
        Vector2 p = new Vector2(pt.x + 0.5f, pt.y + 0.5f);// this is center of tile
        float dX = p.x - p3.x;
        float dY = p.y - p3.y;
        float dX21 = p2.x - p1.x;
        float dY21 = p2.y - p1.y;
        float dX01 = p1.x - p3.x;
        float dY01 = p1.y - p3.y;
        
        float m = (dX21 * dY01) - (dY21 * dX01);
        
        if (Mathf.Abs(m) < 0.0001f) return false; // Avoid division by zero for degenerate triangles

        float w1 = (dX01 * dY) - (dY01 * dX);
        w1 /= m;

        float w2 = (dX21 * dY) - (dY21 * dX);
        w2 /= m;

        return w1 >= 0f && w2 >= 0f && (w1 + w2) <= 1f;
    }
}
