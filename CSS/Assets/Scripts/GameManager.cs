using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{

    public static GameManager Instance;

    public GameObject playerShip;
    public GameObject radarShip;

    public List<GameObject> enemyList = new List<GameObject>();


    // Start is called before the first frame update
    void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
        }

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // Kill an enemy
    public void killShipNum (int idNum)
    {
        radarShip.GetComponent<RadarController>().enemyPointerList[idNum].SetActive(false);
        enemyList[idNum].SetActive(false);
    }
}
