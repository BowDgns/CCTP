using UnityEngine;

public class DetectTapInput : MonoBehaviour
{
    private Vector3 acceleration;
    public float tapThreshold = 0.5f; // detect spikes in acceleration
    public float lowPassFilterFactor = 0.1f; // smooth sensitivity
    private Vector3 smoothedAcceleration;

    //bounds for left/right taps
    private float right_bound_lower;
    private float right_bound_upper;
    private float left_bound_lower;
    private float left_bound_upper;

    private float timeSinceLastTap = 0f;
    private float tapCooldown = 0.5f;

    public PlayerController playerController;

    private bool isCalibrating = false;
    private float leftMin = 360f, leftMax = 0f;
    private float rightMin = 360f, rightMax = 0f;

    private void Start()
    {
        Input.gyro.enabled = true;
        smoothedAcceleration = Input.acceleration;
    }

    private void Update()
    {
        timeSinceLastTap += Time.deltaTime;

        if (isCalibrating)
        {
            CalibrateTaps();
        }
        else
        {
            DetectTap();
        }
    }

    public void StartCalibration()
    {
        isCalibrating = true;
        leftMin = 360f; leftMax = 0f;
        rightMin = 360f; rightMax = 0f;
        Debug.Log("Calibration started: Perform left and right taps.");
    }

    public void StopCalibration()
    {
        isCalibrating = false;

        // Set bounds dynamically based on collected data
        left_bound_lower = leftMin;
        left_bound_upper = leftMax;
        right_bound_lower = rightMin;
        right_bound_upper = rightMax;

        Debug.Log($"Calibration complete: Left [{left_bound_lower}, {left_bound_upper}], Right [{right_bound_lower}, {right_bound_upper}]");
    }

    private void CalibrateTaps()
    {
        Quaternion rotation = Input.gyro.attitude;
        Vector3 rotationEuler = rotation.eulerAngles;

        if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
        {
            Debug.Log("Recording Calibration Tap...");

            if (rotationEuler.x > 180) // Adjust if needed to fit your device orientation
            {
                leftMin = Mathf.Min(leftMin, rotationEuler.x);
                leftMax = Mathf.Max(leftMax, rotationEuler.x);
                Debug.Log($"Left Tap Recorded: {rotationEuler.x}");
            }
            else
            {
                rightMin = Mathf.Min(rightMin, rotationEuler.x);
                rightMax = Mathf.Max(rightMax, rotationEuler.x);
                Debug.Log($"Right Tap Recorded: {rotationEuler.x}");
            }
        }
    }

    private void DetectTap()
    {
        acceleration = Input.acceleration;
        Quaternion rotation = Input.gyro.attitude;
        Vector3 rotationEuler = rotation.eulerAngles;

        smoothedAcceleration = Vector3.Lerp(smoothedAcceleration, acceleration, lowPassFilterFactor);
        Vector3 accelerationDelta = acceleration - smoothedAcceleration;

        if (accelerationDelta.sqrMagnitude > (tapThreshold * tapThreshold) && timeSinceLastTap > tapCooldown)
        {
            timeSinceLastTap = 0f; // Reset cooldown

            if (rotationEuler.x > right_bound_lower && rotationEuler.x < right_bound_upper)
            {
                playerController.Jump(false);
                Debug.Log("Tap to the right");
            }
            else if (rotationEuler.x < left_bound_lower && rotationEuler.x > left_bound_upper)
            {
                playerController.Jump(true);
                Debug.Log("Tap to the left");
            }
            else
            {
                Debug.Log("Tap center");
            }
        }
    }
}
