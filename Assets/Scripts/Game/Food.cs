using UnityEngine;

public class Food : MonoBehaviour
{
    public float add_stamina = 20f; // Amount of stamina to add when eaten

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player")) // Ensure the object has the "Player" tag
        {
            Debug.Log("Food eaten!");

            // Get the PlayerController component from the player
            PlayerController player = other.GetComponent<PlayerController>();
            if (player != null)
            {
                // Increase stamina and clamp it to max_stamina
                player.stamina = Mathf.Min(player.stamina + add_stamina, player.max_stamina);

                Debug.Log("New Stamina: " + player.stamina);
            }

            Destroy(gameObject); // Destroy the food object
        }
    }
}
