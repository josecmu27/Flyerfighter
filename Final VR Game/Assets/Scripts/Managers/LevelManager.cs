using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    public string nextLevelName;
    public string mainOverWorldName;

    private bool isOverWorld;

    public int totalPeopleToRescue;

    public GameObject playerObject;
    public GameObject levelSaveData;
    public GameObject[] savepoints;
    public Transform levelBackSpawnTransform;

    private void Start()
    {
        if (nextLevelName == mainOverWorldName) // we are currently in a game level
        {
            int peopleHolderIndex = 0;
            isOverWorld = false;

            LoadLevelData();

            totalPeopleToRescue = levelSaveData.transform.GetChild(peopleHolderIndex).transform.childCount;

            GameManager.S.currPeopleLeftToRescue = totalPeopleToRescue;
            GameManager.S.tempCurrPeopleToRescue = totalPeopleToRescue; // sets the temp value to be displayed on watch
            GameManager.S.tempTotalPeopleToRescue = GameManager.S.peopleLeftToRescue; // set temp total value equal to overall value when starting level
            GameManager.S.SetLevelGameState(); // set Gamestate to Playing
            GameManager.S.overworldSaveData.SetActive(false); // turn off gas masks
        }
        else // we are currently in the overworld
        {
            if (GameManager.S.mostRecentLevel == nextLevelName) // set player location near level building once they come back from level
            {
                playerObject.transform.position = levelBackSpawnTransform.position;
                GameManager.S.mostRecentLevel = "None";
            }

            isOverWorld = true;

            GameManager.S.currPeopleLeftToRescue = GameManager.S.peopleLeftToRescue;
            GameManager.S.SetOverworldGameState(); // set Gamestate to Preround
            GameManager.S.overworldSaveData.SetActive(true); // turn on gas masks
        }

        //Assign player prefab in this scene to the game manager to use the pause menu
        GameManager.S.DefinePlayer();
    }

    public void ReloadSavePoints(GameObject newSaveData)
    {
        foreach (GameObject obj in savepoints)
        {
            obj.GetComponent<Savepoint>().levelSaveData = newSaveData;
        }
    }
    private void LoadLevelData()
    {
        string levelNum = gameObject.tag;

        switch (levelNum)
        {
            case "Level1":
                if (GameManager.S.level1SaveData != null) // player has made progress before
                {
                    Destroy(levelSaveData);

                    // load player's level data
                    GameObject reloadedSaveData = Instantiate(GameManager.S.level1SaveData.gameObject);
                    levelSaveData = reloadedSaveData;
                    levelSaveData.gameObject.SetActive(true);

                    // deactivate player's save data
                    GameManager.S.level1SaveData.SetActive(false);
                }
                break;
            case "Level2":
                if (GameManager.S.level2SaveData != null) // player has made progress before
                {
                    Destroy(levelSaveData);

                    // load player's level data
                    GameObject reloadedSaveData = Instantiate(GameManager.S.level2SaveData.gameObject);
                    levelSaveData = reloadedSaveData;
                    levelSaveData.gameObject.SetActive(true);

                    // deactivate player's save data
                    GameManager.S.level2SaveData.SetActive(false);
                }
                break;
            default:
                Debug.Log("We are at the Overworld. We should not be loading anything as of 04/05/2025");
                break;
        }
    }

    public void EnterLevel()
    {
        if (nextLevelName == mainOverWorldName)
        {
            // we are currently in a game level
            savepoints[0].GetComponent<Savepoint>().SaveLevelData(); //Save data upon leaving level
        }
        
        SceneManager.LoadScene(nextLevelName);

        if (isOverWorld)
        {
            GameManager.S.mostRecentLevel = nextLevelName;
            GameManager.S.currState = GameState.Playing;
        }
        else
        {
            // player goes back to overworld and can look for another level or return later
            GameManager.S.FindALevel();
        }
    }


}
