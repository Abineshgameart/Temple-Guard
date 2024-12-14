using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    // Private
    PlayerInput playerInput;
    InputAction moveAction;
    private Camera mainCamera;
    [SerializeField] private Animator anim;
    Rigidbody rb;

    // Public
    public float moveSpeed = 5;
    public float runSpeed = 10;

    private void Awake()
    {
        playerInput = GetComponent<PlayerInput>();
    }

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        moveAction = playerInput.actions.FindAction("Move");
        mainCamera = Camera.main; // Get the main camera
    }

    private void Update()
    {
        MoveAndRotatePlayer();
    }

    void MoveAndRotatePlayer()
    {
        // Get movement input
        Vector2 inputDirection = moveAction.ReadValue<Vector2>();

        // Only process movement if there is input
        if (inputDirection != Vector2.zero)
        {
            // Get the camera's forward direction and project it to the ground plane
            Vector3 cameraForward = -mainCamera.transform.forward;
            cameraForward.y = 0; // Ensure movement stays on the horizontal plane
            cameraForward.Normalize();

            // Get the camera's right direction
            Vector3 localRight = transform.right;
            localRight.y = 0;
            localRight.Normalize();

            // Calculate movement direction based on camera orientation
            Vector3 moveDirection = (cameraForward * inputDirection.y + localRight * inputDirection.x) * -1; // Inverted direction
            moveDirection.Normalize();

            float speed = Input.GetKey(KeyCode.LeftShift) ? runSpeed : moveSpeed;

            // Apply movement with Rigidbody
            rb.velocity = moveDirection * speed;

            // Convert moveDirection to local space for animation
            Vector3 localMoveDirection = transform.InverseTransformDirection(rb.velocity);

            // Set the animation parameters
            anim.SetFloat("xVelocity", localMoveDirection.x);
            anim.SetFloat("zVelocity", localMoveDirection.z);

            Debug.Log("xval: " + localMoveDirection.x + " zval: " + localMoveDirection.z);

            // Rotate the player to face the mouse only when moving forward or backward
            if (Mathf.Abs(inputDirection.y) > Mathf.Epsilon) // Check if moving forward/backward
            {
                RotatePlayerToMouse();
            }
        }
        else
        {
            // Stop movement if no input
            rb.velocity = Vector3.zero;

            // Reset animation parameters
            anim.SetFloat("xVelocity", 0);
            anim.SetFloat("zVelocity", 0);
        }
    }

    void RotatePlayerToMouse()
    {
        // Get the mouse position in screen space
        Vector3 mouseScreenPosition = Mouse.current.position.ReadValue();

        // Convert mouse position to world space
        Vector3 mouseWorldPosition = mainCamera.ScreenToWorldPoint(new Vector3(mouseScreenPosition.x, mouseScreenPosition.y, mainCamera.nearClipPlane));

        // Calculate the direction to the mouse position
        Vector3 lookDirection = mouseWorldPosition - transform.position;

        // Keep the y-axis value unchanged to avoid tilting
        lookDirection.y = 0;

        // Rotate the player to face the mouse direction
        if (lookDirection != Vector3.zero)
        {
            Quaternion rotation = Quaternion.LookRotation(lookDirection);
            transform.rotation = Quaternion.Lerp(transform.rotation, rotation, Time.deltaTime * 10f); // Smooth rotation
        }
    }

}
