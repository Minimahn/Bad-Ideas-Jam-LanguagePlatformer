using UnityEngine;

public class PlayerCamera : MonoBehaviour
{
    private Camera cam;
    [SerializeField] private GameObject plr;


    void Start()
    {
        cam = GetComponent<Camera>();
        Cursor.visible = false; //disable mouse cursor
    }

    void Update()
    {
        transform.position = new Vector3(plr.transform.position.x, plr.transform.position.y, transform.position.z);

    }

}
