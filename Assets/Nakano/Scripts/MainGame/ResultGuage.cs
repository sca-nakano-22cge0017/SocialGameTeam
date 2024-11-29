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

    private void Update()
    {
        if (increaseStart)
        {
            if (addCount == 0)
            {
                current = PlayerDataManager.player.GetRankPt(type);
                min = PlayerDataManager.player.GetRankPtLastUp(type);
                max = PlayerDataManager.player.GetRankPtNextUp(type);

                // �����ʌv�Z
                amount = (float)(current - min) / (float)(max - min);
                
                // ����
                if (guage.fillAmount <= amount)
                {
                    guage.fillAmount += increaseSpeed * Time.deltaTime;
                }

                else
                {
                    if (guage.fillAmount >= 1)
                    {
                        // �����N�A�b�v
                        int n = count + (int)lastRank + 1;
                        Rank r = (Rank)Enum.ToObject(typeof(Rank), n);
                        rankText.text = r.ToString();

                        guage.fillAmount = 0;
                    }

                    guage.fillAmount = amount;
                    increaseCompleted = true;
                    increaseStart = false;
                }
            }

            else
            {
                // �����N���ς�����񐔕��A�Q�[�W���ő�܂ŏ㏸�����鉉�o������
                if (count <= addCount && !isFinalUp)
                {
                    if (guage.fillAmount <= 1)
                    {
                        guage.fillAmount += increaseSpeed * Time.deltaTime;
                    }

                    if (guage.fillAmount >= 1)
                    {
                        // �����N�A�b�v
                        int n = count + (int)lastRank + 1;
                        Rank r = (Rank)Enum.ToObject(typeof(Rank), n);
                        rankText.text = r.ToString();

                        guage.fillAmount = 0;

                        count++;
                        if(count >= addCount) isFinalUp = true;
                    }
                }

                if (isFinalUp)
                {
                    current = PlayerDataManager.player.GetRankPt(type);
                    min = PlayerDataManager.player.StatusData.rankPoint.rankPt_NextUp[lastRank].GetStatus(type);
                    max = PlayerDataManager.player.StatusData.rankPoint.rankPt_NextUp[currentRank].GetStatus(type);

                    // �����ʌv�Z
                    amount = (float)(current - min) / (float)(max - min);

                    // ����
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
            }
        }
    }

    /// <summary>
    /// �\��������
    /// </summary>
    public void Initialize()
    {
        getPointText.gameObject.SetActive(false);
        getPointText.text = "+0p/";
        rankText.text = PlayerDataManager.player.GetRank(type).ToString();

        current = PlayerDataManager.player.GetRankPt(type);
        min = PlayerDataManager.player.GetRankPtLastUp(type);
        max = PlayerDataManager.player.GetRankPtNextUp(type);

        amount = (float)(current - min) / (max - min);
        guage.fillAmount = amount;

        //Debug.Log($"���ݒl�F{current}�@�ŏ��l�F{min}�@�ő�l�F{max}�@�Q�[�W�ʁF{amount}");
    }

    /// <summary>
    /// �l���|�C���g�\��
    /// </summary>
    /// <param name="_amount">�l����</param>
    public void SetPointText(int _amount)
    {
        getPointText.gameObject.SetActive(true);
        getPointText.text = "+" + _amount + "p/";
    }

    public void IncreaseAmount()
    {
        increaseCompleted = false;
        increaseStart = true;

        int l = (int)lastRank;
        int c = (int)currentRank;
        addCount = c - l;
        count = 0;
        isFinalUp = false;
    }
}
