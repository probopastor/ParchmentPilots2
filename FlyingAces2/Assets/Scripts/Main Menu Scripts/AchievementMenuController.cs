/*
 * AchievementMenuController.cs
 * Author(s): Grant Frey
 * Created on: 12/4/2020
 * Description: 
 */

using UnityEngine;
using UnityEngine.SceneManagement;

public class AchievementMenuController : MonoBehaviour
{
    public GameObject achievementRibbon1;
    public GameObject achievementRibbon2;
    public GameObject achievementRibbon3;
    public GameObject achievementRibbon4;

    private string currentLevelName;

    void Start()
    {
        currentLevelName = SceneManager.GetActiveScene().name;
        AcievementCheck();
    }

    private void AcievementCheck()
    {
        switch (currentLevelName)
        {
            case "Achievements":
                if (PlayerPrefs.GetInt("Achievement 1", 0) == 1)
                {
                    achievementRibbon1.SetActive(true);
                }
                else
                {
                    achievementRibbon1.SetActive(false);
                }

                if (PlayerPrefs.GetInt("Achievement 2", 0) == 1)
                {
                    achievementRibbon2.SetActive(true);
                }
                else
                {
                    achievementRibbon2.SetActive(false);
                }

                if (PlayerPrefs.GetInt("Achievement 8", 0) == 1)
                {
                    achievementRibbon3.SetActive(true);
                }
                else
                {
                    achievementRibbon3.SetActive(false);
                }

                if (PlayerPrefs.GetInt("Achievement 9", 0) == 1)
                {
                    achievementRibbon4.SetActive(true);
                }
                else
                {
                    achievementRibbon4.SetActive(false);
                }

                break;
            case "Achievements 1":
                if (PlayerPrefs.GetInt("Achievement 7", 0) == 1)
                {
                    achievementRibbon1.SetActive(true);
                }
                else
                {
                    achievementRibbon1.SetActive(false);
                }

                if (PlayerPrefs.GetInt("Achievement 4", 0) == 1)
                {
                    achievementRibbon2.SetActive(true);
                }
                else
                {
                    achievementRibbon2.SetActive(false);
                }

                if (PlayerPrefs.GetInt("Achievement 6", 0) == 1)
                {
                    achievementRibbon3.SetActive(true);
                }
                else
                {
                    achievementRibbon3.SetActive(false);
                }

                if (PlayerPrefs.GetInt("Achievement 3", 0) == 1)
                {
                    achievementRibbon4.SetActive(true);
                }
                else
                {
                    achievementRibbon4.SetActive(false);
                }
                break;
            case "Achievements 2":
                if (PlayerPrefs.GetInt("Achievement 16", 0) == 1)
                {
                    achievementRibbon1.SetActive(true);
                }
                else
                {
                    achievementRibbon1.SetActive(false);
                }

                if (PlayerPrefs.GetInt("Achievement 15", 0) == 1)
                {
                    achievementRibbon2.SetActive(true);
                }
                else
                {
                    achievementRibbon2.SetActive(false);
                }

                if (PlayerPrefs.GetInt("Achievement 14", 0) == 1)
                {
                    achievementRibbon3.SetActive(true);
                }
                else
                {
                    achievementRibbon3.SetActive(false);
                }

                if (PlayerPrefs.GetInt("Achievement 13", 0) == 1)
                {
                    achievementRibbon4.SetActive(true);
                }
                else
                {
                    achievementRibbon4.SetActive(false);
                }
                break;
            case "Achievements 3":
                if (PlayerPrefs.GetInt("Achievement 12", 0) == 1)
                {
                    achievementRibbon1.SetActive(true);
                }
                else
                {
                    achievementRibbon1.SetActive(false);
                }

                if (PlayerPrefs.GetInt("Achievement 11", 0) == 1)
                {
                    achievementRibbon2.SetActive(true);
                }
                else
                {
                    achievementRibbon2.SetActive(false);
                }

                if (PlayerPrefs.GetInt("Achievement 10", 0) == 1)
                {
                    achievementRibbon3.SetActive(true);
                }
                else
                {
                    achievementRibbon3.SetActive(false);
                }

                if (PlayerPrefs.GetInt("Achievement 17", 0) == 1)
                {
                    achievementRibbon4.SetActive(true);
                }
                else
                {
                    achievementRibbon4.SetActive(false);
                }
                break;
            case "Achievements 4":
                if (PlayerPrefs.GetInt("Achievement 18", 0) == 1)
                {
                    achievementRibbon1.SetActive(true);
                }
                else
                {
                    achievementRibbon1.SetActive(false);
                }

                if (PlayerPrefs.GetInt("Achievement 19", 0) == 1)
                {
                    achievementRibbon2.SetActive(true);
                }
                else
                {
                    achievementRibbon2.SetActive(false);
                }

                if (PlayerPrefs.GetInt("Achievement 5", 0) == 1)
                {
                    achievementRibbon3.SetActive(true);
                }
                else
                {
                    achievementRibbon3.SetActive(false);
                }

                if (PlayerPrefs.GetInt("Achievement 20", 0) == 1)
                {
                    achievementRibbon4.SetActive(true);
                }
                else
                {
                    achievementRibbon4.SetActive(false);
                }
                break;
            default:
                break;
        }
    }
}
