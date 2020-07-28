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
        //звезды
        for (int i = 0; i < board.ScoreGoals.Length; i++) 
        {
            if (score > board.ScoreGoals[i] && numberStars < i + 1) // если количество очков больше чем цель очков и при этом количество звезд не превышает макс число звезд то
            {
                numberStars++; // добавляем звезду
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

