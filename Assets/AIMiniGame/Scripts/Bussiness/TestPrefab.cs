using TMPro;
using UnityEngine;

public class TestPrefab : MonoBehaviour {
    public TextMeshProUGUI TxtDemo;

    void Start() {
        var config = TestFileConfig.Get("test_id");
        if (config != null) {
            TxtDemo.text = $"Player Speeds: {string.Join(", ", config.playerSpeeds)}" +
                           $"\r\nMax Scores: {string.Join(", ", config.maxScores)}" +
                           $"\r\nGame Titles: {string.Join(", ", config.gameTitles)}";
        }
    }
}
