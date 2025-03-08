using UnityEngine;

public class Spawner : MonoBehaviour
{
    public GameObject enemyPrefab; // Assign your enemy prefab in the Inspector
    public Transform player; // Assign the player Transform
    public float spawnRate = 2f; // Time interval between spawns
    public float xRange = 3f; // How far enemies can spawn horizontally
    public float spawnOffset = 2f; // How far above the camera enemies should spawn

    private float nextSpawnTime;
    private Camera mainCamera;

    void Start()
    {
        mainCamera = Camera.main; // Get the main camera reference
    }

    void Update()
    {
        if (Time.time > nextSpawnTime)
        {
            SpawnEnemy();
            nextSpawnTime = Time.time + spawnRate;
        }
    }

    void SpawnEnemy()
    {
        float randomX = Random.Range(-xRange, xRange);

        // Get the top of the camera's view
        float spawnY = mainCamera.transform.position.y + (mainCamera.orthographicSize + spawnOffset);

        Vector2 spawnPosition = new Vector2(randomX, spawnY);
        Instantiate(enemyPrefab, spawnPosition, Quaternion.identity);
    }
}
