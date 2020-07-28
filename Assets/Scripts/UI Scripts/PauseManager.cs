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
    void Start()
    {
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

    public void PauseGame() 
    {
        paused = !paused;
    }

    public void ExitGame() 
    {
        SceneManager.LoadScene("Splash");
    }
}
