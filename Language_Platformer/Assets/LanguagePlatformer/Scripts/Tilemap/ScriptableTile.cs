using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

#if UNITY_EDITOR
using UnityEditor;
#endif

[CreateAssetMenu(menuName = "2D/Tiles/ScriptableTile")]
public class ScriptableTile : TileBase, ISerializationCallbackReceiver
{
    [Header("Various Sprites")]
    public Sprite baseSprite;
    //Temporary structure - two matched arrays
    public Sprite[] sprites;
    public string[] activations;
    private string currentActivation = "";


    //Unity Editor only code... thank you temporary vibe code! shocker it doesnt work right, fix tmrw!

    private void OnEnable()
    {
#if UNITY_EDITOR
        // Hook into Unity's system event that tracks Play/Stop buttons
        EditorApplication.playModeStateChanged += HandlePlayModeChanges;
#endif
    }

    private void OnDisable()
    {
#if UNITY_EDITOR
        // Clean up the event hook when this asset is unloaded
        EditorApplication.playModeStateChanged -= HandlePlayModeChanges;
#endif
    }

#if UNITY_EDITOR
    private void HandlePlayModeChanges(PlayModeStateChange state)
    {
        // The instant you hit 'Stop' (Exiting Play Mode), wipe the data!
        if (state == PlayModeStateChange.ExitingPlayMode)
        {
            currentActivation = "";
        }
    }
#endif

//Unity Editor code over --

    public void OnAfterDeserialize()
    {
        currentActivation = "";
    }
    public void OnBeforeSerialize() {}


    public override bool StartUp(Vector3Int position, ITilemap tilemap, GameObject go)
    {
        return base.StartUp(position, tilemap, go);
    }
    public override void GetTileData(Vector3Int position, ITilemap tilemap, ref TileData tileData)
    {
        // default flags so the tile paints normally
        tileData.flags = TileFlags.LockColor | TileFlags.LockTransform;
        tileData.colliderType = Tile.ColliderType.Sprite;

        
        // if currentActivation, sprite equals that
        if (!string.IsNullOrEmpty(currentActivation) && activations.Contains(currentActivation))
        {
            int index = System.Array.IndexOf(activations, currentActivation);
            tileData.sprite = sprites[index];
        }
        else
        {
            tileData.sprite = baseSprite; 
        }
    }

    
    // public method to setSprite instead of resetting Tile (reason for this)
    public void SetSprite(string s, Tilemap targetTilemap, Vector3Int position)
    {
        if (activations.Contains(s))
        {
            currentActivation = s;
            // this is needed to see visual change, calls GetTileData basically, I think?
            targetTilemap.RefreshTile(position);
        }
    }
    
}
