using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RadarController : MonoBehaviour
{

    public Canvas radarBaseCanvas;
    public GameObject enemyPointer;
	public GameObject playerShip;

    public List<GameObject> enemyPointerList = new List<GameObject>();


    private float maxRadarWorldRange = 1000;
    private float minRadarWorldRange = 10;

    private float maxRadarCanvasRange = 80;
    private float minRadarCanvasRange = 10;

    // Start is called before the first frame update
    void Start()
    {
        foreach (GameObject myEnemyShip in GameManager.Instance.enemyList)
        {
            GameObject myEnemyPointer = Instantiate(enemyPointer, transform);
            enemyPointerList.Add(myEnemyPointer);
        }
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = new Vector3(playerShip.transform.position.x, 5, playerShip.transform.position.z);

        // Update Pointers
        for (int i = 0; i < enemyPointerList.Count; i++)
        {
            // adjust rotation
            enemyPointerList[i].transform.rotation = Quaternion.Euler (new Vector3 (90, GameManager.Instance.enemyList[i].transform.eulerAngles.y, 0));

            // adjust position
            float enemyPositionX = GameManager.Instance.enemyList[i].transform.position.x - playerShip.transform.position.x;
            float enemyPositionY = GameManager.Instance.enemyList[i].transform.position.z - playerShip.transform.position.z;

            float newXposition = Utils.convertToNewRange(-maxRadarWorldRange, +maxRadarWorldRange, -maxRadarCanvasRange, maxRadarCanvasRange, enemyPositionX);
            float newYposition = Utils.convertToNewRange(-maxRadarWorldRange, +maxRadarWorldRange, -maxRadarCanvasRange, maxRadarCanvasRange, enemyPositionY);

			enemyPointerList[i].transform.localPosition = new Vector3(newXposition, 0, newYposition);

            // adjust alpha

            //enemyPointerList[i].gameObject.GetComponent<SpriteRenderer>().

        }
    }
}
