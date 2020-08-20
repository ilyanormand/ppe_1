using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class WinPanel : MonoBehaviour
{
    [Header("Level Information")]
    public string levelToLoad;
    public int level;
    private GameData gameData;
    private int starsActive;
    private int highScore;


    [Header("UI stuff")]
    public Image[] stars;
    public Text highScoreText;

    void OnEnable()
    {
        gameData = FindObjectOfType<GameData>(); // находим обьект GameData в котором хранятся все данные
        LoadData(); // загружаем данные
        ActivateStars(); // Активируем звезды
        SetText(); // Set ScoreText

    }


    // метод который загружает данные из обьекта GameData
    void LoadData()
    {
        if (gameData != null)
        { 
            starsActive = gameData.saveData.stars[level];
            highScore = gameData.saveData.highScores[level];
        }
    }

    void SetText()
    {
        highScoreText.text = "" + highScore;
    }

    void ActivateStars()
    {
        for (int i = 0; i < starsActive; i++)
        {
            //Debug.Log(stars[i]);
            stars[i].enabled = true;
        }
    }

    void Update()
    {

    }
}
