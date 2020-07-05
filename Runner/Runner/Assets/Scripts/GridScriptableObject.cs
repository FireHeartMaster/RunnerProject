using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "GridData", menuName = "ScriptableObjects/Obstacle Data", order = 1)]
[System.Serializable]
public class GridScriptableObject : ScriptableObject
{
    public int numberOfBlockTypes = 5;
    public int numberOfGrabbableTypes = 5;
    public int[,] obstacleGrid = new int[5, 5];
    public int[,] collectableGrid = new int[5, 5];

    //auxiliary variables for serialization
    public int[] auxObstacleGrid = new int[25];
    public int[] auxCollectableGrid = new int[25];

    public int length = 5;
    public int width = 5;

}
