using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float jumpForce = 10f; // Force applied when jumping
    public float gravityScale = 1f; // Gravity for the Rigidbody2D
    public float screenPadding = 0.1f; // Padding to prevent the player from going too close to the edges

    private Rigidbody2D rb;
    private Camera mainCamera;
    private float cameraWidth;

    private bool canJump = false; // Flag to determine if player can jump

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 0f; // Disable gravity initially to keep the player still

        mainCamera = Camera.main;

        // Calculate camera bounds in world space based on the camera's viewport
        cameraWidth = mainCamera.orthographicSize * mainCamera.aspect;
    }

    void Update()
    {
        // Check if the first input has been received (either touch or mouse)
        if (!canJump)
        {
            // Check for touch input
            if (Input.touchCount > 0)
            {
                Touch touch = Input.GetTouch(0); // Get the first touch
                if (touch.phase == TouchPhase.Began) // Check if the touch just started
                {
                    Vector2 touchPosition = Camera.main.ScreenToWorldPoint(touch.position); // Convert touch position to world coordinates
                    // Only respond to touch below the player
                    if (touchPosition.y < rb.position.y)
                    {
                        canJump = true; // Allow jumping after first touch
                        rb.gravityScale = gravityScale; // Enable gravity after first input
                        Jump(touchPosition); // Perform the jump
                    }
                }
            }

            // Check for mouse input (for testing on desktop)
            if (Input.GetMouseButtonDown(0)) // Left mouse button
            {
                Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition); // Convert mouse position to world coordinates
                // Only respond to mouse click below the player
                if (mousePosition.y < rb.position.y)
                {
                    canJump = true; // Allow jumping after first click
                    rb.gravityScale = gravityScale; // Enable gravity after first input
                    Jump(mousePosition); // Perform the jump
                }
            }
        }
        else
        {
            // If the player can jump, keep checking for further touch or mouse input
            if (Input.touchCount > 0)
            {
                Touch touch = Input.GetTouch(0); // Get the first touch
                if (touch.phase == TouchPhase.Began) // Check if the touch just started
                {
                    Vector2 touchPosition = Camera.main.ScreenToWorldPoint(touch.position); // Convert touch position to world coordinates
                    // Only respond to touch below the player
                    if (touchPosition.y < rb.position.y)
                    {
                        Jump(touchPosition);
                    }
                }
            }

            // Check for mouse input (for testing on desktop)
            if (Input.GetMouseButtonDown(0)) // Left mouse button
            {
                Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition); // Convert mouse position to world coordinates
                // Only respond to mouse click below the player
                if (mousePosition.y < rb.position.y)
                {
                    Jump(mousePosition);
                }
            }
        }

        // Keep the player within horizontal bounds (left and right)
        KeepPlayerInBounds();
    }

    void Jump(Vector2 targetPosition)
    {
        // Calculate the direction away from the target position (touch or mouse)
        Vector2 playerPosition = rb.position;
        Vector2 jumpDirection = (playerPosition - targetPosition).normalized;

        // Apply force in the calculated direction
        rb.velocity = jumpDirection * jumpForce;
    }

    void KeepPlayerInBounds()
    {
        // Get the player's current position
        Vector2 playerPosition = rb.position;

        // Check if the player is outside the horizontal screen bounds (left and right)
        if (playerPosition.x - screenPadding < -cameraWidth)
        {
            playerPosition.x = -cameraWidth + screenPadding;
            rb.velocity = new Vector2(-rb.velocity.x, rb.velocity.y); // Reverse horizontal velocity to bounce off the left edge
        }
        else if (playerPosition.x + screenPadding > cameraWidth)
        {
            playerPosition.x = cameraWidth - screenPadding;
            rb.velocity = new Vector2(-rb.velocity.x, rb.velocity.y); // Reverse horizontal velocity to bounce off the right edge
        }

        // Apply the adjusted position to keep the player within bounds horizontally
        rb.position = playerPosition;
    }
}
