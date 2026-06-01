using UnityEngine;

public class Gravity : MonoBehaviour
{
    void FixedUpdate()
    {
        GetComponent<Rigidbody2D>().AddForce(GetComponent<Rigidbody2D>().mass * new Vector2(0.0f,-9.8f)); ;
    }
}
