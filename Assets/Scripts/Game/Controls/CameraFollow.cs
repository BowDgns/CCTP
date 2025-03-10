using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform player;  // Reference to the player
    public float offset = 2f; // Vertical offset to keep the player on screen

    void Update()
    {
        if (player.position.y > transform.position.y - offset)
        {
            // Move the camera up with the player
            transform.position = new Vector3(transform.position.x, player.position.y + offset, transform.position.z);
        }
    }
}
