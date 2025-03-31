using UnityEngine;

public class GameState : MonoBehaviour
{
    // GameObject to check for inactivity
    public GameObject objectToCheck;
    // GameObject to activate if objectToCheck is inactive
    public GameObject objectToActivate;

    void Update()
    {
        // Ensure both references are assigned
        if (objectToCheck != null && objectToActivate != null)
        {
            // Check if objectToCheck is inactive
            if (!objectToCheck.activeSelf)
            {
                // Activate objectToActivate
                objectToActivate.SetActive(true);
                // Optionally, disable this script after activation if further checking isn't needed
                // this.enabled = false;
            }
        }
    }
}
