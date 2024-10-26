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

                // �����ʌv�Z
                float amount = max - min != 0 ? (float)(current - min) / (max - min) : 1;

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

            else
            {
                if (count <= addCount)
                {
                    int lastRankNum = (int)lastRank;
                    int currentRankNum = (int)currentRank;

                    int current = PlayerDataManager.player.GetRankPt(type);
                    int min = 0;

                    if (count + lastRankNum - 1 >= 0)
                    {
                        Rank lRank = (Rank)Enum.ToObject(typeof(Rank), count + lastRankNum - 1);
                        min = PlayerDataManager.player.GetRankPtUp(type, lRank);
                    }

                    Rank cRank = (Rank)Enum.ToObject(typeof(Rank), count + currentRankNum - 1);
                    int max = PlayerDataManager.player.GetRankPtUp(type, cRank);

                    // �����ʌv�Z
                    float amount = (float)(current - min) / (max - min);

                    // ����
                    if (guage.fillAmount <= amount)
                    {
                        guage.fillAmount += increaseSpeed * Time.deltaTime;
                    }

                    if (guage.fillAmount >= 1)
                    {
                        // �����N�A�b�v
                        Rank r = (Rank)Enum.ToObject(typeof(Rank), count + currentRankNum);
                        rankText.text = r.ToString();

                        guage.fillAmount = 0;

                        if (count == addCount)
                        {
                            increaseCompleted = true;
                            increaseStart = false;
                        }

                        count++;
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
        getPointText.text = "+0pt";
        rankText.text = PlayerDataManager.player.GetRank(type).ToString();

        int current = PlayerDataManager.player.GetRankPt(type);
        int min = PlayerDataManager.player.GetRankPtLastUp(type);
        int max = PlayerDataManager.player.GetRankPtNextUp(type);

        float amount = (float)(current - min) / (max - min);
        guage.fillAmount = amount;
    }

    /// <summary>
    /// �l���|�C���g�\��
    /// </summary>
    /// <param name="_amount">�l����</param>
    public void SetPointText(int _amount)
    {
        getPointText.gameObject.SetActive(true);
        getPointText.text = "+" + _amount + "pt";
    }

    public void IncreaseAmount()
    {
        increaseCompleted = false;
        increaseStart = true;
        //StartCoroutine(Increase());

        int l = (int)lastRank;
        int c = (int)currentRank;
        addCount = c - l;
        count = 0;
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

            // �����ʌv�Z
            float amount = max - min != 0 ? (float)(current - min) / (max - min) : 1;

            // ����
            while (guage.fillAmount <= amount)
            {
                guage.fillAmount += increaseSpeed * Time.deltaTime;
                yield return null;

                if (guage.fillAmount >= amount)
                {
                    guage.fillAmount = amount;
                    break;
                }
            }
        }

        else
        {
            for (int i = 0; i < count; i++)
            {
                int current = PlayerDataManager.player.GetRankPt(type);
                int min = 0;
                if (i > 0)
                {
                    Rank lRank = (Rank)Enum.ToObject(typeof(Rank), i - 1);
                    min = PlayerDataManager.player.GetRankPtUp(type, lRank);
                }

                Rank cRank = (Rank)Enum.ToObject(typeof(Rank), i);

                int max = PlayerDataManager.player.GetRankPtUp(type, cRank);

                // �����ʌv�Z
                float amount = max - min != 0 ? (float)(current - min) / (max - min) : 1;

                // ����
                while (guage.fillAmount <= 1)
                {
                    guage.fillAmount += increaseSpeed * Time.deltaTime;
                    yield return null;

                    if (guage.fillAmount >= 1)
                    {
                        // �����N�A�b�v
                        Rank r = (Rank)Enum.ToObject(typeof(Rank), i + 1);
                        rankText.text = r.ToString();

                        guage.fillAmount = 0;
                        break;
                    }

                    if (guage.fillAmount >= amount) break;
                }

                yield return null;
            }
        }

        increaseCompleted = true;
    }
}
