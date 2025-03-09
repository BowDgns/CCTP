using UnityEngine;

public class Food : MonoBehaviour
{
    private float lowerBound;

    public float add_stamina;

    void Update()
    {
        // Get the camera's lower Y position dynamically
        lowerBound = Camera.main.transform.position.y - Camera.main.orthographicSize;

        // Destroy if the food moves below the lower bound
        if (transform.position.y < lowerBound - 3)
        {
            Destroy(gameObject);
        }
    }

    // Called when another collider enters this object's trigger zone
    private void OnTriggerEnter2D(Collider2D other)
    {
        // Check if the object colliding is the player (you can tag or compare the object here)
        if (other.CompareTag("Player"))
        {
            // Print message to the console
            Debug.Log("Eaten food");

            // Destroy this food object
            Destroy(gameObject);
        }
    }
}
