using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float jumpForce = 10f;  // Force applied when jumping
    public float gravityScale = 1f;  // Gravity for the Rigidbody2D

    private Rigidbody2D rb;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = gravityScale;  // Set gravity scale
    }

    void Update()
    {
        // Detect jump input (spacebar or screen tap)
        if (Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0))
        {
            Jump();
        }
    }

    void Jump()
    {
        // Apply upward force for jumping
        rb.velocity = new Vector2(rb.velocity.x, jumpForce);
    }
}
