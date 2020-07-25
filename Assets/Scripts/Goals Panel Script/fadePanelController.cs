using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class fadePanelController : MonoBehaviour
{

    public Animator panelAnim;
    public Animator gameInfoAnim;

    public void ConfirmationButton()
    {
        if (panelAnim != null && gameInfoAnim != null) 
        {
            panelAnim.SetBool("fadeOut", true);
            gameInfoAnim.SetBool("Out", true);
            StartCoroutine(gameStartCo());
        }
        
    }

    public void GameOver() 
    {
        panelAnim.SetBool("fadeOut", false);
        panelAnim.SetBool("GameOver", true);
    }

    IEnumerator gameStartCo() 
    {
        yield return new WaitForSeconds(1f);
        Board board = FindObjectOfType<Board>();
        board.currentState = GameState.move;
    }
}
