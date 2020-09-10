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
    public int targetX; // цель свайпа по X
    public int targetY; // цель свайпа по Y
    public float swipeAngle = 0; // переменная для хранения угла

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

        //board = FindObjectOfType<Board>();// Находим таблицу на нашей сцене
        findMatches = FindObjectOfType<FindMatches>();
 
        //targetX = (int)transform.position.x; // координаты элемента таблицы по x преобразованные со float в integer
        //targetY = (int)transform.position.y; // координаты элемента таблицы по y преобразованные со float в integer
        //row = targetY;
        //column = targetX;
        //previousColumn = column;
        //previousRow = row;
    }

    //Это для тестов и дебаггов

    private void OnMouseOver()
    {
        /*if (Input.GetMouseButtonDown(1))
        {
            isAdjacentBomb = true;
            GameObject prefabBooster = SearchNameForBooster("adjacent"); // найденый обьект с помощью тега
            GameObject marker = Instantiate(prefabBooster, transform.position, Quaternion.identity); // генерация обьекта
            marker.transform.parent = this.transform;
        }*/
        /*if (Input.GetMouseButtonDown(1))
        {
            isColorBomb = true;
            GameObject prefabColorBomb = SearchNameForBooster("ColorBomb");
            GameObject color = Instantiate(prefabColorBomb, transform.position, Quaternion.identity);
            color.transform.parent = this.transform;
        }*/

    }

    // Update is called once per frame
    private IEnumerator DestroyFirstMatches() 
    {
        board.debugLog("Запуск функции уничтожение первых матчей", "---------------");
        if (board.currentState == GameState.move)
        {
            board.debugLog("GameState = " + board.currentState.ToString(), "");
            if (board != null)
            {
                yield return new WaitForSeconds(1.5f);
                if (board.currentState != GameState.win) 
                {
                    board.DestroyMatches();
                    board.debugLog("Матчи были уничтожены", "");
                }
                
            }
        }
        board.debugLog("GameState = " + board.currentState.ToString(), "");
    }

    void Update()
    {
        if (isMatched) 
        {
            board.debugLog("isMatched = " + isMatched.ToString(), "");
            SpriteRenderer mySprite = GetComponent<SpriteRenderer>();
            StartCoroutine(DestroyFirstMatches());
        }
        targetX = column;
        targetY = row;
        if (Mathf.Abs(targetX - transform.position.x) > .1) // если позиция по X больше чем 0.1  то значет что свайп идет вправо или влево
        {
            tempPosition = new Vector2(targetX, transform.position.y); // создаем новый вектор куда будет направлено движение
            transform.position = Vector2.Lerp(transform.position, tempPosition, .15f); // Lerp создает плавное скольжение где .1f время скольжение 
            if (board.allDots[column, row] != this.gameObject)
            {
                board.allDots[column, row] = this.gameObject;
            }
            findMatches.FindAllMatches();
        }
        else
        {
            tempPosition = new Vector2(targetX, transform.position.y); // создаем новый вектор куда будет направлено движение
            transform.position = tempPosition; // задаем элементую нужную позицию
            board.allDots[column, row] = this.gameObject; // обращаемся к переменной allDots в классе Board и присваиваем ее значение нашему обьекту то есть позицию точки по x и y
        }
        if (Mathf.Abs(targetY - transform.position.y) > .1) // если позиция по y больше чем 0.1  то значет что свайп идет вверх или вних
        {
            tempPosition = new Vector2(transform.position.x, targetY); 
            transform.position = Vector2.Lerp(transform.position, tempPosition, .15f);
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
        // Уничтожить подсказку
        if (hintManager != null) 
        {
            Debug.Log(hintManager.hint);
            hintManager.DestroyHint();
        }
        
        if (board.currentState == GameState.move) 
        {
            firstTouchPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        }
        
        // фиксируем позицию клика при помощи Input.mousePosition

        //Debug.Log(firstTouchPosition);
    }

    private void OnMouseUp()
    {
        if (board.currentState == GameState.move) 
        {
            finalTouchPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition); // Переводим значение позиции клика из пикселей в глобальные координаты
            CalculateAngle();
        }
        
    }

    void CalculateAngle() 
    {
        board.debugLog("CalculateAngle()", "----------");
        board.currentState = GameState.wait;
        swipeAngle = Mathf.Atan2(finalTouchPosition.y - firstTouchPosition.y, finalTouchPosition.x - firstTouchPosition.x) * 180 / Mathf.PI;
        movePieces();
        board.currentDot = this;
        board.debugLog("GameState = " + board.currentState.ToString(), "");
        if (swipeAngle == 0) 
        {
            board.currentState = GameState.move;
            board.debugLog("GameState = " + board.currentState.ToString(), "----------");
        }
    }

    void movePiecesActual(Vector2 direction) 
    {
        board.debugLog("MovePiecesActual()", "");
        otherDot = board.allDots[column + (int)direction.x, row + (int)direction.y];//передвигает выбраный элемент
        previousColumn = column;
        previousRow = row;
        if (board.lockTiles[column, row] == null && board.lockTiles[column + (int)direction.x, row + (int)direction.y] == null) // запрет на движение заблокировных элемнентов
        {
            if (otherDot != null)
            {
                otherDot.GetComponent<Dot>().column += -1 * (int)direction.x;
                otherDot.GetComponent<Dot>().row += -1 * (int)direction.y;
                column += (int)direction.x;
                row += (int)direction.y;
                tutorial.tutorialShow = false;
                StartCoroutine(CheckMoveCo());// проверка на возврат элемента в исходную позицию
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

    // Передвижение элементов
    void movePieces()
    {
        board.debugLog("MovePieces()", "------------");
        board.debugLog("GameState = " + board.currentState.ToString(), "");
        if (swipeAngle > -45 && swipeAngle <= 45 && column < board.width - 1 && swipeAngle != 0)
        {
            //Правый свайп
            movePiecesActual(Vector2.right); // Vector2(1, 0) передвижение элемента вправо на 1 единицу по оси x
            board.debugLog("GameState = " + board.currentState.ToString(), "");
        }
        else if (swipeAngle > 45 && swipeAngle <= 135 && row < board.height - 1 && swipeAngle != 0)
        {
            //Вверхний свайп
            movePiecesActual(Vector2.up);
            board.debugLog("GameState = " + board.currentState.ToString(), "");
        }
        else if ((swipeAngle > 135 || swipeAngle <= -135) && column > 0 && swipeAngle != 0)
        {
            //Левый свайп
            movePiecesActual(Vector2.left);
            board.debugLog("GameState = " + board.currentState.ToString(), "");
        }
        else if (swipeAngle < -45 && swipeAngle >= -135 && row > 0 && swipeAngle != 0)
        {
            //Свайп вниз
            movePiecesActual(Vector2.down);
            board.debugLog("GameState = " + board.currentState.ToString(), "");
        }
        else 
        {
            board.debugLog("GameState = " + board.currentState.ToString(), "");
        }
        
        
    }

    public IEnumerator CheckMoveCo() 
    {
        board.debugLog("CheckMoveCo()","------------");
        if (isColorBomb)
        {
            board.debugLog("isColorBomb = " + isColorBomb.ToString(), "");
            //этот элемент это молния, а другой элемент это элемент который нужно уничтожить
            findMatches.MatchPiecesOfColors(otherDot.tag);
            Debug.Log("ColorBombEffect = " + colorBombEffect);
            isMatched = true;
            colorBombEffect.StartEffect(findMatches.listOfColorMatches, otherDot.transform.position);
            yield return new WaitForSeconds(1f);
            board.debugLog("isMatched = " + isMatched.ToString(), "");
        } else if (otherDot.GetComponent<Dot>().isColorBomb) 
        {
            //этот элемент это элемент который нужно уничтожить, а другой элемент это молния
            findMatches.MatchPiecesOfColors(this.gameObject.tag);
            Debug.Log("ColorBombEffect = " + colorBombEffect);
            otherDot.GetComponent<Dot>().isMatched = true;
            colorBombEffect.StartEffect(findMatches.listOfColorMatches, otherDot.transform.position);
            yield return new WaitForSeconds(1f);

        }
        yield return new WaitForSeconds(.5f); // пауза 
        if (otherDot != null)
        {
            board.debugLog("isMatched = " + isMatched.ToString(), "");
            if (!isMatched && !otherDot.GetComponent<Dot>().isMatched) // если нету матчей то элементы в исходыне позиции
            {
                otherDot.GetComponent<Dot>().row = row;
                otherDot.GetComponent<Dot>().column = column;
                row = previousRow;
                column = previousColumn;
                yield return new WaitForSeconds(.5f);
                board.currentDot = null;
                board.currentState = GameState.move;
                board.debugLog("GameState = " + board.currentState.ToString(), "-------------------");
            }
            else
            {
                board.debugLog("GameState = " + board.currentState.ToString(), "");
                if (endGameManager != null) 
                {
                    if (endGameManager.requierments.gameType == GameType.Moves) 
                    {
                        endGameManager.DecreaseCounterValue();
                        board.debugLog("GameState = " + board.currentState.ToString(), "");
                    }
                }
                if (board.currentState != GameState.win) 
                {
                    board.DestroyMatches(); // уничтожить заматченые элементы
                    board.debugLog("GameState = " + board.currentState.ToString(), "-----------------");
                }
                
            }
            //otherDot = null;
        }

    }


    // Нахождение матчей
    void FindMatches() 
    {
        board.debugLog("FindMatches(), dot script", "");
        if (column > 0 && column < board.width - 1) // если 
        {
            GameObject leftDot1 = board.allDots[column - 1, row]; // находим элемент слева 
            GameObject rightDot1 = board.allDots[column + 1, row]; // находим элемент справа
            if (leftDot1 != null && rightDot1 !=null) 
            {
                if (leftDot1.tag == this.gameObject.tag && rightDot1.tag == this.gameObject.tag)
                // если элемент справа и элемент слева равны основному обьекту то выйгрыш
                {
                    leftDot1.GetComponent<Dot>().isMatched = true;
                    rightDot1.GetComponent<Dot>().isMatched = true;
                    isMatched = true;
                    board.debugLog("isMatched = " + isMatched.ToString(), "");
                }
            }
            
        }
        if (row > 0 && row < board.height - 1)
        {
            GameObject upDot1 = board.allDots[column, row + 1]; // находим элемент сверху 
            GameObject downDot1 = board.allDots[column, row -1]; // находим элемент снизу
            if (upDot1 != null && downDot1 != null) 
            {
                if (upDot1.tag == this.gameObject.tag && downDot1.tag == this.gameObject.tag)
                // если элемент сверху и элемент снизу равны основному обьекту то выйгрыш
                {
                    upDot1.GetComponent<Dot>().isMatched = true;
                    downDot1.GetComponent<Dot>().isMatched = true;
                    isMatched = true;
                    board.debugLog("isMatched = " + isMatched.ToString(), "");
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
        Debug.Log("There is not needed tag in the listOfBoosters");
        return null;
    }

    public void MakeRowBomb(Dot thisDot) 
    {
        if (isColumnBomb == false && isColorBomb == false && isAdjacentBomb == false)
        {
            board.debugLog("Make Column bomb", "");
            //Debug.Log("Make Column bomb");
            if (board.currentDot != null)
            {
                //Debug.Log("current dot != null");
                string prefabName = board.currentDot.tag + "Row"; // переменная в которой хранится имя префаба который надо сгенерировать
                GameObject prefabBooster = SearchNameForBooster(prefabName); // найденый обьект с помощью тега
                if (prefabBooster != null)
                {
                    GameObject arrow = Instantiate(prefabBooster, transform.position, Quaternion.identity); // генерация обьекта
                    arrow.transform.parent = thisDot.transform;
                    isRowBomb = true;
                }
            }
            else 
            {
                //Debug.Log("thisDot Search");
                string prefabName = thisDot.tag + "Row"; // переменная в которой хранится имя префаба который надо сгенерировать
                Debug.Log("thisDot.tag = " + thisDot.tag);
                GameObject prefabBooster = SearchNameForBooster(prefabName); // найденый обьект с помощью тега
                if (prefabBooster != null)
                {
                    
                    thisDot.isMatched = false;
                    //Debug.Log("thisDot = " + thisDot);
                    //Debug.Log("thisDotP position = " + thisDot.transform.position);
                   // Debug.Log("Dot transform position = " + transform.position);
                    if (isRowBomb == false) 
                    {
                        GameObject arrow = Instantiate(prefabBooster, transform.position, Quaternion.identity); // генерация обьекта
                        Debug.Log("bomb = " + arrow);
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
            board.debugLog("Make Column bomb", "");
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
            board.debugLog("Make adjacent bomb", "");
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

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log("Fall detected");
    }
}
