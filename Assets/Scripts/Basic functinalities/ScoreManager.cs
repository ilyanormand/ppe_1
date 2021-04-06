using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScoreManager : MonoBehaviour
{
    // Start is called before the first frame update
    private Board board;
    public Text scoreText;
    public int score;
    public Image scoreBar;
    private GameData gameData;
    private int numberStars;
    private GoalManager goalManager;
    void Start()
    {
        board = FindObjectOfType<Board>();
        gameData = FindObjectOfType<GameData>();
        goalManager = FindObjectOfType<GoalManager>();
    }

    // Update is called once per frame
    void Update()
    {
        scoreText.text = score.ToString();
    }

    public void InreaseScore(int amountToIncrease) 
    {
        score += amountToIncrease;
        //звезды
        for (int i = 0; i < board.ScoreGoals.Length; i++) 
        {
            if (score > board.ScoreGoals[i] && numberStars < i + 1) // if the number of points is more than the goal of the points and the number of stars does not exceed the maximum number of stars then
            {
                if (goalManager.WinState == true) 
                {
                    numberStars++; // adding star
                }
            }
        }

        //сохранение очков
        if (gameData != null) 
        {
            int highScore = gameData.saveData.highScores[board.level];
            if (score > highScore) 
            {
                gameData.saveData.highScores[board.level] = score;
                //gameData.saveData.stars[board.level] = numberStars;
            }

            int currentStars = gameData.saveData.stars[board.level];
            if (numberStars > currentStars) 
            {
                gameData.saveData.stars[board.level] = numberStars;
                gameData.Save();
            }
            gameData.Save();   
        }

        if (board != null && scoreBar != null) 
        {
            int length = board.ScoreGoals.Length;
            scoreBar.fillAmount = (float)score / (float)board.ScoreGoals[length-1];
        }
    }
}

