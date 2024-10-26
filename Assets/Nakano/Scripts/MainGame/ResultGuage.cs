using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class ResultGuage : MonoBehaviour
{
    [SerializeField] private StatusType type;
    public StatusType Type
    {
        get => type;
        private set => type = value;
    }
    
    [SerializeField] private Text getPointText;
    [SerializeField] private Text rankText;
    [SerializeField] private Image guage;

    private Rank lastRank = Rank.C;
    public Rank LastRank
    {
        get => lastRank;
        set => lastRank = value;
    }
    private Rank currentRank = Rank.C;
    public Rank CurrentRank
    {
        get => currentRank;
        set => currentRank = value;
    }

    [SerializeField] private float increaseSpeed;

    private bool increaseCompleted = false;
    public bool IncreaseCompleted
    {
        get => increaseCompleted;
    }

    /// <summary>
    /// 表示初期化
    /// </summary>
    public void Initialize()
    {
        getPointText.gameObject.SetActive(false);
        getPointText.text = "+0pt";
        rankText.text = PlayerDataManager.player.GetRank(type).ToString();

        int current = PlayerDataManager.player.GetRankPt(type);
        int min = PlayerDataManager.player.GetRankPtLastUp(type);
        int max = PlayerDataManager.player.GetRankPtNextUp(type);

        float amount = (float)(current - min) / (max - min);
        guage.fillAmount = amount;
    }

    /// <summary>
    /// 獲得ポイント表示
    /// </summary>
    /// <param name="_amount">獲得量</param>
    public void SetPointText(int _amount)
    {
        getPointText.gameObject.SetActive(true);
        getPointText.text = "+" + _amount + "pt";
    }

    public void IncreaseAmount()
    {
        increaseCompleted = false;
        StartCoroutine(Increase());
    }

    IEnumerator Increase()
    {
        int l = (int)lastRank;
        int c = (int)currentRank;
        int count = c - l;

        if (count == 0)
        {
            int current = PlayerDataManager.player.GetRankPt(type);
            int min = PlayerDataManager.player.GetRankPtLastUp(type);
            int max = PlayerDataManager.player.GetRankPtNextUp(type);

            // 増加量計算
            float amount = (float)(current - min) / (max - min);

            // 増加
            while (guage.fillAmount < amount)
            {
                guage.fillAmount += increaseSpeed * Time.deltaTime;
                yield return new WaitForEndOfFrame();

                if (guage.fillAmount >= amount)
                {
                    guage.fillAmount = amount;
                    break;
                }
            }
        }

        else
        {
            for (int i = 0; i <= count; i++)
            {
                Rank lRank = (Rank)Enum.ToObject(typeof(Rank), i);
                Rank cRank = (Rank)Enum.ToObject(typeof(Rank), i + 1);

                int current = PlayerDataManager.player.GetRankPt(type);
                int min = PlayerDataManager.player.GetRankPtUp(type, lRank);
                int max = PlayerDataManager.player.GetRankPtUp(type, cRank);

                if (type == StatusType.HP)
                    Debug.Log(min + " / " + max);

                // 増加量計算
                float amount = (float)(current - min) / (max - min);

                // 増加
                while (guage.fillAmount < amount)
                {
                    guage.fillAmount += increaseSpeed * Time.deltaTime;
                    yield return new WaitForEndOfFrame();
                }

                guage.fillAmount = amount;

                // ランクアップ
                if (amount >= 1)
                {
                    Debug.Log("test");
                    Rank r = (Rank)Enum.ToObject(typeof(Rank), i + 1);
                    rankText.text = r.ToString();
                }

                yield return new WaitForEndOfFrame();
            }
        }

        increaseCompleted = true;
    }
}
