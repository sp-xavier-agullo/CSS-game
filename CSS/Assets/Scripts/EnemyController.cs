using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class EnemyController : MonoBehaviour
{

    public float speed = 50f;
    public float turnSpeed = 0.1f;

    private float maxSpeed = 80;
    private float minSpeed = 30;
    private float aheadAngle = 30;

    private float stunTime = 2;
    private float stunCycleTimer = 5;
    private float stunCycleMinTime = 3;
    private float stunCycleMaxTime = 6;

    private float speedTimer = 5;

    private float powerInput;
    private float turnInput;

    private enum AiMode { ChasePlayer, RoamAround };

    [SerializeField] AiMode currentAiMode;

    private Rigidbody shipRigidBody;


    ////////////////////////////////////////////////////////////
    ////////////////////////////////////////////////////////////
    void Awake()
    {
        shipRigidBody = GetComponent<Rigidbody>();

    }

    ////////////////////////////////////////////////////////////
    ////////////////////////////////////////////////////////////
    void Update()
    {

        if (currentAiMode == AiMode.ChasePlayer)
        {
            Vector3 targetDir = GameManager.Instance.playerShip.transform.position - transform.position;
            float angle = Vector3.Angle(targetDir, transform.forward);

            if (angle < aheadAngle || stunCycleTimer < stunTime) // Enemy facing player or pilot is stoned
            {
                turnInput = 0;
            }

            else // Rotate towards player
            {

                Vector3 relativePoint = transform.InverseTransformPoint(GameManager.Instance.playerShip.transform.position);

                if (relativePoint.x < -0.0 && turnInput < 1) // Object is to the left
                {

                    turnInput -= turnSpeed;

                }
                else if (relativePoint.x > 0.0 && turnInput > -1) // Object is to the right
                {
                    
                    turnInput += turnSpeed;

                }

            }

        }

        // Change speed arbitrarily

        speedTimer -= 1 * Time.deltaTime;

        if (speedTimer < 0)
        {
            speed = setRandomSpeed();
            speedTimer = Random.Range(3, 10);

        }

        speed += powerInput;

        if (speed > maxSpeed) { speed = maxSpeed; };
        if (speed < minSpeed) { speed = minSpeed; };

        // Change stun pause arbitrarily

        stunCycleTimer -= 1 * Time.deltaTime;

        if (stunCycleTimer < 0)
        {
            stunCycleTimer = Random.Range(stunCycleMinTime, stunCycleMaxTime);

            Debug.Log("enemy player stunned for: " + stunCycleTimer.ToString() + " seconds");

        }

    }


    ////////////////////////////////////////////////////////////
    ////////////////////////////////////////////////////////////

    private void FixedUpdate()
    {

        shipRigidBody.AddRelativeForce(0f, 0f, speed);

        shipRigidBody.AddRelativeTorque(0f, turnInput * turnSpeed, 0f);

    }

    ////////////////////////////////////////////////////////////
    ////////////////////////////////////////////////////////////

    private float setRandomSpeed()
    {
        float newSpeed = Random.Range(minSpeed, maxSpeed);

        return newSpeed;
    }

}
