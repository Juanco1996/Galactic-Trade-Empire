using UnityEngine;
using System.IO;

[System.Serializable]
public class GameData
{
    public double cosmicCredits;
    public double totalCosmicCredits;
    public double voidOpals;
    // Add other fields as necessary
}

public class SaveManager : MonoBehaviour
{
    public static SaveManager Instance;
    private string saveFilePath;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);

        saveFilePath = Application.persistentDataPath + "/saveData.json";
    }

    public void SaveGame()
    {
        GameData data = new GameData
        {
            cosmicCredits = GameManager.Instance.CosmicCredits,
            totalCosmicCredits = GameManager.Instance.TotalCosmicCredits,
        };

        string json = JsonUtility.ToJson(data);
        File.WriteAllText(saveFilePath, json);
    }

    public void LoadGame()
    {
        if (File.Exists(saveFilePath))
        {
            string json = File.ReadAllText(saveFilePath);
            GameData data = JsonUtility.FromJson<GameData>(json);

            GameManager.Instance.CosmicCredits = data.cosmicCredits;
            GameManager.Instance.TotalCosmicCredits = data.totalCosmicCredits;
        }
        else
        {
            // Handle case when no save file exists
        }
    }
}
