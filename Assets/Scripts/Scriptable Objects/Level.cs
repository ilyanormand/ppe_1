using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "World", menuName = "Level")]
/*A ScriptableObject is a data container that you can use to save large amounts of data, independent of class instances. 
One of the main use cases for ScriptableObjects is to reduce your Project’s memory usage 
by avoiding copies of values.This is useful if your Project has a Prefab
that stores unchanging data in attached MonoBehaviour scripts
.*/
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
