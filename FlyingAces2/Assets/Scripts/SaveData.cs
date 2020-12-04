/*
 * SaveData.cs
 * Author(s): Grant Frey
 * Created on: 9/30/2020
 * Description: 
 */

using UnityEngine;

public class SaveData : MonoBehaviour
{
   
    void Start()
    {
        
    }

    void Update()
    {
        
    }

    public static void SaveHighScore(string level, int score)
    {
        level += " high score";
        if((score < PlayerPrefs.GetInt(level)) || PlayerPrefs.GetInt(level, 0) == 0)
        {
            PlayerPrefs.SetInt(level, score);
        }
    }

    public static void UnlockNextLevel(string currentLevel)
    {
        switch (currentLevel)
        {
            case "Level 1-1":
                if (PlayerPrefs.GetInt("level2Unlock", 0) == 0)
                {
                    PlayerPrefs.SetInt("level2Unlock", 1);
                }
                break;
            case "level 1-2":
                if (PlayerPrefs.GetInt("level3Unlock", 0) == 0)
                {
                    PlayerPrefs.SetInt("level3Unlock", 1);
                }
                break;
            case "level 1-3":
                if (PlayerPrefs.GetInt("level4Unlock", 0) == 0)
                {
                    PlayerPrefs.SetInt("level4Unlock", 1);
                }
                break;
            default:
                break;
        }
    }

    public static void UnlockAllLevels()
    {
        PlayerPrefs.SetInt("level2Unlock", 1);
        PlayerPrefs.SetInt("level3Unlock", 1);
        PlayerPrefs.SetInt("level4Unlock", 1);
    }
}
