using UnityEngine;

public class Bobbing : MonoBehaviour
{
    public float amplitude = 0.5f;  // how high it goes
    public float frequency = 2f;    // speed

    private Vector3 start_pos;

    void Start()
    {
        start_pos = transform.position; // get original position of enemy
    }

    void Update()
    {
        float newY = start_pos.y + Mathf.Sin(Time.time * frequency) * amplitude;
        transform.position = new Vector3(start_pos.x, newY, start_pos.z);
    }
}
