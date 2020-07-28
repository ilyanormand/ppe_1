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
    void Start()
    {
        board = FindObjectOfType<Board>();
        gameData = FindObjectOfType<GameData>();
    }

    // Update is called once per frame
    void Update()
    {
        scoreText.text = score.ToString();
    }

    public void InreaseScore(int amountToIncrease) 
    {
        score += amountToIncrease;

        if (gameData != null) 
        {
            int highScore = gameData.saveData.highScores[board.level];
            if (score > highScore) 
            {
                gameData.saveData.highScores[board.level] = score;
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
