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
                        if (i < board.width - 2)
                        {
                            if (board.SwitchAndCheck(i, j, Vector2.right))
                            {
                                //board.MatchList.Add(board.allDots[i, j]);
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
                                    //animationFinger(i, j, true, finger);
                                }
                                
                                /*else
                                {
                                    Vector2 MovePosition = new Vector2(i + 1, j);
                                    Vector2 startPosition = new Vector2(i, j);
                                    MoveProgress = Mathf.PingPong(MoveSpeed * Time.time, 1);
                                    Vector2 offset = MovePosition * MoveProgress;
                                    finger.transform.position = startPosition + MovePosition;
                                }*/

                                foreach (GameObject k in board.MatchList)
                                {
                                    possibleMoves.Add(k);
                                }
                                board.MatchList.Clear();
                                //Debug.Log(possibleMoves.Count);
                                return possibleMoves;
                            }
                        }
                        if (i > 1)
                        {
                            if (board.SwitchAndCheck(i, j, Vector2.left))
                            {
                                //board.MatchList.Add(board.allDots[i, j]);
                                Vector2 tempPosition = new Vector2(i, j);
                                if (finger == null)
                                {
                                    finger = Instantiate(tutorial.FingerPrefab, tempPosition, Quaternion.identity);
                                    finger.transform.parent = board.allDots[i, j].transform;
                                    finger.name = "FingerTutorial";
                                    finger.GetComponent<Animator>().SetBool("left", true);
                                }
                                else
                                {
                                    //animationFinger(i, j, true, finger);
                                }
                                
                                /*else
                                {
                                    Vector2 MovePosition = new Vector2(i + 1, j);
                                    Vector2 startPosition = new Vector2(i, j);
                                    MoveProgress = Mathf.PingPong(MoveSpeed * Time.time, 1);
                                    Vector2 offset = MovePosition * MoveProgress;
                                    finger.transform.position = startPosition + MovePosition;
                                }*/

                                foreach (GameObject k in board.MatchList)
                                {
                                    possibleMoves.Add(k);
                                }
                                board.MatchList.Clear();
                                Debug.Log(possibleMoves.Count);
                                return possibleMoves;
                            }
                        }

                        if (j < board.height - 2)
                        {
                            if (board.SwitchAndCheck(i, j, Vector2.up))
                            {
                                //board.MatchList.Add(board.allDots[i, j]);
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
                                    //animationFinger(i, j, false, finger);
                                }
                                
                                /*else
                                {
                                    float step = speedOffinger * Time.deltaTime;
                                    Vector2 targetPosition = new Vector2(i, j + 1);
                                    Vector2 StartPosition = new Vector2(i, j);
                                    finger.transform.position = Vector2.MoveTowards(finger.transform.position, targetPosition, step);
                                    finger.transform.position = Vector2.MoveTowards(finger.transform.position, StartPosition, step);

                                }*/
                                foreach (GameObject k in board.MatchList)
                                {
                                    possibleMoves.Add(k);
                                    //Debug.Log("GameObject in board.MacthList = " +  k);
                                }
                                board.MatchList.Clear();
                                //Debug.Log("possibleMoves.count = "+ possibleMoves.Count);
                                return possibleMoves;
                            }
                        }
                        if (j > 1)
                        {
                            if (board.SwitchAndCheck(i, j, Vector2.down))
                            {
                                //board.MatchList.Add(board.allDots[i, j]);
                                Vector2 tempPosition = new Vector2(i, j);
                                if (finger == null)
                                {
                                    finger = Instantiate(tutorial.FingerPrefab, tempPosition, Quaternion.identity);
                                    finger.transform.parent = board.allDots[i, j].transform;
                                    finger.name = "FingerTutorial";
                                    finger.GetComponent<Animator>().SetBool("down", true);
                                }
                                else
                                {
                                    //animationFinger(i, j, false, finger);
                                }

                                /*else
                                {
                                    float step = speedOffinger * Time.deltaTime;
                                    Vector2 targetPosition = new Vector2(i, j + 1);
                                    Vector2 StartPosition = new Vector2(i, j);
                                    finger.transform.position = Vector2.MoveTowards(finger.transform.position, targetPosition, step);
                                    finger.transform.position = Vector2.MoveTowards(finger.transform.position, StartPosition, step);

                                }*/
                                foreach (GameObject k in board.MatchList)
                                {
                                    possibleMoves.Add(k);
                                }
                                board.MatchList.Clear();
                                //Debug.Log(possibleMoves.Count);
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

    private IEnumerator waitSome() 
    {
        //Debug.Log("wait for second");
        yield return new WaitForSeconds(1);
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
                //Debug.Log("possibleMoves is empty");
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
                //Debug.Log("move != null");
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
            //Debug.Log("Move == null");
        }
    }

    public void DestroyHint()
    {
        hintDelaySeconds = hintDelay;
        hint = true;
    }

    public void animationFinger(int column, int row, bool right, GameObject finger)  
    {
        int i = 0;
        bool finishAnimation = false;
        bool loopActive = true;
        Vector2 StartPosition = new Vector2(column, row);
        float step = speedOffinger * Time.deltaTime;
        if (right)
        {
            while (loopActive)
            {
                i++;
                Debug.Log("loop iteration " + i);
                Debug.Log("move finger right");
                Debug.Log("step = " + step);
                Vector2 targetPosition = new Vector2(column + 1, row);
                Debug.Log("target position: " + targetPosition);
                Debug.Log("start position: " + StartPosition);
                finger.transform.position = Vector2.MoveTowards(finger.transform.position, targetPosition, step);
                Debug.Log("move target position: " + finger.transform.position);
                
                if ((Vector2)finger.transform.position == targetPosition)
                {
                    finishAnimation = true;
                    Debug.Log("finishAnimation = " + finishAnimation);
                }
                if (finishAnimation)
                {
                    StartCoroutine("waitSome");
                    finger.transform.position = Vector2.MoveTowards(finger.transform.position, StartPosition, step);
                    Debug.Log("move start position: " + finger.transform.position);
                    if ((Vector2)finger.transform.position == StartPosition)
                    {
                        loopActive = false;
                        Debug.Log("LoopActive = " + loopActive);
                        i = 0;
                    }
                }
            }
        }
        else 
        {
            while (loopActive)
            {
                StartCoroutine("waitSome");
                i++;
                Debug.Log("loop iteration " + i);
                Debug.Log("move finger up");
                Debug.Log("step = " + step);
                Vector2 targetPosition = new Vector2(column, row + 1);
                Debug.Log("target position: " + targetPosition);
                Debug.Log("start position: " + StartPosition);
                finger.transform.position = Vector2.MoveTowards(finger.transform.position, targetPosition, step);
                StartCoroutine("waitSome");
                Debug.Log("move target position: " + finger.transform.position);
                
                if ((Vector2)finger.transform.position == targetPosition)
                {
                    finishAnimation = true;
                    Debug.Log("finishAnimation = " + finishAnimation);
                }
                if (finishAnimation)
                {
                    StartCoroutine("waitSome");
                    finger.transform.position = Vector2.MoveTowards(finger.transform.position, StartPosition, step);
                    Debug.Log("move start position: " + finger.transform.position);
                    if ((Vector2)finger.transform.position == StartPosition)
                    {
                        loopActive = false;
                        Debug.Log("LoopActive = " + loopActive);
                        i = 0;
                    }
                }
            }

        }
        

    }

    // создать подсказку
    // убрать подсказку
}
