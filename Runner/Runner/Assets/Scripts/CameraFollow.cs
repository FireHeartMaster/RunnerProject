using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [SerializeField] Transform target;

    [HideInInspector] public Vector3 offset;

    private void Awake()
    {
        offset = target.position - transform.position;
        //offset.x = 0f;
        //offset.y = 0f;
    }

    private void LateUpdate()
    {

        Vector3 newPosition = transform.position;
        newPosition.z = target.position.z - offset.z;
        transform.position = newPosition;
    }
}
