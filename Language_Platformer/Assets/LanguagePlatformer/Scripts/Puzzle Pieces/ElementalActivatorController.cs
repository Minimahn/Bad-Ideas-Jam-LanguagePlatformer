using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElementalActivatorController : Activator
{
    public ElementalActivator elementalActivator;
    private Collider2D coll;
    private SpriteRenderer spriteRenderer;
    private Dictionary<string, Sprite> spritePairs = new Dictionary<string, Sprite>();
    private Sprite baseSprite;
    private string spellType; //could be an enum setup for spells
    private float duration;
    private bool active = false;
    private bool restart = false;
    void Start()
    {
        coll = GetComponent<Collider2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        baseSprite = elementalActivator.baseSprite;
        spellType = elementalActivator.spellType;
        duration = elementalActivator.duration;

        for (int i = 0; i < elementalActivator.spriteKeys.Length; i++)
        {
            spritePairs.Add(elementalActivator.spriteKeys[i], elementalActivator.spriteValues[i]);
        }

        spriteRenderer.sprite = baseSprite;
    }


    private void OnTriggerEnter2D(Collider2D other)
    {
        print(other);
        if (other.transform.tag == "Spell" && other.transform.GetComponent(spellType) != null)
        {
            restart = true;
            spriteRenderer.sprite = spritePairs["on"];
            Destroy(other.gameObject);
            if (!active)
                active = true;
                StartCoroutine(ActivatorTime());
        }
    }

    override public bool GetActive()
    {
        return active;
    }

    IEnumerator ActivatorTime()
    {
        float incrementer = 0f;

        while (incrementer < duration)
        {
            if (restart)
                incrementer = 0f;
                restart = false;
            incrementer += Time.deltaTime;
            yield return null;
        }
        active = false;
        spriteRenderer.sprite = spritePairs["off"];

    }
}
