using UnityEngine;
using TMPro;

public class ScoreManager : MonoBehaviour
{
    public TMP_Text score_text;
    public TMP_Text score_shadow;
    public Transform player;

    private float max_height = 0f;

    void Update()
    {
        // get player height and set max height
        if (player.position.y > max_height)
        {
            max_height = player.position.y;
        }

        // update score text
        score_text.text = Mathf.Round(max_height * 1.5f).ToString();
        score_shadow.text = Mathf.Round(max_height * 1.5f).ToString();
    }
}
