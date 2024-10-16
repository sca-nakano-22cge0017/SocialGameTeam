using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

[System.Serializable]
public class Result
{
    public StatusType type;
    public Text getPointText;
    public Image guage;
}

[System.Serializable]
public class RankResult
{
    public CombiRankType type;
    public Text rankText;
}

public class ResultManager : MonoBehaviour
{
    [SerializeField] private DropController dropController;
    [SerializeField] private Result[] results;
    [SerializeField] private RankResult[] rankResults;

    private bool resultDispCompleted = false;

    private GameManager gameManager;

    [SerializeField] WindowController wc;
    
    void Start()
    {
        gameManager = FindObjectOfType<GameManager>();
        if (gameManager == null)
        {
            GameManager.GameManagerCreate();
            gameManager = GameManager.Instance;
        }
    }

    private void OnEnable()
    {
        ResultInitialize();

        // Debug
        resultDispCompleted = true;

        AddRankPoint();
    }

    void Update()
    {
        
    }

    /// <summary>
    /// ƒ|ƒCƒ“ƒg‰ÁŽZ
    /// </summary>
    void AddRankPoint()
    {
        for (int i = 0; i < dropController.DropedItems.Count; i++)
        {
            for (int j = 0; j < results.Length; j++)
            {
                int amount = dropController.DropedItems[i].dropAmount;
                StatusType type = dropController.DropedItems[i].itemType;

                if (type == results[j].type && amount > 0)
                {
                    results[j].getPointText.gameObject.SetActive(true);
                    results[j].getPointText.text = "+" + amount + "pt";

                    PlayerDataManager.RankPtUp(type, amount);
                }
            }
        }

        ResultUpdate();
    }

    void ResultInitialize()
    {
        for (int i = 0; i < results.Length; i++)
        {
            results[i].getPointText.gameObject.SetActive(false);
            results[i].getPointText.text = "+0pt";
        }
    }

    void ResultUpdate()
    {
        for (int i = 0; i < results.Length; i++)
        {
            int current = PlayerDataManager.player.GetRankPt(results[i].type);
            int max = PlayerDataManager.player.GetRankPtMax(results[i].type);

            results[i].guage.fillAmount = (float)current / max;
        }

        for (int i = 0; i < rankResults.Length; i++)
        {
            rankResults[i].rankText.text = (PlayerDataManager.player.GetCombiRank(rankResults[i].type)).ToString();
        }
    }

    public void ToSelect()
    {
        if (!resultDispCompleted) return;

        if (GameManager.SelectArea == 1)
        {
            wc.Close();
            //SceneManager.LoadScene("SelectScene_Traning");
        }
        if (GameManager.SelectArea == 2)
        {
            SceneManager.LoadScene("SelectScene_Boss");
        }
    }
}
