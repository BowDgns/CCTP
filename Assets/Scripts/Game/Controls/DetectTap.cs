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

        
            if (PlayerPrefs.HasKey("left_bound_lower"))
            {
                float left_bound_lower = PlayerPrefs.GetFloat("left_bound_lower");
                float left_bound_upper = PlayerPrefs.GetFloat("left_bound_upper");
                float right_bound_lower = PlayerPrefs.GetFloat("right_bound_lower");
                float right_bound_upper = PlayerPrefs.GetFloat("right_bound_upper");

                Debug.Log($"Loaded Calibration: Left [{left_bound_lower}, {left_bound_upper}], Right [{right_bound_lower}, {right_bound_upper}]");
            }
            else
            {
                Debug.Log("No calibration data found. Please calibrate first.");
            }
    }

    private void Update()
    {
        timeSinceLastTap += Time.deltaTime;

        DetectTap();
        
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

            if (rotationEuler.x > right_bound_lower /*&& rotationEuler.x < right_bound_upper*/)
            {
                playerController.Jump(false);
                Debug.Log("Tap to the right");
            }
            else if (rotationEuler.x < left_bound_lower /*&& rotationEuler.x > left_bound_upper*/)
            {
                playerController.Jump(true);
                Debug.Log("Tap to the left");
            }

            Debug.Log(rotationEuler.x);
        }
    }
}
