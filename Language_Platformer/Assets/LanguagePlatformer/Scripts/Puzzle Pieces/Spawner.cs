using UnityEngine;

public class Spawner : MonoBehaviour, Interactable
{
    private GameObject player;
    private PlayerMovement movement;
    private LevelInfo levelInfo;
    private Collider2D coll;
    void Start()
    {
        coll = GetComponent<Collider2D>();
        player = GameObject.Find("Player");
        levelInfo = GameObject.Find("LevelManager").GetComponent<LevelInfo>();
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        movement = other.GetComponent<PlayerMovement>();
        if (movement != null)
        {
            movement.AddInput(this);
        }
    }

    void OTriggerExit2D(Collider2D other)
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
            movement.ShowText("Spawn Point Set.");
            levelInfo.setSpawnPoint(new Vector3(transform.position.x, transform.position.y + 1f, transform.position.z));
        }
    }

}
