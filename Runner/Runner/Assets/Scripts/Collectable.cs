using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Collectable : MonoBehaviour
{
    public int type = 0;

    public int amount0fPointsToAdd = 1;



    private void OnTriggerEnter(Collider other)
    {
        PlayerStats stats = other.GetComponent<PlayerStats>();

        if(stats != null)
        {
            stats.amountOfPoints += amount0fPointsToAdd;

            Pooling.pooling.DestroyCollectable(gameObject, type);
        }
    }
}
