using UnityEngine;

public class RopeIgnite : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.GetComponent<Fire>() != null)
        {
            gameObject.GetComponentInParent<Rope>().Ignited(gameObject);
        }
    }
}
