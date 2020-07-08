using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstacleManager : MonoBehaviour
{
    [SerializeField] GridScriptableObject[] allGrids;

    [SerializeField] GridScriptableObject tutorialGrid;

    float lastObstacleEndingPositionDown = 0f;
    float lastObstacleEndingPositionUp = 0f;

    public ObstaclesPosition obstaclesPosition = ObstaclesPosition.Both;

    [SerializeField] GameObject staticBlockPrefab;
    [SerializeField] GameObject movingBlockPrefab;

    [SerializeField] GameObject[] collectablesPrefabs;

    [SerializeField] Transform target;
    [SerializeField] float distanceAheadOfTarget = 30f;

    [SerializeField] Pooling pooling;

    [SerializeField] Material[] materials;

    bool alreadyStarted = false;
    void LoadAllGrids()
    {
        foreach (GridScriptableObject grid in allGrids)
        {
            grid.obstacleGrid = new int[grid.length, grid.width];
            grid.collectableGrid = new int[grid.length, grid.width];

            for (int i = 0; i < grid.length; i++)
            {
                for (int j = 0; j < grid.width; j++)
                {
                    grid.obstacleGrid[i, j] = grid.auxObstacleGrid[i * grid.width + j];
                    grid.collectableGrid[i, j] = grid.auxCollectableGrid[i * grid.width + j];
                }
            }
        }


        tutorialGrid.obstacleGrid = new int[tutorialGrid.length, tutorialGrid.width];
        tutorialGrid.collectableGrid = new int[tutorialGrid.length, tutorialGrid.width];

        for (int i = 0; i < tutorialGrid.length; i++)
        {
            for (int j = 0; j < tutorialGrid.width; j++)
            {
                tutorialGrid.obstacleGrid[i, j] = tutorialGrid.auxObstacleGrid[i * tutorialGrid.width + j];
                tutorialGrid.collectableGrid[i, j] = tutorialGrid.auxCollectableGrid[i * tutorialGrid.width + j];
            }
        }

    }

    public void ResetObstacleManager()
    {
        lastObstacleEndingPositionDown = 0f;
        lastObstacleEndingPositionUp = 0f;

        //if (obstaclesPosition == ObstaclesPosition.Down || obstaclesPosition == ObstaclesPosition.Both)
        //{
        //    PutSetOfObstacles(1, 0);
        //}

        //if (obstaclesPosition == ObstaclesPosition.Up || obstaclesPosition == ObstaclesPosition.Both)
        //{
        //    PutSetOfObstacles(-1, 0);
        //}

        alreadyStarted = false;
    }

    private void Awake()
    {
        LoadAllGrids();
    }

    float blockWidth = 1f;
    float blockLength = 1f;
    float blockHeight = 0.5f;


    [SerializeField] float heightOfUpperObstacles = 5f;

    [SerializeField] int numberOfTimesToShowTutorial = 3;

    [SerializeField] bool showTutorialIfNeeded = false;
    public void StartObstacleManager()
    {
        if (!alreadyStarted)
        {
            alreadyStarted = true;
            int numberOfTimeTutorialWasShown = PlayerPrefs.GetInt("tutorialNumberOfTimes", 0);
            //if (numberOfTimeTutorialWasShown == -10)
            //{
            //    PlayerPrefs.SetInt("tutorialNumberOfTimes", 1);
            //    numberOfTimeTutorialWasShown = 0;
            //}
            //else
            //{
            //    PlayerPrefs.SetInt("tutorialNumberOfTimes", numberOfTimeTutorialWasShown + 1);
            //}


            if (numberOfTimeTutorialWasShown < numberOfTimesToShowTutorial && showTutorialIfNeeded)
            {
                Debug.Log("With tutorial");
                PlayerPrefs.SetInt("tutorialNumberOfTimes", numberOfTimeTutorialWasShown + 1);

                if (obstaclesPosition == ObstaclesPosition.Down || obstaclesPosition == ObstaclesPosition.Both)
                {
                    PutTutorialObstacles(1);
                }

                if (obstaclesPosition == ObstaclesPosition.Up || obstaclesPosition == ObstaclesPosition.Both)
                {
                    PutTutorialObstacles(-1);
                }

                SetTutorial();
            }
            else
            {
                Debug.Log("Without tutorial");
                if (obstaclesPosition == ObstaclesPosition.Down || obstaclesPosition == ObstaclesPosition.Both)
                {
                    PutSetOfObstacles(1, 0);
                }

                if (obstaclesPosition == ObstaclesPosition.Up || obstaclesPosition == ObstaclesPosition.Both)
                {
                    PutSetOfObstacles(-1, 0);
                }
            }
        }
    }

    void SetTutorial()
    {

    }

    private void Start()
    {
        StartObstacleManager();

    }
    private void Update()
    {
        if (alreadyStarted)
        {
            if (obstaclesPosition == ObstaclesPosition.Down || obstaclesPosition == ObstaclesPosition.Both)
            {
                if (target.position.z + distanceAheadOfTarget >= lastObstacleEndingPositionDown)
                {
                    PutSetOfObstacles();
                }
            }

            if (obstaclesPosition == ObstaclesPosition.Up || obstaclesPosition == ObstaclesPosition.Both)
            {
                if (target.position.z + distanceAheadOfTarget >= lastObstacleEndingPositionUp)
                {
                    PutSetOfObstacles(-1);
                }
            }
        } 
    }

    //List<MovingBlock> setOfMovingBlocks;
    //List<int> directions;
    void PutSetOfObstacles(int directionMultiplier = 1, int selectedGrid = -1)
    {
        if(selectedGrid == -1) selectedGrid = Random.Range(0, allGrids.Length);
        float nextObstacleEndingPosition = (directionMultiplier == 1 ? lastObstacleEndingPositionDown : (directionMultiplier == -1 ? lastObstacleEndingPositionUp : transform.position.z + distanceAheadOfTarget)) + allGrids[selectedGrid].length * blockLength;

        //setOfMovingBlocks = new List<MovingBlock>();
        //directions = new List<int>();

        for (int i = 0; i < allGrids[selectedGrid].length; i++)
        {
            for (int j = 0; j < allGrids[selectedGrid].width; j++)
            {
                Vector3 positionToInstantiate = new Vector3(transform.position.x + j * blockWidth, transform.position.y, transform.position.z + nextObstacleEndingPosition - i * blockLength);
                if (directionMultiplier == -1) positionToInstantiate.y += heightOfUpperObstacles;
                InstantiateObstacle(positionToInstantiate, allGrids[selectedGrid].obstacleGrid[i, j], allGrids[selectedGrid].collectableGrid[i, j], directionMultiplier);
            }
        }

        //SetupThisSetOfMovingBlocks();

        if (directionMultiplier == 1)
        {
            lastObstacleEndingPositionDown = nextObstacleEndingPosition;
        }else if(directionMultiplier == -1)
        {
            lastObstacleEndingPositionUp = nextObstacleEndingPosition;
        }
        
    }

    //void SetupThisSetOfMovingBlocks()
    //{
    //    if (setOfMovingBlocks.Count == 0) return;

    //    for (int i = 0; i < setOfMovingBlocks.Count; i++)
    //    {
    //        setOfMovingBlocks[i].Setup(directions[i]);
    //    }

    //}

    void PutTutorialObstacles(int directionMultiplier = 1)
    {
        float nextObstacleEndingPosition = (directionMultiplier == 1 ? lastObstacleEndingPositionDown : (directionMultiplier == -1 ? lastObstacleEndingPositionUp : transform.position.z + distanceAheadOfTarget)) + tutorialGrid.length * blockLength;

        for (int j = 0; j < tutorialGrid.width; j++) 
        {
            for (int i = 0; i < tutorialGrid.length; i++)
            {
                Vector3 positionToInstantiate = new Vector3(transform.position.x + j * blockWidth, transform.position.y, transform.position.z + nextObstacleEndingPosition - i * blockLength);
                if (directionMultiplier == -1) positionToInstantiate.y += heightOfUpperObstacles;
                InstantiateObstacle(positionToInstantiate, tutorialGrid.obstacleGrid[i, j], tutorialGrid.collectableGrid[i, j], directionMultiplier);
            }
        }

        if (directionMultiplier == 1)
        {
            lastObstacleEndingPositionDown = nextObstacleEndingPosition;
        }
        else if (directionMultiplier == -1)
        {
            lastObstacleEndingPositionUp = nextObstacleEndingPosition;
        }
    }

    void InstantiateObstacle(Vector3 position, int type = 1, int collectableType = 0, int directionMultiplier = 1)
    {
        float collectableHeight = blockHeight;
        switch (type)
        {
            case 0:
                //empty space
                collectableHeight = 2 * blockHeight;
                break;

            case 1:
                //basic floor
                //Instantiate(staticBlockPrefab, position, Quaternion.identity);
                pooling.InstantiateStaticBlock(position, Quaternion.identity);
                collectableHeight = blockHeight;
                break;

            case 2:
                //basic floor with simple obstacle on top of it
                //Instantiate(staticBlockPrefab, position, Quaternion.identity);
                //Instantiate(staticBlockPrefab, position + Vector3.up * blockHeight * directionMultiplier, Quaternion.identity);
                pooling.InstantiateStaticBlock(position, Quaternion.identity);
                pooling.InstantiateStaticBlock(position + Vector3.up * blockHeight * directionMultiplier, Quaternion.identity);
                collectableHeight = 2 * blockHeight;
                break;

            case 3:
                //basic floor with flying obstacle above it
                //Instantiate(staticBlockPrefab, position, Quaternion.identity);
                //Instantiate(staticBlockPrefab, position + Vector3.up * blockHeight * 2 * directionMultiplier, Quaternion.identity);
                pooling.InstantiateStaticBlock(position, Quaternion.identity);
                pooling.InstantiateStaticBlock(position + Vector3.up * blockHeight * 2 * directionMultiplier, Quaternion.identity);
                collectableHeight = blockHeight;
                break;

            case 4:
                //basic floor with simple obstacle on top of it and with flying obstacle above the obstacle
                //Instantiate(staticBlockPrefab, position, Quaternion.identity);
                //Instantiate(staticBlockPrefab, position + Vector3.up * blockHeight * directionMultiplier, Quaternion.identity);
                //Instantiate(staticBlockPrefab, position + Vector3.up * blockHeight * 3 * directionMultiplier, Quaternion.identity);
                pooling.InstantiateStaticBlock(position, Quaternion.identity);
                pooling.InstantiateStaticBlock(position + Vector3.up * blockHeight * directionMultiplier, Quaternion.identity);
                pooling.InstantiateStaticBlock(position + Vector3.up * blockHeight * 3 * directionMultiplier, Quaternion.identity);
                collectableHeight = 2 * blockHeight;
                break;

            case 5:
                //basic floor with simple obstacle on top of it and with another simple obstacle on top of the first one
                //Instantiate(staticBlockPrefab, position, Quaternion.identity);
                //Instantiate(staticBlockPrefab, position + Vector3.up * blockHeight * directionMultiplier, Quaternion.identity);
                //Instantiate(staticBlockPrefab, position + Vector3.up * blockHeight * 2 * directionMultiplier, Quaternion.identity);
                pooling.InstantiateStaticBlock(position, Quaternion.identity);
                pooling.InstantiateStaticBlock(position + Vector3.up * blockHeight * directionMultiplier, Quaternion.identity);
                pooling.InstantiateStaticBlock(position + Vector3.up * blockHeight * 2 * directionMultiplier, Quaternion.identity);
                collectableHeight = 3 * blockHeight;
                break;

            case 6:
                //basic floor with moving obstacle on top of it
                //Instantiate(staticBlockPrefab, position, Quaternion.identity);
                //Instantiate(movingBlockPrefab, position + Vector3.up * blockHeight * directionMultiplier, Quaternion.identity);
                pooling.InstantiateStaticBlock(position, Quaternion.identity);
                /*GameObject moving = */pooling.InstantiateMovingBlock(position + Vector3.up * blockHeight * directionMultiplier, Quaternion.identity, directionMultiplier, 1);
                //setOfMovingBlocks.Add(moving.GetComponent<MovingBlock>());
                //directions.Add(directionMultiplier);
                collectableHeight = 2 * blockHeight;
                break;

            case 7:
                //basic floor with moving obstacle on top of it
                //Instantiate(staticBlockPrefab, position, Quaternion.identity);
                //Instantiate(movingBlockPrefab, position + Vector3.up * blockHeight * directionMultiplier, Quaternion.identity);
                pooling.InstantiateStaticBlock(position, Quaternion.identity);
                /*GameObject moving = */pooling.InstantiateMovingBlock(position + Vector3.up * blockHeight * directionMultiplier, Quaternion.identity, directionMultiplier, -1);
                //setOfMovingBlocks.Add(moving.GetComponent<MovingBlock>());
                //directions.Add(directionMultiplier);
                collectableHeight = 2 * blockHeight;
                break;

            default:
                //Instantiate(staticBlockPrefab, position, Quaternion.identity);
                pooling.InstantiateStaticBlock(position, Quaternion.identity);
                collectableHeight = blockHeight;
                break;
        }

        InstantiateCollectable(position + Vector3.up * collectableHeight * directionMultiplier, collectableType);
    }

    [SerializeField] Vector3 collectableRotation;
    void InstantiateCollectable(Vector3 position, int collectableType)
    {
        if (collectableType == 0) return;
        //Debug.Log("collectableType: " + collectableType);
        //Instantiate(collectablesPrefabs[collectableType - 1], position, Quaternion.Euler(collectableRotation));
        pooling.InstantiateCollectable(position, Quaternion.Euler(collectableRotation), collectableType - 1);
    }
}

public enum ObstaclesPosition
{
    Down,
    Up, 
    Both
}
