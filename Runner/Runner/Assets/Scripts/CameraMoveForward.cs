using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMoveForward : MonoBehaviour
{
    [SerializeField] float speed = 5f;

    [SerializeField] Vector3 direction = Vector3.forward;

    public bool canMove = true;

    private void LateUpdate()
    {
        if(canMove)
            transform.position += direction * speed * Time.deltaTime;
    }
}
