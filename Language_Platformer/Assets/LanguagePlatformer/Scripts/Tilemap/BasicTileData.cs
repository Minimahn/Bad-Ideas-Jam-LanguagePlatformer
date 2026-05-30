using System;
using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu]
public class BasicTileData : ScriptableObject
{
    

    public ScriptableTile tile;

    public float terrainSpeed, friction;
    public bool destructible, flammable, freezable, permeable, glowable;

}
