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
    public int maxEnemies = 10; // Maximum number of enemies allowed at once

    private float next_spawn;
    private Camera mainCamera;
    private int currentEnemyCount = 0;

    void Start()
    {
        mainCamera = Camera.main; // Get the main camera reference
    }

    void Update()
    {
        if (Time.time > next_spawn && currentEnemyCount < maxEnemies)
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
            float spawnY = mainCamera.transform.position.y + (mainCamera.orthographicSize + spawn_offset);
            spawnPosition = new Vector2(randomX, spawnY);

            // Check if the spawn position overlaps with existing enemies
            if (!IsOverlapping(spawnPosition))
            {
                validPositionFound = true;
                break;
            }
        }

        // If a valid position was found, instantiate the enemy
        if (validPositionFound)
        {
            Instantiate(prefab, spawnPosition, Quaternion.identity);
            currentEnemyCount++;
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
            if (collider.gameObject.CompareTag("Enemy") || collider.gameObject.CompareTag("Food"))
            {
                return true;
            }
        }
        return false;
    }

    // Call this method when an enemy is destroyed
    public void OnEnemyDestroyed()
    {
        currentEnemyCount = Mathf.Max(0, currentEnemyCount - 1);
    }
}
