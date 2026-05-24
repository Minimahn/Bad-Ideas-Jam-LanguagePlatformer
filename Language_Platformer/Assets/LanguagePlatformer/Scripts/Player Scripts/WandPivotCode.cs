using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

public class WandPivotCode : MonoBehaviour
{
    GameObject player;
    Camera cam;

    [Header("Modifiable Data")]
    [SerializeField] private float minDistance = 1.0f;
    [SerializeField] private float maxDistance = 3.0f;
    [SerializeField] private float followSpeed = 10.0f;

    void Start()
    {
        player = GameObject.Find("Player");
        cam = GameObject.Find("Main Camera").GetComponent<Camera>();
    }

    // Update is called once per frame
    void Update()
    {
        Vector2 screenSpace = Mouse.current.position.ReadValue();
        Vector2 mousePos = cam.ScreenToWorldPoint(screenSpace);
        Vector2 playerPos = new Vector2(player.transform.position.x, player.transform.position.y);

        Vector2 difference = mousePos - playerPos;
        float currentDistance = difference.magnitude;

        Vector2 direction;

        if (currentDistance > 0.001f)
        {
            direction = difference / currentDistance;
        } else
        {
            direction = Vector2.right;
        }

        float clampedDistance = Mathf.Clamp(currentDistance, minDistance, maxDistance);

        Vector2 targetPosition = playerPos + (direction * clampedDistance);
        transform.position = Vector2.Lerp(transform.position, targetPosition, followSpeed * Time.deltaTime);

        float angle = 180.0f - Mathf.Atan2(difference.y, difference.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.AngleAxis(-angle, Vector3.forward);
    }
}
