using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipTarget : MonoBehaviour
{
    Vector3 myPosition;
    [SerializeField] GameObject ship;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        myPosition = new Vector3(ship.transform.position.x, ship.transform.position.y, ship.transform.localPosition.z + 5f);
        transform.position = myPosition;
    }
}
