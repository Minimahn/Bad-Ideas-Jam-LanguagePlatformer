using UnityEngine;
using UnityEngine.SceneManagement;

public class Leveler : MonoBehaviour, Interactable
{
    
    private SpriteRenderer spriteRenderer;
    private Collider2D coll;
    private AudioSource audioSource;
    private PlayerMovement movement;
    public string nextScene;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        coll = GetComponent<Collider2D>();
        audioSource = GetComponent<AudioSource>();

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
        if (movement != null) {
            SceneManager.LoadScene(nextScene);
        }
    }
}
