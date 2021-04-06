using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class WinPanel : MonoBehaviour
{
    [Header("Level Information")]
    public int level; 
    private GameData gameData; 
    private int starsActive; 
    private int highScore; 
    public GameObject fadePanel; 
    public GameObject backgroundWinPanel;
    public ScoreManager scoreManager;
    private SoundManager soundManager;
    private Board board;

    [Header("UI stuff")]
    public GameObject[] stars;
    public Text highScoreText;
    void OnEnable()
    {
        board = FindObjectOfType<Board>();
        board.currentState = GameState.win;
        Debug.Log("board.currentState = " + board.currentState);
        scoreManager = FindObjectOfType<ScoreManager>();
        board = FindObjectOfType<Board>();
        //Debug.Log("board = " + board);
        if (backgroundWinPanel != null) 
        {
            //Debug.Log("backgroundWinPanel != null");
            backgroundWinPanel.SetActive(true); 
        }
        else
        {
            //Debug.Log("backgroundWinPanel == null");
        }
        

  

        /*Debug.Log("board.ScoreGoals = " + board.ScoreGoals);
        foreach (int i in board.ScoreGoals) 
        {
            Debug.Log("board.scoreGoals i = " + i);
        }*/
    }

    private void Start()
    {
        soundManager = FindObjectOfType<SoundManager>();
        SetText();
        //ActivateStars();
        StartCoroutine(ActivateStarsCo());
    }

    private IEnumerator ActivateStarsCo() 
    {
        for (int i = 0; i < board.ScoreGoals.Length; i++)
        {
            if (scoreManager.score >= board.ScoreGoals[i])
            {
                yield return new WaitForSeconds(0.5f);
                stars[i].SetActive(true);
                soundManager.PlayStarsAppear(i);
                yield return new WaitForSeconds(1f);
                //Debug.Log("stars["+i+"].enabled = " + stars[i].enabled);
            }
            else
            {
                //Debug.Log("Not enough of score to have " + i + " star");
                break;
            }
        }
    }
    private void ActivateStars() 
    {
        for (int i = 0; i < board.ScoreGoals.Length; i++) 
        {
            if (scoreManager.score >= board.ScoreGoals[i])
            {
                stars[i].SetActive(true);
                //Debug.Log("stars["+i+"].enabled = " + stars[i].enabled);
            }
            else 
            {
                //Debug.Log("Not enough of score to have " + i + " star");
                break;
            }
        }
    }

    private void SetText() 
    {
        highScoreText.text = "" + scoreManager.score;
    }


}
