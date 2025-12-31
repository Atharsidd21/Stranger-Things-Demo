using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(Animator))]
public class PlayerMovement : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] float walkSpeed = 2.2f;
    [SerializeField] float runSpeed = 4.5f;
    [SerializeField] float sprintSpeed = 6.5f;
    [SerializeField] float rotationSpeed = 10f;

    [Header("Jump & Gravity")]
    [SerializeField] float gravity = -9.81f;
    [SerializeField] float jumpHeight = 1.4f;

    CharacterController controller;
    Animator animator;

    PlayerInputActions inputActions;

    Vector3 velocity;
    bool isSprinting;

    // =========================
    // UNITY LIFECYCLE
    // =========================
    void Awake()
    {
        controller = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();
        inputActions = new PlayerInputActions();
    }

    void OnEnable()
    {
        inputActions.Player.Enable();

        inputActions.Player.Sprint.performed += _ => isSprinting = true;
        inputActions.Player.Sprint.canceled += _ => isSprinting = false;

        inputActions.Player.Jump.performed += OnJump;
    }

    void OnDisable()
    {
        inputActions.Player.Sprint.performed -= _ => isSprinting = true;
        inputActions.Player.Sprint.canceled -= _ => isSprinting = false;

        inputActions.Player.Jump.performed -= OnJump;
        inputActions.Player.Disable();
    }

    void Update()
    {
        HandleMovement();
        HandleGravity();
    }

    // =========================
    // MOVEMENT (1D LOCOMOTION)
    // =========================
    void HandleMovement()
    {
        Vector2 input = inputActions.Player.Move.ReadValue<Vector2>();

        // =========================
        // ANIMATOR SPEED LOGIC
        // =========================
        float animSpeed = 0f;

        if (input.y > 0.1f)
        {
            animSpeed = isSprinting ? 1f : 0.5f;
        }
        else if (input.y < -0.1f)
        {
            animSpeed = -0.5f;
        }

        animator.SetFloat("Speed", animSpeed, 0.1f, Time.deltaTime);

        //=========================
        // ROTATION (A / D)
        // =========================
        if (Mathf.Abs(input.x) > 0.1f)
        {
            transform.Rotate(Vector3.up, input.x * rotationSpeed * 60f * Time.deltaTime);
        }
        

        // =========================
        // MOVEMENT (W / S ONLY)
        // =========================
        if (Mathf.Abs(input.y) < 0.1f)
            return;

        Vector3 forward = transform.forward;

        float currentSpeed;

        if (input.y > 0.1f && isSprinting)
            currentSpeed = sprintSpeed;
        else if (input.y < -0.1f)
            currentSpeed = walkSpeed * 0.8f;
        else
            currentSpeed = walkSpeed;

        controller.Move(forward * input.y * currentSpeed * Time.deltaTime);
    }


    // =========================
    // GRAVITY
    // =========================
    void HandleGravity()
    {
        if (controller.isGrounded && velocity.y < 0)
        {
            velocity.y = -2f; // stick to ground
        }

        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }

    // =========================
    // JUMP
    // =========================
    void OnJump(InputAction.CallbackContext context)
    {
        if (!controller.isGrounded)
            return;

        velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
    }
}
