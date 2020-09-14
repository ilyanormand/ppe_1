using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
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
    private FindMatches findMatches;
    private GoalManager goalManager;
    void Start()
    {
        board = FindObjectOfType<Board>();
        goalManager = FindObjectOfType<GoalManager>();
        findMatches = FindObjectOfType<FindMatches>();
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
        }

    }


    /*public void WinGame() 
    {
        WinPanel.SetActive(true);
        board.currentState = GameState.wait;
        currentCounterValue = 0;
        counter.text = "" + currentCounterValue;
        fadePanelController fade = FindObjectOfType<fadePanelController>();
        fade.GameOver();
    }*/

    public IEnumerator LoseGame() 
    {
        
        Debug.Log("current matches = " + findMatches.currentMatches.Count);
        yield return new WaitForSeconds(1f);
        if (findMatches.currentMatches.Count == 0 && board.currentState == GameState.move) 
        {
            yield return new WaitForSeconds(1f);
            if (goalManager.WinState != true) 
            {
                board.currentState = GameState.lose;
                Debug.Log("Activate loose panel");
                TryAgainPanel.SetActive(true);
                //Debug.Log("you lose");
                currentCounterValue = 0;
                counter.text = "" + currentCounterValue;
                fadePanelController fade = FindObjectOfType<fadePanelController>();
                fade.GameOver();
            }
            
        }
        
    }
    
    public void LevelRestart() 
    {
        SceneManager.LoadScene(board.level);
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
        if (currentCounterValue <= 0)
        {
            Debug.Log("Loose game");
            StartCoroutine(LoseGame());
        }
        if (goalManager.WinState == true) 
        {
            StartCoroutine(CheckForWin());
        }
    }

    private IEnumerator CheckForWin() 
    {
        yield return new WaitForSeconds(1f);
        if (findMatches.currentMatches.Count == 0 && board.currentState == GameState.move) 
        {
            yield return new WaitForSeconds(1f);
            board.currentState = GameState.wait;
            WinPanel.SetActive(true);
            currentCounterValue = 0;
            counter.text = "" + currentCounterValue;
            fadePanelController fade = FindObjectOfType<fadePanelController>();
            fade.GameOver();
        }
        
    }
    /*private IEnumerator checkForMatchesCo() 
    {
        yield
        if (findMatches.currentMatches.Count == 0 && currentCounterValue <= 0)
        {
            HaveMatchOnBoard = false;
            Debug.Log("HaveMatchOnBoard = " + HaveMatchOnBoard);
        }
        else
        {
            HaveMatchOnBoard = true;
            Debug.Log("HaveMatchOnBoard = " + HaveMatchOnBoard);
        }

        
        
    }*/
}
