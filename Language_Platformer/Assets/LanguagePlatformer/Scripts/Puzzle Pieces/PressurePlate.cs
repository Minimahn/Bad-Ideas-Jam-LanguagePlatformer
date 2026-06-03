using UnityEngine;

[CreateAssetMenu(menuName = "2D/Puzzles/PressurePlate")]
public class PressurePlate : ScriptableObject
{

    public Sprite baseSprite;
    public string[] spriteKeys;
    public Sprite[] spriteValues;
    public bool permanent;
    public float loadNec;
    public float fallTime;
    public float riseTime;
    
}
