using System.ComponentModel;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

public class Rope : MonoBehaviour
{
    // Not Private Variables (aka (also known as (nom de plume))) public variables
    [Header("Customization")]
    [Description("How long each individual rope takes to burn")]
    [SerializeField] public float individualBurnTime = 1.0f;
    [Description("UnityEvent that will play once rope is destroyed")]
    [SerializeField] private UnityEvent onDestroy;
    [Description("Smoke Particle Effect")]
    [SerializeField] public GameObject Smoke;

    // Private Variables
    private GameObject[] ropeSegments;
    private GameObject anchorStart;
    private GameObject anchorEnd;
    private bool startRemoval = false;

    void Start()
    {
        anchorStart = transform.GetChild(0).gameObject;
        anchorEnd = transform.GetChild(transform.childCount-1).gameObject;
        if (anchorEnd.name.Contains("Rope"))
        {
            ropeSegments = new GameObject[transform.childCount - 1];
            anchorEnd = null;
            for (int i = 1; i < transform.childCount; i++)
            {
                ropeSegments[i - 1] = transform.GetChild(i).gameObject;
            }
            return;
        }

        ropeSegments = new GameObject[transform.childCount - 2];
        for (int i = 1; i < transform.childCount - 1; i++)
        {
            ropeSegments[i-1] = transform.GetChild(i).gameObject;
        }
    }

    // Update is called once per frame
    void Update()
    {
        RemovalCheck();
    }

    public void Ignited(GameObject toBurnFrom)
    {
        for (int i = 0; i < ropeSegments.Length; i++)
        {
            if (toBurnFrom == ropeSegments[i])
            {
                if (i-1 >= 0)
                    ropeSegments[i-1].GetComponent<RopeIgnite>().Ignite();
                if (i+1 < ropeSegments.Length)
                    ropeSegments[i+1].GetComponent<RopeIgnite>().Ignite();
                break;
            }
        }
    }
    private void RemovalCheck()
    {
        foreach (var rope in ropeSegments)
        {
            if (rope.gameObject.GetComponent<SpriteRenderer>().color.r != 0.0f)
                return;
        }

        foreach (var rope in ropeSegments)
        {
            SpriteRenderer holder = rope.GetComponent<SpriteRenderer>();
            float alpha = holder.color.a;
            holder.color = new Color(holder.color.r, holder.color.g, holder.color.b, Mathf.Clamp(holder.color.a - Time.deltaTime, 0, 67));
            if (holder.color.a <= 0.0f)
            {
                Destroy(gameObject);
                rope.GetComponent<RopeIgnite>().creationSlop();
            }
        }

    }

    private void OnDestroy()
    {
        onDestroy.Invoke();
    }
}
