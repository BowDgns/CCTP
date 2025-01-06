using UnityEngine;

public class TableTapInput : MonoBehaviour
{
    private Vector3 acceleration;
    private Vector3 previousAcceleration;
    private float tapThreshold = 3.0f; // Threshold for detecting sudden taps
    private float lowPassFilterFactor = 0.1f; // Smoothing factor for accelerometer
    private Vector3 smoothedAcceleration;

    private float timeSinceLastTap = 0f;
    private float tapCooldown = 0.5f; // Prevent multiple detections for a single tap

    private float bottomQuarterY;

    private void Start()
    {
        // Enable the gyro if you need orientation data
        Input.gyro.enabled = true;

        // Calculate Y position for the bottom quarter of the screen
        bottomQuarterY = Screen.height / 4f;

        // Initialize smoothed acceleration
        smoothedAcceleration = Input.acceleration;
    }

    private void Update()
    {
        // Update time since last tap
        timeSinceLastTap += Time.deltaTime;

        // Detect tap-like gestures from accelerometer
        DetectTap();
    }

    private void DetectTap()
    {
        // Get the current acceleration data
        acceleration = Input.acceleration;

        // Apply a simple low-pass filter to smooth the accelerometer data
        smoothedAcceleration = Vector3.Lerp(smoothedAcceleration, acceleration, lowPassFilterFactor);

        // Detect sudden spikes in acceleration (tap-like motions)
        Vector3 accelerationDelta = acceleration - smoothedAcceleration;

        // If the change in acceleration is greater than the threshold and the cooldown is over, it's a tap
        if (accelerationDelta.sqrMagnitude > tapThreshold * tapThreshold && timeSinceLastTap > tapCooldown)
        {
            timeSinceLastTap = 0f; // Reset cooldown timer

            // Simulate a tap at the bottom quarter of the screen
            SimulateTouchInput();
        }
    }

    private void SimulateTouchInput()
    {
        // Simulate touch in the bottom quarter of the screen
        Vector2 simulatedTouchPosition = new Vector2(Screen.width / 2f, bottomQuarterY / 2f);
        Debug.Log($"Simulated touch at: {simulatedTouchPosition}");

        // Example: Interact with a UI button or other objects in the bottom quarter
        Ray ray = Camera.main.ScreenPointToRay(simulatedTouchPosition);
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            // Interact with objects hit by the raycast
            Debug.Log($"Hit object: {hit.collider.name}");
        }
    }
}
