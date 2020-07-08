using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFlyToPosition : MonoBehaviour
{
    [SerializeField] Transform target;
    [SerializeField] float flySpeed = 7f;
    bool canFlyToPosition = true;
    [SerializeField] float distanceToStop = 0.1f;

    [SerializeField] float timeDelayToStartGameAfterFlyingToPosition = 0.5f;

    [SerializeField] GameObject gameTitle;

    private void LateUpdate()
    {
        if (canFlyToPosition)
        {
            Vector3 move = (target.position - transform.position).normalized * flySpeed * Time.deltaTime;
            transform.position += move;

            if((target.position - transform.position).sqrMagnitude <= distanceToStop * distanceToStop)
            {
                canFlyToPosition = false;
                transform.position = target.position;

                IEnumerator delayedStartCoroutine = DelayedStart();
                StartCoroutine(delayedStartCoroutine);
            }
        }
    }

    IEnumerator DelayedStart()
    {
        yield return new WaitForSeconds(timeDelayToStartGameAfterFlyingToPosition);
        GameplayManager.gameplayManager.StartGame();

        Destroy(gameTitle, timeDelayToStartGameAfterFlyingToPosition);
    }
}
