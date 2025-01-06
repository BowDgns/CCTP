using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float jumpForce = 10f; // Force applied when jumping
    public float gravityScale = 1f; // Gravity for the Rigidbody2D
    public float screenPadding = 0.1f; // Padding to prevent the player from going too close to the edges

    // Sprites for player and shadow
    public Sprite playerJumpingSprite; // Player's jumping sprite
    public Sprite playerIdleSprite; // Player's idle sprite
    public Sprite shadowJumpingSprite; // Shadow's jumping sprite
    public Sprite shadowIdleSprite; // Shadow's idle sprite

    public GameObject shadowObject; // Reference to the shadow object (child of the player)

    private Rigidbody2D rb;
    private Camera mainCamera;
    private float cameraWidth;
    private SpriteRenderer spriteRenderer; // Player's SpriteRenderer
    private SpriteRenderer shadowSpriteRenderer; // Shadow's SpriteRenderer

    private bool canJump = false; // Flag to determine if the player can jump

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 0f; // Disable gravity initially to keep the player still

        mainCamera = Camera.main;

        // Calculate camera bounds in world space based on the camera's viewport
        cameraWidth = mainCamera.orthographicSize * mainCamera.aspect;

        // Get the SpriteRenderer component for the player
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null && playerIdleSprite != null)
        {
            spriteRenderer.sprite = playerIdleSprite; // Set the initial sprite to idle
        }

        // Get the SpriteRenderer component for the shadow object
        if (shadowObject != null)
        {
            shadowSpriteRenderer = shadowObject.GetComponent<SpriteRenderer>();
            if (shadowSpriteRenderer != null && shadowIdleSprite != null)
            {
                shadowSpriteRenderer.sprite = shadowIdleSprite; // Set the shadow's initial sprite to idle
            }
        }
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

        // Update the sprite based on vertical movement
        UpdateSpriteBasedOnMovement();

        // Flip the sprite and the shadow if needed
        FlipSpriteBasedOnDirection();
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

    void UpdateSpriteBasedOnMovement()
    {
        if (spriteRenderer != null && shadowSpriteRenderer != null)
        {
            if (rb.velocity.y > 0) // Player is moving upwards
            {
                spriteRenderer.sprite = playerJumpingSprite;
                shadowSpriteRenderer.sprite = shadowJumpingSprite;
            }
            else if (rb.velocity.y <= 0) // Player is falling or idle
            {
                spriteRenderer.sprite = playerIdleSprite;
                shadowSpriteRenderer.sprite = shadowIdleSprite;
            }
        }
    }

    void FlipSpriteBasedOnDirection()
    {
        if (spriteRenderer != null && shadowSpriteRenderer != null)
        {
            if (rb.velocity.x > 0) // Moving to the right
            {
                spriteRenderer.flipX = false;
                shadowSpriteRenderer.flipX = false; // Flip the shadow to match the player
            }
            else if (rb.velocity.x < 0) // Moving to the left
            {
                spriteRenderer.flipX = true;
                shadowSpriteRenderer.flipX = true; // Flip the shadow to match the player
            }
        }
    }
}
