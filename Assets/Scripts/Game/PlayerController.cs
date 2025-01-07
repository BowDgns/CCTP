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
    private bool isFirstJump = true; // Flag to track if it's the first jump

    // References to the empty GameObjects
    public GameObject pointAObject; // First empty GameObject
    public GameObject pointBObject; // Second empty GameObject
    public GameObject pointCObject; // Third empty GameObject

    int jumps = 0;

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
                    if (touchPosition.y < rb.position.y) // Only respond to touch below the player
                    {
                        canJump = true; // Allow jumping after first touch
                        rb.gravityScale = gravityScale; // Enable gravity after first input
                        Jump(touchPosition); // Perform the jump
                    }
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
                    if (touchPosition.y < rb.position.y) // Only respond to touch below the player
                    {
                        Jump(touchPosition);
                    }
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

    public void Jump(Vector2 inputPosition)
    {
        // Get the positions of the empty GameObjects
        Vector2 pointA = pointAObject.transform.position;
        Vector2 pointB = pointBObject.transform.position;
        Vector2 pointC = pointCObject.transform.position;

        // Determine the closest point to the input position
        Vector2 closestPoint;

        if (isFirstJump)
        {
            // Force the first jump to originate from Point B
            closestPoint = pointB;
            isFirstJump = false; // Mark the first jump as complete
        }
        else
        {
            closestPoint = GetClosestPoint(inputPosition, pointA, pointB, pointC);
        }

        // Calculate the jump direction based on the closest point and angle constraints
        Vector2 jumpDirection;

        if (closestPoint == (Vector2)pointAObject.transform.position)
        {
            // Point A: Jump between 15° and 45°
            jumpDirection = CalculateJumpDirection(rb.position, closestPoint, -30f, -45f);
        }
        else if (closestPoint == (Vector2)pointBObject.transform.position)
        {
            // Point B: Jump between -15° and 15°
            jumpDirection = CalculateJumpDirection(rb.position, closestPoint, -5f, 5f);
        }
        else // Point C
        {
            // Point C: Jump between -15° and -45°
            jumpDirection = CalculateJumpDirection(rb.position, closestPoint, 45f, 35f);
        }

        // Apply force in the calculated direction
        rb.velocity = jumpDirection * jumpForce;

        jumps++;
    }

    Vector2 GetClosestPoint(Vector2 inputPosition, Vector2 a, Vector2 b, Vector2 c)
    {
        // Calculate distances to each point
        float distanceToA = Vector2.Distance(inputPosition, a);
        float distanceToB = Vector2.Distance(inputPosition, b);
        float distanceToC = Vector2.Distance(inputPosition, c);

        // Find the closest point
        if (distanceToA < distanceToB && distanceToA < distanceToC)
        {
            return a;
        }
        else if (distanceToB < distanceToC)
        {
            return b;
        }
        else
        {
            return c;
        }
    }


    Vector2 CalculateJumpDirection(Vector2 playerPosition, Vector2 closestPoint, float minAngle, float maxAngle)
    {
        // Calculate the base direction (from the closest point to the player)
        Vector2 baseDirection = (playerPosition - closestPoint).normalized;

        // Convert the base direction to an angle in degrees
        float baseAngle = Mathf.Atan2(baseDirection.y, baseDirection.x) * Mathf.Rad2Deg;

        // Generate a random angle within the specified range
        float randomAngle = Random.Range(minAngle, maxAngle);

        // Calculate the final angle
        float finalAngle = baseAngle + randomAngle;

        // Convert the angle back to a Vector2 direction
        float finalAngleRadians = finalAngle * Mathf.Deg2Rad;
        Vector2 jumpDirection = new Vector2(Mathf.Cos(finalAngleRadians), Mathf.Sin(finalAngleRadians)).normalized;

        return jumpDirection;
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
