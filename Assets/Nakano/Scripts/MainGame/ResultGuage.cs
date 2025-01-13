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
    [SerializeField] private Text rankText;
    [SerializeField] private Image guage;

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

    private void Update()
    {
        if (increaseStart)
        {
            if (addCount == 0)
            {
                if (!isCombiGuage)
                {
                    current = PlayerDataManager.player.GetRankPt(type);
                    min = PlayerDataManager.player.GetRankPtLastUp(type);
                    max = PlayerDataManager.player.GetRankPtNextUp(type);
                }
                else
                {
                    current = PlayerDataManager.player.GetCombiRankPt(combiType);

                    Master.CharacterRankPoint rankPtData = PlayerDataManager.player.StatusData.rankPoint;

                    if (currentRank - 1 < Rank.D) min = 0;
                    else if (currentRank - 1 >= Rank.SS) min = rankPtData.GetCombiRankNextPt(combiType, Rank.S);
                    else min = rankPtData.GetCombiRankNextPt(combiType, currentRank - 1);

                    max = rankPtData.GetCombiRankNextPt(combiType, currentRank);
                }

                // 増加量計算
                amount = (float)(current - min) / (max - min);
                
                // 増加
                if (guage.fillAmount <= amount)
                {
                    guage.fillAmount += increaseSpeed * Time.deltaTime;
                }

                else
                {
                    if (guage.fillAmount >= 1)
                    {
                        // ランクアップ
                        int n = count + (int)lastRank + 1;
                        Rank r = (Rank)Enum.ToObject(typeof(Rank), n);
                        if (r > Rank.SS) r = Rank.SS;

                        rankText.text = r.ToString();

                        guage.fillAmount = 0;
                    }

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
                    if (guage.fillAmount <= 1)
                    {
                        guage.fillAmount += increaseSpeed * Time.deltaTime;
                    }

                    if (guage.fillAmount >= 1)
                    {
                        // ランクアップ
                        int n = count + (int)lastRank + 1;
                        Rank r = (Rank)Enum.ToObject(typeof(Rank), n);
                        if (r > Rank.SS) r = Rank.SS;

                        rankText.text = r.ToString();

                        guage.fillAmount = 0;

                        count++;
                        if(count >= addCount) isFinalUp = true;
                    }
                }

                if (isFinalUp)
                {
                    if (!isCombiGuage)
                    {
                        current = PlayerDataManager.player.GetRankPt(type);
                        min = PlayerDataManager.player.StatusData.rankPoint.rankPt_NextUp[lastRank].GetStatus(type);
                        max = PlayerDataManager.player.StatusData.rankPoint.rankPt_NextUp[currentRank].GetStatus(type);
                    }
                    else
                    {
                        current = PlayerDataManager.player.GetCombiRankPt(combiType);

                        Master.CharacterRankPoint rankPtData = PlayerDataManager.player.StatusData.rankPoint;

                        if (currentRank -1 < Rank.D) min = 0;
                        else if (currentRank - 1 >= Rank.SS) min = rankPtData.GetCombiRankNextPt(combiType, Rank.S);
                        else min = rankPtData.GetCombiRankNextPt(combiType, currentRank - 1);

                        max = rankPtData.GetCombiRankNextPt(combiType, currentRank);
                    }

                    // 増加量計算
                    amount = (float)(current - min) / (max - min);

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

                        if (!isCombiGuage) resultManager.CheckFirstDirectionCompleted();
                        else resultManager.CheckSecondDirectionCompleted();
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
            Master.CharacterRankPoint rankPtData = PlayerDataManager.player.StatusData.rankPoint;

            currentRank = PlayerDataManager.player.GetCombiRank(combiType);
            current = PlayerDataManager.player.GetCombiRankPt(combiType);
            
            if (currentRank - 1 < Rank.D) min = 0;
            else if (currentRank - 1 >= Rank.SS) min = rankPtData.GetCombiRankNextPt(combiType, Rank.S);
            else min = rankPtData.GetCombiRankNextPt(combiType, currentRank - 1);

            max = rankPtData.GetCombiRankNextPt(combiType, currentRank);
        }

        amount = (float)(current - min) / (max - min);
        guage.fillAmount = amount;
        rankText.text = currentRank.ToString();
    }

    /// <summary>
    /// 獲得ポイント表示
    /// </summary>
    /// <param name="_amount">獲得量</param>
    public void SetPointText(int _amount)
    {
        getPointText.gameObject.SetActive(true);
        getPointText.text = "+" + _amount + "p/";
    }

    public void IncreaseAmount()
    {
        if (isSkip) return;

        increaseCompleted = false;
        increaseStart = true;

        int l = (int)lastRank;
        int c = (int)currentRank;
        addCount = c - l;
        count = 0;
        isFinalUp = false;
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

            if (currentRank - 1 < Rank.D) min = 0;
            else if (currentRank - 1 >= Rank.SS) min = rankPtData.GetCombiRankNextPt(combiType, Rank.S);
            else min = rankPtData.GetCombiRankNextPt(combiType, currentRank - 1);

            max = rankPtData.GetCombiRankNextPt(combiType, currentRank);
        }

        // 増加量計算
        amount = (float)(current - min) / (float)(max - min);
        guage.fillAmount = amount;

        rankText.text = currentRank.ToString();
    }
}
