using UnityEngine;

public class Spawner : MonoBehaviour
{
    [Header("References")]
    public GameObject[] enemyPrefabs;  // Array of enemy/platform prefabs
    public GameObject foodPrefab;      // Food item prefab
    public Transform player;           // Player transform

    [Header("Spawn Settings")]
    public float range = 3f;              // Horizontal spawn range relative to the camera's x position
    public float spawnOffset = 2f;        // Extra offset above the camera's top edge for off-screen spawning
    public float spawnRadius = 1f;        // Radius to check for nearby objects to avoid overlap
    public int maxAttempts = 5;           // Maximum attempts to find a non-overlapping spawn position
    public int maxObjects = 10;           // Maximum number of spawned objects (enemy + food) at once
    public float minVerticalGap = 2f;     // Minimum vertical gap between spawns
    public float maxVerticalGap = 4f;     // Maximum vertical gap between spawns

    [Header("Food Spawn Settings")]
    [Range(0f, 1f)]
    public float foodSpawnProbability = 0.7f;  // Chance (0-1) to spawn a food item instead of an enemy

    private Camera mainCamera;
    private int currentObjectCount = 0;
    private float nextSpawnY;

    void Start()
    {
        mainCamera = Camera.main;
        nextSpawnY = GetTopSpawnY();
    }

    void Update()
    {
        float offScreenThreshold = GetTopSpawnY();

        // Spawn new objects until nextSpawnY reaches the off-screen threshold or maxObjects is reached
        while (nextSpawnY < offScreenThreshold && currentObjectCount < maxObjects)
        {
            if (SpawnObjectAtY(nextSpawnY))
            {
                nextSpawnY += Random.Range(minVerticalGap, maxVerticalGap);
            }
            else
            {
                Debug.LogWarning("Failed to spawn object at Y: " + nextSpawnY);
                break;
            }
        }
    }

    // Calculates the top off-screen spawn Y position
    float GetTopSpawnY()
    {
        return mainCamera.transform.position.y + mainCamera.orthographicSize + spawnOffset;
    }

    // Attempts to spawn an object (food or enemy) at the given Y position.
    // Returns true if successful.
    bool SpawnObjectAtY(float spawnY)
    {
        for (int i = 0; i < maxAttempts; i++)
        {
            float randomX = Random.Range(mainCamera.transform.position.x - range, mainCamera.transform.position.x + range);
            Vector2 spawnPos = new Vector2(randomX, spawnY);

            if (!IsOverlapping(spawnPos))
            {
                // Decide what to spawn based on probability
                if (Random.value < foodSpawnProbability)
                {
                    if (foodPrefab != null)
                    {
                        Instantiate(foodPrefab, spawnPos, Quaternion.identity);
                        currentObjectCount++;
                        return true;
                    }
                    else
                    {
                        Debug.LogError("Food prefab is not assigned.");
                        return false;
                    }
                }
                else
                {
                    if (enemyPrefabs != null && enemyPrefabs.Length > 0)
                    {
                        int randomIndex = Random.Range(0, enemyPrefabs.Length);
                        GameObject enemyPrefab = enemyPrefabs[randomIndex];
                        Instantiate(enemyPrefab, spawnPos, Quaternion.identity);
                        currentObjectCount++;
                        return true;
                    }
                    else
                    {
                        Debug.LogError("Enemy prefabs array is empty or not assigned.");
                        return false;
                    }
                }
            }
        }
        return false;
    }

    // Checks if the spawn position overlaps with any objects tagged as "Enemy" or "Food"
    bool IsOverlapping(Vector2 position)
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(position, spawnRadius);
        foreach (Collider2D collider in colliders)
        {
            if (collider.CompareTag("Enemy") || collider.CompareTag("Food"))
                return true;
        }
        return false;
    }

    // Call this method when an object (enemy or food) is destroyed to decrease the count
    public void OnObjectDestroyed()
    {
        currentObjectCount = Mathf.Max(0, currentObjectCount - 1);
    }
}
