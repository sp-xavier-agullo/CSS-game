using System.Collections;
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
    private GameObject myShip;

    private float maxSpeed = 100;
    private float minSpeed = 20;

    private int activeCannon = 1;

    private float powerInput;
    private float turnInput;

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
    public PlayableDirector shipDeadTimeline;


    ////////////////////////////////////////////////////////////
    void Awake()
    {
        shipRigidBody = GetComponent<Rigidbody>();
        anim = GetComponentInChildren<Animator>();
        myShip = GameObject.Find("myShip");
        maxHealthPoints = healthPoints;
    }

    ////////////////////////////////////////////////////////////
    void Update()
    {
        if (useKeyboard)
        {
            powerInput = Input.GetAxis("Vertical");
            turnInput = Input.GetAxis("Horizontal");
        }

        if (!useKeyboard)
        {
            powerInput = fixedJoystick.Vertical;
            turnInput = fixedJoystick.Horizontal;
        }

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
    Ray ray = new Ray(transform.position, -transform.up);
    RaycastHit hit;

        if (Physics.Raycast(ray, out hit, hoverHeight))
        {
        float proportionalHeight = (hoverHeight - hit.distance) / hoverHeight;
        Vector3 appliedHoverForce = Vector3.up * proportionalHeight * hoverForce;
        shipRigidBody.AddForce(appliedHoverForce, ForceMode.Acceleration);

        }

        shipRigidBody.AddRelativeForce(0f, 0f, speed);

        //
        shipRigidBody.AddRelativeTorque(0f, turnInput * turnSpeed, 0f);

        if(anim != null)

            anim.SetFloat("Turn", turnInput);

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
