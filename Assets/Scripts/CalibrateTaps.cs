using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class CalibrateTaps : MonoBehaviour
{
    public TextMeshProUGUI calibrationText; // UI text to show instructions

    public bool isCalibrating = true;
    private int rightTaps = 0;
    private int leftTaps = 0;
    private float leftMin = 360f, leftMax = 0f;
    private float rightMin = 360f, rightMax = 0f;

    // polish
    public Rigidbody2D frog_left;
    public Rigidbody2D frog_right;

    public Sprite player_jump_sprite;
    public Sprite player_idle_sprite;
    public Sprite shadow_jump_sprite;
    public Sprite shadow_idle_sprite;

    public SpriteRenderer sprite_renderer_left;
    public SpriteRenderer shadow_sprite_renderer_left;
    public SpriteRenderer sprite_renderer_right;
    public SpriteRenderer shadow_sprite_renderer_right;

    public GameObject back_button;

    public float jump_force = 5;
    public float tapThreshold = 0.1f; // Detect spikes in acceleration
    public float lowPassFilterFactor = 0.2f; // Smooth sensitivity
    private Vector3 smoothedAcceleration;

    private float timeSinceLastTap = 0f;
    private float tapCooldown = 0.5f;

    private void Start()
    {
        Input.gyro.enabled = true;
        smoothedAcceleration = Input.acceleration;
        UpdateCalibrationText("Tap right 3 times to calibrate.");
        back_button.SetActive(false);
    }

    private void Update()
    {
        if (isCalibrating)
        {
            CalibrateTap();
        }

        AnimatePlayer();
    }

    public void StartCalibration()
    {
        isCalibrating = true;
        rightTaps = 0;
        leftTaps = 0;
        leftMin = 360f; leftMax = 0f;
        rightMin = 360f; rightMax = 0f;
        UpdateCalibrationText("Tap right 3 times to calibrate.");
        Debug.Log("Calibration started.");
    }

    public void StopCalibration()
    {
        isCalibrating = false;
        PlayerPrefs.SetFloat("left_bound_lower", leftMin);
        PlayerPrefs.SetFloat("left_bound_upper", leftMax);
        PlayerPrefs.SetFloat("right_bound_lower", rightMin);
        PlayerPrefs.SetFloat("right_bound_upper", rightMax);
        PlayerPrefs.Save();

        UpdateCalibrationText("Calibration complete! Press back to play.");
        Debug.Log($"Calibration saved: Left [{leftMin}, {leftMax}], Right [{rightMin}, {rightMax}]");
        back_button.SetActive(true);
    }

    private void CalibrateTap()
    {
        timeSinceLastTap += Time.deltaTime;

        // Get the accelerometer values
        Vector3 acceleration = Input.acceleration;
        Quaternion rotation = Input.gyro.attitude;
        Vector3 rotationEuler = rotation.eulerAngles;

        smoothedAcceleration = Vector3.Lerp(smoothedAcceleration, acceleration, lowPassFilterFactor);
        Vector3 accelerationDelta = acceleration - smoothedAcceleration;

        if (accelerationDelta.sqrMagnitude > (tapThreshold * tapThreshold) && timeSinceLastTap > tapCooldown)
        {
            timeSinceLastTap = 0f; // Reset cooldown

            // Detect right tap
            if (rightTaps < 3)
            {
                rightMin = Mathf.Min(rightMin, rotationEuler.x);
                rightMax = Mathf.Max(rightMax, rotationEuler.x);

                rightTaps++;

                // Apply jump force to the right frog
                frog_right.AddForce(Vector2.up * jump_force, ForceMode2D.Impulse);

                UpdateCalibrationText($"Right Taps: {rightTaps}/3 - Angle: {rotationEuler.x}");

                if (rightTaps == 3)
                {
                    UpdateCalibrationText("Now tap left 3 times to calibrate.");
                }
            }
            // Detect left tap after right taps are complete
            else if (leftTaps < 3 && rightTaps == 3)
            {
                leftMin = Mathf.Min(leftMin, rotationEuler.x);
                leftMax = Mathf.Max(leftMax, rotationEuler.x);

                leftTaps++;

                // Apply jump force to the left frog
                frog_left.AddForce(Vector2.up * jump_force, ForceMode2D.Impulse);

                UpdateCalibrationText($"Left Taps: {leftTaps}/3 - Angle: {rotationEuler.x}");

                if (leftTaps == 3)
                {
                    StopCalibration();
                }
            }
        }
    }

    private void UpdateCalibrationText(string message)
    {
        if (calibrationText != null)
        {
            calibrationText.text = message;
        }
    }

    void AnimatePlayer()
    {
        if (frog_left.velocity.y > 0.1)
        {
            sprite_renderer_left.sprite = player_jump_sprite;
            shadow_sprite_renderer_left.sprite = shadow_jump_sprite;
        }

        else if (frog_right.velocity.y > 0.1)
        {
            sprite_renderer_right.sprite = player_jump_sprite;
            shadow_sprite_renderer_right.sprite = shadow_jump_sprite;
        }

        else
        {
            sprite_renderer_left.sprite = player_idle_sprite;
            shadow_sprite_renderer_left.sprite = shadow_idle_sprite;
            sprite_renderer_right.sprite = player_idle_sprite;
            shadow_sprite_renderer_right.sprite = shadow_idle_sprite;
        }
    }
}
