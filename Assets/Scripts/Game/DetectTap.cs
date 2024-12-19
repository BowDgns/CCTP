using UnityEngine;

public class DetectTap : MonoBehaviour
{
    // Sensitivity threshold for detecting a tap. Adjust based on your testing.
    [SerializeField] private float tapThreshold = 2.0f;

    // Cooldown time to prevent multiple detections for a single tap
    [SerializeField] private float tapCooldown = 0.2f;
    private float lastTapTime = 0f;

    void Update()
    {
        // Get the current accelerometer reading
        Vector3 acceleration = Input.acceleration;

        // Calculate the magnitude of the acceleration vector
        float magnitude = acceleration.magnitude;

        // Check if the acceleration exceeds the threshold
        if (magnitude > tapThreshold && Time.time > lastTapTime + tapCooldown)
        {
            Debug.Log("Tap Detected!");
            OnTap();
            lastTapTime = Time.time;
        }
    }

    // This method will be called when a tap is detected
    private void OnTap()
    {
        // Add your custom logic here (e.g., trigger an event, play a sound, etc.)
        Debug.Log("Surface tap detected!");
    }
}
