using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DetectPlayerCollision : MonoBehaviour
{
    public float speed = 5f;
    [SerializeField] float allowance = 0.1f;

    bool canDetectCollisions = false;

    [SerializeField] float timeAfterStartupToBeginDetectingCollisions = 0.4f;

    PlayerStats stats;

    float initialSpeed;
    Vector3 initialPosition;

    [SerializeField] float sphereRadius = 0.5f;
    [SerializeField] float radiusAllowance = 0.1f;
    [SerializeField] LayerMask layerMask;

    private void Awake()
    {
        previousPosition = transform.position;

        stats = GetComponent<PlayerStats>();
    }

    public bool canStartDetectingCollisions = false;

    IEnumerator delayCollisionDetectionCoroutine;

    [ContextMenu("Start Detecting Collisions")]
    public void StartDetectingCollisions()
    {
        initialSpeed = speed;
        initialPosition = transform.position;
        canStartDetectingCollisions = true;
        delayCollisionDetectionCoroutine = DelayCollisionDetection();
        StartCoroutine(delayCollisionDetectionCoroutine);
    }

    public void ResetDetectCollisions()
    {
        speed = initialSpeed;

        contactPoint = Vector3.zero;
        previousPosition = Vector3.zero;
        TrailRenderer trail = GetComponent<TrailRenderer>();
        if (trail != null) trail.enabled = false;
        transform.position = initialPosition;
        if (trail != null) trail.enabled = true;

        if (delayCollisionDetectionCoroutine != null)
        {
            StopCoroutine(delayCollisionDetectionCoroutine);
        }

        canDetectCollisions = false;
        canStartDetectingCollisions = false;

        allowChange = false;
    }

    IEnumerator DelayCollisionDetection()
    {
        yield return new WaitForSeconds(timeAfterStartupToBeginDetectingCollisions);
        canDetectCollisions = true;
    }

    Vector3 contactPoint = Vector3.zero;

    Vector3 previousPosition;

    public bool allowChange = false;
    private void FixedUpdate()
    {
        if (canStartDetectingCollisions)
        {
            float forwardSpeed = (transform.position.z - previousPosition.z) / Time.fixedDeltaTime;
            previousPosition = transform.position;

            if (forwardSpeed < speed - allowance && canDetectCollisions && !allowChange)
            {                
                if (stats.isAlive)
                {
                    Debug.Log("Player lost - forwardSpeed: " + forwardSpeed);
                    stats.isAlive = false;
                    stats.HandleDeath();
                }
            }

            if (allowChange) allowChange = false;

            RaycastHit hit;
            if (Physics.Raycast(transform.position, Vector3.right, out hit, (sphereRadius - radiusAllowance) * transform.localScale.x, layerMask) ||
                Physics.Raycast(transform.position, Vector3.left, out hit, (sphereRadius - radiusAllowance) * transform.localScale.x, layerMask))
            {
                if (stats.isAlive)
                {
                    Debug.Log("Player lost - side wall collision");
                    stats.isAlive = false;
                    stats.HandleDeath();
                }
            }


        }
    }
}
