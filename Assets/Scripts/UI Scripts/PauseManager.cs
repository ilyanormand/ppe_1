using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PauseManager : MonoBehaviour
{
    public GameObject pausePanel;
    private Board board;
    public bool paused;
    public Image soundButton;
    public Sprite musicOnSprite;
    public Sprite musicOffSprite;
    private  SoundManager sound;
    void Start()
    {
        paused = false;
        // in player prefs, the "sound" key is for sound
        // if sound == 0, then mute, if sound == 1, then unmute
        sound = FindObjectOfType<SoundManager>();
        board = GameObject.FindWithTag("Board").GetComponent<Board>();
        pausePanel.SetActive(false);
        if (PlayerPrefs.HasKey("Sound")) 
        {
            if (PlayerPrefs.GetInt("Sound") == 0) 
            {
                soundButton.sprite = musicOffSprite; 
            }
            else
            {
                soundButton.sprite = musicOnSprite; 
            }
        }
        else 
        {
            soundButton.sprite = musicOnSprite; 
        }
        
    }

    // Update is called once per frame
    void Update()
    {
        if (!pausePanel.activeInHierarchy && paused) 
        {
            pausePanel.SetActive(true); 
            board.currentState = GameState.pause; 
        }
        if (pausePanel.activeInHierarchy && !paused)
        {
            pausePanel.SetActive(false);
            board.currentState = GameState.move; 
        }
    }

    public void SoundButton() 
    {
        if (PlayerPrefs.HasKey("Sound")) 
        {
            if (PlayerPrefs.GetInt("Sound") == 0) 
            {
                PlayerPrefs.SetInt("Sound", 1);
                soundButton.sprite = musicOnSprite; 
                sound.adjustVolume(); 
            }
            else
            {
                PlayerPrefs.SetInt("Sound", 0); 
                soundButton.sprite = musicOffSprite;
                sound.adjustVolume();
            }
        }
        else
        {
            soundButton.sprite = musicOnSprite; 
            PlayerPrefs.SetInt("Sound", 0);
            sound.adjustVolume();

        }
    }

    public void PauseGame() 
    {
        paused = !paused;
        Debug.Log("Set to pause");
    }

    public void ExitGame() 
    {
        SceneManager.LoadScene("Splash");
    }
}
