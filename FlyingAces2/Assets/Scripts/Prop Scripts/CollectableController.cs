/*
 * CollectableController.cs
 * Author(s): Grant Frey
 * Created on: 9/30/2020
 * Description: 
 */

using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class CollectableController : MonoBehaviour
{

    public GameObject bobblehead_1;
    public GameObject bobblehead_2;
    public GameObject bobblehead_3;

    public GameObject grayBobblehead_1;
    public GameObject grayBobblehead_2;
    public GameObject grayBobblehead_3;

    private bool bobblehead1Collected = false;
    private bool bobblehead2Collected = false;
    private bool bobblehead3Collected = false;

    private string currentLevelName;

    public AudioSource sfxSource;
    public AudioClip bobbleheadClip;

    public GameObject bobbleheadParticles;

    public Sprite bobbleheadUncollected;
    public Sprite bobbleheadCollected;

    public Image bobbleheadImage1;
    public Image bobbleheadImage2;
    public Image bobbleheadImage3;

    void Start()
    {
        currentLevelName = SceneManager.GetActiveScene().name;
        if(PlayerPrefs.GetInt(currentLevelName + " bobblehead 1", 0) != 0)
        {
            bobblehead1Collected = true;
            bobbleheadImage1.sprite = bobbleheadCollected;
            bobblehead_1.GetComponent<Bobblehead>().GrayOut();
            grayBobblehead_1.SetActive(true);
        }
        else
        {
            bobbleheadImage1.sprite = bobbleheadUncollected;
        }
        if (PlayerPrefs.GetInt(currentLevelName + " bobblehead 2", 0) != 0)
        {
            bobblehead2Collected = true;
            bobbleheadImage2.sprite = bobbleheadCollected;
            bobblehead_2.GetComponent<Bobblehead>().GrayOut();
            grayBobblehead_2.SetActive(true);
        }
        else
        {
            bobbleheadImage2.sprite = bobbleheadUncollected;
        }
        if (PlayerPrefs.GetInt(currentLevelName + " bobblehead 3", 0) != 0)
        {
            bobblehead3Collected = true;
            bobbleheadImage3.sprite = bobbleheadCollected;
            bobblehead_3.GetComponent<Bobblehead>().GrayOut();
            grayBobblehead_3.SetActive(true);
        }
        else
        {
            bobbleheadImage3.sprite = bobbleheadUncollected;
        }
    }

    void Update()
    {
        
    }

    public void CollectBobblehead(int bobbleheadNumber)
    {
        switch (bobbleheadNumber)
        {
            case 1:
                bobblehead1Collected = true;
                bobbleheadImage1.sprite = bobbleheadCollected;
                break;
            case 2:
                bobblehead2Collected = true;
                bobbleheadImage2.sprite = bobbleheadCollected;
                break;
            case 3:
                bobblehead3Collected = true;
                bobbleheadImage3.sprite = bobbleheadCollected;
                break;
            default:
                break;
        }
        if(PlayerPrefs.GetInt("Achievement 8", 0) == 0)
        {
            PlayerPrefs.SetInt("Achievement 8", 1);
        }
    }

    public void SaveBobbleheadCollection(string level)
    {
        if (bobblehead1Collected)
        {
            PlayerPrefs.SetInt(currentLevelName + " bobblehead 1", 1);
        }
        if (bobblehead2Collected)
        {
            PlayerPrefs.SetInt(currentLevelName + " bobblehead 2", 1);
        }
        if (bobblehead3Collected)
        {
            PlayerPrefs.SetInt(currentLevelName + " bobblehead 3", 1);
        }
        if(PlayerPrefs.GetInt(currentLevelName + " bobblehead 1", 0) == 1 && PlayerPrefs.GetInt(currentLevelName + " bobblehead 2", 0) == 1 &&
            PlayerPrefs.GetInt(currentLevelName + " bobblehead 3", 0) == 1 && PlayerPrefs.GetInt("Achievement 9", 0) == 0)
        {
            PlayerPrefs.SetInt("Achievement 9", 1);
        }
        if(PlayerPrefs.GetInt("Level 1-1 bobblehead 1", 0) == 0 && PlayerPrefs.GetInt("Level 1-1 bobblehead 2", 0) == 0 && PlayerPrefs.GetInt("Level 1-1 bobblehead 3", 0) == 0 &&
            PlayerPrefs.GetInt("level 1-2 bobblehead 1", 0) == 0 && PlayerPrefs.GetInt("level 1-2 bobblehead 2", 0) == 0 && PlayerPrefs.GetInt("level 1-2 bobblehead 3", 0) == 0 &&
            PlayerPrefs.GetInt("level 1-3 bobblehead 1", 0) == 0 && PlayerPrefs.GetInt("level 1-3 bobblehead 2", 0) == 0 && PlayerPrefs.GetInt("level 1-3 bobblehead 3", 0) == 0 &&
            PlayerPrefs.GetInt("level 1-4 bobblehead 1", 0) == 0 && PlayerPrefs.GetInt("level 1-4 bobblehead 2", 0) == 0 && PlayerPrefs.GetInt("level 1-4 bobblehead 3", 0) == 0 &&
            PlayerPrefs.GetInt("Achievement 10", 0) == 0)
        {
            PlayerPrefs.SetInt("Achievement 10", 1);
        }
    }

    public void PlayBobbleheadSound()
    {
        sfxSource.clip = bobbleheadClip;
        sfxSource.Play();
    }
}
