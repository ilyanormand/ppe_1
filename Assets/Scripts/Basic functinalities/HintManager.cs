using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HintManager : MonoBehaviour
{
    private Board board;
    public float hintDelay;
    private float hintDelaySeconds;
    public GameObject hintParticle;
    public GameObject currentHint;
    public float animationSpeed;
    public bool hint = false;
    public List<GameObject> tutorialElements;
    private Tutorial tutorial;
    public GameObject finger;
    public float speedOffinger;
    Vector2 scale;
    void Start()
    {
        board = FindObjectOfType<Board>();
        tutorialElements = new List<GameObject>();
        tutorial = FindObjectOfType<Tutorial>();
        hintDelaySeconds = hintDelay;
        hint = false;
    }


    void Update()
    {
        hintDelaySeconds -= Time.deltaTime;
        if (hintDelaySeconds <= 0 && currentHint == null)
        {
            hint = false;
            MarkHint();
            hintDelaySeconds = hintDelay;
        }
        else if (hint == true)
        {
            MarkHint();
        }
    }

    private void OnMouseDown()
    {
        Debug.Log(hint);
        DestroyHint();
    }


    //Нужно найти все всевозможные матчи в таблицы
    List<GameObject> FindAllPossibileMatches()
    {
        if (board.currentState == GameState.move)
        {
            List<GameObject> possibleMoves = new List<GameObject>();
            for (int i = 0; i < board.width; i++)
            {
                for (int j = 0; j < board.height; j++)
                {
                    if (board.allDots[i, j] != null)
                    {
                        if (i < board.width - 1)
                        {
                            if (board.SwitchAndCheck(i, j, Vector2.right))
                            {
                                board.MatchList.Add(board.allDots[i, j]);
                                Vector2 tempPosition = new Vector2(i, j);
                                if (finger == null)
                                {
                                    finger = Instantiate(tutorial.FingerPrefab, tempPosition, Quaternion.identity);
                                    finger.transform.parent = board.allDots[i, j].transform;
                                    finger.name = "FingerTutorial";
                                    finger.GetComponent<Animator>().SetBool("right", true);
                                }
                                else
                                {
                                    float step = speedOffinger * Time.deltaTime;
                                    Vector2 targetPosition = new Vector2(i + 1, j);
                                    finger.transform.position = Vector2.Lerp(finger.transform.position, targetPosition, step);
                                    //finger.transform.position = new Vector2(i, j);

                                }

                                foreach (GameObject k in board.MatchList)
                                {
                                    possibleMoves.Add(k);
                                }
                                board.MatchList.Clear();
                                Debug.Log(possibleMoves.Count);
                                return possibleMoves;
                            }
                        }

                        if (j < board.height - 1)
                        {
                            if (board.SwitchAndCheck(i, j, Vector2.up))
                            {
                                board.MatchList.Add(board.allDots[i, j]);
                                Vector2 tempPosition = new Vector2(i, j);
                                if (finger == null)
                                {
                                    finger = Instantiate(tutorial.FingerPrefab, tempPosition, Quaternion.identity);
                                    finger.transform.parent = board.allDots[i, j].transform;
                                    finger.name = "FingerTutorial";
                                    finger.GetComponent<Animator>().SetBool("up", true);
                                }
                                else
                                {
                                    float step = speedOffinger * Time.deltaTime;
                                    Vector2 targetPosition = new Vector2(i, j + 1);
                                    finger.transform.position = Vector2.MoveTowards(finger.transform.position, targetPosition, step);
                                    //finger.transform.position = new Vector2(i, j);

                                }
                                foreach (GameObject k in board.MatchList)
                                {
                                    possibleMoves.Add(k);
                                }
                                board.MatchList.Clear();
                                Debug.Log(possibleMoves.Count);
                                return possibleMoves;
                            }
                        }
                    }
                }
            }
            return possibleMoves;
        }
        else
        {
            hintDelaySeconds = hintDelay;
            return null;
        }

    }

    //выбрать случайным образом любой матч из всевозможных
    public List<GameObject> pickOneRandomly() 
    {
        List<GameObject> possibleMoves = new List<GameObject>();
        possibleMoves = FindAllPossibileMatches();
        if (possibleMoves != null) 
        {
            if (possibleMoves.Count == 3)
            {
                return possibleMoves;
            }
            else 
            {
                Debug.Log("possibleMoves is empty");
            }
        }
        return null;
    }

    private void MarkHint() 
    {
        List<GameObject> move = pickOneRandomly();
        tutorialElements = pickOneRandomly();
        if (move != null)
        {
            if (hint == true)
            {
                foreach (GameObject i in move)
                {
                    Animator anim = i.GetComponent<Animator>();
                    anim.enabled = false;
                }

                
            }
            else 
            {
                Debug.Log("move != null");
                foreach (GameObject i in move)
                {
                    Animator anim = i.GetComponent<Animator>();
                    anim.enabled = true;
                }
                move.Clear();
            }

            
        }
        else 
        {
            Debug.Log("Move == null");
        }
    }

    public void DestroyHint()
    {
        hintDelaySeconds = hintDelay;
        hint = true;
    }

    // создать подсказку
    // убрать подсказку
}
