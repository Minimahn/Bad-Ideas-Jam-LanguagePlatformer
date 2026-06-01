using UnityEngine;
using UnityEngine.AdaptivePerformance;

public class RopeIgnite : MonoBehaviour
{
    private Rope papaRope;
    [SerializeField] private bool igniting = false;
    private void Start()
    {
        papaRope = gameObject.GetComponentInParent<Rope>();
    }
    private void Update()
    {
        if (igniting)
        {
            SpriteRenderer holder = GetComponent<SpriteRenderer>();
            if (holder.color.r != 0.0f)
            {
                holder.color = new Color(Mathf.Clamp(holder.color.r - (papaRope.individualBurnTime * Time.deltaTime), 0, 67), Mathf.Clamp(holder.color.g - (papaRope.individualBurnTime * Time.deltaTime), 0, 67), Mathf.Clamp(holder.color.b - (papaRope.individualBurnTime * Time.deltaTime), 0, 67), holder.color.a);
                print("Color = " + holder.color);
                if (holder.color.r <= 0.5f)
                    papaRope.Ignited(gameObject);

            }
            else
            {
                igniting = false;
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.GetComponent<Fire>() != null)
        {
            igniting = true;
        }
    }

    public void Ignite()
    {
        igniting = true;
    }
}
