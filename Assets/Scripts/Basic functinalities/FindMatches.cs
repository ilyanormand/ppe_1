using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class FindMatches : MonoBehaviour
{
    // Start is called before the first frame update

    private Board board;
    public List<GameObject> currentMatches = new List<GameObject>();
    public GameObject stripesEffect;
    public GameObject AdjacentExplosion;
    public SoundManager soundManager;
    public bool soundStripe = false;
    public List<GameObject> listOfColorMatches;
    void Start()
    {
        board = FindObjectOfType<Board>();
        stripesEffect = board.StripesBombExplosion;
        AdjacentExplosion = board.AdjacentBombExplosion;
    }

    public void FindAllMatches() 
    {
        //board.debugLog("FindAllMatches()", "-----------");
        //board.debugLog("GameState = " + board.currentState.ToString(), "");
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
        if (!currentMatches.Contains(dot)) 
        {
            currentMatches.Add(dot);
        }
        dot.GetComponent<Dot>().isMatched = true;
    }

    private void GetNearbyPieces(GameObject dot1, GameObject dot2, GameObject dot3) 
    {
        AddToListAndMatch(dot1);
        AddToListAndMatch(dot2);
        AddToListAndMatch(dot3);
    }
    private IEnumerator FindAllmatchesCo()
    {
        yield return new WaitForSeconds(.1f);
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
                            if (leftDot.CompareTag(currenDot.tag) && rightDot.CompareTag(currenDot.tag)) // match verification
                            {
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
                            if (upDot.CompareTag(currenDot.tag) && downDot.CompareTag(currenDot.tag))
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
        listOfColorMatches = new List<GameObject>();
        for (int i = 0; i < board.width; i++) 
        {
            for (int j = 0; j < board.height; j++) 
            {
                if (board.allDots[i, j] != null) 
                {
                    if (board.allDots[i, j].tag == color) 
                    {

                        board.allDots[i, j].GetComponent<Dot>().isMatched = true;
                        listOfColorMatches.Add(board.allDots[i, j]);
                    }
                }
            }
        }
    }


    List<GameObject> GetAdjacentPieces(int column, int row)  
    {
        List<GameObject> dots = new List<GameObject>();   

        int sizeOfExplosion = 1;
        for (int i = column - sizeOfExplosion; i <= column + sizeOfExplosion; i++)  
        {
            for (int j = row - sizeOfExplosion; j <= row + sizeOfExplosion; j++)
            {

                if (i >= 0 && i < board.width && j >= 0 && j < board.height) 
                {
                    if (board.allDots[i, j] != null) 
                    {
                        dots.Add(board.allDots[i, j]); 
                        board.allDots[i, j].GetComponent<Dot>().isMatched = true;
                    }
                    
                }
            }
        }
        GameObject adjacentEffect = Instantiate(AdjacentExplosion, board.allDots[column, row].transform.position, Quaternion.identity);
        soundManager.playAdjacentSound();

        Destroy(adjacentEffect, 1f);
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
        if (board.currentDot != null) 
        {
            if (currentMatches != null) 
            {
                GameObject[] stripeObjects = GameObject.FindGameObjectsWithTag("columnBomb");
                foreach (GameObject element in currentMatches)
                {
                    /*Debug.Log("Startitng searching bombColumn in currentMatches");
                    Debug.Log("CurrentMatches count = " + currentMatches.Count);
                    Debug.Log("StripeObject.tag = " + stripeObjcet.tag);
                    if (stripeObjcet.tag == "columnBomb")
                    {
                        Debug.Log("StripeObject.tag = " + stripeObjcet.tag);
                        GameObject stripeColumn = Instantiate(stripesEffect, stripeObjcet.transform.position, Quaternion.Euler(0f, 0f, 0f));
                        Debug.Log("stripeColumn = " + stripeColumn);
                        soundManager.playStripeSound("column");
                        Destroy(stripeColumn, 0.3f);
                    }*/

                    foreach (GameObject stripe in stripeObjects) 
                    {
                        if (stripe.transform.position == element.transform.position) 
                        {
                            GameObject stripeColumn = Instantiate(stripesEffect, element.transform.position, Quaternion.Euler(0f, 0f, 90f));
                            soundManager.playStripeSound("column");
                            Destroy(stripeColumn, 0.3f);
                        }
                        
                    }
                }
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
        if (board.currentDot != null) 
        {
            if (currentMatches != null) 
            {
                GameObject[] stripeObjects = GameObject.FindGameObjectsWithTag("rowBomb");
                foreach (GameObject element in currentMatches)
                {
                    /*Debug.Log("Startitng searching bombRow in currentMatches");
                    Debug.Log("CurrentMatches count = " + currentMatches.Count);
                    Debug.Log("StripeObject.tag = " + stripeObjcet.tag);
                    if (stripeObjcet.tag == "rowBomb")
                    {
                        Debug.Log("StripeObject.tag = " + stripeObjcet.tag);
                        GameObject stripeRow = Instantiate(stripesEffect, stripeObjcet.transform.position, Quaternion.Euler(0f, 0f, 90f));
                        Debug.Log("stripeRow = " + stripeRow);
                        soundManager.playStripeSound("row");
                        Destroy(stripeRow, 0.3f);
                    }*/
                    foreach (GameObject stripe in stripeObjects) 
                    {
                        if (stripe.transform.position == element.transform.position) 
                        {
                            GameObject stripeRow = Instantiate(stripesEffect, element.transform.position, Quaternion.Euler(0f, 0f, 0f));
                            //Debug.Log("stripeRow = " + stripeRow);
                            soundManager.playStripeSound("row");
                            Destroy(stripeRow, 0.3f);
                        }
                    }
                    
                }
            }
        }
        
        return dots;
    }

    

    public void ChekcBombs() 
    {
        //Debug.Log("CheckBombs()");
        if (board.currentDot != null) 
        {
            board.currentDot.isMatched = false;
            int typeOfBomb = Random.Range(0, 100);
            if (typeOfBomb < 50)
            {
                //Debug.Log("We will create a row bomb");
                board.currentDot.MakeRowBomb(board.currentDot);
            }
            else if (typeOfBomb >= 50)
            {
                //Debug.Log("We will create a column bomb");
                board.currentDot.MakeColumnBomb();
            }

            /*else if (board.currentDot.otherDot != null) 
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
            }*/
        }
    }
    
}
