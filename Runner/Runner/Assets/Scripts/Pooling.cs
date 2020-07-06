using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Pooling : MonoBehaviour
{
    [SerializeField] GameObject staticBlockPrefab;
    [SerializeField] GameObject movingBlockPrefab;

    [SerializeField] GameObject[] collectablesPrefabs;


    List<GameObject> allStaticBlocks = new List<GameObject>();
    List<GameObject> allMovingBlocks = new List<GameObject>();

    List<List<GameObject>> allCollectables = new List<List<GameObject>>();

    [SerializeField] int amountOfStaticBlocksToInstantiateAtStartup = 100;
    [SerializeField] int amountOfMovingBlocksToInstantiateAtStartup = 30;
    [SerializeField] int amountOfCollectablesToInstantiateAtStartup = 30;

    [SerializeField] Transform activeObjectsParent;

    private void Awake()
    {
        FirstInstantiationOfStaticBlocks();
    }

    void FirstInstantiationOfStaticBlocks()
    {
        for (int i = 0; i < amountOfStaticBlocksToInstantiateAtStartup; i++)
        {
            GameObject newStaticBlock = Instantiate(staticBlockPrefab, transform.position, Quaternion.identity);
            allStaticBlocks.Add(newStaticBlock);
            newStaticBlock.transform.SetParent(transform);
            newStaticBlock.SetActive(false);
        }

        for (int i = 0; i < amountOfMovingBlocksToInstantiateAtStartup; i++)
        {
            GameObject newMovingBlock = Instantiate(movingBlockPrefab, transform.position, Quaternion.identity);
            allMovingBlocks.Add(newMovingBlock);
            newMovingBlock.transform.SetParent(transform);
            newMovingBlock.SetActive(false);
        }

        for (int i = 0; i < collectablesPrefabs.Length; i++)
        {
            allCollectables.Add(new List<GameObject>());
            for (int j = 0; j < amountOfCollectablesToInstantiateAtStartup; j++)
            {
                GameObject newCollectable = Instantiate(collectablesPrefabs[i], transform.position, Quaternion.identity);
                allCollectables[allCollectables.Count - 1].Add(newCollectable);
                newCollectable.transform.SetParent(transform);
                newCollectable.SetActive(false);
            }
        }
    }

    public GameObject InstantiateStaticBlock(Vector3 position, Quaternion rotation)
    {
        //return Instantiate(staticBlockPrefab, position, rotation);
        if(allStaticBlocks.Count == 0)
        {
            GameObject newStaticBlock = Instantiate(staticBlockPrefab, position, rotation);
            newStaticBlock.transform.SetParent(activeObjectsParent);
            return newStaticBlock;
        }
        else
        {
            GameObject newStaticBlock = allStaticBlocks[allStaticBlocks.Count - 1];
            newStaticBlock.SetActive(true);
            newStaticBlock.transform.SetParent(activeObjectsParent);
            newStaticBlock.transform.position = position;
            newStaticBlock.transform.rotation = rotation;
            allStaticBlocks.RemoveAt(allStaticBlocks.Count - 1);
            return newStaticBlock;
        }
    }

    public void DestroyStaticBlock(GameObject staticBlockToDestroy)
    {
        //Destroy(staticBlockToDestroy); return;
        allStaticBlocks.Add(staticBlockToDestroy);
        staticBlockToDestroy.transform.SetParent(transform);
        staticBlockToDestroy.SetActive(false);
    }


    public GameObject InstantiateMovingBlock(Vector3 position, Quaternion rotation)
    {
        //return Instantiate(staticBlockPrefab, position, rotation);
        if (allMovingBlocks.Count == 0)
        {
            GameObject newMovingBlock = Instantiate(movingBlockPrefab, position, rotation);
            newMovingBlock.transform.SetParent(activeObjectsParent);
            return newMovingBlock;
        }
        else
        {
            GameObject newMovingBlock = allMovingBlocks[allMovingBlocks.Count - 1];
            newMovingBlock.SetActive(true);
            newMovingBlock.transform.SetParent(activeObjectsParent);
            newMovingBlock.transform.position = position;
            newMovingBlock.transform.rotation = rotation;
            allMovingBlocks.RemoveAt(allMovingBlocks.Count - 1);
            return newMovingBlock;
        }
    }

    public void DestroyMovingBlock(GameObject movingBlockToDestroy)
    {
        //Destroy(movingBlockToDestroy); return;
        allMovingBlocks.Add(movingBlockToDestroy);
        movingBlockToDestroy.transform.SetParent(transform);
        movingBlockToDestroy.SetActive(false);
    }

    public GameObject InstantiateCollectable(Vector3 position, Quaternion rotation, int type)
    {
        if(type >= 0 && type < collectablesPrefabs.Length)
        {
            if(allCollectables[type].Count == 0)
            {
                GameObject newCollectable = Instantiate(collectablesPrefabs[type], position, rotation);
                newCollectable.transform.SetParent(activeObjectsParent);
                return newCollectable;
            }
            else
            {
                GameObject newCollectable = allCollectables[type][allCollectables[type].Count - 1];
                newCollectable.SetActive(true);
                newCollectable.transform.SetParent(activeObjectsParent);
                newCollectable.transform.position = position;
                newCollectable.transform.rotation = rotation;
                allCollectables[type].RemoveAt(allCollectables[type].Count - 1);
                return newCollectable;
            }
        }
        else
        {
            return null;
        }
    }

    public void DestroyCollectable(GameObject collectableToDestroy, int type)
    {
        if(type >= 0 && type < collectablesPrefabs.Length)
        {
            allCollectables[type].Add(collectableToDestroy);
            collectableToDestroy.transform.SetParent(transform);
            collectableToDestroy.SetActive(false);
        }
    }

}
