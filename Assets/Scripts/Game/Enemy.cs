using UnityEngine;

public class Enemy : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("hit player");

            // get player 
            Rigidbody2D playerRb = other.GetComponent<Rigidbody2D>();

            if (playerRb != null)
            {
                // disable the player controller so no more input can be done
                PlayerController playerController = other.GetComponent<PlayerController>();
                if (playerController != null)
                {
                    playerController.enabled = false;
                }

                // disable the DetectTap GameObject in the scene
                GameObject detectTapObject = GameObject.Find("DetectTap");
                if (detectTapObject != null)
                {
                    detectTapObject.SetActive(false);
                }

                // reset player's velocity to zero to stop further movement
                playerRb.velocity = Vector2.zero;
            }
        }
    }
}
