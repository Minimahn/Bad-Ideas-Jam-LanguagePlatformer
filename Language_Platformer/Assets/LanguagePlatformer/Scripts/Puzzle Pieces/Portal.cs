using UnityEngine;
using System.Collections.Generic;

public class Portal : Activator, Interactable
{
    private SpriteRenderer spriteRenderer;
    private Collider2D coll;
    private Animator animator;
    private PlayerMovement movement = null;
    private AudioSource audioSource;
    public Sprite offPortal;
    public List<Sprite> onPortal;
    public List<Activator> activators;
    public Portal destination;
    private bool active = false;
    private bool activatorsReady = false;


    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        coll = GetComponent<Collider2D>();
        animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
    }

    void Update()
    {
        activatorsReady = true;
        foreach (var activator in activators)
        {
            if (!activator.GetActive())
            {
                activatorsReady = false;
                if (active)
                {
                    DeactivePortal();
                }
            }
        }
        if (!active && activatorsReady)
        {
            ActivatePortal();
        }
    }


    void OnTriggerEnter2D(Collider2D other)
    {
        movement = other.transform.GetComponent<PlayerMovement>();
        if (movement != null)
        {
            movement.AddInput(this);
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        movement = other.transform.GetComponent<PlayerMovement>();
        if (movement != null)
        {
            movement.RemoveInput(this);
        }
    }

    private void DeactivePortal()
    {
        active = false;
        animator.Play("Deactivated");
    }
    private void ActivatePortal()
    {
        active = true;
        animator.Play("Activated");
    }

    public void Interact()
    {
        if (active && movement != null)
        {
            //teleport player
            audioSource.Play();
            movement.transform.position = new Vector3(destination.transform.position.x, destination.transform.position.y + 0.2f, destination.transform.position.z);
        }
    }
    public override bool GetActive()
    {
        return active;
    }
}
