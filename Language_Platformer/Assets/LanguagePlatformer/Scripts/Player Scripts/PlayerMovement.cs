
using System.Collections.Generic;
using System.Collections;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

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
    [SerializeField] private float groundTrackInterval = 0.5f;
    [Header("Misc Stuffs")]
    [SerializeField] private float drownTimer = 1.0f;

    // Secret Variables oooh
    private int directionFacing = 0;
    private float _fallAcc = 0f;
    private bool _canJump = false;
    private bool _hasJumped = false;
    private bool _interacting = false;
    private bool _grounded = false;
    private bool _coyoteGrounded = false;
    private bool _submerged = false;
    private bool fromWater = false;
    private bool runningCor = false;
    private Vector2 lastGrounded;
    private int _groundHits = 0;
    private static float INTERACT_TRIGGER_DELAY = 0.5f;
    private static float GROUND_TRIGGER_DELAY = 0.15f;
    private static float TEXT_DISPLAY_LENGTH = 1.8f;

    [SerializeField] private float _currSpeed = 0.0f;
    private float SLOW_MULTIPLIER = 0.67f;
    private float FAST_MULTIPLIER = 1.5f;
    
    private bool slowGliding = false;

    private Rigidbody2D rigidBody;
    private Collider2D col;
    private PlayerInputHandler inputHandler;
    private Animator animator;
    private List<Interactable> inputReceivers;
    private GameObject canvas;
    private TextMeshProUGUI tmPro;

    private void Start()
    {
        _currSpeed = walkSpeed;
        rigidBody = GetComponent<Rigidbody2D>();    // Rigidbody2D > PlayerController for 2D imo so I'm sticking with that
        col = GetComponent<Collider2D>();
        inputHandler = PlayerInputHandler.Instance; // The InputHandler Instance
        animator = GetComponent<Animator>();
        inputReceivers = new List<Interactable>();
        canvas = GameObject.Find("Canvas");
        tmPro = canvas.transform.Find("Panel").Find("Text_Block").GetComponent<TextMeshProUGUI>();
        StartCoroutine(TrackLastGroundedPositionRoutine());
    }

    IEnumerator interactOffset()
    {
        _interacting = true;
        float incrementer = 0f;
        while (incrementer < INTERACT_TRIGGER_DELAY)
        {
            incrementer += Time.deltaTime;
            yield return null;
        }
        _interacting = false;
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
    IEnumerator TrackLastGroundedPositionRoutine()
    {
        float timer = 0f;

        while (true)
        {
            timer += Time.deltaTime;

            if (timer >= groundTrackInterval)
            {
                timer -= groundTrackInterval;

                // ADD CHECKS TO SEE IF TOO CLOSE TO WATER HERE

                if (_grounded)
                {
                    lastGrounded = transform.position;
                }
            }

            yield return null;
        }
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
            fromWater = false;
            _grounded = true;
            _coyoteGrounded = true;
        } else if (_groundHits <= 0 && !runningCor)
        {
            runningCor = true;
            _grounded = false;
            StartCoroutine(offGround());
        }
    }

    [SerializeField] GameObject gumbus;

    private void Update()
    {
        gumbus.transform.position = lastGrounded;

        groundCheck();
        //print("Grounded:" + _grounded + " - CoyoteGrounded: " + _coyoteGrounded);
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

        if (inputHandler.InteractTriggered && !_interacting)
        {
            StartCoroutine(interactOffset());
            foreach (var comp in inputReceivers)
            {
                comp.Interact();
            }
        }
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
            if (!_submerged)
            {
                if (clampDisabled)
                    rigidBody.AddForceY(-weight * _fallAcc, ForceMode2D.Force);
                else
                    rigidBody.AddForceY(Mathf.Clamp(-weight * _fallAcc, -fallSpeedClamp, Mathf.Infinity), ForceMode2D.Force);

                if (!fromWater)
                    _fallAcc += fallAcceleration;
            } else
            {
                transform.position = lastGrounded;
            }

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

    IEnumerator TextDelay(string s)
    {
        float incrementer = 0f;
        float fadeTime = 0.4f;
        float ratio = 0f;
        Color currColor = tmPro.color;


        while (incrementer < fadeTime)
        {
            incrementer += Time.deltaTime;
            ratio = Mathf.Clamp01(incrementer / fadeTime);
            currColor.a = ratio;
            tmPro.color = currColor;
            yield return null;
        }

        incrementer = 0f;
        while (incrementer < TEXT_DISPLAY_LENGTH)
        {
            incrementer += Time.deltaTime;
            yield return null;
        }

        incrementer = 0f;
        while (incrementer < fadeTime)
        {
            incrementer += Time.deltaTime;
            ratio = Mathf.Clamp01(incrementer / fadeTime);
            currColor.a = 1 - ratio;
            tmPro.color = currColor;
            yield return null;
        }

    }
    public void ShowText(string s)
    {
        tmPro.text = s;
        StartCoroutine(TextDelay(s));
    }

    public void AddInput(Interactable comp)
    {
        inputReceivers.Add(comp);
    }
    public void RemoveInput(Interactable comp)
    {
        inputReceivers.Remove(comp);
    }

    public void SetSub(bool b)
    {
        if (_submerged != b)
            _fallAcc = 1;
        if (b)
            fromWater = true;
        _submerged = b;
    }
}
