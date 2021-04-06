using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dot : MonoBehaviour
{
    // Start is called before the first frame update
    [Header ("Переменные для генерации таблицы")]
    public int column;
    public int row;
    public int previousColumn;
    public int previousRow;
    public int targetX; 
    public int targetY; 
    public float swipeAngle = 0; 

    [Header("Базовая логика Матч 3")]
    private EndGameManager endGameManager;
    private HintManager hintManager;
    private FindMatches findMatches;
    private Board board;
    private Vector2 firstTouchPosition, finalTouchPosition, tempPosition;
    public Tutorial tutorial;

    
    public bool isMatched = false;
    public GameObject otherDot;

    [Header("Бустеры")]
    public bool isColumnBomb, isRowBomb, isColorBomb, isAdjacentBomb;
    //public GameObject AdjacentMarker, rowArrow, columnArrow, colorBomb;
    public GameObject[] listOfBoosters;
    private ColorBombEffect colorBombEffect;

    [Header("Для тестов")]
    public GameObject fruit;
    public bool notTutorial;
    void Start()
    {
        colorBombEffect = FindObjectOfType<ColorBombEffect>();
        tutorial = FindObjectOfType<Tutorial>();
        endGameManager = FindObjectOfType<EndGameManager>();
        isColumnBomb = false;
        isRowBomb = false;
        isColorBomb = false;
        isAdjacentBomb = false;

        hintManager = FindObjectOfType<HintManager>();
        board = GameObject.FindWithTag("Board").GetComponent<Board>();

        findMatches = FindObjectOfType<FindMatches>();
    }


    private void OnMouseOver()
    {
        /*if (Input.GetMouseButtonDown(1))
        {
            isAdjacentBomb = true;
            GameObject prefabBooster = SearchNameForBooster("adjacent"); // найденый обьект с помощью тега
            GameObject marker = Instantiate(prefabBooster, transform.position, Quaternion.identity); // генерация обьекта
            marker.transform.parent = this.transform;
        }*/
        if (Input.GetMouseButtonDown(1))
        {
            isColorBomb = true;
            GameObject prefabColorBomb = SearchNameForBooster("ColorBomb");
            GameObject color = Instantiate(prefabColorBomb, transform.position, Quaternion.identity);
            color.transform.parent = this.transform;
        }
    }

    // Update is called once per frame
    private IEnumerator DestroyFirstMatches() 
    {
        if (board.currentState == GameState.move)
        {
            if (board != null)
            {
                yield return new WaitForSeconds(1.5f);
                if (board.currentState != GameState.win) 
                {
                    board.DestroyMatches();
                }
                
            }
        }
    }

    void Update()
    {
        if (isMatched) 
        {
            SpriteRenderer mySprite = GetComponent<SpriteRenderer>();
            StartCoroutine(DestroyFirstMatches());
        }
        targetX = column;
        targetY = row;
        if (Mathf.Abs(targetX - transform.position.x) > .1) // si la position X > 0.1 alors le swipe est vers la droite ou gauche
        {
            tempPosition = new Vector2(targetX, transform.position.y); // creation d'un nouveau vector
            transform.position = Vector2.Lerp(transform.position, tempPosition, 10f * Time.deltaTime); 
            if (board.allDots[column, row] != this.gameObject)
            {
                board.allDots[column, row] = this.gameObject;
            }
            findMatches.FindAllMatches();
        }
        else
        {
            tempPosition = new Vector2(targetX, transform.position.y); // сcreation d'un nouveau vector
            transform.position = tempPosition; 
            board.allDots[column, row] = this.gameObject; 
        }
        if (Mathf.Abs(targetY - transform.position.y) > .1) //  si la position Y > 0.1 alors le swipe est vers le haut ou bas
        {
            tempPosition = new Vector2(transform.position.x, targetY); 
            transform.position = Vector2.Lerp(transform.position, tempPosition, 10f * Time.deltaTime);
            if (board.allDots[column, row] != this.gameObject)
            {
                board.allDots[column, row] = this.gameObject;
            }
            findMatches.FindAllMatches();
        }
        else
        {
            tempPosition = new Vector2(transform.position.x, targetY);
            transform.position = tempPosition;
            board.allDots[column, row] = this.gameObject;
        }
    }

    private void OnMouseDown()
    {
        // Hint Destroy
        if (hintManager != null) 
        {
            //Debug.Log(hintManager.hint);
            hintManager.DestroyHint();
        }
        
        if (board.currentState == GameState.move) 
        {
            firstTouchPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        }
        

        //Debug.Log(firstTouchPosition);
    }

    private void OnMouseUp()
    {
        if (board.currentState == GameState.move) 
        {
            finalTouchPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition); 
            CalculateAngle();
        }
        
    }

    void CalculateAngle() 
    {
        board.currentState = GameState.wait;
        swipeAngle = Mathf.Atan2(finalTouchPosition.y - firstTouchPosition.y, finalTouchPosition.x - firstTouchPosition.x) * 180 / Mathf.PI;
        movePieces();
        board.currentDot = this;
        if (swipeAngle == 0) 
        {
            board.currentState = GameState.move;
        }
    }

    void movePiecesActual(Vector2 direction) 
    {
        otherDot = board.allDots[column + (int)direction.x, row + (int)direction.y];// moving selected items
        previousColumn = column;
        previousRow = row;
        if (board.lockTiles[column, row] == null && board.lockTiles[column + (int)direction.x, row + (int)direction.y] == null) 
        {
            if (otherDot != null)
            {
                otherDot.GetComponent<Dot>().column += -1 * (int)direction.x;
                otherDot.GetComponent<Dot>().row += -1 * (int)direction.y;
                column += (int)direction.x;
                row += (int)direction.y;
                tutorial.tutorialShow = false;
                StartCoroutine(CheckMoveCo());// checking to return the item to his original position 
            }
            else
            {
                board.currentState = GameState.move;
            }
        }
        else
        {
            board.currentState = GameState.move;
        }

    }

    // Items moving
    void movePieces()
    {
        if (swipeAngle > -45 && swipeAngle <= 45 && column < board.width - 1 && swipeAngle != 0)
        {
            //Right swipe
            movePiecesActual(Vector2.right); 
        }
        else if (swipeAngle > 45 && swipeAngle <= 135 && row < board.height - 1 && swipeAngle != 0)
        {
            //Top swipe 
            movePiecesActual(Vector2.up);
        }
        else if ((swipeAngle > 135 || swipeAngle <= -135) && column > 0 && swipeAngle != 0)
        {
            //Left swipe
            movePiecesActual(Vector2.left);
        }
        else if (swipeAngle < -45 && swipeAngle >= -135 && row > 0 && swipeAngle != 0)
        {
            //Bottom swipe
            movePiecesActual(Vector2.down);
        }
        
        
    }

    public IEnumerator CheckMoveCo() 
    {
        if (isColorBomb)
        {
            findMatches.MatchPiecesOfColors(otherDot.tag);
            //Debug.Log("ColorBombEffect = " + colorBombEffect);
            isMatched = true;
            colorBombEffect.StartEffect(findMatches.listOfColorMatches, otherDot.transform.position);
            yield return new WaitForSeconds(1f);
        } else if (otherDot.GetComponent<Dot>().isColorBomb) 
        {
            findMatches.MatchPiecesOfColors(this.gameObject.tag);
            //Debug.Log("ColorBombEffect = " + colorBombEffect);
            otherDot.GetComponent<Dot>().isMatched = true;
            colorBombEffect.StartEffect(findMatches.listOfColorMatches, otherDot.transform.position);
            yield return new WaitForSeconds(1f);

        }
        yield return new WaitForSeconds(.5f); // pause 
        if (otherDot != null)
        {
            if (!isMatched && !otherDot.GetComponent<Dot>().isMatched) // if ther is no match
            {
                otherDot.GetComponent<Dot>().row = row;
                otherDot.GetComponent<Dot>().column = column;
                row = previousRow;
                column = previousColumn;
                yield return new WaitForSeconds(.5f);
                board.currentDot = null;
                board.currentState = GameState.move;
            }
            else
            {
                if (endGameManager != null) 
                {
                    if (endGameManager.requierments.gameType == GameType.Moves) 
                    {
                        endGameManager.DecreaseCounterValue();
                    }
                }
                if (board.currentState != GameState.win) 
                {
                    board.DestroyMatches(); // Destroy match
                }
                
            }
            //otherDot = null;
        }

    }


    // FindingMatch
    void FindMatches() 
    {
        if (column > 0 && column < board.width - 1) 
        {
            GameObject leftDot1 = board.allDots[column - 1, row]; // left item
            GameObject rightDot1 = board.allDots[column + 1, row]; // right item
            if (leftDot1 != null && rightDot1 !=null) 
            {
                if (leftDot1.CompareTag(this.gameObject.tag) && rightDot1.CompareTag(this.gameObject.tag))
                // if item right and item left is equal
                {
                    leftDot1.GetComponent<Dot>().isMatched = true;
                    rightDot1.GetComponent<Dot>().isMatched = true;
                    isMatched = true;
                }
            }
            
        }
        if (row > 0 && row < board.height - 1)
        {
            GameObject upDot1 = board.allDots[column, row + 1]; //top item
            GameObject downDot1 = board.allDots[column, row -1]; //bottom item
            if (upDot1 != null && downDot1 != null) 
            {
                if (upDot1.CompareTag(this.gameObject.tag) && downDot1.CompareTag(this.gameObject.tag))
                // if item top and item bottom is equal
                {
                    upDot1.GetComponent<Dot>().isMatched = true;
                    downDot1.GetComponent<Dot>().isMatched = true;
                    isMatched = true;
                }
            }
            
        }
    }

    public GameObject SearchNameForBooster(string tagName) 
    {
        foreach (GameObject prefab in listOfBoosters) 
        {
            if (prefab.name == tagName) 
            {
                return prefab;
            }
        }
        //Debug.Log("There is not needed tag in the listOfBoosters");
        return null;
    }

    public void MakeRowBomb(Dot thisDot) 
    {
        if (isColumnBomb == false && isColorBomb == false && isAdjacentBomb == false)
        {
            //Debug.Log("Make Column bomb");
            if (board.currentDot != null)
            {
                //Debug.Log("current dot != null");
                string prefabName = board.currentDot.tag + "Row";
                GameObject prefabBooster = SearchNameForBooster(prefabName); //finded object with tag
                if (prefabBooster != null)
                {
                    GameObject arrow = Instantiate(prefabBooster, transform.position, Quaternion.identity); // object generation
                    arrow.transform.parent = thisDot.transform;
                    isRowBomb = true;
                }
            }
            else 
            {
                //Debug.Log("thisDot Search");
                string prefabName = thisDot.tag + "Row";
                //Debug.Log("thisDot.tag = " + thisDot.tag);
                GameObject prefabBooster = SearchNameForBooster(prefabName);
                if (prefabBooster != null)
                {
                    
                    thisDot.isMatched = false;
                    //Debug.Log("thisDot = " + thisDot);
                    //Debug.Log("thisDotP position = " + thisDot.transform.position);
                   // Debug.Log("Dot transform position = " + transform.position);
                    if (isRowBomb == false) 
                    {
                        GameObject arrow = Instantiate(prefabBooster, transform.position, Quaternion.identity);
                        //Debug.Log("bomb = " + arrow);
                        arrow.transform.parent = this.transform;
                        isRowBomb = true;
                    }
                    
                }
            }
            
            
        }   
    }

    public void MakeColumnBomb()
    {
        if (isRowBomb == false && isColorBomb == false && isAdjacentBomb == false) 
        {
            //Debug.Log("Make Column bomb");
            string prefabName = board.currentDot.tag + "Column";
            GameObject prefabBooster = SearchNameForBooster(prefabName);
            if (prefabBooster != null)
            {
                GameObject arrow = Instantiate(prefabBooster, transform.position, Quaternion.identity);
                arrow.transform.parent = this.transform;
                isColumnBomb = true;
            }
        }
    }

    public void MakeColorBomb(Dot thisDot)
    {
        if (isColumnBomb == false && isRowBomb == false && isAdjacentBomb == false) 
        {
            //Debug.Log("Make Column bomb");
            //board.debugLog("Make Color bomb", "");
            string prefabName = "ColorBomb";
            GameObject prefabBooster = SearchNameForBooster(prefabName);
            if (prefabBooster != null)
            {
                
                thisDot.isMatched = false;
                /*Debug.Log("thisDot = " + thisDot);
                Debug.Log("thisDotP position = " + thisDot.transform.position);
                Debug.Log("Dot transform position = " + transform.position);*/
                if (isColorBomb == false) 
                {
                    GameObject color = Instantiate(prefabBooster, thisDot.transform.position, Quaternion.identity);
                    //Debug.Log("bomb = " + color);
                    color.transform.parent = this.transform;
                    thisDot.gameObject.tag = "Color";
                    isColorBomb = true;
                }
                
            }
        }
        
    }

    public void AdjacentBomb(Dot thisDot)
    {
        if (isColumnBomb == false && isRowBomb == false && isColorBomb == false) 
        {
            //Debug.Log("Make Column bomb");
            string prefabName = "adjacentBomb";
            GameObject prefabBooster = SearchNameForBooster(prefabName);
            if (prefabBooster != null)
            {
                thisDot.isMatched = false;
                /*Debug.Log("thisDot = " + thisDot);
                Debug.Log("thisDotP position = " + thisDot.transform.position);
                Debug.Log("Dot transform position = " + transform.position);*/
                if (isAdjacentBomb == false) 
                {
                    GameObject marker = Instantiate(prefabBooster, transform.position, Quaternion.identity);
                    //Debug.Log("bomb = " + marker);
                    marker.transform.parent = this.transform;
                    isAdjacentBomb = true;
                }
                
            }
        }
        
    }

/*    private void OnTriggerEnter2D(Collider2D collision)
    {
        //Debug.Log("Fall detected");
    }*/
}
