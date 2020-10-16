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
    public TMP_Text level1Bobbleheads;
    private int level1BobbleheadCount;

    public GameObject level2Button;
    public TMP_Text level2HighScore;
    public TMP_Text level2Bobbleheads;
    private int level2BobbleheadCount;

    public GameObject level3Button;
    public TMP_Text level3HighScore;
    public TMP_Text level3Bobbleheads;
    private int level3BobbleheadCount;

    public GameObject level4Button;
    public TMP_Text level4HighScore;
    public TMP_Text level4Bobbleheads;
    private int level4BobbleheadCount;

    void Start()
    {
        CheckBobbleheadCount();
        CheckSaveData();
    }

    void Update()
    {
        
    }

    private void CheckSaveData()
    {
        if(PlayerPrefs.GetInt("Level 1-1 high score", 0) == 0)
        {
            level1HighScore.text = "Best-Score: --";
        }
        else
        {
            level1HighScore.text = "Best-Score: " + PlayerPrefs.GetInt("Level 1-1 high score", 0).ToString();
        }

        level1Bobbleheads.text = level1BobbleheadCount.ToString() + "/3";

        if (PlayerPrefs.GetInt("level2Unlock", 0) == 0)
        {
            level2Button.SetActive(false);
        }
        else
        {
            level2Button.SetActive(true);
            if (PlayerPrefs.GetInt("level 1-2 high score", 0) == 0)
            {
                level2HighScore.text = "Best-Score: --";
            }
            else
            {
                level2HighScore.text = "Best-Score: " + PlayerPrefs.GetInt("level 1-2 high score", 0).ToString();
            }
            level2Bobbleheads.text = level2BobbleheadCount.ToString() + "/3";
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
                level3HighScore.text = "Best-Score: --";
            }
            else
            {
                level3HighScore.text = "Best-Score: " + PlayerPrefs.GetInt("level 1-3 high score", 0).ToString();
            }
            level3Bobbleheads.text = level3BobbleheadCount.ToString() + "/3";
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
                level4HighScore.text = "Best-Score: --";
            }
            else
            {
                level4HighScore.text = "Best-Score: " + PlayerPrefs.GetInt("level 1-4 high score", 0).ToString();
            }
            level4Bobbleheads.text = level4BobbleheadCount.ToString() + "/3";
        }
    }

    private void CheckBobbleheadCount()
    {
        level1BobbleheadCount = 0;

        if (PlayerPrefs.GetInt("Level 1-1 bobblehead 1", 0) != 0)
        {
            level1BobbleheadCount++;
        }
        if (PlayerPrefs.GetInt("Level 1-1 bobblehead 2", 0) != 0)
        {
            level1BobbleheadCount++;
        }
        if (PlayerPrefs.GetInt("Level 1-1 bobblehead 3", 0) != 0)
        {
            level1BobbleheadCount++;
        }

        level2BobbleheadCount = 0;

        if (PlayerPrefs.GetInt("level 1-2 bobblehead 1", 0) != 0)
        {
            level2BobbleheadCount++;
        }
        if (PlayerPrefs.GetInt("level 1-2 bobblehead 2", 0) != 0)
        {
            level2BobbleheadCount++;
        }
        if (PlayerPrefs.GetInt("level 1-2 bobblehead 3", 0) != 0)
        {
            level2BobbleheadCount++;
        }

        level3BobbleheadCount = 0;

        if (PlayerPrefs.GetInt("level 1-3 bobblehead 1", 0) != 0)
        {
            level3BobbleheadCount++;
        }
        if (PlayerPrefs.GetInt("level 1-3 bobblehead 2", 0) != 0)
        {
            level3BobbleheadCount++;
        }
        if (PlayerPrefs.GetInt("level 1-3 bobblehead 3", 0) != 0)
        {
            level3BobbleheadCount++;
        }

        level4BobbleheadCount = 0;

        if (PlayerPrefs.GetInt("level 1-4 bobblehead 1", 0) != 0)
        {
            level4BobbleheadCount++;
        }
        if (PlayerPrefs.GetInt("level 1-4 bobblehead 2", 0) != 0)
        {
            level4BobbleheadCount++;
        }
        if (PlayerPrefs.GetInt("level 1-4 bobblehead 3", 0) != 0)
        {
            level4BobbleheadCount++;
        }
    }
}
