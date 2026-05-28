using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Tilemaps;
using Unity.VisualScripting;

public class NPCController : MonoBehaviour
{

    public GameObject player;
    public Tilemap physical;
    private Rigidbody2D rigidbody;

    public float gravityForce = 0f;
    public Vector2 gravityDirection = -Vector2.up;
    private float fallAcceleration = 1;
    private bool _grounded = false;

    private float patrolDistance = 4f;
    private float patrolSpeed = 5f;
    private float patrolDirection = 1;


    void Start()
    {
        rigidbody = GetComponent<Rigidbody2D>();
    }


    private void FixedUpdate()
    {
        groundCheck();
        //apply gravity
        if (_grounded)
        {
            fallAcceleration = 1f;
            rigidbody.AddForce(gravityDirection * gravityForce, ForceMode2D.Force);
        }
        else
        {
            rigidbody.AddForce(gravityDirection * gravityForce * fallAcceleration, ForceMode2D.Force);
            fallAcceleration += 0.05f;
        }

        patrol();
    }

    private void patrol()
    {
        Vector3 groundLevel = new Vector3(transform.position.x, transform.position.y - 0.8f, transform.position.z);
        List<Vector3Int> groundTilePos = TileController.getRowTilesWithinRadius(groundLevel, 2.5f);
        
        foreach (var tilepos in groundTilePos)
        {
            if (physical.GetTile(tilepos) == null)
            {
                if ((patrolDirection > 0 && tilepos.x > transform.position.x) || (patrolDirection < 0 && tilepos.x < transform.position.x))
                {
                    patrolDirection = -patrolDirection; // 1 to -1 to --1, etc. left/right
                    break;
                }
            }
        }
        rigidbody.linearVelocity = new Vector2(patrolDirection * patrolSpeed, rigidbody.linearVelocity.y);

    }

    private void groundCheck()
    {
        Vector2 pos = new Vector2(transform.position.x, transform.position.y);
        Vector2 size = new Vector2(0.75f, 1f); //hard-coded, size change
        ContactFilter2D filter = new ContactFilter2D();
        filter.SetLayerMask(LayerMask.GetMask("Ground"));
        List<RaycastHit2D> list = new List<RaycastHit2D>(); 
        int _groundHits = Physics2D.BoxCast(pos, size, 0, -transform.up, filter, list, 0.5f);
        if (_groundHits > 0)
            _grounded = true;
        else
            _grounded = false;
    }

}
