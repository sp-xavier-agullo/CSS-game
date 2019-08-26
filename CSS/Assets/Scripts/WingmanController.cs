using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class WingmanController : MonoBehaviour
{
    public int wingmanID;

    public float speed = 50f;
    public float turnSpeed;

    public float healthPoints;

    public float bulletSpeed;

    public GameObject laserGreenBullet;
    public GameObject playerBulletsFolder;

    private float maxSpeed = 80;
    private float minSpeed = 30;
    private float aheadAngle = 25;

    private float stunTime = 1;
    private float stunCycleTimer = 6;
    private float stunCycleMinTime = 3;
    private float stunCycleMaxTime = 6;

    private float speedTimer = 5;

    private float powerInput;
    private float turnInput;

    private float distanceToTarget;

    private GameObject targetEnemy;

    private Vector3 randomDestination;

    public enum AiMode { ChaseEnemy, RoamAround, isDead };

    public AiMode currentAiMode;

    private Rigidbody shipRigidBody;


    ////////////////////////////////////////////////////////////
    ////////////////////////////////////////////////////////////
    void Awake()
    {
        shipRigidBody = GetComponent<Rigidbody>();

    }

    void Start()
    {
        randomDestination = setRandomDestination();

        SelectBehavior ();
    }

    ////////////////////////////////////////////////////////////
    ////////////////////////////////////////////////////////////
    void SelectBehavior () {

        int randomNumber = Random.Range(0,2);

        switch (randomNumber) {

            case 0: case 1:
            currentAiMode = AiMode.ChaseEnemy;
            targetEnemy = GameManager.Instance.SelectTarget();
            break;

            case 2: default:
            currentAiMode = AiMode.RoamAround;
            break;

        }

    }

    ////////////////////////////////////////////////////////////
    ////////////////////////////////////////////////////////////
    void Update()
    {

        if (currentAiMode == AiMode.ChaseEnemy)
        {

            if (targetEnemy == null || !targetEnemy.activeSelf)
            {
                SelectBehavior();

            }

            distanceToTarget = Vector3.Distance(transform.position, targetEnemy.transform.position);

            Vector3 targetDir = targetEnemy.transform.position - transform.position;
            float angle = Vector3.Angle(targetDir, transform.forward);

            if (stunCycleTimer < stunTime) // Wingman pilot is stoned
            {
                turnInput = 0;
            }

            else // Rotate towards target enemy
            {

                Vector3 relativePoint = transform.InverseTransformPoint(targetEnemy.transform.position);

                if (relativePoint.x < -0.0 && turnInput < 1) // Object is to the left
                {
                    turnInput -= turnSpeed;
                }
                else if (relativePoint.x > 0.0 && turnInput > -1) // Object is to the right
                {
                    turnInput += turnSpeed;
                }

                if (angle < aheadAngle) // Wingman facing target enemy
                {
                    turnInput = 0;

                    int shootRollDice = Random.Range(0, 100);

                    if (shootRollDice < 4 && distanceToTarget < 300 )
                    {
                        Shoot();
                    }
                }
            }

        }


        if (currentAiMode == AiMode.RoamAround)
        {
            Vector3 targetDir = randomDestination - transform.position;
            float angle = Vector3.Angle(targetDir, transform.forward);


            if (stunCycleTimer < stunTime) // Wingman pilot is stoned
            {
                turnInput = 0;

            }

            else // Rotate towards target enemy
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

            if (angle < aheadAngle) // Enemy facing target
            {
                turnInput = 0;
            }

        }

        if (currentAiMode == AiMode.isDead)
        {
            // Nothing here

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

            randomDestination = setRandomDestination();
            SelectBehavior ();

        }

        // Die

        if (healthPoints < 0)
        {

            StartCoroutine("DieSequence");
            
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

    ////////////////////////////////////////////////////////////
    ////////////////////////////////////////////////////////////

    private Vector3 setRandomDestination ()
    {
        float newX = Random.Range(-500, 500);
        float newY = transform.position.y;
        float newZ = Random.Range(-500, 500);

        Vector3 newRandomDestination = new Vector3(newX, newY, newZ);

        return newRandomDestination;

    }

    ////////////////////////////////////////////////////////////
    ////////////////////////////////////////////////////////////

    private void Shoot ()
    {
        GameObject bullet = Instantiate(laserGreenBullet, playerBulletsFolder.transform);

        bullet.transform.position = transform.position;

        Vector3 targetDir = targetEnemy.transform.position - transform.position;

        bullet.transform.rotation = Quaternion.LookRotation(targetDir, Vector3.up);

        bullet.GetComponent<Rigidbody>().AddForce(targetDir * bulletSpeed);

        bullet.tag = "LaserWingman";

        Destroy(bullet, 3);

    }

    ////////////////////////////////////////////////////////////
    ////////////////////////////////////////////////////////////

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "LaserRed")
        {
            //Debug.Log("wingmanHit");
            healthPoints--;

            Destroy(other.gameObject);

        }

    }

    ////////////////////////////////////////////////////////////
    ////////////////////////////////////////////////////////////

    IEnumerator DieSequence ()
    {
        //Debug.Log("Ship is dead");
        speed = 0;
        currentAiMode = AiMode.isDead;

        transform.GetChild(0).transform.GetChild(1).transform.GetComponent<TrailRenderer>().emitting = false;
        transform.GetChild(0).transform.GetChild(2).transform.gameObject.SetActive(false);

        transform.GetComponent<SphereCollider>().enabled = false;

        transform.GetComponent<Rigidbody>().AddExplosionForce(100, transform.position + new Vector3 (0,0,2), 10);

        transform.GetComponent<Rigidbody>().useGravity = true;
        transform.GetComponent<Rigidbody>().angularDrag = 0;

        transform.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None;

        yield return new WaitForSeconds(5);
        transform.gameObject.SetActive(false);
    }

}
