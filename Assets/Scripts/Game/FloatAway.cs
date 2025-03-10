using UnityEngine;

public class FloatAway : MonoBehaviour
{
    public float speed = 2f;  // Vertical movement speed
    public float waveFrequency = 2f; // Frequency of the wave motion
    public float waveAmplitude = 0.5f; // Amplitude of the wave motion
    private float startX;

    void Start()
    {
        startX = transform.position.x;
    }

    void Update()
    {
        // Move upwards
        transform.position += Vector3.up * speed * Time.deltaTime;

        // Apply wave movement
        float newX = startX + Mathf.Sin(Time.time * waveFrequency) * waveAmplitude;
        transform.position = new Vector3(newX, transform.position.y, transform.position.z);

        // remove when above camera bounds
        if (Camera.main != null && transform.position.y > Camera.main.orthographicSize + transform.localScale.y + 2)
        {
            Destroy(gameObject);
        }
    }
}
