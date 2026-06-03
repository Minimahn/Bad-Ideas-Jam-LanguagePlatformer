using System.Collections.Generic;
using UnityEngine;

public class Door : Activator
{
    private bool opened = false;
    private bool activatorsReady = false;

    private SpriteRenderer spriteRenderer;
    public Sprite openedDoor;
    public Sprite closedDoor;
    private Collider2D coll;

    public List<Activator> activators;

    void Start()
    {
        coll = GetComponent<Collider2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }
    void Update()
    {
        activatorsReady = true;
        foreach (var activator in activators)
        {
            if (!activator.GetActive())
            {
                activatorsReady = false;
                if (opened)
                {
                    opened = false;
                    CloseDoor();
                }
            }
        }

        if (!opened && activatorsReady)
        {
            opened = true;
            OpenDoor();
        }
    }

    private void CloseDoor()
    {
        coll.enabled = true;
        spriteRenderer.sprite = closedDoor;

    }
    private void OpenDoor()
    {
        coll.enabled = false;
        spriteRenderer.sprite = openedDoor;
    }

    override public bool GetActive()
    {
        return opened;
    }
}
