using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class WinPanel : MonoBehaviour
{
    [Header("Level Information")]
    public int level; // здесь хранится наш уровень
    private GameData gameData; // обьект GameData в которм хранится вся информация об уровне
    private int starsActive; // здесь будут хранится все звезды которые нужно активровать
    private int highScore; // здесь будет хранится самый высокий счет
    public GameObject fadePanel; // обьект fadePanel который будет затемнятся при активации winPanel
    public GameObject backgroundWinPanel; // обькет который будет затеменять экран при активации win panel\
    public ScoreManager scoreManager; // создаем обьект с классом ScoreManager
    private Board board;

    [Header("UI stuff")]
    public GameObject[] stars; // здесь хранится все картинки звезд
    public Text highScoreText;
    void OnEnable()
    {
        scoreManager = FindObjectOfType<ScoreManager>();
        board = FindObjectOfType<Board>();
        Debug.Log("board = " + board);
        if (backgroundWinPanel != null) // проверка backgroundWinPanel на существование
        {
            //Debug.Log("backgroundWinPanel != null");
            backgroundWinPanel.SetActive(true); // активируем затемнение экрана
        }
        else
        {
            //Debug.Log("backgroundWinPanel == null");
        }
        

        // нахождение нужных элементов в массиве в ScoreGoals

        /*Debug.Log("board.ScoreGoals = " + board.ScoreGoals);
        foreach (int i in board.ScoreGoals) 
        {
            Debug.Log("board.scoreGoals i = " + i);
        }*/
    }

    private void Start()
    {
        SetText();
        ActivateStars();
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
                Debug.Log("Not enough of score to have " + i + " star");
                break;
            }
        }
    }

    private void SetText() 
    {
        highScoreText.text = "" + scoreManager.score;
    }
}
