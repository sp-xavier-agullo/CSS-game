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

    public FixedJoystick fixedJoystick;

    [SerializeField] GameObject myShip;

    void Awake()
    {
        ship = GetComponent<Rigidbody>();
    }

    void Update()
    {
        horizontalInput = fixedJoystick.Horizontal;
        verticalInput = fixedJoystick.Vertical;

    }

    private void FixedUpdate()
    {
        ship.AddRelativeForce(0f, 0f, speed);
        ship.AddRelativeTorque(verticalInput * turnSpeed, 0f, -horizontalInput * turnSpeed);
        Quaternion shipR = ship.transform.rotation;
        //Vector3 shipAngles = shipR.eulerAngles;
        //roll = ship.transform.rotation.z;
        //rollVertical = ship.transform.rotation.y;
        //float tiltAroundZ = -horizontalInput * turnSpeed;
        //float tiltAroundX = verticalInput * turnSpeed;
        //Quaternion target = Quaternion.Euler(tiltAroundX, 0, tiltAroundZ);

        //if (horizontalInput == 0f && verticalInput == 0)

        //{
        //    Debug.Log(shipR);
        //}      
    }
}
