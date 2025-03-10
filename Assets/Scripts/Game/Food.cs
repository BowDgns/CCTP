using UnityEngine;

public class Food : MonoBehaviour
{
    public float add_stamina = 2f;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("eaten food");

            PlayerController player = other.GetComponent<PlayerController>();   // get stamina from player controller s cript
            if (player != null)
            {
                if(player.stamina < player.max_stamina)
                {
                    player.stamina += add_stamina;
                }
            }

            Destroy(gameObject); // remove food
        }
    }
}
