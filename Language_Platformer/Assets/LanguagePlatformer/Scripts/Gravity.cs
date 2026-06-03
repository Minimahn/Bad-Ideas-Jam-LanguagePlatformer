using UnityEngine;

public class Gravity : MonoBehaviour
{
    [SerializeField] private float gravity = 9.8f;
    void FixedUpdate()
    {
        GetComponent<Rigidbody2D>().AddForce(GetComponent<Rigidbody2D>().mass * new Vector2(0.0f,-gravity)); ;
    }
}
