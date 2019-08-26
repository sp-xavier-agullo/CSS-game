using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class EnemyController : MonoBehaviour
{
    public int enemyID;

    public float speed = 50f;
    public float turnSpeed;

    public float healthPoints;

    public float bulletSpeed;

    public GameObject targetPointer;

    public GameObject laserRedBullet;
    public GameObject playerBulletsFolder;

    private float maxSpeed = 80;
    private float minSpeed = 30;
    private float aheadAngle = 35;

    private float stunTime = 1;
    private float stunCycleTimer = 6;
    private float stunCycleMinTime = 3;
    private float stunCycleMaxTime = 6;

    private float speedTimer = 5;

    private float powerInput;
    private float turnInput;

    private float distanceToTarget;
    public float maxHealthPoints;

    private Vector3 randomDestination;
    private GameObject assignedTargetWingman;

    public enum AiMode { ChasePlayer, RoamAround, ChaseWingman, isDead };

    public AiMode currentAiMode = AiMode.RoamAround;

    private Rigidbody shipRigidBody;

    [SerializeField] GameObject laserHit;



    ////////////////////////////////////////////////////////////
    ////////////////////////////////////////////////////////////
    void Awake()
    {
        shipRigidBody = GetComponent<Rigidbody>();

    }

    void Start()
    {
        randomDestination = setRandomDestination();
        assignedTargetWingman = GameManager.Instance.SelectWingman();
        setTargetPointer(false);
        maxHealthPoints = healthPoints;
    }

    ////////////////////////////////////////////////////////////
    ////////////////////////////////////////////////////////////
    void Update()
    {

        if (currentAiMode == AiMode.ChasePlayer)
        {

            distanceToTarget = Vector3.Distance(transform.position, GameManager.Instance.playerShip.transform.position);

            Vector3 targetDir = GameManager.Instance.playerShip.transform.position - transform.position;
            float angle = Vector3.Angle(targetDir, transform.forward);


            if (stunCycleTimer < stunTime) // Enemy pilot is stoned
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

                if (angle < aheadAngle) // Enemy facing player
                {
                    turnInput = 0;

                    int shootRollDice = Random.Range(0, 100);

                    if (shootRollDice < 5 && distanceToTarget < 300)
                    {
                        Shoot(GameManager.Instance.playerShip);
                    }
                }
            }

        }

        // Chase Wingman
        if (currentAiMode == AiMode.ChaseWingman)
        {

            if (assignedTargetWingman == null || !assignedTargetWingman.activeSelf)
            {
                currentAiMode = AiMode.RoamAround;
            }

            distanceToTarget = Vector3.Distance(transform.position, assignedTargetWingman.transform.position);

            Vector3 targetDir = assignedTargetWingman.transform.position - transform.position;
            float angle = Vector3.Angle(targetDir, transform.forward);


            if (stunCycleTimer < stunTime) // Enemy pilot is stoned
            {
                turnInput = 0;
            }

            else // Rotate towards target
            {

                Vector3 relativePoint = transform.InverseTransformPoint(assignedTargetWingman.transform.position);

                if (relativePoint.x < -0.0 && turnInput < 1) // Object is to the left
                {
                    turnInput -= turnSpeed;
                }
                else if (relativePoint.x > 0.0 && turnInput > -1) // Object is to the right
                {
                    turnInput += turnSpeed;
                }

                if (angle < aheadAngle) // Enemy facing target
                {
                    turnInput = 0;

                    int shootRollDice = Random.Range(0, 100);

                    if (shootRollDice < 5 && distanceToTarget < 300)
                    {
                        Shoot(assignedTargetWingman);
                    }
                }
            }

        }

        // Roam Around
        if (currentAiMode == AiMode.RoamAround)
        {
            Vector3 targetDir = randomDestination - transform.position;
            float angle = Vector3.Angle(targetDir, transform.forward);


            if (stunCycleTimer < stunTime) // Enemy pilot is stoned
            {
                turnInput = 0;

            }

            else // Rotate towards target
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

            //Debug.Log("enemy player stunned for: " + stunCycleTimer.ToString() + " seconds");

        }

        // Die

        if (healthPoints < 0)
        {

            StartCoroutine("DieSequence");

        }

        //
        if (targetPointer.activeSelf)
        {
            targetPointer.transform.LookAt(GameManager.Instance.mainCamera.transform);
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

    private Vector3 setRandomDestination()
    {
        float newX = Random.Range(-500, 500);
        float newY = transform.position.y;
        float newZ = Random.Range(-500, 500);

        Vector3 newRandomDestination = new Vector3(newX, newY, newZ);

        //Debug.Log("new random destination is: " + newRandomDestination.ToString());

        return newRandomDestination;

    }

    ////////////////////////////////////////////////////////////
    ////////////////////////////////////////////////////////////

    public void setTargetPointer (bool isTurnedOn)
    {
        if (isTurnedOn)
        {
            targetPointer.SetActive(true);
        }
        else if (!isTurnedOn)
        {
            targetPointer.SetActive(false);
        }
    }

    ////////////////////////////////////////////////////////////
    ////////////////////////////////////////////////////////////

    private void Shoot(GameObject targetObject)
    {
        GameObject bullet = Instantiate(laserRedBullet, playerBulletsFolder.transform);

        bullet.transform.position = transform.position;

        Vector3 targetDir = targetObject.transform.position - transform.position;

        bullet.transform.rotation = Quaternion.LookRotation(targetDir, Vector3.up);

        bullet.GetComponent<Rigidbody>().AddForce(targetDir * bulletSpeed);

        Destroy(bullet, 3);


    }

    ////////////////////////////////////////////////////////////
    ////////////////////////////////////////////////////////////

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "LaserGreen")
        {
            healthPoints--;

            Instantiate(laserHit, transform.position, Quaternion.identity, transform);
            Destroy(other.gameObject);

            GameManager.Instance.AssignNewEnemyTarget(enemyID);
            GameManager.Instance.radarShip.GetComponent<RadarController>().AssignNewEnemyPointerTarget(enemyID);

            if (healthPoints<0)
            {
                GameManager.Instance.AssignNewEnemyTarget(-1);
                GameManager.Instance.radarShip.GetComponent<RadarController>().AssignNewEnemyPointerTarget(-1);
            }

        }

        if (other.gameObject.tag == "LaserWingman")
        {
            healthPoints--;

            Instantiate(laserHit, transform.position, Quaternion.identity, transform);
            Destroy(other.gameObject);

            if (healthPoints < 0)
            {
                GameManager.Instance.AssignNewEnemyTarget(-1);
                GameManager.Instance.radarShip.GetComponent<RadarController>().AssignNewEnemyPointerTarget(-1);
            }

        }

        checkDamage();

    }

    ////////////////////////////////////////////////////////////
    ////////////////////////////////////////////////////////////

    IEnumerator DieSequence()
    {
        //Debug.Log("Ship is dead");
        speed = 0;
        currentAiMode = AiMode.isDead;

        transform.GetChild(1).transform.GetComponent<TrailRenderer>().emitting = false;
        transform.GetChild(2).transform.gameObject.SetActive(false);

        transform.GetComponent<SphereCollider>().enabled = false;

        transform.GetComponent<Rigidbody>().AddExplosionForce(100, transform.position + new Vector3(0, 0, 2), 10);

        transform.GetComponent<Rigidbody>().useGravity = true;
        transform.GetComponent<Rigidbody>().angularDrag = 0;

        transform.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None;

        yield return new WaitForSeconds(3);
        GameObject newExplosion = Instantiate(GameManager.Instance.explosionShip, transform.position, Quaternion.identity);
        newExplosion.transform.localScale = new Vector3(4, 4, 4);
        Destroy(newExplosion, 5f);
        GameManager.Instance.killShipNum(enemyID);
    }

    ////////////////////////////////////////////////////////////
    ////////////////////////////////////////////////////////////
    
    private void checkDamage()
    {
        if (healthPoints < (maxHealthPoints / 2))
        {
            transform.GetChild(4).gameObject.SetActive(true);
            transform.GetChild(5).gameObject.SetActive(false);
        }

        if (healthPoints < (maxHealthPoints / 3))
        {
            transform.GetChild(4).gameObject.SetActive(false);
            transform.GetChild(5).gameObject.SetActive(true);
        }

        /*if (healthPoints < 0)
        {
            transform.GetChild(4).gameObject.SetActive(false);
            transform.GetChild(5).gameObject.SetActive(false);
        }*/

    }

}