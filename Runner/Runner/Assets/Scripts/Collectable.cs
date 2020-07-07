using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Collectable : MonoBehaviour
{
    public int type = 0;

    public int amount0fPointsToAdd = 1;

    [SerializeField] GameObject particlesWhenGrabbedPrefab;


    private void OnTriggerEnter(Collider other)
    {
        PlayerStats stats = other.GetComponent<PlayerStats>();

        if(stats != null)
        {
            stats.AmountOfPoints += amount0fPointsToAdd;

            GameObject particles = Instantiate(particlesWhenGrabbedPrefab, transform.position, Quaternion.Euler(0f, 0f, Random.Range(0f, 180f)));
            Destroy(particles, 2f);

            Pooling.pooling.DestroyCollectable(gameObject, type);
        }
    }
}
