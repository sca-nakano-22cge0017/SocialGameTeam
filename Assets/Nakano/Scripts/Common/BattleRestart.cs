using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BattleRestart : MonoBehaviour
{
    [SerializeField] private GameObject window;
    [SerializeField] StageDataManager sdm;

    private void Awake()
    {
        window.SetActive(false);
    }

    void Update()
    {
        if (GameManager.isBattleInProgress && SceneManager.GetActiveScene().name == "HomeScene")
        {
            if (!window.activeSelf) window.SetActive(true);
        }
        else
        {
            if (window.activeSelf) window.SetActive(false);
        }
    }

    public void Restart()
    {
        window.SetActive(false);
        sdm.LoadData(GameManager.SelectDifficulty, GameManager.SelectArea, GameManager.SelectStage);
        SceneLoader.LoadScene("MainTest", true);
    }

    public void DataDelete()
    {
        GameManager.isBattleInProgress = false;
        MainGameSystem.OngoingBattleInfomation = null;
        window.SetActive(false);
    }
}
