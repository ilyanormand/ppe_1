using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
public class ConfirmPanel : MonoBehaviour
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
    public Text starText;
    
    
    
    void OnEnable()
    {
        gameData = FindObjectOfType<GameData>();
        LoadData();
        ActivateStars();
        SetText();
    }

    void LoadData() 
    {
        if(gameData != null)
        {
            starsActive = gameData.saveData.stars[level];
            highScore = gameData.saveData.highScores[level];
        }
    }

    void SetText() 
    {
        highScoreText.text = "" + highScore;
        starText.text = "" + starsActive + "/3";  
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

    public void Cancel() 
    {
        this.gameObject.SetActive(false);
    }

    public void PlayConfirmPanel() 
    {
        PlayerPrefs.SetInt("Current Level", level);
        SceneManager.LoadScene(levelToLoad);
    }
}
