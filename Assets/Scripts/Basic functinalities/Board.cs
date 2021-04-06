using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GameState 
{
    wait,
    move,
    win,
    lose,
    pause
}
public enum TileKind 
{
    Breakable,
    Blank,
    Lock,
    Concrete,
    Chocolate,
    Normal
}

[System.Serializable]
public class TileType 
{
    public int x;
    public int y;
    public TileKind tileKind;
}
public class Board : MonoBehaviour
{

    [Header("Scriptable Object Stuff")]
    public World world;
    public int level;
    public GameState currentState = GameState.move;

    [Header("Board Dimensions")]
    public int width, height;
    public float offset;

    [Header("Prefabs")]
    public GameObject breakableTilePrefab, tilePrefab, lockTilePrefab, concreteTilePrefab, chocolateTilePrefab;
    [Space(20)]
    public GameObject[] dots;//array ou on va avoir des fruits
    public Dot thisDot;

    [Header("Prefabs for explosion effects")]
    //les effets standards de l'explosion des fruits
    public GameObject DonutExplosion, HeartExplosion, GreenBlopExplosion, RedSweetExplosion, StarExplosion, VioletExplosion;
    //les effets de l'explosion des boosters
    public GameObject StripesBombExplosion, ColorBombExplosion, ColorBombTrails, AdjacentBombExplosion;

    [Header("Variable thaht allow to construct tiles")]
    public bool GenBlankSpaces = true;
    public bool GenBreakableTiles = false;
    public bool GenChocolateTiles = false;
    public bool GenConcreteTiles = false;
    public bool GenLockTiles = false;




    [Header("Layout")]
    //private BackgroundTile[,] allTiles; 
    private bool[,] blankSpaces;
    private FindMatches findMatches;
    private BackgroundTile[,] breakableTiles;
    public BackgroundTile[,] lockTiles;
    public BackgroundTile[,] concreteTiles;
    public BackgroundTile[,] chocolateTiles;
    public GameObject[,] allDots;
    public Dot currentDot;
    public TileType[] boardLayout;
    public int basePieceScoreValue = 20;
    public int streakValue = 1;
    private ScoreManager scoreManager;
    private SoundManager soundManager;
    private GoalManager goalManager;
    public float refillDelay;
    public int[] ScoreGoals;
    private bool makeSlime = true;
    [Header("for debuging")]
    public bool debug = true;

    [Header("For HintManager")]
    public List<GameObject> MatchList;

    private void Awake()
    {
        if (PlayerPrefs.HasKey("Current Level"))
        {
            level = PlayerPrefs.GetInt("Current Level");
        }
        if (world != null)
        {
            if (level < world.levels.Length) //  verification de value max de niveau
            {
                if (world.levels[level] != null) //verification d'existance de niveau
                {
                    //initialisation de parametres de board en fonction des paramateres de niveau
                    width = world.levels[level].width;
                    height = world.levels[level].height;
                    dots = world.levels[level].dots;
                    ScoreGoals = world.levels[level].scoreGoals;
                    boardLayout = world.levels[level].boardLayout;
                }
            }
        }
    }

    void Start()
    {
        goalManager = FindObjectOfType<GoalManager>();
        soundManager = FindObjectOfType<SoundManager>();
        scoreManager = FindObjectOfType<ScoreManager>();
        breakableTiles = new BackgroundTile[width, height];
        lockTiles = new BackgroundTile[width, height];
        concreteTiles = new BackgroundTile[width, height];
        chocolateTiles = new BackgroundTile[width, height];
        findMatches = FindObjectOfType<FindMatches>();
        //allTiles = new BackgroundTile[width, height];
        blankSpaces = new bool[width, height];
        allDots = new GameObject[width, height];
        SetUp(); // Board Generating
        offset = 10;

    }

    // une methode pour monter des logs
    public void debugLog(string description, string debugElement) 
    {
        if (debug == true) 
        {
            Debug.Log(description);
            Debug.Log(debugElement);
        }
    }

    // Generation des espaces vides
    public void GeneratingBlankSpaces()
    {
        if (GenBlankSpaces) 
        {
            for (int i = 0; i < boardLayout.Length; i++)
            {
                if (boardLayout[i].tileKind == TileKind.Blank)
                {
                    blankSpaces[boardLayout[i].x, boardLayout[i].y] = true;
                }
            }
        }
        
    }

    /*public bool VerifyTypeTile(TileKind TileType)
    {
        for (int i = 0; i < boardLayout.Length; i++)
        {
            if (boardLayout[i].tileKind == TileType)
            {
                return true;
            }
        }
        return false;
    }

    public void GenerateTiles(int Iteratator, GameObject TilePrefab, BackgroundTile[,] list) 
    {
        // Создать ломаемую плитку в данной позиции
        Vector2 tempPosition = new Vector2(boardLayout[Iteratator].x, boardLayout[Iteratator].y);
        GameObject tile = Instantiate(TilePrefab, tempPosition, Quaternion.identity);
        list[boardLayout[Iteratator].x, boardLayout[Iteratator].y] = tile.GetComponent<BackgroundTile>();
    }*/

    // generations des tiles qui peuvent etre casse
    public void GenerateBreakableTiles() 
    {
        if (GenBreakableTiles)
        {
            for (int i = 0; i < boardLayout.Length; i++)
            {
                if (boardLayout[i].tileKind == TileKind.Breakable)
                {
                    // creation de tile
                    Vector2 tempPosition = new Vector2(boardLayout[i].x, boardLayout[i].y);
                    GameObject tile = Instantiate(breakableTilePrefab, tempPosition, Quaternion.identity);
                    breakableTiles[boardLayout[i].x, boardLayout[i].y] = tile.GetComponent<BackgroundTile>();
                }
            }
        }
    }


    // Generations de locked tiles
    private void GeneratLockTiles() 
    {
        if (GenLockTiles)
        {
            for (int i = 0; i < boardLayout.Length; i++)
            {
                if (boardLayout[i].tileKind == TileKind.Lock)
                {
                    // creation de tile
                    Vector2 tempPosition = new Vector2(boardLayout[i].x, boardLayout[i].y);
                    GameObject tile = Instantiate(lockTilePrefab, tempPosition, Quaternion.identity);
                    lockTiles[boardLayout[i].x, boardLayout[i].y] = tile.GetComponent<BackgroundTile>();
                }
            }
        }
    }

    // Generations de concreteTiles
    private void GeneratConcreteTiles()
    {
        if (GenConcreteTiles)
        {
            for (int i = 0; i < boardLayout.Length; i++)
            {
                
                if (boardLayout[i].tileKind == TileKind.Concrete)
                {
                    // creation de tile
                    Vector2 tempPosition = new Vector2(boardLayout[i].x, boardLayout[i].y);
                    GameObject tile = Instantiate(concreteTilePrefab, tempPosition, Quaternion.identity);
                    concreteTiles[boardLayout[i].x, boardLayout[i].y] = tile.GetComponent<BackgroundTile>();
                }
            }
        }
    }

    //Generations des tiles en chocolat
    private void GeneratChocolateTiles()
    {
        if (GenChocolateTiles)
        {
            for (int i = 0; i < boardLayout.Length; i++)
            {
                if (boardLayout[i].tileKind == TileKind.Chocolate)
                {
                    Vector2 tempPosition = new Vector2(boardLayout[i].x, boardLayout[i].y);
                    GameObject tile = Instantiate(chocolateTilePrefab, tempPosition, Quaternion.identity);
                    chocolateTiles[boardLayout[i].x, boardLayout[i].y] = tile.GetComponent<BackgroundTile>();
                }
            }
        }
    }

    /*public void GenerateTypeTiles(GameObject PrefabOfTile, BackgroundTile[,] ListOfTiles) 
    {
        for (int i = 0; i < boardLayout.Length; i++) 
        {
            bool TileVerifaction = VerifyTypeTile(TileKind.Lock);
            if (TileVerifaction)
            {
                GenerateTiles(i, PrefabOfTile, ListOfTiles);
            }
        }
    }*/

    private void SetUp() 
    {
        GeneratingBlankSpaces();
        GenerateBreakableTiles();
        GeneratLockTiles();
        GeneratConcreteTiles();
        GeneratChocolateTiles();
        for (int i = 0; i < width; i++) 
        {
            // pour chaque tiles sur x on genere des tiles sur y
            for (int j = 0; j < height; j++) 
            {
                if (!blankSpaces[i, j] && !concreteTiles[i, j] && !chocolateTiles[i, j]) 
                {
                    Vector2 tempPosition = new Vector2(i, j + offset); // initialisation de position pour tiles
                    Vector2 tilePosition = new Vector2(i, j);

                    //creation de tile sur la scene
                    GameObject backgroundTile = Instantiate(tilePrefab, tilePosition, Quaternion.identity) as GameObject;


                    backgroundTile.transform.parent = this.transform;  //on place l'objet backgroundTile dans Objet board

                    backgroundTile.name = "(" + i + ", " + j + ")"; 

                    int dotToUse = Random.Range(0, dots.Length); // Choix random de fruit qui va etre genere sur tile
                    int maxIteration = 0;
                    while (MatchesAt(i, j, dots[dotToUse]) && maxIteration < 100)
                    {
                        dotToUse = Random.Range(0, dots.Length);
                        maxIteration++;
                    }

                    maxIteration = 0;
                    // creation d'objet fruit a partir de nombre random
                    GameObject dot = Instantiate(dots[dotToUse], tempPosition, Quaternion.identity);
                    dot.GetComponent<Dot>().row = j;
                    dot.GetComponent<Dot>().column = i;
                    dot.transform.parent = this.transform; 
                    dot.name = "(" + i + ", " + j + ")";  
                    allDots[i, j] = dot;
                }
            }
        }
    }

    // verification des matches sur les bords
    private bool MatchesAt(int column, int row, GameObject piece) 
    {
        if (column > 1 && row > 1) 
        {
            if (allDots[column - 1, row] != null && allDots[column - 2, row] != null)
            {
                if (allDots[column - 1, row].CompareTag(piece.tag) && allDots[column - 2, row])//si le fruit de droit et le fruit secondaire de droit sont égaux au fruit de base

                {
                    return true;
                }
            }
            if (allDots[column, row - 1] != null && allDots[column, row - 2] != null) 
            {
                if (allDots[column, row - 1].CompareTag(piece.tag) && allDots[column, row - 2])//si le fruit de bas et le fruit secondaire de bas sont égaux au fruit de base
                {
                    
                    return true;
                }
            }
            
            else if (column <= 1 || row <= 1 )
            {
                if (row > 1) 
                {
                    if (allDots[column, row - 1] != null && allDots[column, row - 2] != null) 
                    {
                        if (allDots[column, row - 1].CompareTag(piece.tag) && allDots[column, row - 2].CompareTag(piece.tag)) ////si le fruit de bas et le fruit secondaire de bas sont égaux au fruit de base
                        {
                            
                            return true;
                        }
                    }
                    
                }
                if (column > 1)
                {
                    if (allDots[column-1, row] != null && allDots[column-2, row] != null) 
                    {
                        if (allDots[column - 1, row].CompareTag(piece.tag) && allDots[column - 2, row].CompareTag(piece.tag)) //si le fruit de droit et le fruit secondaire de droit sont égaux au fruit de base
                        {
                            return true;
                        }
                    }
                    
                }
            }
        }

        return false;
    }

    private int ColumnOrRow()  //cette methode definit le type de match

    {
        // creations de copy des matches
        List<GameObject> matchCopy = findMatches.currentMatches as List<GameObject>;
        // verifaction de tout les match pour voir si on peut creer un booster
        for (int i = 0; i < matchCopy.Count; i++) 
        {
            Dot thisDot = matchCopy[i].GetComponent<Dot>();
            int column = thisDot.column;
            int row = thisDot.row; 
            int columnMatch = 0;
            int rowMatch = 0;

            for (int j = 0; j < matchCopy.Count; j++) 
            {
                Dot nextDot = matchCopy[j].GetComponent<Dot>();
                if (nextDot == thisDot)
                {
                    continue;
                }
                if (nextDot.column == column && nextDot.CompareTag(thisDot.tag)) 
                {
                    columnMatch++;
                }
                if (nextDot.row == row && nextDot.CompareTag(thisDot.tag)) 
                {
                    rowMatch++;
                }
            }
            //return 3 si c'est un match de row ou column
            //return 2 si c'est un adjacent bombe
            //return 1 si c'est un color bomb
            if (columnMatch == 4 || rowMatch == 4) 
            {
                thisDot.MakeColorBomb(thisDot);
                return 1;
            }
            if (columnMatch == 2 && rowMatch == 2)
            {
                thisDot.AdjacentBomb(thisDot);
                return 2;
            }else if (columnMatch == 5 || rowMatch == 5)
            {
                thisDot.AdjacentBomb(thisDot);
                return 2;
            }
            if (columnMatch == 3 || rowMatch == 3) 
            {
                //Debug.Log("currentDot == null");
                thisDot.MakeRowBomb(thisDot);
                return 3;
            }
        }

        return 0;
        /*int numberHorizontal = 0;
        int numberVertical = 0;
        Dot firstPiece = findMatches.currentMatches[0].GetComponent<Dot>();
        if (firstPiece != null) 
        {
            foreach (GameObject currentPiece in findMatches.currentMatches)
            {
                Dot dot = currentPiece.GetComponent<Dot>();
                if (dot.row == firstPiece.row) 
                {
                    numberHorizontal++;
                }
                if (dot.column == firstPiece.column) 
                {
                    numberVertical++;
                }
            }
        }
        return (numberVertical == 5 || numberHorizontal == 5);*/
        

    }

    private void CheckToMakeBombs()
    {
        //проверка на количество обьектов в массиве currentMatches который показывает нам сколько всего заматченных элементов
        // verifacation de nombre d'objets dans le currentMatches
        if (findMatches.currentMatches.Count > 3)
        {
            int typeOfMatch = ColumnOrRow(); // recevoir le type de match
            if (typeOfMatch == 1) // si le type de match est 1 alors creation de color bomb
            {
                if (currentDot != null)
                {
                    if (currentDot.isMatched)
                    {
                        if (!currentDot.isColorBomb)
                        {
                            currentDot.isMatched = false;
                            currentDot.MakeColorBomb(currentDot);
                        }
                    }
                    else
                    {
                        if (currentDot.otherDot != null) 
                        {
                            Dot otherDot = currentDot.otherDot.GetComponent<Dot>();
                            if (otherDot.isMatched)
                            {
                                if (!otherDot.isColorBomb)
                                {
                                    otherDot.isMatched = false;
                                    otherDot.MakeColorBomb(currentDot);
                                }
                            }
                        }
                    }
                }
            }
            else if (typeOfMatch == 2)
            {
                if (currentDot != null)
                {
                    if (currentDot.isMatched)
                    {
                        if (!currentDot.isColorBomb)
                        {
                            currentDot.isMatched = false;
                            currentDot.AdjacentBomb(currentDot);
                        }
                    }
                    else
                    {
                        if (currentDot.otherDot != null)
                        {
                            Dot otherDot = currentDot.otherDot.GetComponent<Dot>();
                            if (otherDot.isMatched)
                            {
                                if (!otherDot.isColorBomb)
                                {
                                    otherDot.isMatched = false;
                                    otherDot.AdjacentBomb(otherDot);
                                }
                            }
                        }
                    }
                }
            }
            else if (typeOfMatch == 3) 
            {
                findMatches.ChekcBombs();
            }
        }

        /*if (findMatches.currentMatches.Count == 4 || findMatches.currentMatches.Count == 7) 
        {
            findMatches.ChekcBombs();
        }

        if (findMatches.currentMatches.Count == 5 || findMatches.currentMatches.Count == 8) 
        {
            if (ColumnOrRow())
            {
                //Создать color bomb
                if (currentDot != null)
                {
                    if (currentDot.isMatched)
                    {
                        if (!currentDot.isColorBomb)
                        {
                            currentDot.isMatched = false;
                            currentDot.MakeColorBomb();
                        }
                    }
                    else
                    {
                        if (currentDot.otherDot != null)
                        {
                            Dot otherDot = currentDot.otherDot.GetComponent<Dot>();
                            if (otherDot.isMatched)
                            {
                                if (!otherDot.isColorBomb)
                                {
                                    otherDot.isMatched = false;
                                    otherDot.MakeColorBomb();
                                }
                            }
                        }
                    }
                }
                else
                {
                    // Создать сплеш бомбу
                    if (currentDot != null)
                    {
                        if (currentDot.isMatched)
                        {
                            if (!currentDot.isColorBomb)
                            {
                                currentDot.isMatched = false;
                                currentDot.AdjacentBomb();
                            }
                        }
                        else
                        {
                            if (currentDot.otherDot != null)
                            {
                                Dot otherDot = currentDot.otherDot.GetComponent<Dot>();
                                if (otherDot.isMatched)
                                {
                                    if (!otherDot.isColorBomb)
                                    {
                                        otherDot.isMatched = false;
                                        otherDot.AdjacentBomb();
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }*/
    }
    
    // methode qui detruit concreteTiles quand les bomb s'activent
    public void BombRow(int row) 
    {
        for (int i = 0; i < width; i++) 
        {
            if (concreteTiles[i, row]) 
            {
                concreteTiles[i, row].TakeDamage(1);
                if (concreteTiles[i, row].hitPoints <= 0)
                {
                    concreteTiles[i, row] = null;
                }
            }
        }
    }
    // methode qui detruit concreteTiles quand les bomb s'activent
    public void BombColumn(int column)
    {
        for (int i = 0; i < height; i++)
        {
            if (concreteTiles[column, i])
            {
                concreteTiles[column, i].TakeDamage(1);
                if (concreteTiles[column, i].hitPoints <= 0)
                {
                    concreteTiles[column, i] = null;
                }
            }
        }
    }
    // Destruction des elements qui sont en match
    private void DestroyMatchesAt(int column, int row, Dot thisDot) 
    {
        if (allDots[column, row].GetComponent<Dot>().isMatched) 
        {
            if (findMatches.currentMatches.Count >= 4)
            {
                CheckToMakeBombs();
            }
            else 
            {
                //Debug.Log("Заматченных элементов < 4, currentMatches = " + findMatches.currentMatches.Count);
            }


            if(breakableTiles[column, row] != null)
            {
                breakableTiles[column, row].TakeDamage(1);
                if (breakableTiles[column, row].hitPoints <= 0)
                {
                    breakableTiles[column, row] = null;
                }
            }

            if (lockTiles[column, row] != null)
            {
                lockTiles[column, row].TakeDamage(1);
                if (lockTiles[column, row].hitPoints <= 0)
                {
                    lockTiles[column, row] = null;
                }
            }
            DamageConcrete(column, row);
            DamageChocolate(column, row);
            if (goalManager != null) 
            {
                goalManager.ComparedGoal(allDots[column, row].tag.ToString());
                goalManager.UpdateGoals();
            }

            if (soundManager != null && findMatches.soundStripe == false) 
            {
                soundManager.playDestroyNoise();
            }
            /*GameObject particle = Instantiate(explosionEffect, allDots[column, row].transform.position, Quaternion.identity);
            Destroy(particle, 0.2f);*/
            checkForColor(allDots[column, row], column, row); // creation d'effet en fonction de couleur de fruit
            Destroy(allDots[column, row]);
            scoreManager.InreaseScore(basePieceScoreValue);
            allDots[column, row] = null;
            
            
            
            
        }
    }

    public void checkForColor(GameObject dot, int column, int row)
    {
        if (dot.CompareTag("Donut"))
        {
            GameObject particle = Instantiate(DonutExplosion, allDots[column, row].transform.position, Quaternion.identity);
            Destroy(particle, 0.3f);
        }
        else if (dot.CompareTag("Heart"))
        {
            GameObject particle = Instantiate(HeartExplosion, allDots[column, row].transform.position, Quaternion.identity);
            Destroy(particle, 0.3f);
        }
        else if (dot.CompareTag("GreenBlop"))
        {
            GameObject particle = Instantiate(GreenBlopExplosion, allDots[column, row].transform.position, Quaternion.identity);
            Destroy(particle, 0.3f);
        }
        else if (dot.CompareTag("RedSweet"))
        {
            GameObject particle = Instantiate(RedSweetExplosion, allDots[column, row].transform.position, Quaternion.identity);
            Destroy(particle, 0.3f);
        }
        else if (dot.CompareTag("StartElement"))
        {
            GameObject particle = Instantiate(StarExplosion, allDots[column, row].transform.position, Quaternion.identity);
            Destroy(particle, 0.3f);
        }
        else if (dot.CompareTag("VioletSweet"))
        {
            GameObject particle = Instantiate(VioletExplosion, allDots[column, row].transform.position, Quaternion.identity);
            Destroy(particle, 0.3f);
        }
    }


    public void DestroyMatches()
    {
        currentState = GameState.wait;
        for (int i = 0; i < width; i++) 
        {
            for (int j = 0; j < height; j++) 
            {
                if (allDots[i, j] != null) 
                {
                    DestroyMatchesAt(i, j, thisDot);
                }
            }
        }
        //Debug.Log("End DestroyMatches, GameState = " + currentState);
        findMatches.soundStripe = false;
        findMatches.currentMatches.Clear();
        StartCoroutine(DecreaseRowCo2());
    }
    private void DamageConcrete(int column, int row) 
    {
        if (column > 0) 
        {
            if (concreteTiles[column - 1, row]) 
            {
                concreteTiles[column - 1, row].TakeDamage(1);
                if (concreteTiles[column - 1, row].hitPoints <= 0)
                {
                    concreteTiles[column - 1, row] = null;
                }
            }
        }
        if (column < width - 1)
        {
            if (concreteTiles[column + 1, row])
            {
                concreteTiles[column + 1, row].TakeDamage(1);
                if (concreteTiles[column + 1 , row].hitPoints <= 0)
                {
                    concreteTiles[column + 1, row] = null;
                }
            }
        }
        if (row > 0)
        {
            if (concreteTiles[column, row - 1])
            {
                concreteTiles[column, row - 1].TakeDamage(1);
                if (concreteTiles[column, row - 1].hitPoints <= 0)
                {
                    concreteTiles[column, row - 1] = null;
                }
            }
        }
        if (row < height - 1)
        {
            if (concreteTiles[column, row + 1])
            {
                concreteTiles[column, row + 1].TakeDamage(1);
                if (concreteTiles[column, row + 1].hitPoints <= 0)
                {
                    concreteTiles[column, row + 1] = null;
                }
            }
        }
    }
    private void DamageChocolate(int column, int row)
    {
        if (column > 0)
        {
            if (chocolateTiles[column - 1, row])
            {
                chocolateTiles[column - 1, row].TakeDamage(1);
                if (chocolateTiles[column - 1, row].hitPoints <= 0)
                {
                    chocolateTiles[column - 1, row] = null;
                }
                makeSlime = false;
            }
        }
        if (column < width - 1)
        {
            if (chocolateTiles[column + 1, row])
            {
                chocolateTiles[column + 1, row].TakeDamage(1);
                if (chocolateTiles[column + 1, row].hitPoints <= 0)
                {
                    chocolateTiles[column + 1, row] = null;
                }
                makeSlime = false;
            }
        }
        if (row > 0)
        {
            if (chocolateTiles[column, row - 1])
            {
                chocolateTiles[column, row - 1].TakeDamage(1);
                if (chocolateTiles[column, row - 1].hitPoints <= 0)
                {
                    chocolateTiles[column, row - 1] = null;
                }
                makeSlime = false;
            }
        }
        if (row < height - 1)
        {
            if (chocolateTiles[column, row + 1])
            {
                chocolateTiles[column, row + 1].TakeDamage(1);
                if (chocolateTiles[column, row + 1].hitPoints <= 0)
                {
                    chocolateTiles[column, row + 1] = null;
                }
                makeSlime = false;
            }
        }
    }
    //decalge des objets apres un match
    private IEnumerator DecreaseRowCo2() 
    {
        //currentState = GameState.wait;
        //debugLog("Запуск сдвига фруктов", "--------------");
        //Debug.Log("Start DecreaseRowCo2(), GameState = " + currentState);
        for (int i = 0; i < width; i++) 
        {
            for (int j = 0; j < height; j++)
            {
                if (!blankSpaces[i, j] && allDots[i, j] == null && !concreteTiles[i, j] && !chocolateTiles[i, j]) 
                {
                    for (int k = j + 1; k < height; k++) 
                    {
                         if (allDots[i, k] != null) 
                         {
                            allDots[i, k].GetComponent<Dot>().row = j;
                            //allDots[i, k].GetComponentInChildren<Animator>().enabled = true;
                            GameObject child = allDots[i, k].transform.Find("body").gameObject;
                            //Debug.Log("child =" + child);
                            child.GetComponent<Animator>().enabled =true;
                            child.GetComponent<Animator>().Play("DotFakk");
                            soundManager.FallingPiecesSound();
                            allDots[i, k] = null;
                            break;
                         }
                    }
                }
            }
        }
        //debugLog("Ожидание:" + refillDelay.ToString(), "");
        yield return new WaitForSeconds(0.001f);
        StartCoroutine(FillBoardCo());
    }
    

    //Board refilling method
    private void RefilBoard() 
    {
        for (int i = 0; i < width; i++) 
        {
            for (int j = 0; j < height; j++) 
            {
                if (allDots[i, j] == null && !blankSpaces[i, j] && !concreteTiles[i, j] && !chocolateTiles[i, j])
                {
                    Vector2 tempPosition = new Vector2(i, j + offset);
                    int dotToUse = Random.Range(0, dots.Length);
                    int maxIterations = 0;
                    while (MatchesAt(i, j, dots[dotToUse]) && maxIterations < 100)
                    {
                        maxIterations++;
                        dotToUse = Random.Range(0, dots.Length);
                    }
                    maxIterations = 0;
                    GameObject piece = Instantiate(dots[dotToUse], tempPosition, Quaternion.identity);
                    allDots[i, j] = piece;
                    piece.GetComponent<Dot>().row = j;
                    piece.GetComponent<Dot>().column = i;
                }
            }
        }
        //Debug.Log("End Refil Board, GameState = " + currentState);
    }

    // trouver des nouvaux match en board
    private bool MatchesOnBoard() 
    {
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                if (allDots[i, j] != null) 
                {
                    if (allDots[i, j].GetComponent<Dot>().isMatched) 
                    {
                        return true;
                    }
                }
            }
        }
        return false;
    }

    

    // Main methode for bord refilling
    private IEnumerator FillBoardCo() 
    {
        RefilBoard();
        yield return new WaitForSeconds(refillDelay);
        if (currentState != GameState.win) 
        {
            while (MatchesOnBoard())
            {
                streakValue++;
                yield return new WaitForSeconds(.7f);
                DestroyMatches();
                yield return new WaitForSeconds(refillDelay);
            }
        }
      
        findMatches.currentMatches.Clear();
        
        currentDot = null;
        yield return new WaitForSeconds(refillDelay);
        checkToMakeChoco();
        if (isDeadLocked()) 
        {
            ShuffleBoard();
            Debug.LogError("DeadLocked");
        }
        makeSlime = true;
        streakValue = 1;
        yield return new WaitForSeconds(1f);
        currentState = GameState.move; 
    }


    //verification, avant de creer chocolate 
    private void checkToMakeChoco() 
    {
        debugLog("checkToMakeChoco()", "------------");

        for (int i = 0; i < width; i++) 
        {
            for (int j = 0; j < height; j++) 
            {
                debugLog("Проверка makeSlime: ", "makeSlime = " + makeSlime.ToString());
                if (chocolateTiles[i, j] != null && makeSlime) 
                {
                    MakeNewSlime();
                    return;
                }
            }
        }

    }

    //methode qui definis direction de vector
    private Vector2 CheckForAdjacent(int column, int row) 
    {
        if (column < width - 1)
        {
            if (allDots[column + 1, row])
            {
                
                return Vector2.right;
            }
        }
        if (column < 0)
        {
            if (allDots[column - 1, row])
            {
                return Vector2.left;
            }
        }
        if (row < height - 1)
        {
            if (allDots[column, row + 1])
            {
                return Vector2.up;
            }
        }
        if (row < 0)
        {
            if (allDots[column, row - 1])
            {
                return Vector2.down;
            }
        }
        return Vector2.zero;
    }

    private void MakeNewSlime() 
    {
        bool slime = false;
        int maxIterations = 0;
        while (!slime && maxIterations < 10000)
        {
            int newX = Random.Range(0, width); // choisir place random pour spawn le slime(x)
            int newY = Random.Range(0, height); //  choisir place random pour spawn le slime(y)
            if (chocolateTiles[newX, newY]) 
            {
                Vector2 adjacent = CheckForAdjacent(newX, newY); 
                if (adjacent != Vector2.zero) 
                {
                    Destroy(allDots[newX + (int)adjacent.x, newY + (int)adjacent.y]); //  // destruction de fruit où on a besoin de spawn slime tile
                    Vector2 tempPosition = new Vector2(newX + (int)adjacent.x, newY + (int)adjacent.y); // nouvelle position de spawn de slime tile
                    GameObject tile = Instantiate(chocolateTilePrefab, tempPosition, Quaternion.identity); 
                    chocolateTiles[newX + (int)adjacent.x, newY + (int)adjacent.y] = tile.GetComponent<BackgroundTile>(); 
                    slime = true;
                    
                }
            }

            maxIterations++;
        }
    }

    private void SwitchPieces(int column, int row, Vector2 direction) 
    {
        debugLog("SwitchPieces()", "-----------");
        if (allDots[column + (int)direction.x, row + (int)direction.y] != null)
        {
            
            GameObject holder = allDots[column + (int)direction.x, row + (int)direction.y] as GameObject;

            allDots[column + (int)direction.x, row + (int)direction.y] = allDots[column, row];
            allDots[column, row] = holder;
        }
        //Debug.Log("End SwitchPieces, GameState = " + currentState);
    }

    private bool CheckForMatches() // verification de match
    {
        MatchList = new List<GameObject>();
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                if (allDots[i, j] != null) 
                {
                    if (i < width - 2) 
                    {
                        if (allDots[i + 1, j] != null && allDots[i + 2, j] != null)
                        {
                            if (allDots[i + 1, j].CompareTag(allDots[i, j].tag) && allDots[i + 2, j].CompareTag(allDots[i, j].tag))
                            {
                                debugLog("Match true", "");
                                MatchList.Add(allDots[i, j]);
                                MatchList.Add(allDots[i + 1, j]);
                                MatchList.Add(allDots[i + 2, j]);
                                /*Debug.Log(MatchList);
                                Debug.Log("column");
                                foreach (GameObject k in MatchList)
                                {
                                    Debug.Log(k);
                                }*/
                                return true;
                            }
                        }
                    }

                    if (j < height - 2) 
                    {
                        if (allDots[i, j + 1] != null && allDots[i, j + 2] != null)
                        {
                            if (allDots[i, j + 1].CompareTag(allDots[i, j].tag) && allDots[i, j + 2].CompareTag(allDots[i, j].tag))
                            {
                                //Debug.Log("CheckForMatches(), GameState = " + currentState);
                                MatchList.Add(allDots[i, j]);
                                MatchList.Add(allDots[i, j+1]);
                                MatchList.Add(allDots[i, j+2]);
                                /*Debug.Log(MatchList);
                                Debug.Log("row");
                                foreach (GameObject k in MatchList)
                                {
                                    Debug.Log(k);
                                }*/
                                return true;
                            }
                        }
                    }
                    
                    
                }
            }
        }
        //Debug.Log("CheckForMacthes(), GameState = " + currentState);
        MatchList.Clear();
        return false;
    }

    public bool SwitchAndCheck(int column, int row, Vector2 direction) 
    {
        SwitchPieces(column, row, direction);
        if (CheckForMatches())
        {
            SwitchPieces(column, row, direction);
            return true;
        }
        else 
        {
            SwitchPieces(column, row, direction);
            return false;
        }
    }

    private bool isDeadLocked() 
    {
        for (int i = 0; i < width; i++) 
        {
            for (int j = 0; j < height; j++)
            {
                if (allDots[i, j] != null) 
                {
                    if (i < width - 1) 
                    {
                        if (SwitchAndCheck(i, j, Vector2.right)) 
                        {
                            return false;
                        }
                    }

                    if (j < height- 1)
                    {
                        if (SwitchAndCheck(i, j, Vector2.up))
                        {
                            return false;
                        }
                    }
                }
            }
        }
        return true;
    }

    // une methode qui change l'eplacement des fruit si il n'y a pas de match possible
    private void ShuffleBoard() 
    {
        // cree une liste des objets
        List<GameObject> newBoard = new List<GameObject>();
        //ajouter chaque fruit de board dans cette liste
        for (int i = 0; i < width; i++) 
        {
            for (int j = 0; j < height; j++) 
            {
                if (allDots[i, j] != null) 
                {
                    newBoard.Add(allDots[i, j]);
                }
            }
        }

        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                if (!blankSpaces[i, j] && !concreteTiles[i, j] && !chocolateTiles[i, j])
                {
                    int pieceToUse = Random.Range(0, newBoard.Count);
                    // creation de container pour le fruit
                    Dot piece = newBoard[pieceToUse].GetComponent<Dot>();
                    piece.column = i;
                    piece.row = j;
                    allDots[i, j] = newBoard[pieceToUse];
                    newBoard.Remove(newBoard[pieceToUse]);
                }
            }
        }
        // Verification si board est deadLocked
        if (isDeadLocked()) 
        {
            ShuffleBoard();
        }
    }
}
