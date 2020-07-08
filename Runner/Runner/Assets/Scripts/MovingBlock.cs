using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingBlock : MonoBehaviour
{
    Vector3 initialPosition;
    Vector3 finalPosition;
    [SerializeField] float movingBlockSpeed = 1f;
    bool alreadySetup = false;

    [SerializeField] float blockWidth = 1f;
    [SerializeField] float blockHeight = 0.5f;
    [SerializeField] LayerMask layerMask;

    private void OnDisable()
    {
        alreadySetup = false;
    }

    IEnumerator delayedSetupCoroutine;

    int directionMultiplier = 1;
    [SerializeField] float minDistance = 0.05f;

    private void OnEnable()
    {
        //if(delayedSetupCoroutine != null)
        //{
        //    StopCoroutine(delayedSetupCoroutine);
        //}
        //delayedSetupCoroutine = DelayedSetup();

    }

    IEnumerator DelayedSetup()
    {
        yield return new WaitForSeconds(0.05f);
        Setup();
    }

    public void Setup(int upDownReference = 1, int direction = 1)
    {
        initialPosition = transform.position;
        directionMultiplier = 1;

        finalPosition = initialPosition + direction * Vector3.right * blockWidth;

        //RaycastHit hit;
        //if (Physics.Raycast(transform.position + Vector3.right * blockWidth, Vector3.down, out hit, 1f, layerMask))
        //{
        //    Debug.Log("name: " + hit.collider.name);
        //    finalPosition = initialPosition + Vector3.right * blockWidth;
        //}else if (Physics.Raycast(transform.position + Vector3.left * blockWidth, Vector3.down, out hit, 1f, layerMask))
        //{
        //    Debug.Log("name: " + hit.collider.name);
        //    finalPosition = initialPosition + Vector3.left * blockWidth;
        //}
        //else
        //{
        //    Debug.Log("No hit");
        //    finalPosition = initialPosition;
        //}

        //RaycastHit hit;
        //bool toTheRight = false;
        //bool toTheLeft = false;

        //if (Physics.Raycast(transform.position - Vector3.up * upDownReference * blockHeight, Vector3.right, out hit, blockWidth, layerMask))
        //{
        //    //Debug.Log("name: " + hit.collider.name);
        //    //finalPosition = initialPosition + Vector3.right * blockWidth;
        //    toTheRight = true;
        //}

        //if (Physics.Raycast(transform.position - Vector3.up * upDownReference * blockHeight, Vector3.left, out hit, blockWidth, layerMask))
        //{
        //    //Debug.Log("name: " + hit.collider.name);
        //    //finalPosition = initialPosition + Vector3.left * blockWidth;
        //    toTheLeft = true;
        //}
        //else
        //{
        //    //Debug.Log("No hit");
        //    int direction = (Random.Range(0, 2)) * 2 - 1;
        //    finalPosition = initialPosition + direction * Vector3.right * blockWidth;
        //    //finalPosition = initialPosition;
        //}

        //if(toTheLeft && !toTheRight)
        //{
        //    Debug.Log("Left");
        //    finalPosition = initialPosition + Vector3.left * blockWidth;
        //} else if (!toTheLeft && toTheRight)
        //{
        //    Debug.Log("Right");
        //    finalPosition = initialPosition + Vector3.right * blockWidth;
        //}
        //else
        //{
        //    Debug.Log("No hit or both hits");
        //    int direction = (Random.Range(0, 2)) * 2 - 1;
        //    finalPosition = initialPosition + direction * Vector3.right * blockWidth;
        //}
    }

    private void Update()
    {
        Vector3 move = (finalPosition - initialPosition).normalized * movingBlockSpeed * Time.deltaTime * directionMultiplier;

        //if(((directionMultiplier == 1 ? finalPosition : initialPosition) - transform.position).sqrMagnitude > minDistance * minDistance)
        //{
        //    transform.position += move;
        //}
        //else
        //{
        //    directionMultiplier *= -1;
        //}

        if ((finalPosition.x > initialPosition.x && ((directionMultiplier == 1 && transform.position.x < finalPosition.x) || directionMultiplier == -1 && transform.position.x > initialPosition.x)) ||
            (finalPosition.x < initialPosition.x && ((directionMultiplier == 1 && transform.position.x > finalPosition.x) || directionMultiplier == -1 && transform.position.x < initialPosition.x)))
        {
            transform.position += move;
        }
        else
        {
            directionMultiplier *= -1;
        }
    }
}
