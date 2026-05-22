using System;
using System.Collections;
using UnityEngine;

public class GroundDetection : MonoBehaviour
{
    
    private Collider2D col;
    private int groundAdjacents = 0;
    private bool runningCor = false;
    private static float GROUND_TRIGGER_DELAY = 0.15f;
    private bool onGround = false;
    private bool canJump = false;
    void Start()
    {
        col = GetComponent<Collider2D>();
    }

    private void updateGrounded()
    {
        if (groundAdjacents > 0)
        {
            onGround = true;
            canJump = true;
        } 
        else if (groundAdjacents <= 0 && !runningCor)
        {
            onGround = false;
            runningCor = true;
            StartCoroutine(offGround());
        }
    }

    IEnumerator offGround()
    {
        float incrementer = 0f;
        while (incrementer < GROUND_TRIGGER_DELAY)
        {
            if (groundAdjacents > 0)
            {
                runningCor = false;
                yield break;
            }
            incrementer += Time.deltaTime;
            yield return null;
        }
            canJump = false;
            runningCor = false;
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        print("triggering");
        int layerIndex = collision.transform.gameObject.layer;
        if (LayerMask.LayerToName(layerIndex) == "Ground")
        {
            print("touching ground");
            groundAdjacents++; //add to count of ground colliders feet are on
            updateGrounded();
        } 
        
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        LayerMask layerIndex = collision.transform.gameObject.layer;
        if (LayerMask.LayerToName(layerIndex) == "Ground")
        {
            groundAdjacents--;
            updateGrounded();
        }
    }

    public bool getJumpStatus()
    {
        return canJump;
    }

    public bool getGroundStatus()
    {
        return onGround;
    }
}
