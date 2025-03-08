using UnityEngine;

public class Enemy : MonoBehaviour
{
    private float lowerBound;

    void Update()
    {
        // Get the camera's lower Y position dynamically
        lowerBound = Camera.main.transform.position.y - Camera.main.orthographicSize;

        // Destroy if the enemy moves below the lower bound
        if (transform.position.y < lowerBound - 3)
        {
            Destroy(gameObject);
        }
    }
}