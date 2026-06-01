using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class Rope : MonoBehaviour
{
    // Not Private Variables (aka (also known as (nom de plume))) public variables
    [Header("Customization")]
    [SerializeField] float individualBurnTime = 1.0f;

    // Private Variables
    private GameObject[] ropeSegments;
    private GameObject anchorStart;
    private GameObject anchorEnd;
    void Start()
    {
        ropeSegments = new GameObject[transform.childCount - 2];
        anchorStart = transform.GetChild(0).gameObject;
        anchorEnd = transform.GetChild(transform.childCount-1).gameObject;
        if (anchorEnd.name.Contains("Rope"))
            anchorEnd = null;
        for (int i = 1; i < transform.childCount - 1; i++)
        {
            ropeSegments[i-1] = transform.GetChild(i).gameObject;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Ignited(GameObject toBurnFrom)
    {
        if (ropeSegments.Last<GameObject>() == toBurnFrom && anchorEnd != null)
            Destroy(anchorEnd);
        else if (ropeSegments.First<GameObject>() == toBurnFrom)
            Destroy(anchorStart);
        
        Destroy(toBurnFrom);
    }
}
