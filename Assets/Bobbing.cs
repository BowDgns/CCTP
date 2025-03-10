using UnityEngine;

public class Bobbing : MonoBehaviour
{
    public float amplitude = 0.5f; // Height of the bobbing
    public float frequency = 2f;   // Speed of the bobbing

    private Vector3 startPosition;

    void Start()
    {
        startPosition = transform.position; // Store the initial position
    }

    void Update()
    {
        float newY = startPosition.y + Mathf.Sin(Time.time * frequency) * amplitude;
        transform.position = new Vector3(startPosition.x, newY, startPosition.z);
    }
}
