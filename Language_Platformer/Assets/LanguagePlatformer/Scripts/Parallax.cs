using Unity.VisualScripting;
using UnityEditor.Rendering;
using UnityEngine;

public class Parallax : MonoBehaviour
{
    float length;
    float startpos;
    public GameObject camera;
    [SerializeField] float parallaxAmount;

    void Start()
    {
        startpos = transform.position.x;
        length = GetComponent<SpriteRenderer>().bounds.size.x;
    }

    // Update is called once per frame
    void Update()
    {
        float distFromCamera = (camera.transform.position.x * (1 - parallaxAmount));

        float distance = (camera.transform.position.x * parallaxAmount);
        transform.position = new Vector3(startpos + distance, transform.position.y, transform.position.z);

        if (distFromCamera > startpos + length)
            startpos += length;

        else if (distFromCamera < startpos - length)
            startpos -= length;
    }
}
