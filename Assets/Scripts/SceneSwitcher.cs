using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI; 

public class SceneSwitcher : MonoBehaviour
{
    public Button button;  
    public string scene_name; 

    void Start()
    { 
        button.onClick.AddListener(SwitchScene);
    }

    void SwitchScene()
    {
        SceneManager.LoadScene(scene_name);
    }
}
