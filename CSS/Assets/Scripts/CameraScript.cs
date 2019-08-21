using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraScript : MonoBehaviour
{

    private Transform myTransform;
    private float shakeDuration = 0f;
    private float shakeMagnitude = 0.7f;
    private float dampingSpeed = 1.0f;
    Vector3 initialPosition;
    Vector3 originalPosition;

    // Update is called once per frame
    void Awake()
    {
        originalPosition = transform.localPosition;

        if (myTransform == null)
        {
            myTransform = transform;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (shakeDuration > 0)
        {
            transform.localPosition = initialPosition + Random.insideUnitSphere * shakeMagnitude;

            shakeDuration -= Time.deltaTime * dampingSpeed;
        }
        else
        {
            initialPosition = myTransform.localPosition;
            transform.localPosition = originalPosition;
            shakeDuration = 0f;
        }
    }

    public void TriggerShake()
    {
        shakeDuration = 0.5f;
    }
}