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

    
    public bool isMatched = false;
    public GameObject otherDot;

    [Header("Бустеры")]
    public bool isColumnBomb, isRowBomb, isColorBomb, isAdjacentBomb;
    public GameObject AdjacentMarker, rowArrow, columnArrow, colorBomb;
    void Start()
    {
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
        if (Input.GetMouseButtonDown(1))
        {
            isRowBomb = true;
            GameObject marker = Instantiate(rowArrow, transform.position, Quaternion.identity);
            marker.transform.parent = this.transform;
        }
        /*if (Input.GetMouseButtonDown(1))
        {
            isColorBomb = true;
            GameObject color = Instantiate(colorBomb, transform.position, Quaternion.identity);
            color.transform.parent = this.transform;
        }*/
    }

    // Update is called once per frame
    private IEnumerator DestroyFirstMatches() 
    {
        if (board != null) 
        {
            yield return new WaitForSeconds(1.5f);
            board.DestroyMatches();
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
                StartCoroutine(CheckMoveCo());// проверка на возврат элемента в исходную позицию
            }
            else
            {
                //board.currentState = GameState.move;
            }
        }
        else
        {
            //board.currentState = GameState.move;
        }

    }

    // Передвижение элементов
    void movePieces()
    {
        Debug.Log(board.currentState);
        board.currentState = GameState.wait;
        Debug.Log(board.currentState);
        if (swipeAngle > -45 && swipeAngle <= 45 && column < board.width - 1 && swipeAngle != 0)
        {
            //Правый свайп
            movePiecesActual(Vector2.right);
        }
        else if (swipeAngle > 45 && swipeAngle <= 135 && row < board.height - 1 && swipeAngle != 0)
        {
            //Вверхний свайп
            movePiecesActual(Vector2.up);
        }
        else if ((swipeAngle > 135 || swipeAngle <= -135) && column > 0 && swipeAngle != 0)
        {
            //Левый свайп
            movePiecesActual(Vector2.left);
        }
        else if (swipeAngle < -45 && swipeAngle >= -135 && row > 0 && swipeAngle != 0)
        {
            //Свайп вниз
            movePiecesActual(Vector2.down);
        }
        else 
        {
            board.currentState = GameState.move;
        }
        
        
    }

    // если нету матчей то функция возращает элемент в пердыдущию позицию
    public IEnumerator CheckMoveCo() 
    {
        if (isColorBomb)
        {
            //этот элемент это молния, а другой элемент это элемент который нужно уничтожить
            findMatches.MatchPiecesOfColors(otherDot.tag);
            isMatched = true;
        } else if (otherDot.GetComponent<Dot>().isColorBomb) 
        {
            //этот элемент это элемент который нужно уничтожить, а другой элемент это молния
            findMatches.MatchPiecesOfColors(this.gameObject.tag);
            otherDot.GetComponent<Dot>().isMatched = true;
        }
        yield return new WaitForSeconds(.5f); // пауза 
        if (otherDot != null)
        {
            if (!isMatched && !otherDot.GetComponent<Dot>().isMatched) // если нету матчей то элементы в исходыне позиции
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
                board.DestroyMatches(); // уничтожить заматченые элементы     
            }
            //otherDot = null;
        }

    }


    // Нахождение матчей
    void FindMatches() 
    {
        if (column > 0 && column < board.width - 1) 
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
                }
            }
            
        }
    }

    public void MakeRowBomb() 
    {
        if (!isColumnBomb && !colorBomb && !isAdjacentBomb) 
        {
            isRowBomb = true;
            GameObject arrow = Instantiate(rowArrow, transform.position, Quaternion.identity);
            arrow.transform.parent = this.transform;
        }
        
    }

    public void MakeColumnBomb()
    {
        if (!isRowBomb && !colorBomb && !isAdjacentBomb) 
        {
            isColumnBomb = true;
            GameObject arrow = Instantiate(columnArrow, transform.position, Quaternion.identity);
            arrow.transform.parent = this.transform;
        }
        
    }

    public void MakeColorBomb()
    {
        if (!isColumnBomb && !isRowBomb && !isAdjacentBomb) 
        {
            isColorBomb = true;
            GameObject color = Instantiate(colorBomb, transform.position, Quaternion.identity);
            color.transform.parent = this.transform;
            this.gameObject.tag = "Color";
        }
        
    }

    public void AdjacentBomb()
    {
        if (!isColumnBomb && !isRowBomb && !isColorBomb) 
        {
            isAdjacentBomb = true;
            GameObject marker = Instantiate(AdjacentMarker, transform.position, Quaternion.identity);
            marker.transform.parent = this.transform;
        }
        
    }
}
