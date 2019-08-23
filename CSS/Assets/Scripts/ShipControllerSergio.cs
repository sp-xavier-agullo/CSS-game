using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.Serialization;

    public class ShipControllerSergio : MonoBehaviour
    {

        public float speed = 50f;
        public float turnSpeed = 0.5f;

        public float hoverForce = 65f;
        public float hoverHeight = 10f;

        public GameObject laserGreenBullet;
        public GameObject playerBulletsFolder;

        private float maxSpeed = 100;
        private float minSpeed = 20;

        private float powerInput;
        private float turnInput;

        public FixedJoystick fixedJoystick;

        private Rigidbody shipRigidBody;

        private Animator anim;


        [SerializeField] Text speedText;
        //[SerializeField] CameraScript myCameraScript;

        
        ////////////////////////////////////////////////////////////
        void Awake()
        {
            shipRigidBody = GetComponent<Rigidbody>();
            anim = GetComponentInChildren<Animator>();
        }

        ////////////////////////////////////////////////////////////
        void Update()
        {
            powerInput = fixedJoystick.Vertical;
            turnInput = fixedJoystick.Horizontal;

            speed += powerInput;

            if (speed > maxSpeed) { speed = maxSpeed; };
            if (speed < minSpeed) { speed = minSpeed; };

            speedText.text = "Speed: " + Mathf.RoundToInt(speed).ToString();

        //float testConvertRange = Utils.convertToNewRange(-100, 100, -50, 50, 90);
        //Debug.Log(testConvertRange.ToString());

        //if (Input.GetKeyDown("space"))
        //{
        //    Shoot();
        //}
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

            //
            //shipRigidBody.AddRelativeForce(0f, 0f, powerInput * speed);
            shipRigidBody.AddRelativeForce(0f, 0f, speed);


            //
            shipRigidBody.AddRelativeTorque(0f, turnInput * turnSpeed, 0f);

        }

        public void Shoot()
        {
            Debug.Log("player has shot");
        anim.SetTrigger("ShootLeft");
        anim.SetTrigger("ShootRight");

        GameObject bullet = Instantiate(laserGreenBullet, playerBulletsFolder.transform);

            bullet.transform.position = transform.position;
            //bullet.AddRelativeForce(0f, 0, 1000, ForceMode.VelocityChange);
            bullet.transform.rotation = transform.rotation;

            /*
            GameObject bullet = ObjectPooler.SharedInstance.GetPooledObject();
            if (bullet != null)
            {
                bullet.transform.position = transform.position;
                bullet.transform.rotation = transform.rotation;
            }*/
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

    private void OnTriggerEnter(Collider collision)
        {
            if (collision.gameObject.tag == "Asteroid" || collision.gameObject.tag == "EnemyShip")
            {
                //myCameraScript.TriggerShake();
            }

        }


}
