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
    public Text rankText;
}

public class ResultManager : MonoBehaviour
{
    [SerializeField] private WindowController wc;
    [SerializeField] private DropController dropController;
    [SerializeField] private Result[] results;

    private bool resultDispCompleted = false;

    private GameManager gameManager;

    void Start()
    {
        gameManager = GameManager.Instance;
    }

    private void OnEnable()
    {
        ResultInitialize();

        // Debug
        StartCoroutine(Direction());

        if (dropController.DropedItems.Count == 0) return;
        
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
            int min = PlayerDataManager.player.GetRankPtLastUp(results[i].type);
            int max = PlayerDataManager.player.GetRankPtNextUp(results[i].type);

            results[i].guage.fillAmount = (float)(current - min) / (max - min);
            results[i].rankText.text = (PlayerDataManager.player.GetRank(results[i].type)).ToString();
        }
    }

    public void ToSelect()
    {
        if (!resultDispCompleted) return;

        if (GameManager.SelectArea == 1)
        {
            //wc.Close();
            SceneManager.LoadScene("SelectScene_Traning");
        }
        else if (GameManager.SelectArea == 2)
        {
            SceneManager.LoadScene("SelectScene_Boss");
        }
        else SceneManager.LoadScene("SelectScene_Traning");
    }

    IEnumerator Direction()
    {
        yield return new WaitForSeconds(0.1f);
        resultDispCompleted = true;
    }
}
