﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Timeline;
using UnityEngine.UI;
using UnityEngine.Playables;
using UnityEngine.Events;
using UnityEngine.Serialization;

public class ShipControllerSergio : MonoBehaviour
{

    public bool useKeyboard;

    public float healthPoints;

    public float speed = 50f;
    public float turnSpeed = 0.5f;

    public float hoverForce = 65f;
    public float hoverHeight = 10f;

    public float bulletSpeed = 1000;

    public GameObject laserGreenBullet;
    public GameObject playerBulletsFolder;

    private float maxSpeed = 100;
    private float minSpeed = 20;

    private int activeCannon = 1;

    private float powerInput;
    private float verticalInput;
    private float horizontalInput;

    public FixedJoystick fixedJoystick;

    private Rigidbody shipRigidBody;

    private Animator anim;

    private GameObject[] getCount;
    private int count;
    private float maxHealthPoints;

    [SerializeField] Text speedText;
    [SerializeField] Text healthText;
    [SerializeField] GameObject shootFlareRight;
    [SerializeField] GameObject shootFlareLeft;
    [SerializeField] GameObject explosionShipDead;
    [SerializeField] GameObject laserHit;
    [SerializeField] GameObject myShip;
    public PlayableDirector shipDeadTimeline;


    /// ///////////////////////////////////////////////////////

    [Tooltip("X: Lateral thrust\nY: Vertical thrust\nZ: Longitudinal Thrust")]
    public Vector3 linearForce = new Vector3(100.0f, 100.0f, 100.0f);

    [Tooltip("X: Pitch\nY: Yaw\nZ: Roll")]
    public Vector3 angularForce = new Vector3(100.0f, 100.0f, 100.0f);

    [Range(0.0f, 1.0f)]
    [Tooltip("Multiplier for longitudinal thrust when reverse thrust is requested.")]
    public float reverseMultiplier = 1.0f;

    [Tooltip("Multiplier for all forces. Can be used to keep force numbers smaller and more readable.")]
    public float forceMultiplier = 100.0f;

    private Vector3 appliedLinearForce = Vector3.zero;
    private Vector3 appliedAngularForce = Vector3.zero;

    //public bool UsingMouseInput { get { return useMouseInput; } }
    //public Vector3 Velocity { get { return GetVelocity(); } }

    //private static Vector3 GetVelocity()
    //{
    //    return Rigidbody.velocity;
    //}
    public Vector3 Velocity = new Vector3(); 

    public float Throttle { get { return throttle; } }

    [Range(-1, 1)]
    public float pitch;
    [Range(-1, 1)]
    public float yaw;
    [Range(-1, 1)]
    public float roll;
    [Range(-1, 1)]
    public float strafe;
    [Range(0, 1)]
    public float throttle;

    public bool addRoll = true;

    private const float THROTTLE_SPEED = 0.5f;

    ////////////////////////////////////////////////////////////
    void Awake()
    {
        shipRigidBody = GetComponent<Rigidbody>();
        anim = GetComponentInChildren<Animator>();
        maxHealthPoints = healthPoints;
    }

    ////////////////////////////////////////////////////////////
    void Update()
    {
                                                                                            //if (useKeyboard)
                                                                                            //{
                                                                                            //    verticalInput = Input.GetAxis("Vertical");
                                                                                            //    horizontalInput = Input.GetAxis("Horizontal");
                                                                                            //}

                                                                                            //if (!useKeyboard)
                                                                                            //{
                                                                                            //    verticalInput = fixedJoystick.Vertical;
                                                                                            //    horizontalInput = fixedJoystick.Horizontal;
                                                                                            //}

        pitch = Input.GetAxis("Vertical");
        yaw = Input.GetAxis("Horizontal");

        if (addRoll)
            roll = -Input.GetAxis("Horizontal") * 0.5f;

        strafe = 0.0f;
        //UpdateKeyboardThrottle(KeyCode.R, KeyCode.F);

        speed += powerInput;

        if (speed > maxSpeed) { speed = maxSpeed; };
        if (speed < minSpeed) { speed = minSpeed; };

        speedText.text = "Speed: " + Mathf.RoundToInt(speed).ToString();

        if (Input.GetKeyDown("space"))
        {
            if (healthPoints>1)
            {
                healthPoints = 1;
            } else
            {

            }
            healthPoints--;
        }

        updateHUD();

        if (healthPoints < 0)
        {
            GameManager.Instance.levelLose();
            Debug.Log("GAME OVER");

            shipDead();
        }

    }


    ////////////////////////////////////////////////////////////

    private void FixedUpdate()
    {
        
            if (shipRigidBody != null)
            {
            shipRigidBody.AddRelativeForce(appliedLinearForce * forceMultiplier, ForceMode.Force);
            shipRigidBody.AddRelativeTorque(appliedAngularForce * forceMultiplier, ForceMode.Force);
            }
        
                                                                                                                //Ray ray = new Ray(transform.position, -transform.up);
                                                                                                                //RaycastHit hit;

                                                                                                                //    if (Physics.Raycast(ray, out hit, hoverHeight))
                                                                                                                //    {
                                                                                                                //    float proportionalHeight = (hoverHeight - hit.distance) / hoverHeight;
                                                                                                                //    Vector3 appliedHoverForce = Vector3.up * proportionalHeight * hoverForce;
                                                                                                                //    shipRigidBody.AddForce(appliedHoverForce, ForceMode.Acceleration);

                                                                                                                //    }

        //shipRigidBody.AddRelativeForce(0f, verticalInput * verticalInput * turnSpeed, speed);

 
        //shipRigidBody.AddRelativeTorque(verticalInput , horizontalInput * turnSpeed, verticalInput);

        if(anim != null)

            anim.SetFloat("Turn", horizontalInput);

    }

    public void SetPhysicsInput(Vector3 linearInput, Vector3 angularInput)
    {
        appliedLinearForce = MultiplyByComponent(linearInput, linearForce);
        appliedAngularForce = MultiplyByComponent(angularInput, angularForce);
    }

    private Vector3 MultiplyByComponent(Vector3 a, Vector3 b)
    {
        Vector3 ret;

        ret.x = a.x * b.x;
        ret.y = a.y * b.y;
        ret.z = a.z * b.z;

        return ret;
    }

    public void Shoot()
    {
    GameObject bullet = Instantiate(laserGreenBullet, playerBulletsFolder.transform);

        bullet.transform.rotation = transform.rotation;

        if (activeCannon == 1)
        {
            shootFlareLeft.SetActive(false);
            bullet.transform.position = transform.position + new Vector3(2, 0, 0);
            shootFlareRight.SetActive(true);
            activeCannon = 2;
        }
        else if (activeCannon == 2)
        {
            shootFlareRight.SetActive(false);
            bullet.transform.position = transform.position + new Vector3(-2, 0, 0);
            shootFlareLeft.SetActive(true);
            activeCannon = 1;
        }

        bullet.GetComponent<Rigidbody>().AddForce(transform.forward * bulletSpeed);

        Destroy(bullet, 3);

    }

    public void DodgeLeft()
    {
        Debug.Log("dash left");
        anim.SetTrigger("DodgeLeft");
        shipRigidBody.AddRelativeForce(-50f, 0, 0, ForceMode.Impulse);
    }

    public void DodgeRight()
    {
        Debug.Log("dash right");
        anim.SetTrigger("DodgeRight");
        shipRigidBody.AddRelativeForce(50f, 0, 0, ForceMode.Impulse);
    }

    ////////////////////////////////////////////////////////////

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Asteroid" || other.gameObject.tag == "EnemyShip")
        {
            healthPoints--;
            checkDamage();

        }

        if (other.gameObject.tag == "LaserRed")
        {
            Debug.Log("playerHit");
            healthPoints--;
            checkDamage();

            Instantiate(laserHit, transform.position, Quaternion.identity, transform);
            Destroy(other.gameObject);

        }

    }

    ////////////////////////////////////////////////////////////
    private void updateHUD()
    {
        speedText.text = "Speed: " + Mathf.RoundToInt(speed).ToString();
        healthText.text = "Health: " + healthPoints.ToString();
    }

    private void shipDead()
    {
        getCount = GameObject.FindGameObjectsWithTag("DeadExplosion");
        count = getCount.Length;
        if (count == 0)
            {
            Instantiate(explosionShipDead, transform.position, Quaternion.identity, transform);
            }
        shipDeadTimeline.Play();
        Destroy(myShip);
    }

    ////////////////////////////////////////////////////////////
    private void checkDamage()
    {
        if (healthPoints < (maxHealthPoints/2))
        {
            transform.GetChild(0).GetChild(5).gameObject.SetActive(true);
            transform.GetChild(0).GetChild(6).gameObject.SetActive(false);
        }

        if (healthPoints < (maxHealthPoints / 3))
        {
            transform.GetChild(0).GetChild(5).gameObject.SetActive(false);
            transform.GetChild(0).GetChild(6).gameObject.SetActive(true);
        }

        if (healthPoints < 0)
        {
            transform.GetChild(0).GetChild(5).gameObject.SetActive(false);
            transform.GetChild(0).GetChild(6).gameObject.SetActive(false);
        }

    }

}
