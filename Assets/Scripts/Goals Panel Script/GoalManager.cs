using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class BlankGoal 
{
    public int numberNeeded;
    public int numberCollected;
    public Sprite goalSprite;
    public string matchValue;
}

public class GoalManager : MonoBehaviour
{
    public BlankGoal[] levelGoals;
    public List<GoalPanel> currentGoals = new List<GoalPanel>();
    public GameObject goalPrefab;
    public GameObject goalIntroParent;
    public GameObject goalGameParent;
    private EndGameManager endGame;
    private Board board;
    private FindMatches findMatches;
    public bool WinState = false;
    void Start()
    {
        findMatches = FindObjectOfType<FindMatches>();
        board = FindObjectOfType<Board>();
        endGame = FindObjectOfType<EndGameManager>();
        GetGoals();
        SetupIntroGoals();
    }

    void GetGoals() 
    {
        if (board != null) 
        {
            if (board.world != null) 
            {
                if (board.level < board.world.levels.Length) 
                {
                    if (board.world.levels[board.level] != null)
                    {
                        levelGoals = board.world.levels[board.level].levelGoals;
                        for (int i = 0; i < levelGoals.Length; i++) 
                        {
                            levelGoals[i].numberCollected = 0;
                        }
                    }
                }
            }
        }
    }

    void SetupIntroGoals() 
    {
        for (int i = 0; i < levelGoals.Length; i++) 
        {
            
            GameObject goal = Instantiate(goalPrefab, goalIntroParent.transform.position, Quaternion.identity);
            goal.transform.SetParent(goalIntroParent.transform, false);

            GoalPanel panel = goal.GetComponent<GoalPanel>();
            panel.thisSprite = levelGoals[i].goalSprite;
            panel.thisString = "0/" + levelGoals[i].numberNeeded;

            GameObject gameGoal = Instantiate(goalPrefab, goalGameParent.transform.position, Quaternion.identity);
            gameGoal.transform.SetParent(goalGameParent.transform, false);
            panel = gameGoal.GetComponent<GoalPanel>();
            currentGoals.Add(panel);
            panel.thisSprite = levelGoals[i].goalSprite;
            panel.thisString = "0/" + levelGoals[i].numberNeeded;
        }

    }

    public void UpdateGoals() 
    {
        int goalsCompleted = 0;
        for (int i = 0; i < levelGoals.Length; i++) 
        {
            currentGoals[i].thisText.text = "" + levelGoals[i].numberCollected + "/" + levelGoals[i].numberNeeded;
            if (levelGoals[i].numberCollected >= levelGoals[i].numberNeeded) 
            {
                goalsCompleted++;
                currentGoals[i].thisText.text = "" + levelGoals[i].numberNeeded + "/" + levelGoals[i].numberNeeded;
            }
            if (goalsCompleted >= levelGoals.Length) 
            {
                /*Debug.Log("current Matches in UpdateGoals() = " + findMatches.currentMatches.Count);
                Debug.Log("board.current state in UpdateGoals() = " + board.currentState);*/
                if (endGame != null) 
                {
                    //Debug.Log("endGame.WinGame()");
                    WinState = true;
                    //endGame.WinGame();
                    //Debug.Log("You win");                   
                }
                
            }
        }
    }


    void Update()
    {
        
    }

    public void ComparedGoal(string goalToCompare) 
    {
        for (int i = 0; i < levelGoals.Length; i++) 
        {
            if (goalToCompare == levelGoals[i].matchValue) 
            {
                levelGoals[i].numberCollected++;
            }
        }
    }
}
