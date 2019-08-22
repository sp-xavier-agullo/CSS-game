using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FarAwayPlanets : MonoBehaviour
{
    private float speed;
    private Vector3 mySpeed;
    private Rigidbody me;

    [SerializeField] private Rigidbody playerShip;

    void Start()
    {
        //mySpeed = playerShip.velocity;
        me = GetComponent<Rigidbody>();
    }

    void Update()
    {
        mySpeed = playerShip.velocity;
        me.velocity = mySpeed;
    }

}
