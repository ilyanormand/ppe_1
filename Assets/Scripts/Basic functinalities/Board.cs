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
    public int width, offset, height;

    [Header("Prefabs")]
    public GameObject explosionEffect, breakableTilePrefab, tilePrefab, lockTilePrefab, concreteTilePrefab, chocolateTilePrefab;
    public GameObject[] dots;// массив где будут храниться элементы игры матч 3

    [Header("Layout")]
    //private BackgroundTile[,] allTiles; // пустой массив для хранение плиток
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

    private void Awake()
    {
        if (PlayerPrefs.HasKey("Current Level"))
        {
            level = PlayerPrefs.GetInt("Current Level");
        }
        if (world != null)
        {
            if (level < world.levels.Length)
            {
                if (world.levels[level] != null) // проверка на существование уровня
                {
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
        //allTiles = new BackgroundTile[width, height]; // заполняем массив хранения плиток
        blankSpaces = new bool[width, height];
        allDots = new GameObject[width, height];
        SetUp();
        currentState = GameState.pause;
    }

    //Генерация пустых  пространств
    public void GeneratingBlankSpaces()
    {
        for (int i = 0; i < boardLayout.Length; i++)
        {
            if (boardLayout[i].tileKind == TileKind.Blank)
            {
                blankSpaces[boardLayout[i].x, boardLayout[i].y] = true;
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

    public void GenerateBreakableTiles() 
    {
        //Перебрать все плитки на экране
        for (int i = 0; i < boardLayout.Length; i++) 
        {
            //если плитка является "ломаемой" плиткой
            if (boardLayout[i].tileKind == TileKind.Breakable) 
            {
                // Создать ломаемую плитку в данной позиции
                Vector2 tempPosition = new Vector2(boardLayout[i].x, boardLayout[i].y);
                GameObject tile = Instantiate(breakableTilePrefab, tempPosition, Quaternion.identity);
                breakableTiles[boardLayout[i].x, boardLayout[i].y] = tile.GetComponent<BackgroundTile>();
            }
        }
    }

    private void GeneratLockTiles() 
    {
        for (int i = 0; i < boardLayout.Length; i++)
        {
            //если плитка является "ломаемой" плиткой
            if (boardLayout[i].tileKind == TileKind.Lock)
            {
                // Создать ломаемую плитку в данной позиции
                Vector2 tempPosition = new Vector2(boardLayout[i].x, boardLayout[i].y);
                GameObject tile = Instantiate(lockTilePrefab, tempPosition, Quaternion.identity);
                lockTiles[boardLayout[i].x, boardLayout[i].y] = tile.GetComponent<BackgroundTile>();
            }
        }
    }
    private void GeneratConcreteTiles()
    {
        for (int i = 0; i < boardLayout.Length; i++)
        {
            //если плитка является "ломаемой" плиткой
            if (boardLayout[i].tileKind == TileKind.Concrete)
            {
                // Создать ломаемую плитку в данной позиции
                Vector2 tempPosition = new Vector2(boardLayout[i].x, boardLayout[i].y);
                GameObject tile = Instantiate(concreteTilePrefab, tempPosition, Quaternion.identity);
                concreteTiles[boardLayout[i].x, boardLayout[i].y] = tile.GetComponent<BackgroundTile>();
            }
        }
    }

    private void GeneratChocolateTiles()
    {
        for (int i = 0; i < boardLayout.Length; i++)
        {
            //если плитка является "ломаемой" плиткой
            if (boardLayout[i].tileKind == TileKind.Chocolate)
            {
                // Создать ломаемую плитку в данной позиции
                Vector2 tempPosition = new Vector2(boardLayout[i].x, boardLayout[i].y);
                GameObject tile = Instantiate(chocolateTilePrefab, tempPosition, Quaternion.identity);
                chocolateTiles[boardLayout[i].x, boardLayout[i].y] = tile.GetComponent<BackgroundTile>();
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

    //Генерация плиток на основе массива alltiles
    private void SetUp() 
    {
        GeneratingBlankSpaces();
        GenerateBreakableTiles();
        GeneratLockTiles();
        GeneratConcreteTiles();
        GeneratChocolateTiles();
        for (int i = 0; i < width; i++) 
        {
            // на каждую плитку по x мы генерируем все плитки по y
            for (int j = 0; j < height; j++) 
            {
                if (!blankSpaces[i, j] && !concreteTiles[i, j] && !chocolateTiles[i, j]) 
                {
                    Vector2 tempPosition = new Vector2(i, j + offset); // определение позиции для плитки
                    Vector2 tilePosition = new Vector2(i, j);

                    // создание обьекта в самой сцене(Quaternion.identity берет изначальную ротацию префаба)
                    GameObject backgroundTile = Instantiate(tilePrefab, tilePosition, Quaternion.identity) as GameObject;


                    backgroundTile.transform.parent = this.transform;  // Помещаем обьект bakcgroundTile в Board(this в данном случае это Board)
                    backgroundTile.name = "(" + i + ", " + j + ")"; // даем имя обьекту

                    int dotToUse = Random.Range(0, dots.Length); // Генерируем рандомное число между нулем передлом массива dots
                    int maxIteration = 0;
                    while (MatchesAt(i, j, dots[dotToUse]) && maxIteration < 100)
                    {
                        dotToUse = Random.Range(0, dots.Length);
                        maxIteration++;
                    }

                    maxIteration = 0;
                    // создаем обьект dot выбирая случайный элемент из массива dots
                    GameObject dot = Instantiate(dots[dotToUse], tempPosition, Quaternion.identity);
                    dot.GetComponent<Dot>().row = j;
                    dot.GetComponent<Dot>().column = i;
                    dot.transform.parent = this.transform; // помощение элемента в плитку
                    dot.name = "(" + i + ", " + j + ")";  // задаем такое же имя как и у плитки
                    allDots[i, j] = dot;
                }
            }
        }
    }


    // проверка матчей по краям
    private bool MatchesAt(int column, int row, GameObject piece) 
    {
        if (column > 1 && row > 1) 
        {
            if (allDots[column - 1, row] != null && allDots[column - 2, row] != null)
            {
                if (allDots[column - 1, row].tag == piece.tag && allDots[column - 2, row])// если элемент справа и второй элемент справа равны основному обьекту то выйгрыш
                {
                    return true;
                }
            }
            if (allDots[column, row - 1] != null && allDots[column, row - 2] != null) 
            {
                if (allDots[column, row - 1].tag == piece.tag && allDots[column, row - 2])// если элемент снизу и второй элемент снизу равны основному обьекту то выйгрыш
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
                        if (allDots[column, row - 1].tag == piece.tag && allDots[column, row - 2].tag == piece.tag)
                        {
                            return true;
                        }
                    }
                    
                }
                if (column > 1)
                {
                    if (allDots[column-1, row] != null && allDots[column-2, row] != null) 
                    {
                        if (allDots[column - 1, row].tag == piece.tag && allDots[column - 2, row].tag == piece.tag)
                        {
                            return true;
                        }
                    }
                    
                }
            }
        }

        return false;
    }

    private int ColumnOrRow()  // Это функция определяет какой бедт тип матча если
    {
        //Make a copy of the current matches
        List<GameObject> matchCopy = findMatches.currentMatches as List<GameObject>;

        // Cycle through all of match copy and decide if a bomb needs to be made
        for (int i = 0; i < matchCopy.Count; i++) 
        {
            Dot thisDot = matchCopy[i].GetComponent<Dot>();
            int column = thisDot.column;
            int row = thisDot.row;
            int columnMatch = 0;
            int rowMatch = 0;

            // Cycle throgh the rest of the pieces and compare
            for (int j = 0; j < matchCopy.Count; j++) 
            {
                Dot nextDot = matchCopy[j].GetComponent<Dot>();
                if (nextDot == thisDot)
                {
                    continue;
                }
                if (nextDot.column == thisDot.column && nextDot.CompareTag(thisDot.tag)) 
                {
                    columnMatch++;
                }
                if (nextDot.row == thisDot.row && nextDot.CompareTag(thisDot.tag)) 
                {
                    rowMatch++;
                }
            }
            //return 3 of column or row match
            //return 2 if adjacent bomb
            //return 1 if color bomb
            if (columnMatch == 4 || rowMatch == 4) 
            {
                return 1;
            }
            if (columnMatch == 2 || rowMatch == 2)
            {
                return 2;
            }
            if (columnMatch == 3 || rowMatch == 3) 
            {
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
        //How many objects are in findMatches currentMatches?
        if (findMatches.currentMatches.Count > 3)
        {
            //What type of match
            int typeOfMatch = ColumnOrRow();
            if (typeOfMatch == 1)
            {
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
    // уничтожение заматченых элементов
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
    private void DestroyMatchesAt(int column, int row) 
    {
        if (allDots[column, row].GetComponent<Dot>().isMatched) 
        {
            //узнать сколько заматченых элементов
            if (findMatches.currentMatches.Count >= 4) 
            {
                CheckToMakeBombs();
            }

            // проверка на необходимость уничтожение плитки

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

            if (soundManager != null) 
            {
                soundManager.playDestroyNoise();
            }
            GameObject particle = Instantiate(explosionEffect, allDots[column, row].transform.position, Quaternion.identity);
            Destroy(particle, .2f);
            Destroy(allDots[column, row]);
            scoreManager.InreaseScore(basePieceScoreValue);
            allDots[column, row] = null;
        }
    }
    // перебор всех элементов для того чтобы уничтожить матчи
    public void DestroyMatches()
    {
        for (int i = 0; i < width; i++) 
        {
            for (int j = 0; j < height; j++) 
            {
                if (allDots[i, j] != null) 
                {
                    DestroyMatchesAt(i, j);
                }
            }
        }
        findMatches.currentMatches.Clear();
        StartCoroutine(DecreaseRowCo2());
    }
    // сдвиг элементов при уничтожение матча
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
    private IEnumerator DecreaseRowCo2() 
    {
        for (int i = 0; i < width; i++) 
        {
            for (int j = 0; j < height; j++)
            {
                //Проверка на игровые плитки
                if (!blankSpaces[i, j] && allDots[i, j] == null && !concreteTiles[i, j] && !chocolateTiles[i, j]) 
                {
                    for (int k = j + 1; k < height; k++) 
                    {
                        // если элемент найден
                         if (allDots[i, k] != null) 
                         {
                            //Передвинуть данный желмент в пустое пространство
                            allDots[i, k].GetComponent<Dot>().row = j;
                            allDots[i, k] = null;
                            break;
                         }
                    }
                }
            }
        }
        yield return new WaitForSeconds(refillDelay * 0.5f);
        StartCoroutine(FillBoardCo());
    }
    private IEnumerator DecreaseRowCo() 
    {
        int nullcount = 0;
        for (int i = 0; i < width; i++) 
        {
            for (int j = 0; j < height; j++) 
            {
                if (allDots[i, j] == null) // если отсутсвует эелмент
                {
                    nullcount++; 
                }
                else if (nullcount > 0) // если отстутсвуещех элемнтов больше чем 0 то сдвигаме ряд вниз на то количество отстувущих элементов
                {
                    allDots[i, j].GetComponent<Dot>().row -= nullcount;
                    allDots[i, j] = null;
                }
            }
            nullcount = 0; 
        }
        yield return new WaitForSeconds(refillDelay * 0.5f); // пауза в 0.4 секунды
        StartCoroutine(FillBoardCo());
    }

    //Заполнение таблицы
    private void RefilBoard() 
    {
        for (int i = 0; i < width; i++) 
        {
            for (int j = 0; j < height; j++) 
            {
                if (allDots[i, j] == null && !blankSpaces[i, j] && !concreteTiles[i, j] && !chocolateTiles[i, j])  // элемент массива равен null то рандомно сгенироввать новый элемент
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
    }

    // нахождение новыых матчей
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

    

    // Основная функция заполнения таблицы 
    private IEnumerator FillBoardCo() 
    {
        yield return new WaitForSeconds(refillDelay);
        RefilBoard();
        while (MatchesOnBoard()) 
        {
            streakValue++;
            DestroyMatches();
            yield return new WaitForSeconds(refillDelay);
        }
        findMatches.currentMatches.Clear();
        currentDot = null;
        checkToMakeChoco();
        yield return new WaitForSeconds(refillDelay);
        if (isDeadLocked()) 
        {
            ShuffleBoard();
            Debug.LogError("DeadLocked");
        }
        makeSlime = true;
        currentState = GameState.move;
        streakValue = 1;
    }

    private void checkToMakeChoco() 
    {
        //check the chocolate tiles array
        for (int i = 0; i < width; i++) 
        {
            for (int j = 0; j < height; j++) 
            {
                if (chocolateTiles[i, j] != null && makeSlime) 
                {
                    //call another method to make a new slime
                    MakeNewSlime();
                    return;
                }
            }
        }

    }

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
            int newX = Random.Range(0, width); // choose a random spot to spawn a slime coordinate x
            int newY = Random.Range(0, height); // choose a random spot to spawn a slime coordinate y 

            if (chocolateTiles[newX, newY]) // check if the random spot it the slime on the board 
            {
                Vector2 adjacent = CheckForAdjacent(newX, newY); // check if new position where we need to spawn it's not a slime tile or another type of tile
                if (adjacent != Vector2.zero) 
                {
                    Destroy(allDots[newX + (int)adjacent.x, newY + (int)adjacent.y]); // destroy the fruit where we need to spawn a slime tile
                    Vector2 tempPosition = new Vector2(newX + (int)adjacent.x, newY + (int)adjacent.y); // new postion of the spawn of slime tile
                    GameObject tile = Instantiate(chocolateTilePrefab, tempPosition, Quaternion.identity); // creatuing a new gameobject slime tile on the scene
                    chocolateTiles[newX + (int)adjacent.x, newY + (int)adjacent.y] = tile.GetComponent<BackgroundTile>(); // adding new slime tile to the array of slime tiles
                    slime = true;
                    
                }
            }

            maxIterations++;
        }
    }

    private void SwitchPieces(int column, int row, Vector2 direction) // поменять элементы местами
    {
        if (allDots[column + (int)direction.x, row + (int)direction.y] != null)
        {
            //Взять второй элемент и сохранить его в холдер
            GameObject holder = allDots[column + (int)direction.x, row + (int)direction.y] as GameObject;

            allDots[column + (int)direction.x, row + (int)direction.y] = allDots[column, row];
            allDots[column, row] = holder;
        }

    }

    private bool CheckForMatches() // проверка на матч
    {
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
                            if (allDots[i + 1, j].tag == allDots[i, j].tag && allDots[i + 2, j].tag == allDots[i, j].tag)
                            {
                                return true;
                            }
                        }
                    }

                    if (j < height - 2) 
                    {
                        if (allDots[i, j + 1] != null && allDots[i, j + 2] != null)
                        {
                            if (allDots[i, j + 1].tag == allDots[i, j].tag && allDots[i, j + 2].tag == allDots[i, j].tag)
                            {
                                return true;
                            }
                        }
                    }
                    
                    
                }
            }
        }

        return false;
    }

    public bool SwitchAndCheck(int column, int row, Vector2 direction) // замена местами и проверка на матч
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

    private void ShuffleBoard() 
    {
        // создать лист из обьектов
        List<GameObject> newBoard = new List<GameObject>();
        //добавить какждый элемент таблицы в этот лист
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
                    // соддаем контейнер для элемента
                    Dot piece = newBoard[pieceToUse].GetComponent<Dot>();
                    piece.column = i;
                    piece.row = j;
                    // заполнить массив из элементов таблицы с этим новым элементом
                    allDots[i, j] = newBoard[pieceToUse];
                    //Убрать этот элемент в массиве newBoard
                    newBoard.Remove(newBoard[pieceToUse]);
                }
            }
        }
        // проверка на deadlock
        if (isDeadLocked()) 
        {
            ShuffleBoard();
        }
    }
}
