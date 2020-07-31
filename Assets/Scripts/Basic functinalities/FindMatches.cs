using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class FindMatches : MonoBehaviour
{
    // Start is called before the first frame update

    private Board board;
    public List<GameObject> currentMatches = new List<GameObject>();
    void Start()
    {
        board = FindObjectOfType<Board>();
    }

    public void FindAllMatches() 
    {
        board.debugLog("FindAllMatches()", "-----------");
        board.debugLog("GameState = " + board.currentState.ToString(), "");
        StartCoroutine(FindAllmatchesCo());
    }

    private List<GameObject> IsAdjacentBomb(Dot dot1, Dot dot2, Dot dot3) 
    {
        List<GameObject> CurrentDots = new List<GameObject>();

        if (dot1.isAdjacentBomb)
        {
            currentMatches.Union(GetAdjacentPieces(dot1.column, dot1.row));
        }

        if (dot2.isAdjacentBomb)
        {
            currentMatches.Union(GetAdjacentPieces(dot2.column, dot2.row));
        }

        if (dot3.isAdjacentBomb)
        {
            currentMatches.Union(GetAdjacentPieces(dot3.column, dot3.row));
        }

        return CurrentDots;

    }

    private List<GameObject> IsRowBomb(Dot dot1,  Dot dot2, Dot dot3) 
    {
        List<GameObject> CurrentDots = new List<GameObject>();

        if(dot1.isRowBomb)
                                {
            currentMatches.Union(GetRowPieces(dot1.row));
            board.BombRow(dot1.row);
        }

        if (dot2.isRowBomb)
        {
            currentMatches.Union(GetRowPieces(dot2.row));
            board.BombRow(dot2.row);
        }

        if (dot3.isRowBomb)
        {
            currentMatches.Union(GetRowPieces(dot3.row));
            board.BombRow(dot3.row);
        }

        return CurrentDots;
    }

    private List<GameObject> IsColumnBomb(Dot dot1, Dot dot2, Dot dot3)
    {
        List<GameObject> CurrentDots = new List<GameObject>();

        if (dot1.isColumnBomb)
        {
            currentMatches.Union(GetColumnPieces(dot1.column));
            board.BombColumn(dot1.column);
        }

        if (dot2.isColumnBomb)
        {
            currentMatches.Union(GetColumnPieces(dot2.column));
            board.BombColumn(dot2.column);
        }

        if (dot3.isColumnBomb)
        {
            currentMatches.Union(GetColumnPieces(dot3.column));
            board.BombColumn(dot3.column);
        }

        return CurrentDots;
    }

    private void AddToListAndMatch(GameObject dot) 
    {
        board.debugLog("AddToListAndMatch()", "----------");
        board.debugLog("GameState = " + board.currentState.ToString(), "");
        if (!currentMatches.Contains(dot)) // если нынешний матч не имеет левый элемент то добавить левый элемент
        {
            currentMatches.Add(dot);
            board.debugLog("CurrentMatches = " + currentMatches.ToString(), "");
        }
        dot.GetComponent<Dot>().isMatched = true;
    }

    private void GetNearbyPieces(GameObject dot1, GameObject dot2, GameObject dot3) 
    {
        board.debugLog("GetNearbyPieces()", "----------");
        board.debugLog("GameState = " + board.currentState.ToString(), "");
        AddToListAndMatch(dot1);
        AddToListAndMatch(dot2);
        AddToListAndMatch(dot3);
    }
    private IEnumerator FindAllmatchesCo()
    {
        board.debugLog("FindAllMatchesCo()", "------------");
        board.debugLog("GameState = " + board.currentState.ToString(), "");
        yield return new WaitForSeconds(.2f); // пауза 0,2 секунды
        for (int i = 0; i < board.width; i++) 
        {
            for (int j = 0; j < board.height; j++) 
            {
                GameObject currenDot = board.allDots[i, j];
                
                if (currenDot != null) 
                {
                    Dot currentDotComponent = currenDot.GetComponent<Dot>();
                    if (i > 0 && i < board.width - 1) 
                    {
                        GameObject leftDot = board.allDots[i - 1, j]; 
                        GameObject rightDot = board.allDots[i + 1, j];
                        if (leftDot != null && rightDot != null) 
                        {
                            Dot leftDotComponent = leftDot.GetComponent<Dot>();
                            Dot rightDotComponent = rightDot.GetComponent<Dot>();
                            if (leftDot.tag == currenDot.tag && rightDot.tag == currenDot.tag) // проверка на матч
                            {
                                //проверка на бомбу
                                currentMatches.Union(IsRowBomb(leftDotComponent, currentDotComponent, rightDotComponent));

                                currentMatches.Union(IsColumnBomb(leftDotComponent, currentDotComponent, rightDotComponent));

                                currentMatches.Union(IsAdjacentBomb(leftDotComponent, currentDotComponent, rightDotComponent));

                                GetNearbyPieces(leftDot, currenDot, rightDot);
                                
                            }
                        }
                    }
                    if (j > 0 && j < board.height - 1)
                    {
                        GameObject downDot = board.allDots[i, j - 1];
                        GameObject upDot = board.allDots[i, j + 1];
                        
                        if (upDot != null && downDot != null)
                        {
                            Dot upDotComponent = upDot.GetComponent<Dot>();
                            Dot downDotComponent = downDot.GetComponent<Dot>();
                            if (upDot.tag == currenDot.tag && downDot.tag == currenDot.tag)
                            {
                                currentMatches.Union(IsColumnBomb(upDotComponent, currentDotComponent, downDotComponent));

                                currentMatches.Union(IsRowBomb(upDotComponent, currentDotComponent, downDotComponent));

                                currentMatches.Union(IsAdjacentBomb(upDotComponent, currentDotComponent, downDotComponent));

                                GetNearbyPieces(upDot, currenDot, downDot);
                            }
                        }
                    }
                }

            }
        }
    }

    public void MatchPiecesOfColors(string color) 
    {
        board.debugLog("MatchPiecesOfColors", "---------");
        board.debugLog("GameState = " + board.currentState.ToString(), "");
        for (int i = 0; i < board.width; i++) 
        {
            for (int j = 0; j < board.height; j++) 
            {
                //проверяем существует ли элемент
                if (board.allDots[i, j] != null) 
                {
                    //проверить тег на элементе
                    if (board.allDots[i, j].tag == color) 
                    {
                        //Заматчить эти элементы
                        board.allDots[i, j].GetComponent<Dot>().isMatched = true;
                    }
                }
            }
        }
    }


    List<GameObject> GetAdjacentPieces(int column, int row)  // где column, row это позиция нашей бомбочки
    {
        List<GameObject> dots = new List<GameObject>(); //  создаем массив для хранение нужных нам жлементов для уничтожение
        // перебор массива сразу от позиции бомбочик вокруг нее
        int sizeOfExplosion = 1;
        for (int i = column - sizeOfExplosion; i <= column + sizeOfExplosion; i++)  
        {
            for (int j = row - sizeOfExplosion; j <= row + sizeOfExplosion; j++)
            {
                // Проверка есть ли элемент в таблицы
                if (i >= 0 && i < board.width && j >= 0 && j < board.height) 
                {
                    if (board.allDots[i, j] != null) 
                    {
                        dots.Add(board.allDots[i, j]); // добавляем элемент в массив
                        board.allDots[i, j].GetComponent<Dot>().isMatched = true; // делаем данный элемент заматченнным 
                    }
                    
                }
            }
        }
        return dots;
    }

    List<GameObject> GetColumnPieces(int column) 
    {
        List<GameObject> dots = new List<GameObject>();
        for (int i = 0; i < board.height; i++) 
        {
            if (board.allDots[column, i] != null) 
            {
                Dot dot = board.allDots[column, i].GetComponent<Dot>();
                if (dot.isRowBomb) 
                {
                    dots.Union(GetRowPieces(i)).ToList();
                }
                dots.Add(board.allDots[column, i]);
                dot.isMatched = true;
            }
        }

        return dots;
    }

    List<GameObject> GetRowPieces(int row)
    {
        List<GameObject> dots = new List<GameObject>();
        for (int i = 0; i < board.width; i++)
        {
            if (board.allDots[i, row] != null)
            {
                Dot dot = board.allDots[i, row].GetComponent<Dot>();
                if (dot.isColumnBomb)
                {
                    dots.Union(GetColumnPieces(i)).ToList();
                }

                dots.Add(board.allDots[i, row]);
                dot.isMatched = true;
            }
        }

        return dots;
    }

    public void ChekcBombs() 
    {
        board.debugLog("CheckBombs()", "-------------");
        board.debugLog("GameState = " + board.currentState.ToString(), "");
        // проверка на то что двигал ли что то игрок
        if (board.currentDot != null) 
        {
            // проверка на то что двигаемый элемент заматченый
            if (board.currentDot.isMatched)
            {
                board.currentDot.isMatched = false;
                //выбрать какую бомбу сделать
                int typeOfBomb = Random.Range(0, 100);
                if (typeOfBomb < 50)
                {
                    // сделать бомбу которая взрывает горизонтальный ряд
                    board.currentDot.MakeRowBomb();
                }
                else if (typeOfBomb >= 50)
                {
                    // сделать бомбу которая взрывает вертикальный ряд
                    board.currentDot.MakeColumnBomb();
                }
            }
            // проверка на то что другой элемент заматчен
            else if (board.currentDot.otherDot != null) 
            {
                Dot otherDot = board.currentDot.otherDot.GetComponent<Dot>();
                //проверка на матч другой кнопки на которую мы меняем
                if (otherDot.isMatched) 
                {
                    otherDot.isMatched = false;
                    // выбираем какую бомбу сделать
                    int typeOfBomb = Random.Range(0, 100);
                    if (typeOfBomb < 50)
                    {
                        // сделать бомбу которая взрывает горизонтальный ряд
                        otherDot.MakeRowBomb();
                    }
                    else if (typeOfBomb >= 50)
                    {
                        // сделать бомбу которая взрывает вертикальный ряд
                        otherDot.MakeColumnBomb();
                    }
                }
            }
        }
        board.debugLog("GameState = " + board.currentState.ToString(), "---------------");
    }
    
}
