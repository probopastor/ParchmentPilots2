/*
 * AchievementsController.cs
 * Author(s): Grant Frey
 * Created on: 12/4/2020
 * Description: 
 */

using TMPro;
using UnityEngine;

public class AchievementsController : MonoBehaviour
{
    public GameObject achievementBanner;
    public TextMeshProUGUI achievementNameText;
    public TextMeshProUGUI achievementNameDescription;
    public AudioSource SoundEffectSource;
    public AudioClip achievementSFX;

    void Start()
    {
        
    }

    void Update()
    {
        
    }

    public void AchievementGet(int achievementNumber)
    {
        switch (achievementNumber)
        {
            case 1:
                achievementNameText.text = "You’re Number 1!";
                achievementNameDescription.text = "Get a hole-in-one";
                break;
            case 2:
                achievementNameText.text = "*Quiet Golf Clap*";
                achievementNameDescription.text = "Get a hole-in-one on all levels";
                break;
            case 3:
                achievementNameText.text = "Gliding the Waves";
                achievementNameDescription.text = "Fly into a total of 5 fans";
                break;
            case 4:
                achievementNameText.text = "Riding the Waves";
                achievementNameDescription.text = "Fly into a total of 10 fans";
                break;
            case 5:
                achievementNameText.text = "Fan of the Fans";
                achievementNameDescription.text = "Fly into every fan in a single level";
                break;
            case 6:
                achievementNameText.text = "MAXIMUM POWER!";
                achievementNameDescription.text = "Make a throw at full charge";
                break;
            case 7:
                achievementNameText.text = "minimum power...";
                achievementNameDescription.text = "Make a throw at  the lowest charge";
                break;
            case 8:
                achievementNameText.text = "A Fine Addition";
                achievementNameDescription.text = "Get a collectible";
                break;
            case 9:
                achievementNameText.text = "3’s a Crowd";
                achievementNameDescription.text = "Collect all three collectibles on a level";
                break;
            case 10:
                achievementNameText.text = "Collection Complete!";
                achievementNameDescription.text = "Collect all the collectibles in the game";
                break;
            case 11:
                achievementNameText.text = "A stock in reserve";
                achievementNameDescription.text = "Complete a level with only weak throws";
                break;
            case 12:
                achievementNameText.text = "Step on the gas";
                achievementNameDescription.text = "Complete a level with only strong throws";
                break;
            case 13:
                achievementNameText.text = "Parked in Idle";
                achievementNameDescription.text = "Be afk for 3 minutes";
                break;
            case 14:
                achievementNameText.text = "Airborne";
                achievementNameDescription.text = "Don’t hit any objects for 30 seconds";
                break;
            case 15:
                achievementNameText.text = "Breakfast is served!";
                achievementNameDescription.text = "Fly into the toast in level 4";
                break;
            case 16:
                achievementNameText.text = "Please Recycle!";
                achievementNameDescription.text = "Fly into a trash bin";
                break;
            case 17:
                achievementNameText.text = "Thanks for the love!";
                achievementNameDescription.text = "Check out the Credits Menu";
                break;
            case 18:
                achievementNameText.text = "Ludicrous speed, GO!";
                achievementNameDescription.text = "Hit maximum velocity";
                break;
            case 19:
                achievementNameText.text = "Party time!";
                achievementNameDescription.text = "Party with birds and robots";
                break;
            case 20:
                achievementNameText.text = "101%";
                achievementNameDescription.text = "Collect all other achievements";
                break;
            default:
                break;
        }
        achievementBanner.SetActive(true);
        SoundEffectSource.clip = achievementSFX;
        SoundEffectSource.Play();
        Invoke("CloseAchievement", 2f);
    }

    void CloseAchievement()
    {
        achievementBanner.SetActive(false);
        if (PlayerPrefs.GetInt("Achievement 1", 0) == 1 && PlayerPrefs.GetInt("Achievement 2", 0) == 1 && PlayerPrefs.GetInt("Achievement 3", 0) == 1 &&
            PlayerPrefs.GetInt("Achievement 4", 0) == 1 && PlayerPrefs.GetInt("Achievement 5", 0) == 1 && PlayerPrefs.GetInt("Achievement 6", 0) == 1 &&
            PlayerPrefs.GetInt("Achievement 7", 0) == 1 && PlayerPrefs.GetInt("Achievement 8", 0) == 1 && PlayerPrefs.GetInt("Achievement 9", 0) == 1 &&
            PlayerPrefs.GetInt("Achievement 10", 0) == 1 && PlayerPrefs.GetInt("Achievement 11", 0) == 1 && PlayerPrefs.GetInt("Achievement 12", 0) == 1 &&
            PlayerPrefs.GetInt("Achievement 13", 0) == 1 && PlayerPrefs.GetInt("Achievement 14", 0) == 1 && PlayerPrefs.GetInt("Achievement 15", 0) == 1 &&
            PlayerPrefs.GetInt("Achievement 16", 0) == 1 && PlayerPrefs.GetInt("Achievement 17", 0) == 1 && PlayerPrefs.GetInt("Achievement 18", 0) == 1 &&
            PlayerPrefs.GetInt("Achievement 19", 0) == 1 && PlayerPrefs.GetInt("Achievement 20", 0) == 0)
        {
            PlayerPrefs.SetInt("Achievement 20", 1);
            AchievementGet(20);
        }
    }
}
