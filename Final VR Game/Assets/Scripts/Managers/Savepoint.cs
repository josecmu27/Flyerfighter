using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using TMPro;
public class Savepoint : MonoBehaviour
{
    public LevelManager levelManager;

    public GameObject levelSaveData;
    public TextMeshProUGUI monitorText;

    private bool hasSaved;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        monitorText.text = "Save Your Progress Here";
    }

    private void SaveMessage()
    {
        monitorText.text = "Progress Saved";
    }

    public void SaveLevelData()
    {
        string levelNum = gameObject.tag;

        GameManager.S.SetNumPeopleRescued(); // set number of people on save

        switch (levelNum)
        {
            case "Level1":

                if (GameManager.S.level1SaveData.gameObject != null) // player has saved data from this level
                {
                    Destroy(GameManager.S.level1SaveData);
                }

                levelSaveData.transform.SetParent(GameManager.S.transform);

                GameManager.S.level1SaveData = levelSaveData;
                GameManager.S.level1SaveData.SetActive(false);

                GameObject reloadedData = Instantiate(GameManager.S.level1SaveData.gameObject);

                reloadedData.SetActive(true);

                levelManager.ReloadSavePoints(reloadedData);
    
                break;
            case "Level2":
                if (GameManager.S.level2SaveData.gameObject != null) // player has saved data from this level
                {
                    Destroy(GameManager.S.level2SaveData);
                }

                levelSaveData.transform.SetParent(GameManager.S.transform);

                GameManager.S.level2SaveData = levelSaveData;
                GameManager.S.level2SaveData.SetActive(false);

                GameObject reloadedData2 = Instantiate(GameManager.S.level2SaveData.gameObject);

                reloadedData2.SetActive(true);

                levelManager.ReloadSavePoints(reloadedData2);

                break;
            default:
                Debug.Log("We are at the Overworld. We should not be loading anything as of 04/05/2025");
                break;
        }
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == ("PlayerHand") && !hasSaved)
        {
            // only save progress once so player doesn't repeatedly use the save point
            SaveLevelData();
            SaveMessage();
            hasSaved = true;
            SoundManager.Instance.PlaySound("Save");
        }
    }
}
