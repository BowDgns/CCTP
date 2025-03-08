using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float jumpForce = 10f; // Jump force
    public float gravityScale = 1f; // Gravity for Rigidbody2D
    public float screenPadding = 0.1f; // Screen padding to keep player in bounds

    public Sprite playerJumpingSprite;
    public Sprite playerIdleSprite;
    public Sprite shadowJumpingSprite;
    public Sprite shadowIdleSprite;

    public GameObject shadowObject; // Shadow reference
    public GameObject pointAObject; // Left jump point
    public GameObject pointBObject; // Right jump point

    private Rigidbody2D rb;
    private Camera mainCamera;
    private float cameraWidth;
    private SpriteRenderer spriteRenderer;
    private SpriteRenderer shadowSpriteRenderer;

    private bool canJump = false;
    private bool isFirstJump = true;
    private Vector2 currentJumpPoint; // Stores the current jump point for vertical jumps

    private bool tapped_left = true;
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 0f; // Disable gravity initially
        mainCamera = Camera.main;
        cameraWidth = mainCamera.orthographicSize * mainCamera.aspect;

        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null && playerIdleSprite != null)
        {
            spriteRenderer.sprite = playerIdleSprite;
        }

        if (shadowObject != null)
        {
            shadowSpriteRenderer = shadowObject.GetComponent<SpriteRenderer>();
            if (shadowSpriteRenderer != null && shadowIdleSprite != null)
            {
                shadowSpriteRenderer.sprite = shadowIdleSprite;
            }
        }

        // Start player in the center
        //rb.position = Vector2.zero;
        currentJumpPoint = Vector2.zero; // Default to center
    }

    void Update()
    {
        // first jump
        // - determine which side to jump towards
        // - allow gravity
        if (!canJump)
        {
            if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
            {
                Vector2 touchPosition = Camera.main.ScreenToWorldPoint(Input.GetTouch(0).position);

                // check which side of the screen is pressed
                float screenCenterX = mainCamera.pixelWidth / 2f;

                if (touchPosition.x < screenCenterX) // left
                {
                    Debug.Log("tapped left !");
                    tapped_left = true;
                    canJump = true;
                    currentJumpPoint = pointAObject.transform.position;
                }
                else // right
                {
                    tapped_left = false;
                    Debug.Log("tapped right !");
                    canJump = true;
                    currentJumpPoint = pointBObject.transform.position; 
                }
            }
        }

        // same logic for all jumps except no change in gravity

        if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
        {
            Vector2 touchPosition = Input.GetTouch(0).position;

            // check which side of the screen is pressed
            float screenCenterX = Screen.width / 2f;

            if (touchPosition.x < screenCenterX)  // left
            {
                Debug.Log("tapped left");
                tapped_left = true;
                canJump = true;
                currentJumpPoint = pointAObject.transform.position;
                Jump(true); // Set to left point
            }
            else  // right
            {
                Debug.Log("tapped right");
                tapped_left = false;
                canJump = true;
                currentJumpPoint = pointBObject.transform.position;  
                Jump(false);
            }
        }

        KeepPlayerInBounds();
        AnimatePlayer();
        FlipPlayer();
    }


    void Jump(bool jump_left)   // true = player wants to jump on the left side, false means player wants to jump on the right
    {
        if (isFirstJump)
        {
            rb.gravityScale = gravityScale;
            isFirstJump = false;
        }

        Vector2 leftPoint = pointAObject.transform.position;
        Vector2 rightPoint = pointBObject.transform.position;

        // If the touch is on the same side as the current jump point, jump straight up
        // Otherwise, switch sides

        // continue upwards
        if(jump_left && currentJumpPoint == leftPoint)
        {
            Debug.Log("Jumping upwards");
            rb.velocity = Vector2.up * jumpForce;
        }
        if (!jump_left && currentJumpPoint == rightPoint)
        {
            Debug.Log("Jumping upwards");
            rb.velocity = Vector2.up * jumpForce;
        }

        // switch sides
        if (jump_left && currentJumpPoint == rightPoint)
        {
            Debug.Log("Swapping sides");
            rb.velocity = (currentJumpPoint - rb.position).normalized * jumpForce;
        }
        /*
        else
        {
            currentJumpPoint = currentJumpPoint == leftPoint ? rightPoint : leftPoint;
            rb.velocity = (currentJumpPoint - rb.position).normalized * jumpForce;
        }
        */
    }

    void KeepPlayerInBounds()
    {
        Vector2 playerPosition = rb.position;
        if (playerPosition.x - screenPadding < -cameraWidth)
        {
            playerPosition.x = -cameraWidth + screenPadding;
            rb.velocity = new Vector2(-rb.velocity.x, rb.velocity.y);
        }
        else if (playerPosition.x + screenPadding > cameraWidth)
        {
            playerPosition.x = cameraWidth - screenPadding;
            rb.velocity = new Vector2(-rb.velocity.x, rb.velocity.y);
        }
        rb.position = playerPosition;
    }

    void AnimatePlayer()
    {
        if (spriteRenderer != null && shadowSpriteRenderer != null)
        {
            if (rb.velocity.y > 0)
            {
                spriteRenderer.sprite = playerJumpingSprite;
                shadowSpriteRenderer.sprite = shadowJumpingSprite;
            }
            else
            {
                spriteRenderer.sprite = playerIdleSprite;
                shadowSpriteRenderer.sprite = shadowIdleSprite;
            }
        }
    }

    void FlipPlayer()
    {
        if (spriteRenderer != null && shadowSpriteRenderer != null)
        {
            if (rb.velocity.x > 0)
            {
                spriteRenderer.flipX = false;
                shadowSpriteRenderer.flipX = false;
            }
            else if (rb.velocity.x < 0)
            {
                spriteRenderer.flipX = true;
                shadowSpriteRenderer.flipX = true;
            }
        }
    }
}
