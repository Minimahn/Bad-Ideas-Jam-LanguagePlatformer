using Unity.VisualScripting;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement Variables")]
    [SerializeField] private float walkSpeed = 3.0f;
    [SerializeField] private float braking = 1.0f;
    [SerializeField] private float sprintSpeed = 4.5f;
    [SerializeField] private float acceleration = 1.0f;
    [SerializeField] private float jumpForce = 5.0f;
    [SerializeField] private float fallAcceleration = 1f;
    [Header("Ground Checking")]
    [SerializeField] public LayerMask groundLayer;

    // Secret Variables oooh
    private bool _canJump = false;
    private bool _hasJumped = false;
    [SerializeField] private float _currSpeed = 0.0f;

    private Rigidbody2D rigidBody;
    private Collider2D col;
    private Collider2D feetCol;
    private PlayerInputHandler inputHandler;

    private void Start()
    {
        _currSpeed = walkSpeed;
        rigidBody = GetComponent<Rigidbody2D>();    // Rigidbody2D > PlayerController for 2D imo so I'm sticking with that
        col = GetComponent<Collider2D>();
        feetCol = transform.Find("FeetBox").GetComponent<Collider2D>();
        inputHandler = PlayerInputHandler.Instance; // The InputHandler Instance
    }

    bool IsGrounded()
    {
        return feetCol.transform.GetComponent<GroundDetection>().getGroundStatus();
    }
    bool IsCoyoteGrounded()
    {
        return feetCol.transform.GetComponent<GroundDetection>().getJumpStatus();
    }



    private void Update()
    {
        // work towards making _currspeed sprint speed
        if (inputHandler.SprintValue > 0)
            _currSpeed = Mathf.Clamp(_currSpeed + acceleration * Time.deltaTime, -6.7f, sprintSpeed);
        else
            _currSpeed = Mathf.Clamp(_currSpeed - braking * Time.deltaTime, walkSpeed, sprintSpeed);

        

        if (inputHandler.MoveInput != Vector2.zero && IsCoyoteGrounded()) 
        {
            rigidBody.linearVelocity = new Vector2(inputHandler.MoveInput.x * _currSpeed, rigidBody.linearVelocityY);
        } 
        else if (inputHandler.MoveInput != Vector2.zero && !IsCoyoteGrounded())
        {
            //Greg question with new unity input, save 4 later.
        }
        else
        {
            rigidBody.linearVelocity = new Vector2(0, rigidBody.linearVelocityY);
        } 
        

        // Jump Logic
        if (IsCoyoteGrounded() && inputHandler.JumpTriggered && !_hasJumped)
        {
            rigidBody.AddForceY(jumpForce, ForceMode2D.Impulse);
            _hasJumped = true;
        }

        if (IsCoyoteGrounded() && !inputHandler.JumpTriggered) // ???
            _hasJumped = false;
    }

    private void FixedUpdate()
    {
        if (IsGrounded()) // have player fall
        {
            fallAcceleration = 1f;
        }
        else
        {
            rigidBody.AddForceY(-4f * fallAcceleration, ForceMode2D.Impulse);
            fallAcceleration += 0.02f;
        }

        if (!inputHandler.JumpTriggered && rigidBody.linearVelocityY > 0)
        {
            //rigidBody.linearVelocityY = rigidBody.linearVelocityY * 0.7f;
        }
    }
}
