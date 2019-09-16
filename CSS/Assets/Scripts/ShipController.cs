using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Playables;

public class ShipController : MonoBehaviour
{
    public ShipModel Ship;

    public bool useKeyboard;

    public GameObject laserGreenBullet;
    public GameObject playerBulletsFolder;

    private int activeCannon = 1;

    private float powerInput;
    private float turnInput;

    public FixedJoystick fixedJoystick;

    private Rigidbody shipRigidBody;

    private Animator anim;

    private GameObject[] getCount;
    private int count;

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
        Ship.WeaponEnergyMax = Ship.WeaponEnergy;
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

        Ship.Speed += powerInput * Ship.Acceleration;

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Ship.Kill();
        }

        if(Ship.IsDead)
        {
            GameManager.Instance.levelLose();
            Debug.Log("GAME OVER");

            shipDead();
        }

        Ship.WeaponEnergy += Ship.WeaponEnergyRegenerationPerSecond * Time.deltaTime;

        UpdateHUD();
    }

    ////////////////////////////////////////////////////////////

    private void FixedUpdate()
    {
        Ray ray = new Ray(transform.position, -transform.up);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, Ship.HoverHeight))
        {
            float proportionalHeight = (Ship.HoverHeight - hit.distance) / Ship.HoverHeight;
            Vector3 appliedHoverForce = proportionalHeight * Ship.HoverForce * Vector3.up;
            shipRigidBody.AddForce(appliedHoverForce, ForceMode.Acceleration);
        }

        shipRigidBody.AddRelativeForce(0f, 0f, Ship.Speed);

        //
        shipRigidBody.AddRelativeTorque(0f, turnInput * Ship.TurnSpeed, 0f);

        if (anim != null)
        {
            anim.SetFloat("Turn", turnInput);
        }

    }

    public void Shoot()
    {

        if (Ship.WeaponEnergy > Ship.Weapon.EnergyPerProjectile)
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

            bullet.GetComponent<Rigidbody>().AddForce(transform.forward * Ship.Weapon.Projectile.Speed);

            Destroy(bullet, 3);

        }

        Ship.WeaponEnergy -= Ship.Weapon.EnergyPerProjectile;
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
            Ship.DoDamage(1);
            checkDamage();

        }

        if (other.gameObject.tag == "LaserRed")
        {
            Debug.Log("playerHit");
            Ship.DoDamage(1); // TODO: Change to use the projectile data
            checkDamage();

            Instantiate(laserHit, transform.position, Quaternion.identity, transform);
            Destroy(other.gameObject);

        }

        if (other.gameObject.tag == "energyPowerup")
        {
            Ship.WeaponEnergy += Ship.WeaponEnergyMax/5;

            GameObject collectParticles = Instantiate(GameManager.Instance.PowerupCollect, transform);
            Destroy(collectParticles, 3f);

            Destroy(other.gameObject);

        }

        if (other.gameObject.tag == "healthPowerup")
        {
            Ship.DoHeal(Ship.MaxHealth / 5); // TODO: Change this if a proper power up system is implemented

            GameObject collectParticles = Instantiate(GameManager.Instance.PowerupCollect, transform);
            Destroy(collectParticles, 3f);

            Destroy(other.gameObject);

        }

    }

    ////////////////////////////////////////////////////////////
    private void UpdateHUD()
    {
        float healthToShow = Utils.convertToNewRange(0, Ship.MaxHealth,0,1,Ship.Health);
        float speedToShow = Utils.convertToNewRange(0, Ship.MaxSpeed,0,1,Ship.Speed);
        float energyToShow = Utils.convertToNewRange(0, Ship.WeaponEnergyMax,0,1,Ship.WeaponEnergy);

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
        if (Ship.Health < (Ship.MaxHealth / 2))
        {
            transform.GetChild(0).GetChild(5).gameObject.SetActive(true);
            transform.GetChild(0).GetChild(6).gameObject.SetActive(false);
        }

        if (Ship.Health < (Ship.MaxHealth / 3))
        {
            transform.GetChild(0).GetChild(5).gameObject.SetActive(false);
            transform.GetChild(0).GetChild(6).gameObject.SetActive(true);
        }

        if (Ship.IsDead)
        {
            transform.GetChild(0).GetChild(5).gameObject.SetActive(false);
            transform.GetChild(0).GetChild(6).gameObject.SetActive(false);
        }

    }

}
