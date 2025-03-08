using UnityEngine;

public class Spawner : MonoBehaviour
{
    public GameObject prefab; // Assign your enemy prefab in the Inspector
    public Transform player; // Assign the player Transform
    public float spawn_rate = 2f; // Time interval between spawns
    public float range = 3f; // How far enemies can spawn horizontally
    public float spawn_offset = 2f; // (above camera)
    public float spawnRadius = 1f; // The radius to check for nearby enemies to avoid overlap
    public int maxAttempts = 5; // Maximum attempts to find a non-overlapping spawn location

    private float next_spawn;
    private Camera mainCamera;

    void Start()
    {
        mainCamera = Camera.main; // Get the main camera reference
    }

    void Update()
    {
        if (Time.time > next_spawn)
        {
            SpawnEnemy();
            next_spawn = Time.time + spawn_rate;
        }
    }

    void SpawnEnemy()
    {
        bool validPositionFound = false;
        Vector2 spawnPosition = Vector2.zero; // Initialize the spawnPosition variable here

        // Try to find a valid position within a limited number of attempts
        for (int attempt = 0; attempt < maxAttempts; attempt++)
        {
            float randomX = Random.Range(-range, range);

            // Get the top of the camera's view
            float spawnY = mainCamera.transform.position.y + (mainCamera.orthographicSize + spawn_offset);

            spawnPosition = new Vector2(randomX, spawnY);

            // Check if the spawn position overlaps with existing enemies
            if (!IsOverlapping(spawnPosition))
            {
                validPositionFound = true;
                break; // Position is valid, break out of the loop
            }
        }

        // If a valid position was found, instantiate the enemy
        if (validPositionFound)
        {
            Instantiate(prefab, spawnPosition, Quaternion.identity);
        }
        else
        {
            Debug.LogWarning("Failed to find a non-overlapping spawn position.");
        }
    }

    // Check if the given spawn position overlaps with any existing enemies
    bool IsOverlapping(Vector2 spawnPosition)
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(spawnPosition, spawnRadius);
        foreach (var collider in colliders)
        {
            // Check if the collider is an enemy (you can use a tag, layer, or compare the object type)
            if (collider.gameObject.CompareTag("Enemy") || collider.gameObject.CompareTag("Food"))
            {
                return true; // Overlap found
            }
        }
        return false; // No overlap
    }
}
