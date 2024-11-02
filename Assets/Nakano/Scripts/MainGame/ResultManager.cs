using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ResultManager : MonoBehaviour
{
    [SerializeField] private WindowController wc;
    [SerializeField] private DropController dropController;
    [SerializeField] private ResultGuage[] resultGuages;
    [SerializeField] private Text[] plusStatus;

    // 特殊技能解放
    [SerializeField] private GameObject window_st;
    [SerializeField] private Text explain_st;
    [SerializeField] private Image icon_st;

    private bool resultDispCompleted = false; // 表示完了

    void Start()
    {
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.G))
        {
            wc.Close();
        }
    }

    private void OnEnable()
    {
        ResultInitialize();
        window_st.SetActive(false);

        if (GameManager.SelectArea == 2)
        {
            DifficultyManager.SetBossClearDifficulty(GameManager.SelectDifficulty);
            PlayerDataManager.Save();
        }

        StartCoroutine(DispDirection());

        if (dropController.DropedItems.Count == 0) return;
        
        AddRankPoint();
    }

    /// <summary>
    /// ポイント加算
    /// </summary>
    void AddRankPoint()
    {
        for (int i = 0; i < resultGuages.Length; i++)
        {
            resultGuages[i].LastRank = PlayerDataManager.player.GetRank(resultGuages[i].Type);
        }

        for (int i = 0; i < dropController.DropedItems.Count; i++)
        {
            for (int j = 0; j < resultGuages.Length; j++)
            {
                int amount = dropController.DropedItems[i].dropAmount;
                amount *= 100;
                StatusType type = dropController.DropedItems[i].itemType;

                if (type == resultGuages[j].Type && amount > 0)
                {
                    resultGuages[j].SetPointText(amount);

                    PlayerDataManager.RankPtUp(type, amount);
                    
                    resultGuages[i].CurrentRank = PlayerDataManager.player.GetRank(resultGuages[i].Type);
                }
            }
        }

        StartCoroutine(AddRankPtDirection());
    }

    /// <summary>
    /// リザルト画面の初期化
    /// </summary>
    void ResultInitialize()
    {
        for (int i = 0; i < resultGuages.Length; i++)
        {
            resultGuages[i].Initialize();
        }

        for (int i = 0; i < plusStatus.Length; i++)
        {
            plusStatus[i].enabled = false;
        }

        Status plus = PlayerDataManager.player.GetPlusStatus();
        for (int i = 0; i < System.Enum.GetValues(typeof(StatusType)).Length; i++)
        {
            StatusType type = (StatusType)System.Enum.ToObject(typeof(StatusType), i);
            int s = plus.GetStatus(type);

            if (s > 0)
            {
                plusStatus[i].text = "+" + s.ToString();
                plusStatus[i].enabled = true;
            }
        }
    }

    /// <summary>
    /// 特殊技能解放演出
    /// </summary>
    void ReleaseSpecialTecnique()
    {
        window_st.SetActive(true);
        explain_st.text = "デバッグ用";
    }

    /// <summary>
    /// 選択画面へ移行
    /// </summary>
    public void ToSelect()
    {
        if (!resultDispCompleted) return;

        if (GameManager.SelectArea == 1)
        {
            SceneManager.LoadScene("SelectScene_Traning");
        }
        else if (GameManager.SelectArea == 2)
        {
            SceneManager.LoadScene("SelectScene_Boss");
        }
        else SceneManager.LoadScene("SelectScene_Traning");
    }

    /// <summary>
    /// 再戦
    /// </summary>
    public void Retry()
    {
        SceneManager.LoadScene("Main");
    }

    /// <summary>
    /// リザルト表示演出
    /// </summary>
    /// <returns></returns>
    IEnumerator DispDirection()
    {
        yield return new WaitForSeconds(0.1f);
        resultDispCompleted = true;
    }

    /// <summary>
    /// Pt加算演出
    /// </summary>
    /// <returns></returns>
    IEnumerator AddRankPtDirection()
    {
        for (int i = 0; i < resultGuages.Length; i++)
        {
            resultGuages[i].IncreaseAmount();
        }

        yield return new WaitForSeconds(1f);
        ReleaseSpecialTecnique();
    }
}
