using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
public class LevelButton : MonoBehaviour
{
    [Header("Active Stuff")]
    public bool isActive;
    public Sprite activeSprite;
    public Sprite lockedSprite;
    private Image buttonImage;
    private Button myButton;
    private int starsActive;

    [Header("Level ui")]
    public Text levelText;
    public Image[] stars;
    public int level;
    public GameObject confirmPanel;
    

    private GameData gameData;

    private void Start()
    {
        gameData = FindObjectOfType<GameData>();
        buttonImage = GetComponent<Image>();
        myButton = GetComponent<Button>();
        loadData();
        ActivateStars();
        ShowLevel();
        DecideSprite();
        

    }

    void loadData() 
    {
        if (gameData != null)
        {
            if (gameData.saveData.isActive[level-1])
            {
                isActive = true;
            }
            else 
            {
                isActive = false;
            }
            starsActive = gameData.saveData.stars[level-1];
        }
    }

    void ActivateStars() 
    {
        for (int i = 0; i < starsActive; i++) 
        {
            stars[i].enabled = true;
        }
    }

    void DecideSprite() 
    {
        if (isActive)
        {
            buttonImage.sprite = activeSprite;
            myButton.enabled = true;
            levelText.enabled = true;
        }
        else 
        {
            buttonImage.sprite = lockedSprite;
            myButton.enabled = false;
            levelText.enabled = false;
        }
    }

    void ShowLevel() 
    {
        levelText.text = "" + level;
    }

    public void ConfirmPanel(int level) 
    {
        PlayerPrefs.SetInt("Current Level", level);
        SceneManager.LoadScene("Main");
    }
}
