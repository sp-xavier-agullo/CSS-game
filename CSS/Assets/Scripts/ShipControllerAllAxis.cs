using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipControllerAllAxis : MonoBehaviour
{
    private Rigidbody ship;
    public float turnSpeed = 10f;
    public float speed = 50f;
    public float speedMultiplier;
    private float horizontalInput;
    private float verticalInput;
    public float stabilize = 0.8f;


    [SerializeField] GameObject myShip;
    [SerializeField] GameObject target;


    void Awake()
    {
        ship = GetComponent<Rigidbody>();
    }

    void Update()
    {
        horizontalInput = Input.GetAxis("Horizontal");
        verticalInput = Input.GetAxis("Vertical");

    }

    private void FixedUpdate()
    {
        ship.AddRelativeForce(0f, 0f, speed);
        ship.AddRelativeTorque(verticalInput * turnSpeed, 0f, -horizontalInput * turnSpeed);
        //roll = ship.transform.rotation.z;
        //rollVertical = ship.transform.rotation.y;
        //float tiltAroundZ = -horizontalInput * turnSpeed;
        //float tiltAroundX = verticalInput * turnSpeed;
        //Quaternion target = Quaternion.Euler(tiltAroundX, 0, tiltAroundZ);

        if (horizontalInput == 0f && verticalInput == 0)

        //{
        //    ship.transform.Rotate(ship.transform.rotation.z * 0.5f, 0f, ship.transform.rotation.y * 0.5f, Space.Self);
        //}      
    }
}
