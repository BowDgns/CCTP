using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;  // For UI Button

public class SceneSwitcher : MonoBehaviour
{
    public Button switchButton;  // Reference to your button

    void Start()
    {
        // Add listener to the button to call the SwitchScene method when clicked
        switchButton.onClick.AddListener(SwitchScene);
    }

    void SwitchScene()
    {
        // Change "SceneName" to the name of the scene you want to load
        SceneManager.LoadScene("Game");
    }
}
