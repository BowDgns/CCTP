using Unity.VisualScripting;
using UnityEngine;

public class DetectTapInput : MonoBehaviour
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

    public PlayerController playerController;

    private void Start()
    {
        // Enable the gyro if you need orientation data
        Input.gyro.enabled = true;

        // Calculate Y position for the bottom quarter of the screen
        bottomQuarterY = Screen.height / 4f;

        // Initialize smoothed acceleration
        smoothedAcceleration = Input.acceleration;
        //previousAcceleration = smoothedAcceleration;
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
                playerController.Jump(Vector2.right);
                Debug.Log($"Tap to the right");
            }
            else if (rotationEuler.x < left_bound_lower && rotationEuler.x > left_bound_upper) // Device tilted to the left (portrait view)
            {
                playerController.Jump(Vector2.left);
                Debug.Log($"Tap to the left");
            }
            else
            {
                playerController.Jump(Vector2.zero);
                Debug.Log($"Tap center");
            }
        }

        //previousAcceleration = acceleration;
    }
}
