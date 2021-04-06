using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "World", menuName = "Level")]

public class Level : ScriptableObject 
{
    [Header("Board Dimensions")]
    public int width, height;

    [Header("Starting tiles")]
    public TileType[] boardLayout;

    [Header("Available Dots")]
    public GameObject[] dots;

    [Header("Score Goals")]
    public int[] scoreGoals;

    [Header("End Game Requierments")]
    public EndGameRequierments endGameRequierments;
    public BlankGoal[] levelGoals;
}
