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

    public GameObject laserRedBullet;
    public GameObject playerBulletsFolder;

    private float maxSpeed = 80;
    private float minSpeed = 30;
    private float aheadAngle = 30;

    private float stunTime = 1;
    private float stunCycleTimer = 6;
    private float stunCycleMinTime = 3;
    private float stunCycleMaxTime = 6;

    private float speedTimer = 5;

    private float powerInput;
    private float turnInput;

    private enum AiMode { ChasePlayer, RoamAround, isDead };

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
                    //turnSpeed = 0.05f;

                    turnInput = 0;

                    int shootRollDice = Random.Range(0, 100);

                    if (shootRollDice < 5)
                    {
                        Shoot();
                    }

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

    private void Shoot ()
    {
        GameObject bullet = Instantiate(laserRedBullet, playerBulletsFolder.transform);

        //bullet.transform.rotation = transform.rotation;
        bullet.transform.position = transform.position;

        Vector3 targetDir = GameManager.Instance.playerShip.transform.position - transform.position;

        bullet.transform.rotation = Quaternion.LookRotation(targetDir, Vector3.up);

        bullet.GetComponent<Rigidbody>().AddForce(targetDir * bulletSpeed);

        //bullet.GetComponent<Rigidbody>().AddForce(transform.forward * bulletSpeed);

        Destroy(bullet, 3);


    }

    ////////////////////////////////////////////////////////////
    ////////////////////////////////////////////////////////////

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "LaserGreen")
        {
            Debug.Log("enemyHit");
            healthPoints--;

            Destroy(other.gameObject);

            //myCameraScript.TriggerShake();

        }

    }

    ////////////////////////////////////////////////////////////
    ////////////////////////////////////////////////////////////

    IEnumerator DieSequence ()
    {
        Debug.Log("Ship is dead");
        speed = 0;
        currentAiMode = AiMode.isDead;

        transform.GetChild(1).transform.GetComponent<TrailRenderer>().emitting = false;
        transform.GetChild(2).transform.gameObject.SetActive(false);

        transform.GetComponent<SphereCollider>().enabled = false;

        transform.GetComponent<Rigidbody>().AddExplosionForce(100, transform.position + new Vector3 (0,0,2), 10);

        transform.GetComponent<Rigidbody>().useGravity = true;
        //transform.GetComponent<Rigidbody>().drag = 0;
        transform.GetComponent<Rigidbody>().angularDrag = 0;

        transform.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None;

        yield return new WaitForSeconds(5);
        GameManager.Instance.killShipNum(enemyID);
    }

}
