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

    // ����Z�\���
    [SerializeField] private GameObject window_st;
    [SerializeField] private Text explain_st;
    [SerializeField] private Image icon_st;

    private bool resultDispCompleted = false; // �\������

    void Start()
    {
    }

    private void OnEnable()
    {
        ResultInitialize();
        window_st.SetActive(false);

        if (GameManager.SelectArea == 2)
        {
            DifficultyManager.SetBossClearDifficulty(GameManager.SelectDifficulty);
        }

        // Debug
        StartCoroutine(DispDirection());

        if (dropController.DropedItems.Count == 0) return;
        
        AddRankPoint();
        StartCoroutine(AddRankPtDirection());
    }

    /// <summary>
    /// �|�C���g���Z
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

    /// <summary>
    /// ���U���g��ʂ̏�����
    /// </summary>
    void ResultInitialize()
    {
        for (int i = 0; i < results.Length; i++)
        {
            results[i].getPointText.gameObject.SetActive(false);
            results[i].getPointText.text = "+0pt";
        }
    }

    /// <summary>
    /// ���U���g��ʂ̍X�V
    /// </summary>
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

    /// <summary>
    /// ����Z�\���
    /// </summary>
    void ReleaseSpecialTecnique()
    {
        window_st.SetActive(true);
        explain_st.text = "�f�o�b�O�p";
    }

    /// <summary>
    /// �I����ʂֈڍs
    /// </summary>
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

    /// <summary>
    /// �Đ�
    /// </summary>
    public void Retry()
    {
        SceneManager.LoadScene("Main");
    }

    /// <summary>
    /// ���U���g�\�����o
    /// </summary>
    /// <returns></returns>
    IEnumerator DispDirection()
    {
        yield return new WaitForSeconds(0.1f);
        resultDispCompleted = true;
    }

    /// <summary>
    /// Pt���Z���o
    /// </summary>
    /// <returns></returns>
    IEnumerator AddRankPtDirection()
    {
        yield return new WaitForSeconds(1f);
        ReleaseSpecialTecnique();
    }
}
