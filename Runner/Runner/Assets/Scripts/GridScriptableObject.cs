using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "GridData", menuName = "ScriptableObjects/Obstacle Data", order = 1)]
public class GridScriptableObject : ScriptableObject
{
    public int numberOfBlockTypes = 5;
    public int numberOfGrabbableTypes = 5;
    public int[,] obstacleGrid = new int[5, 5];
    public int[,] collectableGrid = new int[5, 5];




}
