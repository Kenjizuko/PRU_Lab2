using UnityEngine;
using TMPro;
using System.IO;
using Unity.Netcode;

public class MainMenuManager : MonoBehaviour
{
    public TMP_Text highScoreText;

    void Start()
    {
        if (NetworkManager.Singleton != null)
        {
            NetworkManager.Singleton.Shutdown();
            Debug.Log("NetworkManager shut down.");
        }
        float highScore = LoadHighScoreFromFile();
        highScoreText.text = "High Score: " + highScore;
        Debug.Log("Loaded High Score in Menu: " + highScore);
    }


    private float LoadHighScoreFromFile()
    {
        string filePath = Application.dataPath + "/highscore.json";  

        if (File.Exists(filePath))
        {
            try
            {

                string json = File.ReadAllText(filePath);

                ScoreData scoreData = JsonUtility.FromJson<ScoreData>(json);
                return scoreData.score;
            }
            catch (System.Exception e)
            {
                Debug.LogError("Failed to load high score: " + e.Message);
                return 0;
            }
        }
        else
        {
            Debug.LogWarning("High score file not found. Returning 0.");
            return 0;  
        }
    }
}


[System.Serializable]
public class ScoreData
{
    public float score;
}
