using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] float moveSpeed = 5f;
    [SerializeField] float rotationSpeed = 10f;

    [Header("Jump & Gravity")]
    [SerializeField] float gravity = -9.81f;
    [SerializeField] float jumpHeight = 1.5f;

    CharacterController controller;
    private Animator animator;
    Vector3 velocity;

    PlayerInputActions inputActions;
    Vector2 moveInput;
    bool jumpPressed;

    void Awake()
    {
        animator = GetComponent<Animator>();
        controller = GetComponent<CharacterController>();
        inputActions = new PlayerInputActions();
    }

    void OnEnable()
    {
        inputActions.Player.Enable();
        inputActions.Player.Jump.performed += OnJump;
    }

    void OnDisable()
    {
        inputActions.Player.Jump.performed -= OnJump;
        inputActions.Player.Disable();
    }

    void Update()
    {
        Debug.Log("Update running");

        HandleMovement();
        HandleGravity();
    }

    void HandleMovement()
    {
        moveInput = inputActions.Player.Move.ReadValue<Vector2>();

        float speed = Mathf.Clamp01(moveInput.magnitude);
        animator.SetFloat("Speed", moveInput.magnitude, 0.2f, Time.deltaTime);

        if (speed < 0.1f)
            return;

        Transform cam = Camera.main.transform;

        Vector3 camForward = cam.forward;
        Vector3 camRight = cam.right;

        camForward.y = 0;
        camRight.y = 0;

        camForward.Normalize();
        camRight.Normalize();

        Vector3 moveDirection = camForward * moveInput.y + camRight * moveInput.x;

        Quaternion targetRotation = Quaternion.LookRotation(moveDirection);
        transform.rotation = Quaternion.Slerp(
            transform.rotation,
            targetRotation,
            rotationSpeed * Time.deltaTime
        );

        controller.Move(moveDirection * moveSpeed * Time.deltaTime);
    }


    void HandleGravity()
    {

        if (controller.isGrounded)
        {
            if (velocity.y < 0)
                velocity.y = -2f; // stick to ground
        }
        else
        {
            velocity.y += gravity * Time.deltaTime;
        }

        controller.Move(velocity * Time.deltaTime);
    }

    void OnJump(InputAction.CallbackContext context)
    {
        if (controller.isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }
    }
}
