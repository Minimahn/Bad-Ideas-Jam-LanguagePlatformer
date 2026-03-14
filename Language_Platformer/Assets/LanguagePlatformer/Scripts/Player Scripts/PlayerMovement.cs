using Unity.VisualScripting;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement Variables")]
    [SerializeField] private float walkSpeed = 3.0f;
    [SerializeField] private float sprintSpeed = 4.5f;
    [SerializeField] private float jumpForce = 5.0f;

    [Header("Ground Checking")]
    [SerializeField] public LayerMask groundLayer;

    // Secret Variables oooh
    private bool _hasJumped = false;

    private Rigidbody2D rigidBody;
    private Collider2D col;
    private PlayerInputHandler inputHandler;

    private void Start()
    {
        rigidBody = GetComponent<Rigidbody2D>();    // Rigidbody2D > PlayerController for 2D imo so I'm sticking with that
        col = GetComponent<Collider2D>();
        inputHandler = PlayerInputHandler.Instance; // The InputHandler Instance
    }

    bool IsGrounded()
    {
        return Physics2D.Raycast(new Vector2(transform.position.x, col.bounds.min.y), Vector2.down, 0.1f, groundLayer);
    }

    private void Update()
    {
        // L&R Movement Logic
        float speed = walkSpeed;

        if (inputHandler.SprintValue > 0)
            speed = sprintSpeed;

        if (inputHandler.MoveInput != Vector2.zero)
            rigidBody.linearVelocity = new Vector2(inputHandler.MoveInput.x * speed, rigidBody.linearVelocityY);
        else
            rigidBody.linearVelocity = new Vector2(0, rigidBody.linearVelocityY); // erm... 0 * anything = 0 dumbass FUCK YOU!!!

        // Jump Logic
        if (IsGrounded() && inputHandler.JumpTriggered && !_hasJumped)
        {
            rigidBody.AddForceY(jumpForce, ForceMode2D.Impulse);
            _hasJumped = true;
        }

        if (IsGrounded() && !inputHandler.JumpTriggered)
            _hasJumped = false;
    }
}
