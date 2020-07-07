using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameplayManager : MonoBehaviour
{
    [SerializeField] float minimumTimeToFirstInvertGravity = 10f;

    [SerializeField] float minTimeLimitForGravityInversion = 10f;
    [SerializeField] float maxTimeLimitForGravityInversion = 30f;

    float timeSinceLastGravityInversion = 0;

    float currentGravityInversionInterval;

    Vector3 originalGravity;
    Vector3 fullGravity;

    [SerializeField] float gravityInversionSpeed = 2f;

    [SerializeField] GameObject gravityInversionAnimation;
    [SerializeField] Slider gravitySlider;

    private void Start()
    {
        currentGravityInversionInterval = minimumTimeToFirstInvertGravity + Random.Range(minTimeLimitForGravityInversion, maxTimeLimitForGravityInversion);

        originalGravity = Physics.gravity;
        fullGravity = Physics.gravity;

        gravityInversionAnimation.SetActive(false);

        gravitySlider.value = (Physics.gravity.y + 1) * 0.5f;
    }


    private void Update()
    {
        timeSinceLastGravityInversion += Time.deltaTime;

        if(timeSinceLastGravityInversion >= currentGravityInversionInterval)
        {
            //invert gravity
            timeSinceLastGravityInversion = 0;

            currentGravityInversionInterval = Random.Range(minTimeLimitForGravityInversion, maxTimeLimitForGravityInversion);

            //Physics.gravity *= -1;
            if(gravityInversionCoroutine != null)
            {
                StopCoroutine(gravityInversionCoroutine);
            }

            gravityInversionCoroutine = InvertGravity();
            StartCoroutine(gravityInversionCoroutine);
        }

        //Debug.Log("gravity: " + Physics.gravity.y);
    }

    IEnumerator gravityInversionCoroutine;
    IEnumerator InvertGravity()
    {
        //fullGravity = originalGravity * Mathf.Sign(Physics.gravity.y);
        fullGravity *= -1;

        gravityInversionAnimation.SetActive(true);

        Vector3 animationScale = gravityInversionAnimation.transform.localScale;
        animationScale.y = Mathf.Sign(fullGravity.y);
        gravityInversionAnimation.transform.localScale = animationScale;

        while (Physics.gravity.y * Mathf.Sign(fullGravity.y) < fullGravity.y * Mathf.Sign(fullGravity.y))
        {
            Vector3 currentGravity = Physics.gravity;
            currentGravity.y += gravityInversionSpeed * Time.deltaTime * Mathf.Sign(fullGravity.y);
            Physics.gravity = currentGravity;

            gravitySlider.value = Remap01(Physics.gravity.y, -9.81f * Mathf.Sign(fullGravity.y), 9.81f * Mathf.Sign(fullGravity.y));
            //gravitySlider.value = Remap01(Physics.gravity.y, -9.81f, 9.81f);
            //gravitySlider.value = (Physics.gravity.y + 1) * 0.5f;

            yield return new WaitForSeconds(Time.deltaTime);
        }

        Physics.gravity = fullGravity;

        gravityInversionAnimation.SetActive(false);
    }


    float Remap01(float value, float minLimit, float maxLimit)
    {
        return (value - minLimit) / (maxLimit - minLimit);
    }
}
