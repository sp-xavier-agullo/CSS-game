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
    public float energyPoints = 100;

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

    private float verticalInput;
    private float horizontalInput;

    public FixedJoystick fixedJoystick;

    private Rigidbody shipRigidBody;

    private Animator anim;

    private GameObject[] getCount;
    private int count;

    private float maxHealthPoints;
    private float maxEnergyPoints;

    private float energyPerLaser = 5;
    private float energyRegenerationPerSecond = 4.5f;

    [SerializeField] Image LifeBar;
    [SerializeField] Image SpeedBar;
    [SerializeField] Image EnergyBar;

    [SerializeField] GameObject shootFlareRight;
    [SerializeField] GameObject shootFlareLeft;
    [SerializeField] GameObject explosionShipDead;
    [SerializeField] GameObject laserHit;
    [SerializeField] GameObject myShip;
    public PlayableDirector shipDeadTimeline;


    ////////////////////////////////////////////////////////////
    void Awake()
    {
        shipRigidBody = GetComponent<Rigidbody>();
        anim = GetComponentInChildren<Animator>();
        maxHealthPoints = healthPoints;
        maxEnergyPoints = energyPoints;
    }

    ////////////////////////////////////////////////////////////
    void Update()
    {
        //if (useKeyboard)
        //{
        //    powerInput = Input.GetAxis("Vertical");
        //    turnInput = Input.GetAxis("Horizontal");
        //}

        if (!useKeyboard)
        {
            verticalInput = fixedJoystick.Vertical;
            horizontalInput = fixedJoystick.Horizontal;
        }

        //speed += powerInput;

        if (speed > maxSpeed) { speed = maxSpeed; };
        if (speed < minSpeed) { speed = minSpeed; };

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

        if (healthPoints < 0)
        {
            GameManager.Instance.levelLose();
            Debug.Log("GAME OVER");

            shipDead();
        }


        energyPoints += energyRegenerationPerSecond * Time.deltaTime;

        if (energyPoints > maxEnergyPoints)
        {
            energyPoints = maxEnergyPoints;
        }


        UpdateHUD();
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


        shipRigidBody.AddRelativeTorque(verticalInput * turnSpeed, 0f, -horizontalInput * turnSpeed);

        //if (anim != null)

        //    anim.SetFloat("Turn", horizontalInput);

    }

    public void Shoot()
    {

        if (energyPoints > energyPerLaser)
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

        energyPoints -= energyPerLaser;

        if (energyPoints < 0)
        {
            energyPoints = 0;
        }

    }

    public void DodgeLeft()
    {
        //Debug.Log("dash left");
        anim.SetTrigger("DodgeLeft");
        shipRigidBody.AddRelativeForce(-50f, 0, 0, ForceMode.Impulse);
    }

    public void DodgeRight()
    {
        //Debug.Log("dash right");
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

        if (other.gameObject.tag == "energyPowerup")
        {
            energyPoints+= maxEnergyPoints/5;

            if (energyPoints>maxEnergyPoints)
            {
                energyPoints = maxEnergyPoints;
            }

            GameObject collectParticles = Instantiate(GameManager.Instance.PowerupCollect, transform);
            Destroy(collectParticles, 3f);

            Destroy(other.gameObject);

        }

        if (other.gameObject.tag == "healthPowerup")
        {
            healthPoints += maxHealthPoints / 5;

            if (healthPoints > maxHealthPoints)
            {
                healthPoints = maxHealthPoints;
            }

            GameObject collectParticles = Instantiate(GameManager.Instance.PowerupCollect, transform);
            Destroy(collectParticles, 3f);

            Destroy(other.gameObject);

        }

    }

    ////////////////////////////////////////////////////////////
    private void UpdateHUD()
    {
        float healthToShow = Utils.convertToNewRange(0,maxHealthPoints,0,1,healthPoints);
        float speedToShow = Utils.convertToNewRange(0,maxSpeed,0,1,speed);
        float energyToShow = Utils.convertToNewRange(0,maxEnergyPoints,0,1,energyPoints);

        LifeBar.fillAmount = healthToShow;
        SpeedBar.fillAmount = speedToShow;
        EnergyBar.fillAmount = energyToShow;

        //speedText.text = "Speed: " + Mathf.RoundToInt(speed).ToString();
        //healthText.text = "Health: " + healthPoints.ToString();
    }

    private void shipDead()
    {
        GameManager.Instance.radarShip.GetComponent<RadarController>().hideRadar();

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
