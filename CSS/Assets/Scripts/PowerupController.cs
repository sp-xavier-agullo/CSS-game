using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PowerupController : MonoBehaviour
{

    [SerializeField] Canvas canvasPowerup;

    private float distanceToTarget;
    private float magnetDistance = 100;

    private Rigidbody powerupRigidBody;

    // Start is called before the first frame update
    void Start()
    {
        powerupRigidBody = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        canvasPowerup.transform.LookAt(GameManager.Instance.mainCamera.transform);

        distanceToTarget = Vector3.Distance(transform.position, GameManager.Instance.playerShip.transform.position);

        if (distanceToTarget < magnetDistance)
        {

            Vector3 targetDir = GameManager.Instance.playerShip.transform.position - transform.position;

            //Vector3 GravityDirection = targetDir.Normalize();

            powerupRigidBody.AddForce(targetDir*2);
        }
    }
}
