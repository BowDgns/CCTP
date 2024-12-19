using UnityEngine;

public class ParallaxBackground : MonoBehaviour
{
    public Transform[] background_layers; 
    public float[] moving_speeds;

    public float reset_bounds = 1920f; 
    public float reset_pos = 1600f; 

    private Vector3[] initial_positions; 

    void Start()
    {
        initial_positions = new Vector3[background_layers.Length];  // store inital positions of backgrounds to reset to on y and z

        for (int i = 0; i < background_layers.Length; i++)
        {
            initial_positions[i] = background_layers[i].position;
        }
    }

    void Update()
    {
        float dt = Time.deltaTime;

        for (int i = 0; i < background_layers.Length; i++)
        {
            // move background
            background_layers[i].position += Vector3.left * moving_speeds[i] * dt;

            if (background_layers[i].position.x < -reset_bounds)    // reset when over bounds
            {
                background_layers[i].position = new Vector3(reset_pos, initial_positions[i].y, initial_positions[i].z);
            }
        }
    }
}
