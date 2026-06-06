using UnityEngine;

public class PlayerCamera : MonoBehaviour
{
    private Camera cam;
    private GameObject plr;


    void Start()
    {
        cam = GetComponent<Camera>();
        plr = GameObject.Find("Player");
        Cursor.visible = false; //disable mouse cursor
    }

    void Update()
    {
        transform.position = new Vector3(plr.transform.position.x, plr.transform.position.y, transform.position.z);

    }

}
