using UnityEngine;

public class ScrollingBackground : MonoBehaviour
{
    public GameObject[] backgrounds; // Array for your background objects
    public Transform player; // Reference to the player object
    public float scrollSpeedMultiplier = 1f; // Speed multiplier based on player movement
    private float backgroundHeight; // Height of the background image
    private Camera mainCamera;
    private Vector3 lastPlayerPosition; // To track the player's last position
    private float playerMovementDelta; // Player movement difference since the last frame

    void Start()
    {
        mainCamera = Camera.main;

        // Calculate the height of the background sprite based on its size and scale
        backgroundHeight = backgrounds[0].GetComponent<SpriteRenderer>().bounds.size.y;

        // Store the initial player position
        lastPlayerPosition = player.position;
    }

    void Update()
    {
        // Calculate how much the player has moved since the last frame
        playerMovementDelta = player.position.y - lastPlayerPosition.y;

        // Move the backgrounds only if the player has moved
        if (Mathf.Abs(playerMovementDelta) > 0.01f) // A small threshold to ignore tiny movements
        {
            foreach (GameObject bg in backgrounds)
            {
                // Move the background based on player movement
                bg.transform.position += Vector3.down * playerMovementDelta * scrollSpeedMultiplier;

                // Handle upward or downward repositioning
                if (bg.transform.position.y <= mainCamera.transform.position.y - backgroundHeight)
                {
                    RepositionBackgroundAbove(bg); // Reposition above when moving down
                }
                else if (bg.transform.position.y >= mainCamera.transform.position.y + backgroundHeight)
                {
                    RepositionBackgroundBelow(bg); // Reposition below when moving up
                }
            }
        }

        // Update the player's last position
        lastPlayerPosition = player.position;
    }

    void RepositionBackgroundAbove(GameObject bg)
    {
        // Find the highest background and place this one above it
        float highestY = backgrounds[0].transform.position.y;
        foreach (GameObject otherBg in backgrounds)
        {
            if (otherBg.transform.position.y > highestY)
            {
                highestY = otherBg.transform.position.y;
            }
        }

        // Reposition this background above the highest one
        bg.transform.position = new Vector3(bg.transform.position.x, highestY + backgroundHeight, bg.transform.position.z);
    }

    void RepositionBackgroundBelow(GameObject bg)
    {
        // Find the lowest background and place this one below it
        float lowestY = backgrounds[0].transform.position.y;
        foreach (GameObject otherBg in backgrounds)
        {
            if (otherBg.transform.position.y < lowestY)
            {
                lowestY = otherBg.transform.position.y;
            }
        }

        // Reposition this background below the lowest one
        bg.transform.position = new Vector3(bg.transform.position.x, lowestY - backgroundHeight, bg.transform.position.z);
    }
}
