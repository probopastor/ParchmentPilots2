/*
 * LevelSelectUIController.cs
 * Author(s): Grant Frey
 * Created on: 9/30/2020
 * Description: 
 */

using TMPro;
using UnityEngine;

public class LevelSelectUIController : MonoBehaviour
{
    public TMP_Text level1HighScore;

    public GameObject level2Button;
    public TMP_Text level2HighScore;

    public GameObject level3Button;
    public TMP_Text level3HighScore;

    public GameObject level4Button;
    public TMP_Text level4HighScore;

    void Start()
    {
        CheckSaveData();
    }

    void Update()
    {
        
    }

    private void CheckSaveData()
    {
        if(PlayerPrefs.GetInt("Level 1-1 high score", 0) == 0)
        {
            //level1HighScore.text = "--";
        }
        else
        {
            //level1HighScore.text = PlayerPrefs.GetInt("Level 1-1 high score", 0).ToString();
        }

        if (PlayerPrefs.GetInt("level2Unlock", 0) == 0)
        {
            level2Button.SetActive(false);
        }
        else
        {
            level2Button.SetActive(true);
            if (PlayerPrefs.GetInt("level 1-2 high score", 0) == 0)
            {
                //level2HighScore.text = "--";
            }
            else
            {
                //level2HighScore.text = PlayerPrefs.GetInt("level 1-2 high score", 0).ToString();
            }
        }

        if (PlayerPrefs.GetInt("level3Unlock", 0) == 0)
        {
            level3Button.SetActive(false);
        }
        else
        {
            level3Button.SetActive(true);
            if (PlayerPrefs.GetInt("level 1-3 high score", 0) == 0)
            {
                //level3HighScore.text = "--";
            }
            else
            {
                //level3HighScore.text = PlayerPrefs.GetInt("level 1-3 high score", 0).ToString();
            }
        }

        if (PlayerPrefs.GetInt("level4Unlock", 0) == 0)
        {
            level4Button.SetActive(false);
        }
        else
        {
            level4Button.SetActive(true);
            if (PlayerPrefs.GetInt("level 1-4 high score", 0) == 0)
            {
                //level4HighScore.text = "--";
            }
            else
            {
                //level4HighScore.text = PlayerPrefs.GetInt("level 1-4 high score", 0).ToString();
            }
        }
    }
}
