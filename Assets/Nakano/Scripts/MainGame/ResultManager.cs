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
    private SpecialTecniqueManager specialTecniqueManager;

    [Header("特殊技能解放 スキル")]
    [SerializeField] private GameObject window_st_Skill;
    [SerializeField] private Text name_st_Skill;
    [SerializeField] private Text explain_st_Skill;
    [SerializeField] private Image icon_st_Skill;

    [Header("特殊技能解放 パッシブ")]
    [SerializeField] private GameObject window_st_Passive;
    [SerializeField] private Text name_st_Passive;
    [SerializeField] private Text explain_st_Passive;

    private Rank lastRank;
    private Rank currentRank;

    private bool resultDispCompleted = false; // 表示完了

    StaminaManager staminaManager;
    [SerializeField] private Text costStamina;

    void Start()
    {
        specialTecniqueManager = FindObjectOfType<SpecialTecniqueManager>();
        staminaManager = FindObjectOfType<StaminaManager>();

        if (GameManager.SelectArea == 1)
        {
            costStamina.text = "-" + staminaManager.GetCost_Traning;
        }
        if (GameManager.SelectArea == 2)
        {
            costStamina.text = "-" + staminaManager.GetCost_Boss;
        }
    }

    private void OnEnable()
    {
        ResultInitialize();
        window_st_Skill.SetActive(false);
        window_st_Passive.SetActive(false);

        StartCoroutine(DispDirection());

        if (dropController.DropedItems.Count == 0) return;

        if (GameManager.SelectArea == 1)
        {
            AddRankPoint();
        }
        else
        {
            if (GameManager.IsBossClear[GameManager.SelectDifficulty - 1]) return;
            AddRankPoint();

            GameManager.IsBossClear[GameManager.SelectDifficulty - 1] = true;

            DifficultyManager.SetBossClearDifficulty(GameManager.SelectDifficulty);
            GameManager.SelectDifficulty++;
            PlayerDataManager.Save();
        }
    }

    private void OnDisable()
    {
        dropController.Initialize();
        ResultInitialize();
    }

    /// <summary>
    /// ポイント加算
    /// </summary>
    void AddRankPoint()
    {
        StatusType t = (StatusType)(GameManager.SelectStage - 1);
        lastRank = PlayerDataManager.player.GetRank(t);

        for (int i = 0; i < resultGuages.Length; i++)
        {
            resultGuages[i].LastRank = PlayerDataManager.player.GetRank(resultGuages[i].Type);
        }

        for (int i = 0; i < dropController.DropedItems.Count; i++)
        {
            for (int j = 0; j < resultGuages.Length; j++)
            {
                int amount = dropController.DropedItems[i].dropAmount;
                StatusType type = dropController.DropedItems[i].itemType;

                if (type == resultGuages[j].Type && amount > 0)
                {
                    var cType = PlayerDataManager.NormalStatusToCombiStatus(type);

                    // 最大までポイントがたまっていたら0Pt表記
                    if (PlayerDataManager.player.GetRankPt(type) >= PlayerDataManager.player.GetRankPtMax(type) || 
                        PlayerDataManager.player.GetCombiRankPt(cType) >= PlayerDataManager.player.GetCombiRankPtMax(cType))
                    {
                        amount = 0;
                    }

                    resultGuages[j].SetPointText(amount);

                    PlayerDataManager.RankPtUp(type, amount);

                    resultGuages[j].CurrentRank = PlayerDataManager.player.GetRank(resultGuages[j].Type);
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
        StatusType type = (StatusType)(GameManager.SelectStage - 1);
        currentRank = PlayerDataManager.player.GetRank(type);

        if (!specialTecniqueManager) return;

        SpecialTecnique st = null;
        
        if (currentRank != lastRank)
        {
            st = specialTecniqueManager.ReleaseSpecialTecniqueAndGetData((Rank)(lastRank + 1), type);
        }
        else return;

        if (st == null) return;
        
        if (st.m_isSkill)
        {
            // スキルの場合
            name_st_Skill.text = "スキル " + st.m_name + " 解放";
            explain_st_Skill.text = st.m_effects;
            icon_st_Skill.sprite = st.m_illust;
            window_st_Skill.SetActive(true);
        }
        else
        {
            // パッシブの場合
            name_st_Passive.text = "特殊技能 " + st.m_name + " 解放";
            explain_st_Passive.text = st.m_effects;
            window_st_Passive.SetActive(true);
        }

        Invoke("CanBack", 0.1f);
    }

    private bool canBack = false;

    void CanBack()
    {
        canBack = true;
    }

    public void BackSpecialTecnique()
    {
        if (canBack)
        {
            window_st_Skill.SetActive(false);
            window_st_Passive.SetActive(false);

            lastRank++;
            Invoke("ReleaseSpecialTecnique", 0.1f);
        }
    }

    /// <summary>
    /// 選択画面へ移行
    /// </summary>
    public void ToSelect()
    {
        if (!resultDispCompleted) return;

        if (GameManager.SelectArea == 1)
        {
            //SceneManager.LoadScene("SelectScene_Traning");
            SceneLoader.LoadScene("SelectScene_Traning");
        }
        else if (GameManager.SelectArea == 2)
        {
            //SceneManager.LoadScene("SelectScene_Boss");
            SceneLoader.LoadScene("SelectScene_Boss");
        }
        else SceneLoader.LoadScene("SelectScene_Traning");
    }

    /// <summary>
    /// 再戦
    /// </summary>
    public void Retry()
    {
        if ((GameManager.SelectArea == 1 && staminaManager.Traning()) ||
            (GameManager.SelectArea == 2 && staminaManager.Boss()))
        {
            SceneLoader.LoadScene("MainTest");
        }
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
        yield return new WaitForSeconds(0.5f);

        for (int i = 0; i < resultGuages.Length; i++)
        {
            resultGuages[i].IncreaseAmount();
        }

        yield return new WaitForSeconds(1f);
        ReleaseSpecialTecnique();
    }
}
