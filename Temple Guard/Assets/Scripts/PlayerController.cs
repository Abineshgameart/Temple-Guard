using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    // Private
    PlayerInput playerInput;
    InputAction moveAction;
    private Camera mainCamera;

    // Public
    public float moveSpeed;

    private void Awake()
    {
        playerInput = GetComponent<PlayerInput>();
    }

    // Start is called before the first frame update
    void Start()
    {
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
            Vector3 cameraForward = mainCamera.transform.forward;
            cameraForward.y = 0; // Ensure movement stays on the horizontal plane
            cameraForward.Normalize();

            // Get the camera's right direction
            Vector3 cameraRight = mainCamera.transform.right;
            cameraRight.y = 0;
            cameraRight.Normalize();

            // Calculate movement direction based on camera orientation
            Vector3 moveDirection = (cameraForward * inputDirection.y + cameraRight * inputDirection.x) * -1; // Inverted direction
            transform.position += moveDirection * moveSpeed * Time.deltaTime;

            // Rotate the player to face the mouse
            RotatePlayerToMouse();
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
