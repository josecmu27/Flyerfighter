using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.SceneManagement;


public enum GameState { PreRound, Playing, PostRound }


public class GameManager : MonoBehaviour
{
    public static GameManager S; // define the singleton
    public PauseMenu pauseMenu;
    public HPBar hpBar;
    public GameObject currPlayer;
    public GameState currState;


    [Header("Levels")]
    public GameObject overworldSaveData;
    public GameObject level1SaveData;
    public GameObject level2SaveData;
    public string mostRecentLevel;


    [Header("Player Game Stats")]
    public float playerHP;
    public float playerAirMax;
    public float stopwatchTime = 0f;
    public int tempPeopleRescued;
    public int currPeopleLeftToRescue;
    public int tempCurrPeopleToRescue;
    public int peopleLeftToRescue;
    public int tempTotalPeopleToRescue;
    public int firesExtinguished;
    public int overallScore;

    private void Awake()
    {
        if (GameManager.S)
        {
            Destroy(this.gameObject);
        }
        else
        {
            S = this;
        }

    }


    void Start()
    {
        DontDestroyOnLoad(this);


        if (currState != GameState.Playing)
        {
            // we are starting from the overworld
            FindALevel();
            overworldSaveData.SetActive(true);
        }
        else
        {
            overworldSaveData.SetActive(false);
        }
    }


    void Update()
    {
        // the stop watch only runs when the player is in overworld or in level
        // (Not including win and lose state)
        if (currState != GameState.PostRound)
        {
            stopwatchTime += Time.deltaTime;
        }
    }

    //------------------------------ Game State Functions------------------------------------//

    public void FindALevel()
    {
        currState = GameState.PreRound;
    }


    public void GameOver()
    {
        currState = GameState.PostRound;
        playerHP = 100; // refill player health for next attempt when they restart
        hpBar.currHP = playerHP;

        if (pauseMenu != null)
        {
            pauseMenu.GameOverMenu();
        }
        else
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex); //Reload current Scene
        }
    }


    public void GameWon()
    {
        overallScore = CalculateOverallScore();

        currState = GameState.PostRound;

        // Set Winning Scene
        SceneManager.LoadScene("Win");
    }


    public void SetLevelGameState()
    {
        currState = GameState.Playing;
    }
    public void SetOverworldGameState()
    {
        currState = GameState.PreRound;
    }

    //------------------------------ Game Score Functions------------------------------------//
    public void IncreaseNumPeopleRescuedTemp() // holds the number of people you rescue, until you save the game so it can be reset if you die.
    {
        tempPeopleRescued++;
        tempCurrPeopleToRescue--;
        tempTotalPeopleToRescue--;

        if (tempTotalPeopleToRescue <= 0)
        {
            GameWon();
        }
    }

    public void SetNumPeopleRescued()
    {
        currPeopleLeftToRescue -= tempPeopleRescued; // subtract the temp from the level total
        peopleLeftToRescue -= tempPeopleRescued; // subtract the temp from the game total
        tempPeopleRescued = 0; // set temp back to total


        if (peopleLeftToRescue == 0)
        {
            GameWon();
        }
    }


    public void IncreaseNumFiresExtinct()
    {
        firesExtinguished++;
    }


    public int CalculateOverallScore()
    {
        int fireScore = firesExtinguished * 750;
        int timeScore = Mathf.Max(0, 10000 - Mathf.RoundToInt(stopwatchTime) * 10);

        return fireScore + timeScore;
    }
    
    public void DefinePlayer()
    {
        currPlayer = GameObject.FindGameObjectWithTag("Player");
        pauseMenu = currPlayer.GetComponent<PauseMenu>(); //gets pause menu from player in scene


        GameObject healthGauge = GameObject.Find("HealthGauge");
        hpBar = healthGauge.GetComponent<HPBar>(); // gets hp from health gauge in scene
        hpBar.currHP = playerHP;
    }
}


