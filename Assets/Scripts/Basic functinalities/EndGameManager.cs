using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public enum GameType
{
    Moves,
    Time
}
[System.Serializable]
public class EndGameRequierments 
{
    public GameType gameType;
    public int counterValues;
}

public class EndGameManager : MonoBehaviour
{
    public EndGameRequierments requierments;
    public GameObject movesLabel, timeLabel;
    public GameObject WinPanel, TryAgainPanel;
    public Text counter;
    public int currentCounterValue;
    private float timerSeconds;
    private Board board;
    void Start()
    {
        board = FindObjectOfType<Board>();
        SetGameType();
        SetupGame();
    }

    void SetGameType() 
    {
        if(board.world != null) 
        {
            if (board.level < board.world.levels.Length) 
            {
                if (board.world.levels[board.level] != null)
                {
                    requierments = board.world.levels[board.level].endGameRequierments;
                }
            }
        }
        
    }

    void SetupGame() 
    {
        currentCounterValue = requierments.counterValues;
        if (requierments.gameType == GameType.Moves)
        {
            movesLabel.SetActive(true);
            timeLabel.SetActive(false);
        }
        else 
        {
            timerSeconds = 1;
            movesLabel.SetActive(false);
            timeLabel.SetActive(true);
        }
        counter.text = "" + currentCounterValue;
    }

    public void DecreaseCounterValue() 
    {
        if (board.currentState != GameState.pause) 
        {
            currentCounterValue--;
            counter.text = "" + currentCounterValue;
            if (currentCounterValue <= 0)
            {
                LoseGame();
            }
        }

    }

    public void WinGame() 
    {
       WinPanel.SetActive(true);
        board.currentState = GameState.wait;
        currentCounterValue = 0;
        counter.text = "" + currentCounterValue;
        fadePanelController fade = FindObjectOfType<fadePanelController>();
        fade.GameOver();
    }

    public void LoseGame() 
    {
        TryAgainPanel.SetActive(true);
        board.currentState = GameState.lose;
        Debug.Log("you lose");
        currentCounterValue = 0;
        counter.text = "" + currentCounterValue;
        fadePanelController fade = FindObjectOfType<fadePanelController>();
        fade.GameOver();
    }

    // Update is called once per frame
    void Update()
    {
        if (requierments.gameType == GameType.Time && currentCounterValue > 0)
        {
            timerSeconds -= Time.deltaTime;
            if (timerSeconds <= 0) 
            {
                DecreaseCounterValue();
                timerSeconds = 1;
            }
        }   
    }
}
