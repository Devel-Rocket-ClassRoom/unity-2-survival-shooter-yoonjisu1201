using TMPro;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;
using UnityEngine.UI;

public class UiManager : MonoBehaviour
{
    private int Score;
    public TextMeshProUGUI scoreText;

    private void start()
    {
        scoreText.text = "score: " + Score;
    }

    public void AddScore(int add)
    {
        Score += add;
        scoreText.text = $"Score: {Score}"; ;
    }

}
