﻿using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.PlayerLoop;

public class MovePlayer : MonoBehaviour
{
    public float speed = 5f;
    float initialSpeed;

    [SerializeField] Vector3 forwardMoveDirection = Vector3.forward;

    public bool canMove = false;

    bool jump = false;

    Rigidbody rigidbody;

    [SerializeField] float forceMagnitude = 20f;

    [SerializeField] float distanceForGroundCheck = 0.55f;
    [SerializeField] LayerMask layersForGroundCheck;

    [SerializeField] float sideSpeed = 1f;

    [SerializeField] float minTimeBetweenJumps = 0.2f;
    float timeSinceLastJump;

    Vector3 originalScale;
    [SerializeField, Range(0f, 1f)] float verticalPercentOfOriginalScaleWhenReducingScale = 0.5f;
    [SerializeField, Range(0f, 1f)] float horizontalPercentOfOriginalScaleWhenReducingScale = 0.2f;
    [SerializeField] float timeToReduceScale = 0.1f;
    [SerializeField] float timeToKeepReducedScale = 2f;
    [SerializeField] float timeToReverseScaleBack = 0.3f;
    [SerializeField] int numberOfBouncesOnReversingScaleBack = 2;
    [SerializeField, Range(1f, 2f)] float scaleMultiplierWhenBounceReversingScale = 1.2f;

    Vector3 initialScale;
    Vector3 initialPosition;

    [ContextMenu("Start Moving")]
    public void StartMoving()
    {
        canMove = true;

        initialSpeed = speed;

        initialScale = transform.localScale;
        initialPosition = transform.position;

        rigidbody.isKinematic = false;

        ballReferencePoint = transform.position;
        moveDirection = new Vector3(ballReferencePoint.x, 0f, 0f);
        displacedPoint = transform.position;
        playerReferencePoint = transform.position;
    }

    public void ResetMovePlayer()
    {
        speed = initialSpeed;

        canMove = false;
        jump = false;
        timeSinceLastJump = minTimeBetweenJumps;
        shouldTryToJump = false;

        transform.localScale = initialScale;
        originalScale = transform.localScale;

        TrailRenderer trail = GetComponent<TrailRenderer>();
        if (trail != null) trail.enabled = false;
        transform.position = initialPosition;
        if (trail != null) trail.enabled = true;

        if (squash != null)
        {
            StopCoroutine(squash);
        }

        if (keepSquashed != null)
        {
            StopCoroutine(keepSquashed);
        }

        if (reverseSquash != null)
        {
            StopCoroutine(reverseSquash);
        }
    }

    private void Awake()
    {
        rigidbody = GetComponent<Rigidbody>();

        timeSinceLastJump = minTimeBetweenJumps;

        originalScale = transform.localScale;
    }

    Vector3 ballReferencePoint;
    Vector3 playerReferencePoint;
    Vector3 displacedPoint;

    Vector2 touchStartPoint;
    Vector2 touchEndPoint;

    Touch touch;

    [SerializeField] float minTimeToConsiderLongTouch = 0.3f;

    enum TouchState
    {
        shortTouch,
        longTouch,
        notTouching
    }

    TouchState touchState = TouchState.notTouching;
    Vector2 touchInitialPosition;
    [SerializeField] float minSpeedToConsiderLongTouch = 10f;

    float timeWhenLastTouchBegan = 0f;
    Vector2 lastTouchPosition;

    bool previousMouseTouching = false;

    float ScreenToWorldHorizontal(float screenPositionX, float trackWidth)
    {
        return (screenPositionX / Screen.width - 0.5f) /** 2f*/ * trackWidth;
    }

    [SerializeField] float minXToConsiderSideSwipe = 5f;
    [SerializeField] float percentOfScreenToConsiderSideSwipe = 0.025f;
    void HandleTouchInput(bool simulate = false)
    {
        if ((!simulate && Input.touchCount > 0) || (simulate && Input.GetMouseButton(0)))
        {
            touch = !simulate && Input.touchCount > 0 ? Input.GetTouch(0) : new Touch();
            
            lastTouchPosition = !simulate ? touch.position : new Vector2(Input.mousePosition.x, Input.mousePosition.y);

            if ((!simulate && touch.phase == TouchPhase.Began) || (simulate && Input.GetMouseButtonDown(0)))
            {
                timeWhenLastTouchBegan = Time.time;
                touchInitialPosition = !simulate ? touch.position : new Vector2(Input.mousePosition.x, Input.mousePosition.y); ;
                touchStartPoint = !simulate ? touch.position : new Vector2(Input.mousePosition.x, Input.mousePosition.y);

                ballReferencePoint = transform.position;

                playerReferencePoint = new Vector3(ScreenToWorldHorizontal(touchInitialPosition.x, 5f), 0f, 0f);
            }
            else if ((!simulate && touch.phase == TouchPhase.Moved) || (simulate && !Input.GetMouseButtonDown(0) && Input.GetMouseButton(0)))
            {
                    if (Mathf.Abs(lastTouchPosition.x - touchInitialPosition.x) > percentOfScreenToConsiderSideSwipe * Screen.width && touchState != TouchState.longTouch)
                    {
                    touchState = TouchState.longTouch;
                    playerReferencePoint = new Vector3(ScreenToWorldHorizontal(touchInitialPosition.x, 5f), 0f, 0f);
                }
                else if (touchState == TouchState.longTouch)
                {
                    displacedPoint = new Vector3(ScreenToWorldHorizontal(lastTouchPosition.x, 5f), 0f, 0f);

                    moveDirection += new Vector3(displacedPoint.x - playerReferencePoint.x, 0f, 0f);
                }
            }
            else if ((!simulate && touch.phase == TouchPhase.Ended) || (simulate && Input.GetMouseButtonUp(0)))
            {
                if (Time.time - timeWhenLastTouchBegan < minTimeToConsiderLongTouch)                
                {
                    if (lastTouchPosition.y > touchInitialPosition.y && Vector2.Angle(Vector2.up, (lastTouchPosition - touchInitialPosition)) < 45f)
                    {
                        if (Mathf.Sign(Physics.gravity.y) < 0f)
                        {
                            //jump
                            jumpWill = true;
                        }
                        else
                        {
                            //squash
                            shouldSquash = true;
                        }
                    }
                    else if (lastTouchPosition.y < touchInitialPosition.y && Vector2.Angle(Vector2.down, (lastTouchPosition - touchInitialPosition)) < 45f)
                    {
                        if (Mathf.Sign(Physics.gravity.y) < 0f)
                        {
                            //squash
                            shouldSquash = true;
                        }
                        else
                        {
                            //jump
                            jumpWill = true;
                        }
                    }
                }
                else
                {
                    //ballReferencePoint = Camera.main.ScreenToWorldPoint(lastTouchPosition);
                    ballReferencePoint = transform.position;
                }


                touchState = TouchState.notTouching;



            }

        }
        else
        {


            if ((!simulate && touch.phase == TouchPhase.Ended) || (simulate && Input.GetMouseButtonUp(0)))
            {
                if (Time.time - timeWhenLastTouchBegan < minTimeToConsiderLongTouch)
                {                    
                    if (lastTouchPosition.y > touchInitialPosition.y && Vector2.Angle(Vector2.up, (lastTouchPosition - touchInitialPosition)) < 45f)
                    {
                        if (Mathf.Sign(Physics.gravity.y) < 0f)
                        {
                            //jump
                            jumpWill = true;
                        }
                        else
                        {
                            //squash
                            shouldSquash = true;
                        }
                    }
                    else if (lastTouchPosition.y < touchInitialPosition.y && Vector2.Angle(Vector2.down, (lastTouchPosition - touchInitialPosition)) < 45f)
                    {
                        if (Mathf.Sign(Physics.gravity.y) < 0f)
                        {
                            //squash
                            shouldSquash = true;
                        }
                        else
                        {
                            //jump
                            jumpWill = true;
                        }
                    }
                }
                else
                {
                    //ballReferencePoint = Camera.main.ScreenToWorldPoint(lastTouchPosition);
                    ballReferencePoint = transform.position;
                }


                touchState = TouchState.notTouching;



            }                       
            touchState = TouchState.notTouching;

        }
        previousMouseTouching = Input.GetMouseButton(0);
    }
    bool shouldSquash = false;
    bool jumpWill = false;
    private void Update()
    {
        if (canMove)
        {
            timeSinceLastJump += Time.deltaTime;

            shouldSquash = false;
            jumpWill = false;
            moveDirection = Vector3.zero;
            HandleTouchInput(true);
            moveDirection += new Vector3(ballReferencePoint.x, 0f, 0f);

            if (canMove && (Input.GetKeyDown(KeyCode.DownArrow) || shouldSquash))
            {
                Squash();
            }

            if (canMove && (Input.GetButtonDown("Jump") || jumpWill) && timeSinceLastJump >= minTimeBetweenJumps)
            {
                shouldTryToJump = true;
            }
        }
    }

    Vector3 moveDirection;
    private void FixedUpdate()
    {
        if (canMove)
        {

            if (canMove)
            {
                moveDirection.y = transform.position.y;
                moveDirection.z = transform.position.z;
                moveDirection += forwardMoveDirection * speed * Time.fixedDeltaTime;
                moveDirection += Input.GetAxis("Horizontal") * sideSpeed * Vector3.right * Time.fixedDeltaTime;
            }

            Move(moveDirection);

            if (shouldTryToJump)
            {
                shouldTryToJump = false;
                TryToJump();
            }
        }
    }
    bool shouldTryToJump = false;

    void Move(Vector3 moveDirection)
    {
        rigidbody.MovePosition(moveDirection);
    }

    [SerializeField] float offsetForJumpCheckRaycast = 0.05f;

    void TryToJump()
    {
        bool canJump = false;
        RaycastHit hit;
        float raycastCheckDistance = distanceForGroundCheck * transform.localScale.y + offsetForJumpCheckRaycast;
        if (Physics.Raycast(transform.position, Vector3.up * Mathf.Sign(Physics.gravity.y), out hit, raycastCheckDistance, layersForGroundCheck))
        {
            canJump = true;
        }

        if (canJump)
        {
            timeSinceLastJump = 0f;
            Jump();
        }
    }

    void Jump()
    {        
        Vector3 force = forceMagnitude * Mathf.Sign(Physics.gravity.y) * Vector3.down;        
        rigidbody.AddForce(force, ForceMode.VelocityChange);
        SoundManager.soundManager.JumpSound();
    }

    IEnumerator squash;
    IEnumerator keepSquashed;
    IEnumerator reverseSquash;
    void Squash()
    {
        if (squash != null)
            StopCoroutine(squash);

        if (keepSquashed != null)
            StopCoroutine(keepSquashed);

        if (reverseSquash != null)
            StopCoroutine(reverseSquash);

        squash = SquashCoroutine();
        StartCoroutine(squash);
    }


    IEnumerator SquashCoroutine()
    {        
        transform.localScale = originalScale;
        float verticalScaleChangeSpeed = (originalScale.y - originalScale.y * verticalPercentOfOriginalScaleWhenReducingScale) / timeToReduceScale;
        float horizontalScaleChangeSpeed = (originalScale.x * (horizontalPercentOfOriginalScaleWhenReducingScale + 1f) - originalScale.x) / timeToReduceScale;
        while (transform.localScale.y > originalScale.y * verticalPercentOfOriginalScaleWhenReducingScale)
        {
            Vector3 localScale = transform.localScale;
            localScale.y -= (verticalScaleChangeSpeed) * Time.deltaTime;
            localScale.x += horizontalScaleChangeSpeed * Time.deltaTime;
            localScale.z += horizontalScaleChangeSpeed * Time.deltaTime;
            transform.localScale = localScale;

            yield return new WaitForSeconds(Time.deltaTime);
        }
        keepSquashed = KeepSquashed();
        StartCoroutine(keepSquashed);
    }

    IEnumerator KeepSquashed()
    {
        float timeSinceCall = 0f;
        while (timeSinceCall < timeToKeepReducedScale)
        {
            timeSinceCall += Time.deltaTime;            
            yield return new WaitForSeconds(Time.deltaTime);
        }

        reverseSquash = ReverseSquash();
        StartCoroutine(reverseSquash);
    }

    IEnumerator ReverseSquash()
    {        
        float verticalScaleChangeSpeed = (originalScale.y - originalScale.y * verticalPercentOfOriginalScaleWhenReducingScale) / timeToReverseScaleBack;
        float horizontalScaleChangeSpeed = (originalScale.x * (horizontalPercentOfOriginalScaleWhenReducingScale + 1f) - originalScale.x) / timeToReverseScaleBack;

        while (transform.localScale.y < originalScale.y)
        {
            Vector3 localScale = transform.localScale;
            localScale.y += (verticalScaleChangeSpeed) * Time.deltaTime;
            localScale.x -= horizontalScaleChangeSpeed * Time.deltaTime;
            localScale.z -= horizontalScaleChangeSpeed * Time.deltaTime;
            transform.localScale = localScale;

            yield return new WaitForSeconds(Time.deltaTime);
        }

        bool goingUp = true;
        int numberOfCycles = 0;
        int directionMutiplier = 1;

        while (numberOfCycles < numberOfBouncesOnReversingScaleBack)
        {


            Vector3 localScale = transform.localScale;
            localScale.y += (verticalScaleChangeSpeed) * Time.deltaTime * directionMutiplier;
            localScale.x -= horizontalScaleChangeSpeed * Time.deltaTime * directionMutiplier;
            localScale.z -= horizontalScaleChangeSpeed * Time.deltaTime * directionMutiplier;

            if (transform.localScale.y < originalScale.y && localScale.y >= originalScale.y)
            {
                numberOfCycles++;
            }

            transform.localScale = localScale;

            yield return new WaitForSeconds(Time.deltaTime);

            if ((goingUp && (transform.localScale.y >= originalScale.y * scaleMultiplierWhenBounceReversingScale)))
            {
                goingUp = false;
                directionMutiplier = -1;
            }
            else if ((!goingUp && (transform.localScale.y <= originalScale.y * (1f - (scaleMultiplierWhenBounceReversingScale - 1f)))))
            {
                goingUp = true;
                directionMutiplier = 1;
            }
        }

        transform.localScale = originalScale;
    }
}     