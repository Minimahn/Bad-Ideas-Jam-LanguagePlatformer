using System.Collections.Generic;
using System.ComponentModel.Design;
using UnityEngine;

public class Door : Activator, Interactable
{
    private bool opened = false;
    private bool activatorsReady = false;

    private SpriteRenderer spriteRenderer;
    public Sprite openedDoor;
    public Sprite closedDoor;
    private Collider2D coll;
    private PlayerMovement movement;

    public List<Activator> activators;
    public AudioClip doorOpen;
    public AudioClip doorClose;
    private AudioSource audioSource;

    void Start()
    {
        coll = GetComponent<Collider2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        audioSource = GetComponent<AudioSource>();
        audioSource.playOnAwake = false;
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
        audioSource.clip = doorClose;
        audioSource.Play();

    }
    private void OpenDoor()
    {
        coll.enabled = false;
        spriteRenderer.sprite = openedDoor;
        audioSource.clip = doorOpen;
        audioSource.Play();
    }

    
    void OnCollisionEnter2D(Collision2D other)
    {
        movement = other.transform.GetComponent<PlayerMovement>();
        if (movement != null)
        {
            movement.AddInput(this);
        }
    }
    void OnCollisionExit2D(Collision2D other)
    {
        movement = other.transform.GetComponent<PlayerMovement>();
        if (movement != null)
        {
            movement.RemoveInput(this);
        }
    }
    

    public void Interact()
    {
        string message = "You'll need to activate this door to get through, look for something nearby!";
        if (!opened && movement != null)
        {
            movement.ShowText(message);
        }
    }
    override public bool GetActive()
    {
        return opened;
    }
}
