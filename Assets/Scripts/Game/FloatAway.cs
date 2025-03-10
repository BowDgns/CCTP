using UnityEngine;

public class FloatAway : MonoBehaviour
{
    // make the frog float away

    public float speed = 2f;  // speed
    public float waveFrequency = 2f; // Frequency of the wave motion
    public float waveAmplitude = 0.5f; // Amplitude of the wave motion
    private float start_point;

    void Start()
    {
        start_point = transform.position.x;
    }

    void Update()
    {
        // move frog up
        transform.position += Vector3.up * speed * Time.deltaTime;

        // make it wave side to side 
        float newX = start_point + Mathf.Sin(Time.time * waveFrequency) * waveAmplitude;
        transform.position = new Vector3(newX, transform.position.y, transform.position.z);

        // remove when above camera bounds
        if (Camera.main != null && transform.position.y > Camera.main.orthographicSize + transform.localScale.y + 2)
        {
            Destroy(gameObject);
        }
    }
}
