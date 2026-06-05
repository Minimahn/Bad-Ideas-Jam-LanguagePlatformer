using UnityEngine;

public class Sign : MonoBehaviour, Interactable
{
    
    private SpriteRenderer spriteRenderer;
    private Collider2D coll;
    private PlayerMovement movement;
    public string message;


    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        coll = GetComponent<Collider2D>();
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        movement = other.GetComponent<PlayerMovement>();
        if (movement != null)
        {
            movement.AddInput(this);
        }
    }
    void OnTriggerExit2D(Collider2D other)
    {
        movement = other.GetComponent<PlayerMovement>();
        if (movement != null)
        {
            movement.RemoveInput(this);
        }
    }

    public void Interact()
    {
        if (movement != null)
        {
            movement.ShowText(message);
        }
    }
}
