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

    public Image stamina_bar;
    public float stamina;
    public float max_stamina;
    public float jump_cost;

    // animation and shadows
    public Sprite player_jump_sprite;
    public Sprite player_idle_sprite;
    public Sprite shadow_jump_sprite;
    public Sprite shadow_idle_sprite;

    private SpriteRenderer sprite_renderer;
    private SpriteRenderer shadow_sprite_renderer;

    public GameObject shadowObject;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 0f; // set gravity to 0 so the player doesnt fall before the game starts

        // get values to ensure player doesnt go off screen 
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

            // check which side of the screen is pressed
            float screenCenterX = Screen.width / 2f;

            if (touchPosition.x < screenCenterX)  // left
            {
                Debug.Log("tapped left");
                Jump(true); // set to left point
            }
            else  // right
            {
                Debug.Log("tapped right");
                Jump(false); // set to right
            }
        }

        KeepPlayerInBounds();
        AnimatePlayer();
        FlipPlayer();

        if(stamina < 0)
        {
            stamina = 0;
        }

        stamina_bar.fillAmount = stamina / max_stamina;
    }

    // bug with jumping right first keeps you going straight
    void Jump(bool jump_left) // true = left tap, false = right tap
    {
        if (first_jump)
        {
            rb.gravityScale = gravity;
            first_jump = false;
        }

        float screenCenterX = Screen.width / 2f;    // center of screen
        float playerScreenX = Camera.main.WorldToScreenPoint(transform.position).x; // where player is on screen

        bool playerOnLeftScreen = playerScreenX < screenCenterX;

        if ((jump_left && playerOnLeftScreen) || (!jump_left && !playerOnLeftScreen))   // on the same side so jump straight up
        {
            Debug.Log("Jumping upwards");
            rb.velocity = Vector2.up * jump_force;
        }
        else    // tap on opposite side, so jump over towards the jumping points.
        {
            Debug.Log("Swapping sides");
            Vector2 targetPosition = jump_left ? left_point.transform.position : right_point.transform.position;
            Vector2 jumpDirection = (targetPosition - (Vector2)transform.position).normalized;

            rb.velocity = jumpDirection * jump_force;
        }

        stamina -= jump_cost;
        number_of_jumps++;
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
