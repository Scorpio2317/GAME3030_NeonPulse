using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement")]
    public float maxSpeed = 10.0f;
    public float sprintSpeed = 16.0f;
    public float gravity = -30.0f;
    public float jumpHeight = 3.0f;

    [Header("Ground Detection")]
    public Transform groundPoint;
    public float groundRadius = 0.5f;
    public LayerMask groundMask;
    public bool isGrounded;

    [SerializeField] InputActionAsset inputActions;
    InputAction movementInput;
    InputAction jumpInput;
    InputAction sprintInput;

    CharacterController controller;

    [HideInInspector] public Vector3 velocity;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        movementInput = inputActions.FindAction("Move");
        jumpInput = inputActions.FindAction("Jump");
        sprintInput = inputActions.FindAction("Sprint");
    }

    void OnEnable()
    {
        movementInput?.Enable();
        jumpInput?.Enable();
        sprintInput?.Enable();
    }

    void OnDisable()
    {
        movementInput?.Disable();
        jumpInput?.Disable();
        sprintInput?.Disable();
    }

    void Update()
    {
        isGrounded = Physics.CheckSphere(groundPoint.position, groundRadius, groundMask);

        if (isGrounded && velocity.y < 0.0f)
            velocity.y = -2.0f;

        float x = movementInput.ReadValue<Vector2>().x;
        float z = movementInput.ReadValue<Vector2>().y;

        bool isSprinting = sprintInput.IsPressed() && z > 0;
        float currentSpeed = isSprinting ? sprintSpeed : maxSpeed;

        Vector3 move = transform.right * x + transform.forward * z;
        controller.Move(move * currentSpeed * Time.deltaTime);

        if (jumpInput.IsPressed() && isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2.0f * gravity);
        }


        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.magenta;
        Gizmos.DrawWireSphere(groundPoint.position, groundRadius);
    }
}
