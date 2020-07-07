using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DetectPlayerCollision : MonoBehaviour
{
    //[SerializeField, Range(0f, 90f)] float maxAllowedAngle = 10f;
    //float sineOfAngle = 0f;

    //Vector3 originalScale;

    //Rigidbody rigidbody;

    public float speed = 5f;
    [SerializeField] float allowance = 0.1f;

    bool canDetectCollisions = false;

    [SerializeField] float timeAfterStartupToBeginDetectingCollisions = 0.4f;

    PlayerStats stats;

    private void Awake()
    {
        //sineOfAngle = Mathf.Sin(maxAllowedAngle * Mathf.PI / 180f);

        //originalScale = transform.localScale;

        //rigidbody = GetComponent<Rigidbody>();

        previousPosition = transform.position;

        stats = GetComponent<PlayerStats>();
    }

    private void Start()
    {
        IEnumerator delayCollisionDetectionCoroutine = DelayCollisionDetection();
        StartCoroutine(delayCollisionDetectionCoroutine);
    }
    IEnumerator DelayCollisionDetection()
    {
        yield return new WaitForSeconds(timeAfterStartupToBeginDetectingCollisions);
        canDetectCollisions = true;
    }

    Vector3 contactPoint = Vector3.zero;

    Vector3 previousPosition;

    private void FixedUpdate()
    {
        float forwardSpeed = (transform.position.z - previousPosition.z) / Time.fixedDeltaTime;

        previousPosition = transform.position;
        //Debug.Log("forwardSpeed: " + forwardSpeed);

        if(forwardSpeed < speed - allowance && canDetectCollisions)
        {
            if (stats.isAlive)
            {
                Debug.Log("Player lost - forwardSpeed: " + forwardSpeed);
                stats.isAlive = false;
                stats.HandleDeath();
            }
        }
    }



    /*private void OnCollisionEnter(Collision collision)
    {
        contactPoint = collision.GetContact(0).point;

        float cosineProjection = transform.position.y / originalScale.y;    //when squashed, the sphere has the shape of an ellipsoid, ellipses are projections of circumferences, 
                                                                            //and we want to calculate the angle that would be formed if the player's sphere weren't squashed


        //original center of player if it wasn't squashed
        //Vector3 centerOfOriginalSphere = transform.position - Mathf.Sign(Physics.gravity.y)*(- Vector3.up * 0.5f * transform.localScale.y + Vector3.up * 0.5f * originalScale.y);   //transform.position + Vector3.down * 0.5f * transform.localScale.y + Vector3.up * 0.5f * originalScale.y;   

        //float angle = Vector3.Angle(Vector3.up * Mathf.Sign(Physics.gravity.y), Vector3.ProjectOnPlane(collision.GetContact(0).point - centerOfOriginalSphere, Vector3.right));
        //if (collision.GetContact(0).point.z > transform.position.z &&  angle > maxAllowedAngle)
        //{
        //    //Collision detected, player lost
        //    //Debug.Log("angle: " + angle);
        //    Debug.LogError("Player lost - " + " angle: " + angle);
        //    //Debug.LogError("Mathf.Sin(angle): " + Mathf.Sin(angle) + ", sineOfAngle: " + sineOfAngle);
        //}

        //if (collision.GetContact(0).point.z > transform.position.z && angle > maxAllowedAngle)
        //{
        //    //Collision detected, player lost
        //    Debug.LogError("Player lost");
        //}
        //Debug.Log(rigidbody.velocity.z);
    }*/

    /*private void OnDrawGizmos()
    {
        Gizmos.DrawSphere(contactPoint, 0.1f);
    }*/
}
