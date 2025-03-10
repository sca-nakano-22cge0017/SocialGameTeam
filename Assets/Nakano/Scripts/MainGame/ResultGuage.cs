using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class ResultGuage : MonoBehaviour
{
    [SerializeField] private ResultManager resultManager;

    [SerializeField] private StatusType type;
    public StatusType Type
    {
        get => type;
        private set => type = value;
    }

    [SerializeField] private CombiType combiType;
    public CombiType CombiType
    {
        get => combiType;
        private set => combiType = value;
    }
    [SerializeField] private bool isCombiGuage = false;

    [SerializeField] private Text getPointText;
    [SerializeField] private Image guage;
    [SerializeField] private RankIcon rankIcon;

    private Rank lastRank = (Rank)System.Enum.ToObject(typeof(Rank), 0);
    public Rank LastRank
    {
        get => lastRank;
        set => lastRank = value;
    }
    private Rank currentRank = (Rank)System.Enum.ToObject(typeof(Rank), 0);
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
    private bool isFinalUp = false;

    private int current = 0;
    private int min = 0;
    private int max = 0;
    private float amount = 0;

    private bool isSkip = false;
    private int addAmount = 0;

    private void Update()
    {
        if (!increaseStart || isSkip) return;

        if (addCount == 0)
        {
            // 増加
            if (guage.fillAmount < amount)
            {
                guage.fillAmount += increaseSpeed * Time.deltaTime;

                if (guage.fillAmount >= amount)
                {
                    guage.fillAmount = amount;
                    increaseCompleted = true;
                    increaseStart = false;

                    if (!isCombiGuage) resultManager.CheckFirstDirectionCompleted();
                    else resultManager.CheckSecondDirectionCompleted();
                }
            }

            else
            {
                guage.fillAmount = amount;
                increaseCompleted = true;
                increaseStart = false;

                if (!isCombiGuage) resultManager.CheckFirstDirectionCompleted();
                else resultManager.CheckSecondDirectionCompleted();
            }
        }

        else
        {
            // ランクが変わった回数分、ゲージを最大まで上昇させる演出を挟む
            if (count <= addCount && !isFinalUp)
            {
                if (guage.fillAmount < 1)
                {
                    guage.fillAmount += increaseSpeed * Time.deltaTime;
                }

                if (guage.fillAmount >= 1)
                {
                    // ランクアップ
                    int n = count + (int)lastRank + 1;
                    Rank r = (Rank)Enum.ToObject(typeof(Rank), n);
                    if (r > Rank.SS) r = Rank.SS;

                    rankIcon.RankIconChange(r);

                    guage.fillAmount = 0;

                    count++;
                    if (count >= addCount) isFinalUp = true;
                }
            }

            if (isFinalUp)
            {
                // 増加
                if (guage.fillAmount < amount)
                {
                    guage.fillAmount += increaseSpeed * Time.deltaTime;

                    if (guage.fillAmount >= amount)
                    {
                        guage.fillAmount = amount;
                        increaseCompleted = true;
                        increaseStart = false;

                        if (!isCombiGuage) resultManager.CheckFirstDirectionCompleted();
                        else resultManager.CheckSecondDirectionCompleted();
                    }
                }

                else
                {
                    guage.fillAmount = amount;
                    increaseCompleted = true;
                    increaseStart = false;

                    if (!isCombiGuage) resultManager.CheckFirstDirectionCompleted();
                    else resultManager.CheckSecondDirectionCompleted();
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
        getPointText.text = "+0p/";

        if (!isCombiGuage)
        {
            currentRank = PlayerDataManager.player.GetRank(type);
            current = PlayerDataManager.player.GetRankPt(type);
            min = PlayerDataManager.player.GetRankPtLastUp(type);
            max = PlayerDataManager.player.GetRankPtNextUp(type);
        }
        else
        {
            currentRank = PlayerDataManager.player.GetCombiRank(combiType);
            current = PlayerDataManager.player.GetCombiRankPt(combiType);
            
            min = PlayerDataManager.player.GetCombiRankPtLastUp(combiType);
            max = PlayerDataManager.player.GetCombiRankPtNextUp(combiType);

            //Debug.Log($"test 初期化 Type: {combiType} CurrentRank: {currentRank}, LastRank: {lastRank}, Min: {min}, Max: {max}, Current: {current}");
        }

        addAmount = 0;
        addCount = 0;
        amount = (float)(current - min) / (max - min);
        guage.fillAmount = amount;
        rankIcon.RankIconChange(currentRank);
    }

    /// <summary>
    /// 獲得ポイント表示
    /// </summary>
    /// <param name="_amount">獲得量</param>
    public void SetPointText(int _amount)
    {
        getPointText.gameObject.SetActive(true);
        getPointText.text = "+" + _amount + "p/";

        addAmount = _amount;
    }

    public void IncreaseAmount()
    {
        if (isSkip) return;

        // 増加なし
        if (addAmount <= 0)
        {
            increaseStart = false;
            increaseCompleted = true;

            if (!isCombiGuage) resultManager.CheckFirstDirectionCompleted();
            else resultManager.CheckSecondDirectionCompleted();

            return;
        }

        if (!isCombiGuage)
        {
            currentRank = PlayerDataManager.player.GetRank(type);
            current = PlayerDataManager.player.GetRankPt(type);
            min = PlayerDataManager.player.GetRankPtLastUp(type);
            max = PlayerDataManager.player.GetRankPtNextUp(type);
        }
        else
        {
            currentRank = PlayerDataManager.player.GetCombiRank(combiType);
            current = PlayerDataManager.player.GetCombiRankPt(combiType);

            min = PlayerDataManager.player.GetCombiRankPtLastUp(combiType);
            max = PlayerDataManager.player.GetCombiRankPtNextUp(combiType);

            //Debug.Log($"test 増加開始 Type: {combiType} CurrentRank: {currentRank}, LastRank: {lastRank}, Min: {min}, Max: {max}, Current: {current}");
        }

        // 増加量計算
        amount = (float)(current - min) / (max - min);

        int l = (int)lastRank;
        int c = (int)currentRank;
        addCount = c - l;
        count = 0;
        isFinalUp = false;

        increaseCompleted = false;
        increaseStart = true;
    }

    /// <summary>
    /// 増加演出スキップ
    /// </summary>
    public void Skip()
    {
        isSkip = true;
        increaseStart = false;
        increaseCompleted = true;
        
        Master.CharacterRankPoint rankPtData = PlayerDataManager.player.StatusData.rankPoint;

        if (!isCombiGuage)
        {
            currentRank = PlayerDataManager.player.GetRank(type);
            current = PlayerDataManager.player.GetRankPt(type);
            min = currentRank - 1 <= Rank.D ? 0 : rankPtData.rankPt_NextUp[currentRank - 1].GetStatus(type);
            max = rankPtData.rankPt_NextUp[currentRank].GetStatus(type);
        }
        else
        {
            currentRank = PlayerDataManager.player.GetCombiRank(combiType);
            current = PlayerDataManager.player.GetCombiRankPt(combiType);

            min = PlayerDataManager.player.GetCombiRankPtLastUp(combiType);
            max = PlayerDataManager.player.GetCombiRankPtNextUp(combiType);
        }

        // 増加量計算
        amount = (float)(current - min) / (float)(max - min);
        guage.fillAmount = amount;

        rankIcon.RankIconChange(currentRank);
    }
}
