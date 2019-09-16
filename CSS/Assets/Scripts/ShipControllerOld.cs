using UnityEngine;
using UnityEngine.UI;

public class ShipControllerOld : MonoBehaviour
{
    public float healthPoints;

    public float speed = 50f;
    public float turnSpeed = 0.5f;

    public float hoverForce = 65f;
    public float hoverHeight = 20f;

    public float bulletSpeed = 1000;

    public GameObject laserGreenBullet;
    public GameObject playerBulletsFolder;

    private float maxSpeed = 100;
    private float minSpeed = 20;

    private int activeCannon = 1;

    private float powerInput;
    private float turnInput;

    private Rigidbody shipRigidBody;


    [SerializeField] Text speedText;
    [SerializeField] Text healthText;
    //[SerializeField] CameraScript myCameraScript;



    ////////////////////////////////////////////////////////////
    void Awake()
    {
        shipRigidBody = GetComponent<Rigidbody>();
    }

    ////////////////////////////////////////////////////////////
    void Update()
    {
        powerInput = Input.GetAxis("Vertical");
        turnInput = Input.GetAxis("Horizontal");

        speed += powerInput;

        if (speed > maxSpeed) { speed = maxSpeed; };
        if (speed < minSpeed ) { speed = minSpeed; };


        if (Input.GetKeyDown("space"))
        {
            Shoot();
        }

        updateHUD();

        if (healthPoints<0)
        {
            Debug.Log("GAME OVER");
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

    }

    private void Shoot ()
    {
        
        GameObject bullet = Instantiate(laserGreenBullet, playerBulletsFolder.transform);
        
        bullet.transform.rotation = transform.rotation;

        if (activeCannon==1)
        {
            bullet.transform.position = transform.position + new Vector3 (2,0,0);
            activeCannon = 2;
        }
        else if (activeCannon == 2)
        {
            bullet.transform.position = transform.position + new Vector3(-2, 0, 0);
            activeCannon = 1;
        }

        bullet.GetComponent<Rigidbody>().AddForce(transform.forward * bulletSpeed);

        Destroy(bullet, 3);
        
        //GameObject bullet = ObjectPooler.SharedInstance.GetPooledObject();
        
    }

    ////////////////////////////////////////////////////////////
    private void updateHUD ()
    {
        speedText.text = "Speed: " + Mathf.RoundToInt(speed).ToString();
        healthText.text = "Health: " + healthPoints.ToString();
    }

    ////////////////////////////////////////////////////////////

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Asteroid" || other.gameObject.tag == "EnemyShip")
        {
            healthPoints--;
            //myCameraScript.TriggerShake();
        }

        if (other.gameObject.tag == "LaserRed")
        {
            Debug.Log("playerHit");
            healthPoints--;

            Destroy(other.gameObject);

        }

    }



}
