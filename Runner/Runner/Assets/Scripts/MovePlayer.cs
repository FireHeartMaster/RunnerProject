using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PlayerLoop;

public class MovePlayer : MonoBehaviour
{
    [SerializeField] float speed = 5f;
    float initialSpeed;

    [SerializeField] Vector3 forwardMoveDirection = Vector3.forward;

    public bool canMove = false;

    //bool canJump = true;
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

        //StartMoving();
    }

    private void Awake()
    {
        rigidbody = GetComponent<Rigidbody>();

        timeSinceLastJump = minTimeBetweenJumps;

        originalScale = transform.localScale;
    }

    private void Update()
    {
        //transform.position += forwardMoveDirection * speed * Time.fixedDeltaTime;
        //if (Input.GetButtonDown("Jump"))
        //{
        //    TryToJump();
        //}

        if (canMove)
        {
            timeSinceLastJump += Time.deltaTime;

            if (canMove && Input.GetKeyDown(KeyCode.DownArrow))
            {
                Squash();
            }

            if (canMove && Input.GetButtonDown("Jump") && timeSinceLastJump >= minTimeBetweenJumps)
            {
                shouldTryToJump = true;
            }
        }
    }
    private void FixedUpdate()
    {
        if (canMove)
        {
            Vector3 moveDirection = Vector3.zero;

            if (canMove)
            {
                moveDirection += forwardMoveDirection * speed * Time.fixedDeltaTime;
                moveDirection += Input.GetAxis("Horizontal") * sideSpeed * Vector3.right * Time.fixedDeltaTime;
            }



            //if (Input.GetButton("Left"))
            //{
            //    moveDirection += Vector3.left * sideSpeed;
            //}

            //if (Input.GetButton("Right"))
            //{
            //    moveDirection += Vector3.right * sideSpeed;
            //}


            Move(moveDirection);


            //if (Input.GetButtonDown("Jump") && timeSinceLastJump >= minTimeBetweenJumps)
            //{
            //    TryToJump();
            //}

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
        rigidbody.MovePosition(transform.position + moveDirection);
    }

    [SerializeField] float offsetForJumpCheckRaycast = 0.05f;

    void TryToJump()
    {
        bool canJump = false;
        //Debug.Log("Try To Jump");
        RaycastHit hit;
        float raycastCheckDistance = distanceForGroundCheck * transform.localScale.y + offsetForJumpCheckRaycast;
        //Debug.Log("raycast distance: " + dist);
        if (Physics.Raycast(transform.position, Vector3.up * Mathf.Sign(Physics.gravity.y), out hit, raycastCheckDistance, layersForGroundCheck))
        {
            //Debug.Log("Raycast hit");
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
        //Debug.Log("Jump");
        Vector3 force = forceMagnitude * Mathf.Sign(Physics.gravity.y) * Vector3.down;
        //Debug.Log("force: ");
        //Debug.Log(force);
        rigidbody.AddForce(force, ForceMode.VelocityChange);
        SoundManager.soundManager.JumpSound();
    }

    IEnumerator squash;
    IEnumerator keepSquashed;
    IEnumerator reverseSquash;
    void Squash()
    {
        if(squash != null)
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
        //Debug.Log("SquashCoroutine");
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
        //Debug.Log("KeepSquashed");
        float timeSinceCall = 0f;
        while(timeSinceCall < timeToKeepReducedScale)
        {
            timeSinceCall += Time.deltaTime;
            //Debug.Log("timeSinceCall: " + timeSinceCall);
            yield return new WaitForSeconds(Time.deltaTime);
        }

        reverseSquash = ReverseSquash();
        StartCoroutine(reverseSquash);
    }

    IEnumerator ReverseSquash()
    {
        //Debug.Log("ReverseSquash");
        float verticalScaleChangeSpeed = (originalScale.y - originalScale.y * verticalPercentOfOriginalScaleWhenReducingScale) / timeToReverseScaleBack;
        float horizontalScaleChangeSpeed = (originalScale.x * (horizontalPercentOfOriginalScaleWhenReducingScale + 1f) - originalScale.x) / timeToReverseScaleBack;
    
        while(transform.localScale.y < originalScale.y)
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

        while (/*((goingUp && (transform.localScale.y < originalScale.y * scaleMultiplierWhenBounceReversingScale)) || (!goingUp && (transform.localScale.y > originalScale.y * (1f - (scaleMultiplierWhenBounceReversingScale - 1f))))) &&*/ numberOfCycles < numberOfBouncesOnReversingScaleBack)
        {
            

            Vector3 localScale = transform.localScale;
            localScale.y += (verticalScaleChangeSpeed) * Time.deltaTime * directionMutiplier;
            localScale.x -= horizontalScaleChangeSpeed * Time.deltaTime * directionMutiplier;
            localScale.z -= horizontalScaleChangeSpeed * Time.deltaTime * directionMutiplier;

            if(transform.localScale.y < originalScale.y && localScale.y >= originalScale.y)
            {
                numberOfCycles++;
            }

            transform.localScale = localScale;

            yield return new WaitForSeconds(Time.deltaTime);

            if((goingUp && (transform.localScale.y >= originalScale.y * scaleMultiplierWhenBounceReversingScale)))
            {
                goingUp = false;
                directionMutiplier = -1;
            }
            else if((!goingUp && (transform.localScale.y <= originalScale.y * (1f - (scaleMultiplierWhenBounceReversingScale - 1f)))))
            {
                goingUp = true;
                directionMutiplier = 1;
            }
        }

        transform.localScale = originalScale;
    }

}
