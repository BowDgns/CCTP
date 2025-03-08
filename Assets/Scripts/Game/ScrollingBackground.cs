using UnityEngine;

public class ScrollingBackground : MonoBehaviour
{
    public GameObject[] backgrounds; // Background objects
    public Transform player; // Player reference
    public float scrollSpeedMultiplier = 1f; // Adjust scrolling speed
    private float backgroundHeight; // Height of a single background
    private Camera mainCamera;
    private Vector3 lastPlayerPosition;

    void Start()
    {
        mainCamera = Camera.main;
        backgroundHeight = backgrounds[0].GetComponent<SpriteRenderer>().bounds.size.y;
        lastPlayerPosition = player.position;
    }

    void Update()
    {
        float playerMovementDelta = player.position.y - lastPlayerPosition.y;

        if (Mathf.Abs(playerMovementDelta) > 0.01f) // Ignore tiny movements
        {
            foreach (GameObject bg in backgrounds)
            {
                bg.transform.position += Vector3.down * playerMovementDelta * scrollSpeedMultiplier;
            }

            // Handle both upward and downward repositioning
            CheckAndRepositionBackgrounds();
        }

        lastPlayerPosition = player.position;
    }

    void CheckAndRepositionBackgrounds()
    {
        // Sort backgrounds based on their Y position
        System.Array.Sort(backgrounds, (a, b) => a.transform.position.y.CompareTo(b.transform.position.y));

        GameObject lowestBG = backgrounds[0];
        GameObject highestBG = backgrounds[backgrounds.Length - 1];

        // Move background UP if the lowest one moves too far down
        if (lowestBG.transform.position.y <= mainCamera.transform.position.y - backgroundHeight)
        {
            lowestBG.transform.position = new Vector3(
                highestBG.transform.position.x,
                highestBG.transform.position.y + backgroundHeight,
                highestBG.transform.position.z
            );
            ShiftArrayLeft();
        }

        // Move background DOWN if the highest one moves too far up
        if (highestBG.transform.position.y >= mainCamera.transform.position.y + backgroundHeight)
        {
            highestBG.transform.position = new Vector3(
                lowestBG.transform.position.x,
                lowestBG.transform.position.y - backgroundHeight,
                lowestBG.transform.position.z
            );
            ShiftArrayRight();
        }
    }

    void ShiftArrayLeft()
    {
        GameObject first = backgrounds[0];

        for (int i = 0; i < backgrounds.Length - 1; i++)
        {
            backgrounds[i] = backgrounds[i + 1];
        }

        backgrounds[backgrounds.Length - 1] = first;
    }

    void ShiftArrayRight()
    {
        GameObject last = backgrounds[backgrounds.Length - 1];

        for (int i = backgrounds.Length - 1; i > 0; i--)
        {
            backgrounds[i] = backgrounds[i - 1];
        }

        backgrounds[0] = last;
    }
}
