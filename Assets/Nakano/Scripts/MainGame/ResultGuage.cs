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

    private bool increaseStart = false;

    private bool increaseCompleted = false;
    public bool IncreaseCompleted
    {
        get => increaseCompleted;
    }

    private int addCount = 0;
    private int count = 0;

    private void Update()
    {
        if (increaseStart)
        {
            if (addCount == 0)
            {
                int current = PlayerDataManager.player.GetRankPt(type);
                int min = PlayerDataManager.player.GetRankPtLastUp(type);
                int max = PlayerDataManager.player.GetRankPtNextUp(type);

                // 増加量計算
                float amount = (float)(current - min) / (max - min);

                // 増加
                if (guage.fillAmount <= amount)
                {
                    guage.fillAmount += increaseSpeed * Time.deltaTime;
                }

                else
                {
                    guage.fillAmount = amount;
                    increaseCompleted = true;
                    increaseStart = false;
                }
            }

            else
            {
                // ランクが変わった回数分、ゲージを最大まで上昇させる演出を挟む
                if (count <= addCount)
                {
                    if (guage.fillAmount <= 1)
                    {
                        guage.fillAmount += increaseSpeed * Time.deltaTime;
                    }

                    if (guage.fillAmount >= 1)
                    {
                        // ランクアップ
                        int n = count + (int)lastRank + 1;
                        Rank r = (Rank)Enum.ToObject(typeof(Rank), n);
                        rankText.text = r.ToString();

                        guage.fillAmount = 0;

                        count++;
                        if(count >= addCount) addCount = 0;
                    }
                }
            }
        }
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
        increaseStart = true;

        int l = (int)lastRank;
        int c = (int)currentRank;
        addCount = c - l;
        count = 0;
    }
}
