using UnityEngine;

[CreateAssetMenu(menuName = "2D/Puzzles/ElementalActivator")]
public class ElementalActivator : ScriptableObject
{
    public Sprite baseSprite;
    public string[] spriteKeys;
    public Sprite[] spriteValues;
    public float duration;
    public string spellType;

}
