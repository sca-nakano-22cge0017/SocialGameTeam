using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BattleRestart : MonoBehaviour
{
    [SerializeField] private WindowController wc;
    [SerializeField] private GameObject window;
    [SerializeField] StageDataManager sdm;

    private LoadManager loadManager;

    private void Start()
    {
        loadManager = FindObjectOfType<LoadManager>();
    }

    void Update()
    {
        if (loadManager && !loadManager.DidFadeComplete) return;

        if (GameManager.isBattleInProgress && SceneManager.GetActiveScene().name == "HomeScene")
        {
            wc.Open();
        }
        else
        {
            wc.Close();
        }
    }

    public void Restart()
    {
        wc.Close();
        sdm.LoadData(GameManager.SelectDifficulty, GameManager.SelectArea, GameManager.SelectStage);
        SceneLoader.LoadScene("MainTest", true);
    }

    public void DataDelete()
    {
        wc.Close();
        GameManager.isBattleInProgress = false;
        MainGameSystem.OngoingBattleInfomation = null;
    }
}
