using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.Video;

public class UIHandler : MonoBehaviour
{
    [Tooltip("Number of strokes to score a par")]
    public int par = 0;

    [Tooltip("The hole the player is currently on")]
    public int hole = 0;

    [Tooltip("The text object that displays the stroke the player is on")]
    public TextMeshProUGUI strokeText;

    [Tooltip("The text object that displays the par for the course")]
    public TextMeshProUGUI parText;

    [Tooltip("The text object that displays the hole the player is on")]
    public TextMeshProUGUI holeText;

    [Tooltip("The text that displays what score the player got on the hole")]
    public TextMeshProUGUI scoreText;

    [Tooltip("The notecard the score text is displayed on")]
    public GameObject scoreCard;

    private ScoreManager scoreManager;

    void OnEnable()
    {
        scoreManager = GameObject.FindObjectOfType<ScoreManager>();
    }

    // Start is called before the first frame update
    void Start()
    {
        scoreManager = GameObject.FindObjectOfType<ScoreManager>();
        strokeText.text = scoreManager.GetStroke().ToString();
        parText.text = par.ToString();
        holeText.text = hole.ToString();
        scoreCard.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void FinishScore()
    {
        int scoreStroke = scoreManager.GetStroke();
        strokeText.text = scoreManager.GetStroke().ToString();

        scoreCard.SetActive(true);

        if (scoreStroke == 1)
        {
            scoreText.text = "Hole in one! Smooth flying!";
        }
        else if (scoreStroke == par - 1)
        {
            scoreText.text = "You got a birdie! Nice!";
        }
        else if (scoreStroke == par - 2)
        {
            scoreText.text = "You got an eagle! Good job!";
        }
        else if (scoreStroke == par)
        {
            scoreText.text = "You got a par!";
        }
        else if (scoreStroke == par - 3)
        {
            scoreText.text = "You got an albatross! Amazing!";
        }
        else if (scoreStroke == par + 1)
        {
            scoreText.text = "You got a bogey. So close...";
        }
        else if (scoreStroke == par + 2)
        {
            scoreText.text = "Double bogey. Better luck next time.";
        }
        else if (scoreStroke == par + 3)
        {
            scoreText.text = "Triple bogey. Next hole will be better.";
        }
        else if (scoreStroke > par + 4)
        {
            scoreText.text = "Let's not talk about that hole...";
        }
    }

    public void UpdateScoreText()
    {
        strokeText.text = scoreManager.GetStroke().ToString();
    }
}
