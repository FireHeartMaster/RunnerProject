using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerFall : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        PlayerStats stats = other.GetComponent<PlayerStats>();

        if(stats != null)
        {
            Debug.Log("Player lost from falling");
            stats.isAlive = false;
            stats.HandleDeath();
        }
    }
}
