using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputHandler : MonoBehaviour
{
    [SerializeField] private InputActionAsset playerControls;
    private InputAction moveAction, jumpAction, sprintAction;
    public Vector2 MoveInput { get; private set; }
    public bool JumpTriggered { get; private set; }
    public float SprintValue { get; private set; }

    public static PlayerInputHandler Instance { get; private set; }
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            // Won't destroy gameobject if already exists
            DontDestroyOnLoad(gameObject);
        } else
        {
            // Will destroy gameobject if it does exist already
            Destroy(gameObject);
            return;
        }

        moveAction = playerControls.FindActionMap("Player").FindAction("Move");
        jumpAction = playerControls.FindActionMap("Player").FindAction("Jump");
        sprintAction = playerControls.FindActionMap("Player").FindAction("Sprint");

        RegisterInputActions();
    }
    void RegisterInputActions()
    {
        // Whenever input is started it reads it and sets the value;
        moveAction.performed += context => MoveInput = context.ReadValue<Vector2>();
        // Movement input is stopped
        moveAction.canceled += context => MoveInput = Vector2.zero;

        jumpAction.performed += context => JumpTriggered = true;
        jumpAction.canceled += context => JumpTriggered = false;

        sprintAction.performed += context => SprintValue = context.ReadValue<float>();
        sprintAction.canceled += context => SprintValue = 0.0f;
    }
    private void OnEnable()
    {
        playerControls.FindActionMap("Player").Enable();
    }
    private void OnDisable()
    {
        playerControls.FindActionMap("Player").Disable();
    }
}
