using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{

    public static GameManager Instance;

    public GameObject playerShip;
    public GameObject radarShip;

    public GameObject mainCamera;

    public List<GameObject> enemyList = new List<GameObject>();
    public List<GameObject> wingmanList = new List<GameObject>();

    public GameObject youWinPopup;
    public GameObject youLosePopup;

    public GameObject explosionShip;

    public int numShooters;

    private GameObject activeEnemyTarget;


    // Awake is called before the first frame update
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }

    }

    // Start
    void Start()
    {
        AssignShooters();
    }

    // Kill an enemy
    public void killShipNum(int idNum)
    {
        radarShip.GetComponent<RadarController>().enemyPointerList[idNum].SetActive(false);
        enemyList[idNum].SetActive(false);
        AssignShooters();

        int numEnemies = CountEnemies();
        Debug.Log("num enemies is: " + numEnemies.ToString());

        if (numEnemies < 1)
        {
            levelWin();
        }
    }

    // Level Win
    public void levelWin()
    {
        playerShip.transform.GetComponent<ShipControllerSergio>().shipDeadTimeline.Play();
        StartCoroutine(showPopup("win"));
    }

    // Level Lose
    public void levelLose()
    {
        StartCoroutine(showPopup("lose"));
    }

    IEnumerator showPopup(string winLose)
    {
        yield return new WaitForSeconds(5f);

        if (winLose == "win")
        {
            youWinPopup.SetActive(true);
        } else if (winLose == "lose")
        {
            youLosePopup.SetActive(true);
        }


    }

    // Assign new EnemyTarget
    public void AssignNewEnemyTarget (int enemyID)
    {
        for (int i=0; i < enemyList.Count; i++)
        {
            enemyList[i].GetComponent<EnemyController>().setTargetPointer(false);
        }

        if (enemyID != -1)
        {
            enemyList[enemyID].GetComponent<EnemyController>().setTargetPointer(true);
        }

    }


    // Go to Main Menu
    public void goToMainMenu()
    {
        SceneManager.LoadScene("LoadingScene");
    }

    // Assign Shooters
    public void AssignShooters()
    {
        // shooters to wingman

        for (int i = 0; i < enemyList.Count; i++)
        {
            if (enemyList[i].GetComponent<EnemyController>().currentAiMode != EnemyController.AiMode.isDead)
            {

                int currentNumWingman = CountWingmen();

                if (currentNumWingman > 0)
                {
                    enemyList[i].GetComponent<EnemyController>().currentAiMode = EnemyController.AiMode.ChaseWingman;
                }
                else
                {
                    enemyList[i].GetComponent<EnemyController>().currentAiMode = EnemyController.AiMode.RoamAround;
                }

            }
        }

        // shooters to player
        int currentShooters = 0;

        for (int i = 0; i < enemyList.Count; i++)
        {
            if (currentShooters < numShooters)
            {
                if (enemyList[i].GetComponent<EnemyController>().currentAiMode != EnemyController.AiMode.isDead)
                {
                    enemyList[i].GetComponent<EnemyController>().currentAiMode = EnemyController.AiMode.ChasePlayer;
                    currentShooters++;
                }
            }
        }

    }

    // Select Target
    public GameObject SelectTarget()
    {

        GameObject newTarget = null;
        List<GameObject> aliveEnemyList = new List<GameObject>();

        for (int i = 0; i < enemyList.Count; i++)
        {
            if (enemyList[i].activeSelf)
            {
                GameObject selectedEnemy = enemyList[i];
                aliveEnemyList.Add(selectedEnemy);
            }
        }

        if (aliveEnemyList.Count > 0)
        {
            int randomNumber = Random.Range(0, aliveEnemyList.Count);
            newTarget = aliveEnemyList[randomNumber];
        }

        return newTarget;
    }

    // Select Target
    public GameObject SelectWingman()
    {

        GameObject newTarget = null;
        List<GameObject> aliveWingmanList = new List<GameObject>();

        for (int i = 0; i < wingmanList.Count; i++)
        {
            if (wingmanList[i].activeSelf)
            {
                GameObject selectedWingman = wingmanList[i];
                aliveWingmanList.Add(selectedWingman);
            }
        }

        if (aliveWingmanList.Count > 0)
        {
            int randomNumber = Random.Range(0, aliveWingmanList.Count);
            newTarget = aliveWingmanList[randomNumber];
        }

        return newTarget;
    }

    // Count Enemies
    public int CountEnemies()
    {
        int numEnemies = 0;

        for (int i = 0; i < enemyList.Count; i++)
        {
            if (enemyList[i].activeSelf)
            {
                numEnemies++;
            }
        }

        return numEnemies;

    }

    // Count Wingmen
    public int CountWingmen()
    {
        int numWingmen = 0;

        for (int i = 0; i < wingmanList.Count; i++)
        {
            if (wingmanList[i].activeSelf)
            {
                numWingmen++;
            }
        }

        return numWingmen;

    }
}
