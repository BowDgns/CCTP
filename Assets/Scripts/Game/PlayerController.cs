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

    // points to jump away from
    public GameObject pointAObject;
    public GameObject pointBObject; 
    public GameObject pointCObject; 

    int jumps = 0;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 0f; // set gravity to zero so the player doesnt fall before the first jump

        mainCamera = Camera.main;

        // get camera size for keeping player in bounds
        cameraWidth = mainCamera.orthographicSize * mainCamera.aspect;

        // get player sprite
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null && playerIdleSprite != null)
        {
            spriteRenderer.sprite = playerIdleSprite; // set to idle
        }

        // do the same for the shadow
        if (shadowObject != null)
        {
            shadowSpriteRenderer = shadowObject.GetComponent<SpriteRenderer>();
            if (shadowSpriteRenderer != null && shadowIdleSprite != null)
            {
                shadowSpriteRenderer.sprite = shadowIdleSprite; 
            }
        }
    }

    void Update()
    {
        // can player jump (yes after first input)
        if (!canJump)
        {
            if (Input.touchCount > 0)
            {
                Touch touch = Input.GetTouch(0); // Get the first touch
                if (touch.phase == TouchPhase.Began) // Check if the touch just started
                {
                    Vector2 touchPosition = Camera.main.ScreenToWorldPoint(touch.position); // Convert touch position to world coordinates
                    if (touchPosition.y < rb.position.y) // Only respond to touch below the player
                    {
                        canJump = true; // Allow jumping after first touch
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

        // keep the player within the screen bounds
        KeepPlayerInBounds();

        // update the sprite and shadow
        UpdateSpriteBasedOnMovement();
        FlipSpriteBasedOnDirection();
    }

    public void Jump(Vector2 inputPosition)
    {
        // Enable gravity on the first jump
        if (isFirstJump)
        {
            rb.gravityScale = gravityScale; // Activate gravity on the first jump
            isFirstJump = false; // Mark the first jump as complete
        }

        // changed jumping to jump away from certain points (for tap input)
        // simulate more random and directional jumps from doing it between two andgles

        // positions to jump away from
        Vector2 pointA = pointAObject.transform.position;
        Vector2 pointB = pointBObject.transform.position;
        Vector2 pointC = pointCObject.transform.position;

        // choose closest point from the three points
        Vector2 closestPoint = GetClosestPoint(inputPosition, pointA, pointB, pointC);

        Vector2 jumpDirection;

        if (closestPoint == (Vector2)pointAObject.transform.position)
        {
            jumpDirection = CalculateJumpDirection(rb.position, closestPoint, -30f, -45f);
        }
        else if (closestPoint == (Vector2)pointBObject.transform.position)
        {
            jumpDirection = CalculateJumpDirection(rb.position, closestPoint, -5f, 5f);
        }
        else 
        {
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
