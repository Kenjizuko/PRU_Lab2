using UnityEngine;
using TMPro;
using System.IO;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance;
    public TMP_Text scoreText; // Assign UI Text in Inspector
    private float score = 0;
    private string filePath;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        filePath = Application.dataPath + "/highscore.json";
        Debug.Log("High Score File Path: " + filePath);
    }

    void Update()
    {
        scoreText.text = "Score: " + Mathf.FloorToInt(score);
    }
    public float GetScore()
    {
        return score;
    }

    public void AddPoints(float points)
    {
        score += points;
        scoreText.text = "Score: " + score; // Update TMP text
        Debug.Log("Score Updated: " + score); // Debug to check if it's working
    }

    public void SaveHighScore()
    {
        if (string.IsNullOrEmpty(filePath))
        {
            Debug.LogError("File path is null or empty!");
            return;
        }

        float highScore = LoadHighScore();
        if (score > highScore)
        {
            try
            {
                File.WriteAllText(filePath, JsonUtility.ToJson(new ScoreData { score = score }));
                Debug.Log("New High Score Saved: " + score);
            }
            catch (System.Exception e)
            {
                Debug.LogError("Failed to save high score: " + e.Message);
            }
        }
    }

    public float LoadHighScore()
    {
        if (string.IsNullOrEmpty(filePath))
        {
            Debug.LogError("File path is null or empty!");
            return 0;
        }

        if (File.Exists(filePath))
        {
            try
            {
                string json = File.ReadAllText(filePath);
                return JsonUtility.FromJson<ScoreData>(json).score;
            }
            catch (System.Exception e)
            {
                Debug.LogError("Failed to load high score: " + e.Message);
                return 0;
            }
        }
        return 0;
    }

    [System.Serializable]
    public class ScoreData
    {
        public float score;
    }
}
