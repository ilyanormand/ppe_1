using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PauseManager : MonoBehaviour
{
    public GameObject pausePanel;
    private Board board;
    public bool paused = false;
    public Image soundButton;
    public Sprite musicOnSprite;
    public Sprite musicOffSprite;
    void Start()
    {
        // in player prefs, the "sound" key is for sound
        // if sound == 0, then mute, if sound == 1, then unmute
        if (PlayerPrefs.HasKey("Sound")) // проверяет есть ли ключ sound в найстройках у игрока
        {
            if (PlayerPrefs.GetInt("Sound") == 0) // если значение ключа в настройках игрока равно 0 то
            {
                soundButton.sprite = musicOffSprite; // меняем картинку включенного звука на выключеный звук
            }
            else
            {
                soundButton.sprite = musicOnSprite; // меняем картинку выключеного звука на включеный звук
            }
        }
        else 
        {
            soundButton.sprite = musicOnSprite; // если же не нашел ключ sound то оставляем включенным звук
        }
        pausePanel.SetActive(false);
        board = GameObject.FindWithTag("Board").GetComponent<Board>();
    }

    // Update is called once per frame
    void Update()
    {
        if (paused && !pausePanel.activeInHierarchy) // если paused = true и панель паузы не активна то 
        {
            pausePanel.SetActive(true); // показать панель паузы
            board.currentState = GameState.pause; // переключить режим игра на режим паузы
        }
        if (!paused && pausePanel.activeInHierarchy) // если игра не на паузе и панель паузы активна то
        {
            pausePanel.SetActive(false); // выключить панель паузы
            board.currentState = GameState.move; // переключить режим игры на игровой режим
        }
    }

    public void SoundButton() 
    {
        if (PlayerPrefs.HasKey("Sound")) // проверяет есть ли ключ sound в найстройках у игрока
        {
            if (PlayerPrefs.GetInt("Sound") == 0) // если значение ключа в настройках игрока равно 0 то
            {
                soundButton.sprite = musicOnSprite; // меняем картинку выключеного звука на включеный звук
                PlayerPrefs.SetInt("Sound", 1); // добавляем к нашуме ключу sound значение 1, что означает что звук включен
            }
            else
            {
                soundButton.sprite = musicOffSprite; // меняем картинку включеного звука на выключеный звук
                PlayerPrefs.SetInt("Sound", 0); // добавляем к нашуме ключу sound значение 0, что означает что звук выключен
            }
        }
        else
        {
            soundButton.sprite = musicOnSprite; // если же не нашел ключ sound то оставляем звук выключеным
            PlayerPrefs.SetInt("Sound", 0);
        }
    }

    public void PauseGame() 
    {
        paused = !paused;
    }

    public void ExitGame() 
    {
        SceneManager.LoadScene("Splash");
    }
}
