
using System.Collections.Generic;
using System.Collections;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement Variables")]
    [SerializeField] private float walkSpeed = 3.0f;
    [SerializeField] private float braking = 1.0f;
    [SerializeField] private float acceleration = 1.0f;
    [SerializeField] private float jumpForce = 5.0f;
    [SerializeField] private float fallAcceleration = 1f;
    [SerializeField] private float gravity = 9.8f;
    [SerializeField] private float mass = 22.4f;
    [SerializeField] private float fallSpeedClamp = 440.0f;
    [SerializeField] private bool clampDisabled = false;
    [Header("Ground Checking")]
    [SerializeField] public LayerMask groundLayer;

    // Secret Variables oooh
    private int directionFacing = 0;
    private float _fallAcc = 0f;
    private bool _canJump = false;
    private bool _hasJumped = false;
    private bool _grounded = false;
    private bool _coyoteGrounded = false;
    private bool runningCor = false;
    private int _groundHits = 0;
    private static float GROUND_TRIGGER_DELAY = 0.15f;

    [SerializeField] private float _currSpeed = 0.0f;
    private float SLOW_MULTIPLIER = 0.67f;
    private float FAST_MULTIPLIER = 1.5f;
    private bool slowGliding = false;

    private Rigidbody2D rigidBody;
    private Collider2D col;
    private PlayerInputHandler inputHandler;
    private Animator animator;

    private void Start()
    {
        _currSpeed = walkSpeed;
        rigidBody = GetComponent<Rigidbody2D>();    // Rigidbody2D > PlayerController for 2D imo so I'm sticking with that
        col = GetComponent<Collider2D>();
        inputHandler = PlayerInputHandler.Instance; // The InputHandler Instance
        animator = GetComponent<Animator>();
    }


    IEnumerator offGround()
    {
        float incrementer = 0f;
        while (incrementer < GROUND_TRIGGER_DELAY)
        {
            if (_groundHits > 0)
            {
                runningCor = false;
                yield break;
            }
            incrementer += Time.deltaTime;
            yield return null;
        }
            _coyoteGrounded = false;
            runningCor = false;
    }
    private void groundCheck()
    {
        Vector2 pos = new Vector2(transform.position.x, transform.position.y);
        Vector2 size = new Vector2(0.75f, 1f); //hard-coded
        ContactFilter2D filter = new ContactFilter2D();
        filter.SetLayerMask(LayerMask.GetMask("Ground"));
        List<RaycastHit2D> list = new List<RaycastHit2D>(); 
        _groundHits = Physics2D.BoxCast(pos, size, 0, -transform.up, filter, list, 0.5f);
        if (_groundHits > 0)
        {
            _grounded = true;
            _coyoteGrounded = true;
        } else if (_groundHits <= 0 && !runningCor)
        {
            runningCor = true;
            _grounded = false;
            StartCoroutine(offGround());
        }
    }


    private void Update()
    {
        groundCheck();
        print("Grounded:" + _grounded + " - CoyoteGrounded: " + _coyoteGrounded);
        // work towards making _currspeed sprint speed
        if (inputHandler.SprintValue > 0)
            _currSpeed = Mathf.Clamp(_currSpeed + acceleration * Time.deltaTime, -6.7f, (walkSpeed * FAST_MULTIPLIER));
        else
            _currSpeed = Mathf.Clamp(_currSpeed - braking * Time.deltaTime, walkSpeed, (walkSpeed * FAST_MULTIPLIER));

        
        if (inputHandler.MoveInput != Vector2.zero && _coyoteGrounded) 
        {
            rigidBody.linearVelocity = new Vector2(inputHandler.MoveInput.x * _currSpeed, rigidBody.linearVelocityY);
            changeAnimState(inputHandler.MoveInput.x);
        } 
        else if (inputHandler.MoveInput != Vector2.zero && !_coyoteGrounded)
        {
            //print("RB: " + rigidBody.linearVelocity.x + " <-> Input: " + inputHandler.MoveInput.x);
            if (rigidBody.linearVelocityX == 0 || slowGliding)
            {
                slowGliding = true;
                rigidBody.linearVelocity = new Vector2(inputHandler.MoveInput.x * (_currSpeed * SLOW_MULTIPLIER), rigidBody.linearVelocityY);
            }
            else
            {
                rigidBody.linearVelocity = new Vector2(inputHandler.MoveInput.x * _currSpeed, rigidBody.linearVelocityY);
            }
        }
        else
        {
            rigidBody.linearVelocity = new Vector2(0, rigidBody.linearVelocityY);
        } 
        

        // Jump Logic
        if (_coyoteGrounded && inputHandler.JumpTriggered && !_hasJumped && rigidBody.linearVelocityY < 0.02f)
        {
            rigidBody.linearVelocity = new Vector2(rigidBody.linearVelocityX, 0);
            rigidBody.AddForceY(jumpForce, ForceMode2D.Impulse);
            _hasJumped = true;
        }

        if (_coyoteGrounded && !inputHandler.JumpTriggered) // ???
            _hasJumped = false;
        changeAnimState(inputHandler.MoveInput.x);
    }

    private void FixedUpdate()
    {
        float weight = mass * gravity;
        if (_grounded) // have player fall
        {
            _fallAcc = 1f;
            rigidBody.AddForceY(-weight, ForceMode2D.Force);
            slowGliding = false;
        }
        else
        {
            if (clampDisabled)
                rigidBody.AddForceY(-weight * _fallAcc, ForceMode2D.Force);
            else
                rigidBody.AddForceY(Mathf.Clamp(-weight * _fallAcc, -fallSpeedClamp, Mathf.Infinity), ForceMode2D.Force);

            _fallAcc += fallAcceleration;
        }
        if (!inputHandler.JumpTriggered && rigidBody.linearVelocityY > 0)
        {
            //rigidBody.linearVelocityY = rigidBody.linearVelocityY * 0.7f;
        }
    }

    private void changeAnimState(float dir)
    {
        if (dir < 0 && _grounded)
        {
            animator.Play("WalkLeft");
            directionFacing = -1;
        }
        else if (dir > 0 && _grounded)
        {
            animator.Play("WalkRight");
            directionFacing = 1;
        }
        else if (dir < 0 && !_grounded)
        {
            animator.Play("JumpLeft");
            directionFacing = -1;
        }
        else if (dir > 0 && !_grounded)
        {
            animator.Play("JumpRight");
            directionFacing = 1;
        }
        else if (dir < 0.1f && dir > -0.1f)
        {
            if (directionFacing > 0)
                animator.Play("IdleRight");
            else if (directionFacing < 0)
                animator.Play("IdleLeft");
        }

    }
}
