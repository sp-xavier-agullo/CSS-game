using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{

    public static GameManager Instance;

    public GameObject playerShip;
    public GameObject radarShip;

    public List<GameObject> enemyList = new List<GameObject>();

    public GameObject youWinPopup;
    public GameObject youLosePopup;

    public int numShooters;


    // Start is called before the first frame update
    void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
        }

    }

    // Update is called once per frame
    void Start()
    {
        AssignShooters();
    }

    // Kill an enemy
    public void killShipNum (int idNum)
    {
        radarShip.GetComponent<RadarController>().enemyPointerList[idNum].SetActive(false);
        enemyList[idNum].SetActive(false);
        AssignShooters();

        int numEnemies = CountEnemies();
        Debug.Log("num enemies is: " + numEnemies.ToString());

        if (numEnemies < 1 )
        {
            levelWin();
        }
    }

    // Level Win
    public void levelWin ()
    {
        youWinPopup.SetActive(true);
        Time.timeScale = 0;
    }

    // Level Lose
    public void levelLose()
    {
        youLosePopup.SetActive(true);
        Time.timeScale = 0;
    }

    // Go to Main Menu
    public void goToMainMenu ()
    {
        SceneManager.LoadScene("LoadingScene");
    }

    // Assign Shooters
    public void AssignShooters ()
    {
        int currentShooters = 0;

        for (int i=0; i<enemyList.Count; i++)
        {
            if (enemyList[i].GetComponent<EnemyController>().currentAiMode==EnemyController.AiMode.ChasePlayer)
            {
                currentShooters++;
            }
        }
        
        if (currentShooters<numShooters)
        {
            for (int i=currentShooters; i<numShooters; i++)
            {
                for (int j=0; j<enemyList.Count; j++)
                {
                    if (enemyList[j].activeSelf && enemyList[j].GetComponent<EnemyController>().currentAiMode == EnemyController.AiMode.RoamAround)
                    {
                        enemyList[j].GetComponent<EnemyController>().currentAiMode = EnemyController.AiMode.ChasePlayer;
                        break;
                    }
                }
            }
        }

    }

    // Count Enemies
    public int CountEnemies ()
    {
        int numEnemies = 0;

        for (int i=0; i<enemyList.Count; i++)
        {
            if (enemyList[i].activeSelf)
            {
                numEnemies++;
            }
        }

        return numEnemies;

    }


}
