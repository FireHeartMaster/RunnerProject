using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [SerializeField] Transform target;

    [HideInInspector] public Vector3 offset;

    public bool cameraCanFollow = false;


    Vector3 initialPosition;
    public void CameraCanFollow()
    {
        cameraCanFollow = true;

        offset = target.position - transform.position;

        initialPosition = transform.position;
    }

    private void Awake()
    {
        //offset = target.position - transform.position;
        //offset.x = 0f;
        //offset.y = 0f;
    }

    public void CameraFollowReset()
    {
        transform.position = initialPosition;
        //CameraCanFollow();
    }

    private void LateUpdate()
    {
        if (cameraCanFollow)
        {
            Vector3 newPosition = transform.position;
            newPosition.z = target.position.z - offset.z;
            transform.position = newPosition;
        }
    }
}
