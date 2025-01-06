using UnityEngine;

public class HybridInput : MonoBehaviour
{
    private Vector3 acceleration;
    private Vector3 previousAcceleration;
    public float tapThreshold = 0.5f; // Threshold for detecting sudden taps
    public float lowPassFilterFactor = 0.1f; // Smoothing factor for accelerometer
    private Vector3 smoothedAcceleration;

    // bounds for how much the gyroscope moved to detect directional tap
    public float right_bound_lower = 1f;
    public float right_bound_upper = 89f;
    public float left_bound_lower = 359f;
    public float left_bound_upper = 271f;

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
        previousAcceleration = smoothedAcceleration;
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
        // Get the current accelerometer data
        acceleration = Input.acceleration;

        // Get the current rotation from the gyroscope
        Quaternion rotation = Input.gyro.attitude;
        Vector3 rotationEuler = rotation.eulerAngles;

        // Apply low-pass filter to accelerometer data for smoothing
        smoothedAcceleration = Vector3.Lerp(smoothedAcceleration, acceleration, lowPassFilterFactor);

        // Calculate the change in acceleration (delta) from the smoothed value
        Vector3 accelerationDelta = acceleration - smoothedAcceleration;

        // If a significant change in acceleration is detected and the cooldown period has passed, it's a tap
        if (accelerationDelta.sqrMagnitude > (tapThreshold * tapThreshold) && timeSinceLastTap > tapCooldown)
        {
            timeSinceLastTap = 0f; // Reset cooldown

            // Detect tilt based on the X-axis (for left-right motion)
            if (rotationEuler.x > right_bound_lower && rotationEuler.x < right_bound_upper) // Device tilted to the right (portrait view)
            {
                SimulateTouchInput(Vector2.right);
            }
            else if (rotationEuler.x < left_bound_lower && rotationEuler.x > left_bound_upper) // Device tilted to the left (portrait view)
            {
                SimulateTouchInput(Vector2.left);
            }
            else
            {
                // Center input (if no significant tilt detected)
                SimulateTouchInput(Vector2.zero);
            }
        }

        // Store the current acceleration for the next frame
        previousAcceleration = acceleration;
    }

    private void SimulateTouchInput(Vector2 direction)
    {
        // Default to center position
        Vector2 simulatedTouchPosition = new Vector2(Screen.width / 2f, bottomQuarterY / 2f);

        // Adjust touch position based on detected direction
        if (direction == Vector2.right)
        {
            simulatedTouchPosition.x += 100; // Simulate touch to the right
            Debug.Log($"Tap to the right");

        }
        else if (direction == Vector2.left)
        {
            simulatedTouchPosition.x -= 100; // Simulate touch to the left
            Debug.Log($"Tap to the left");
        }
        else
        {
            Debug.Log($"Tap center");
        }

        //Debug.Log($"Simulated touch at: {simulatedTouchPosition}");

        // Example: Interact with a UI button or other objects in the bottom quarter
        Ray ray = Camera.main.ScreenPointToRay(simulatedTouchPosition);
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            // Interact with objects hit by the raycast
            Debug.Log($"Hit object: {hit.collider.name}");
        }
    }
}
