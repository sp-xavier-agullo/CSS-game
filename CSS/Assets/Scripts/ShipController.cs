using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShipController : MonoBehaviour
{



    public float speed = 50f;
    public float turnSpeed = 0.5f;


    private float maxSpeed = 100;
    private float minSpeed = 20;


    public float hoverForce = 65f;
    public float hoverHeight = 10f;


    private float powerInput;
    private float turnInput;


    private Rigidbody shipRigidBody;


    [SerializeField] Text speedText;



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

        speedText.text = "Speed: " + Mathf.RoundToInt(speed).ToString();

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
}
