using System.ComponentModel;
using UnityEngine;
using UnityEngine.InputSystem;

public class RotateWith : MonoBehaviour
{
    [Header("Speed of the rotation")]
    [SerializeField] float speed = 1;

    [Header("Ignore this and leave it as false unless you found it on true")]
    [SerializeField] bool annoyingPOS = false;

    private float angularVelocity;
    Camera cam;
    GameObject player;

    private void Start()
    {
        GameObject wand = GameObject.Find("Wand");
        transform.rotation = Quaternion.Euler(0, 0, wand.transform.rotation.eulerAngles.z);
        transform.position = new Vector3(0, 0, 0);
        cam = GameObject.Find("Main Camera").GetComponent<Camera>();
        player = GameObject.Find("Player");
    }
    // Update is called once per frame
    void Update()
    {
        // psuedo-parent cuz it's being annoying
        if (annoyingPOS)
            transform.position = player.transform.position;

        Vector2 screenSpace = Mouse.current.position.ReadValue();
        Vector2 mousePos = cam.ScreenToWorldPoint(screenSpace);
        Vector2 playerPos = new Vector2(player.transform.position.x, player.transform.position.y);
        Vector2 difference = mousePos - playerPos;

        float targetAngle = 180.0f - Mathf.Atan2(difference.y, difference.x) * Mathf.Rad2Deg;
        float currentAngle = transform.rotation.eulerAngles.z;
        float angleDelta = Mathf.DeltaAngle(currentAngle, -targetAngle);

        angularVelocity += angleDelta * speed * Time.deltaTime;
        float damping = 0.9f;
        angularVelocity *= Mathf.Pow(damping, Time.deltaTime * 60f);
        float newAngle = currentAngle + angularVelocity * Time.deltaTime;
        transform.rotation = Quaternion.Euler(0, 0, newAngle);
    }
}
