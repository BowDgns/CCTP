using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    public float jump_force = 10f;
    public float gravity = 1f;
    public float screen_padding = 0.1f; // padding for how far the player can jump from the screen

    // points for the player to jump towards
    public GameObject left_point;
    public GameObject right_point;

    private Rigidbody2D rb;
    private Camera mainCamera;
    private float camera_width;

    private bool first_jump = true;
    public float number_of_jumps = 0;

    [Header("Stamina")]
    public Image stamina_bar;
    public float stamina;
    public float max_stamina;
    public float jump_cost;

    // animation and shadows
    [Header("Visuals")]
    public Sprite player_jump_sprite;
    public Sprite player_idle_sprite;
    public Sprite shadow_jump_sprite;
    public Sprite shadow_idle_sprite;

    private SpriteRenderer sprite_renderer;
    private SpriteRenderer shadow_sprite_renderer;

    public GameObject shadowObject;

    // New variable to track game start time
    private float gameStartTime;

    void Start()
    {
        gameStartTime = Time.time;  // Record when the game starts

        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 0f; // set gravity to 0 so the player doesn't fall before the game starts

        // get values to ensure player doesn't go off screen 
        mainCamera = Camera.main;
        camera_width = mainCamera.orthographicSize * mainCamera.aspect;

        sprite_renderer = GetComponent<SpriteRenderer>();
        if (sprite_renderer != null && player_idle_sprite != null)
        {
            sprite_renderer.sprite = player_idle_sprite;
        }

        if (shadowObject != null)
        {
            shadow_sprite_renderer = shadowObject.GetComponent<SpriteRenderer>();
            if (shadow_sprite_renderer != null && shadow_idle_sprite != null)
            {
                shadow_sprite_renderer.sprite = shadow_idle_sprite;
            }
        }
    }

    void Update()
    {
        // get touch screen input to determine where to jump from
        if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
        {
            Vector2 touchPosition = Input.GetTouch(0).position;
            float screenCenterX = Screen.width / 2f;

            if (touchPosition.x < screenCenterX)  // left
            {
                Jump(true); // set to left point
            }
            else  // right
            {
                Jump(false); // set to right point
            }
        }

        KeepPlayerInBounds();
        AnimatePlayer();
        FlipPlayer();

        if (stamina < 0)
        {
            stamina = 0;
        }

        stamina_bar.fillAmount = stamina / max_stamina;
    }

    public void Jump(bool jump_left) // true = left tap, false = right tap
    {
        // Prevent jumping during the first 3 seconds of the game
        if (Time.time - gameStartTime < 3f)
        {
            Debug.Log("Jump disabled for the first 3 seconds.");
            return;
        }

        if (stamina > 0)
        {
            if (first_jump)
            {
                rb.gravityScale = gravity;
                first_jump = false;
            }

            float screenCenterX = Screen.width / 2f;    // center of screen
            float playerScreenX = Camera.main.WorldToScreenPoint(transform.position).x; // where player is on screen

            // When the player is on the same side as the tap, jump straight up.
            // Otherwise (player is in the middle or on the opposite side), jump towards the designated point.
            if ((jump_left && playerScreenX < screenCenterX) || (!jump_left && playerScreenX > screenCenterX))
            {
                rb.velocity = Vector2.up * jump_force;
            }
            else
            {
                Vector2 targetPosition = jump_left ? left_point.transform.position : right_point.transform.position;
                Vector2 jumpDirection = (targetPosition - (Vector2)transform.position).normalized;
                rb.velocity = jumpDirection * jump_force;
            }

            stamina -= jump_cost;
            number_of_jumps++;
        }
    }

    void KeepPlayerInBounds()
    {
        Vector2 playerPosition = rb.position;
        if (playerPosition.x - screen_padding < -camera_width)
        {
            playerPosition.x = -camera_width + screen_padding;
            rb.velocity = new Vector2(-rb.velocity.x, rb.velocity.y);
        }
        else if (playerPosition.x + screen_padding > camera_width)
        {
            playerPosition.x = camera_width - screen_padding;
            rb.velocity = new Vector2(-rb.velocity.x, rb.velocity.y);
        }
        rb.position = playerPosition;
    }

    void AnimatePlayer()
    {
        if (sprite_renderer != null && shadow_sprite_renderer != null)
        {
            if (rb.velocity.y > 0)
            {
                sprite_renderer.sprite = player_jump_sprite;
                shadow_sprite_renderer.sprite = shadow_jump_sprite;
            }
            else
            {
                sprite_renderer.sprite = player_idle_sprite;
                shadow_sprite_renderer.sprite = shadow_idle_sprite;
            }
        }
    }

    void FlipPlayer()
    {
        if (sprite_renderer != null && shadow_sprite_renderer != null)
        {
            if (rb.velocity.x > 0)
            {
                sprite_renderer.flipX = false;
                shadow_sprite_renderer.flipX = false;
            }
            else if (rb.velocity.x < 0)
            {
                sprite_renderer.flipX = true;
                shadow_sprite_renderer.flipX = true;
            }
        }
    }
}
