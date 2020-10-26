/*
 * CollectableController.cs
 * Author(s): Grant Frey
 * Created on: 9/30/2020
 * Description: 
 */

using UnityEngine;
using UnityEngine.SceneManagement;

public class CollectableController : MonoBehaviour
{

    public GameObject bobblehead_1;
    public GameObject bobblehead_2;
    public GameObject bobblehead_3;

    private bool bobblehead1Collected = false;
    private bool bobblehead2Collected = false;
    private bool bobblehead3Collected = false;

    private string currentLevelName;

    public AudioSource sfxSource;
    public AudioClip bobbleheadClip;

    public GameObject bobbleheadParticles;

    void Start()
    {
        currentLevelName = SceneManager.GetActiveScene().name;
        if(PlayerPrefs.GetInt(currentLevelName + " bobblehead 1", 0) != 0)
        {
            bobblehead1Collected = true;
            bobblehead_1.GetComponent<Bobblehead>().GrayOut();
        }
        if (PlayerPrefs.GetInt(currentLevelName + " bobblehead 2", 0) != 0)
        {
            bobblehead2Collected = true;
            bobblehead_2.GetComponent<Bobblehead>().GrayOut();
        }
        if (PlayerPrefs.GetInt(currentLevelName + " bobblehead 3", 0) != 0)
        {
            bobblehead3Collected = true;
            bobblehead_3.GetComponent<Bobblehead>().GrayOut();
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
                break;
            case 2:
                bobblehead2Collected = true;
                break;
            case 3:
                bobblehead3Collected = true;
                break;
            default:
                break;
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
    }

    public void PlayBobbleheadSound()
    {
        sfxSource.clip = bobbleheadClip;
        sfxSource.Play();
    }
}
