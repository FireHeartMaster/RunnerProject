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
    }

    private void Update()
    {
        Vector3 move = (finalPosition - initialPosition).normalized * movingBlockSpeed * Time.deltaTime * directionMultiplier;

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
