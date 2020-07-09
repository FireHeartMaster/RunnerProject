using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Eraser : MonoBehaviour
{
    [SerializeField] Pooling pooling;
    private void OnTriggerEnter(Collider other)
    {
        if(other.GetComponent<MovingBlock>() != null)
        {
            //Destroy(other.gameObject);
            pooling.DestroyMovingBlock(other.gameObject);
        }
        else if (other.GetComponent<Collectable>() != null)
        {            
            //Destroy(other.gameObject);
            pooling.DestroyCollectable(other.gameObject, other.GetComponent<Collectable>().type);
        }
        else
        {
            pooling.DestroyStaticBlock(other.gameObject);
            //Destroy(other.gameObject);
        }


    }
}
