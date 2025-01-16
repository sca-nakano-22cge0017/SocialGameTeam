using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ResultManager : MonoBehaviour
{
    [SerializeField] private WindowController resultWindowController;
    [SerializeField] private DropController dropController;

    [SerializeField] private GameObject window1;
    [SerializeField] private ResultGuage[] resultGuages;
    [SerializeField] private Text[] plusStatus;

    [SerializeField] private GameObject window2;
    [SerializeField] ResultGuage[] resultCombiGuages;

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

    private Dictionary<StatusType, Rank> lastRank = new();
    private Dictionary<StatusType, Rank> currentRank = new();
    private StatusType checkType = 0; // 確認中のステータス

    private Dictionary<CombiType, Rank> lastCombiRank = new();
    private Dictionary<CombiType, Rank> currentCombiRank = new();

    StaminaManager staminaManager;
    [SerializeField] private Text costStamina;

    TutorialWindow tutorial = null;

    private bool didSkillReleaseComplete = false;
    private bool didSkipDirection1 = false;
    private bool didSkipDirection2 = false;

    private bool didRankUp = false;
    private bool didCombiRankDisp = false;

    void Start()
    {
        specialTecniqueManager = FindObjectOfType<SpecialTecniqueManager>();
        staminaManager = FindObjectOfType<StaminaManager>();
        tutorial = FindObjectOfType<TutorialWindow>();

        window1.SetActive(true);
        window2.SetActive(false);

        checkType = 0;
        didSkillReleaseComplete = false;
        didSkipDirection1 = false;
        didSkipDirection2 = false;
        didRankUp = false;
        didCombiRankDisp = false;

        if (GameManager.SelectArea == 1)
        {
            costStamina.text = "-" + staminaManager.GetCost_Traning;
        }
        if (GameManager.SelectArea == 2)
        {
            costStamina.text = "-" + staminaManager.GetCost_Boss;

            // スタミナ上限解放
            staminaManager.LevelUp();
        }
    }

    public void Initialize()
    {
        tutorial = FindObjectOfType<TutorialWindow>();
        tutorial.Result();

        window1.SetActive(true);
        window2.SetActive(false);

        ResultInitialize();
        window_st_Skill.SetActive(false);
        window_st_Passive.SetActive(false);

        checkType = 0;
        didSkillReleaseComplete = false;
        didSkipDirection1 = false;
        didSkipDirection2 = false;
        didRankUp = false;
        didCombiRankDisp = false;

        for (int i = 0; i < System.Enum.GetValues(typeof(StatusType)).Length; i++)
        {
            StatusType type = (StatusType)System.Enum.ToObject(typeof(StatusType), i);
            lastRank[type] = Rank.D;
            currentRank[type] = Rank.D;
        }
        for (int i = 0; i < System.Enum.GetValues(typeof(CombiType)).Length - 1; i++)
        {
            CombiType type = (CombiType)System.Enum.ToObject(typeof(CombiType), i);
            lastCombiRank[type] = Rank.D;
            currentCombiRank[type] = Rank.D;
        }

        if (dropController.DropedItems.Count == 0) return;

        StartCoroutine(WaitTutorial(() =>
        {
            if (GameManager.SelectArea == 1)
            {
                AddRankPoint();
            }
            else if (GameManager.SelectArea == 2)
            {
                var clearDifficulty = 0;

                // クリア状況の設定
                if (GameManager.SelectChara == 1)
                {
                    if (GameManager.IsBossClear1[GameManager.SelectDifficulty - 1]) return;
                    GameManager.IsBossClear1[GameManager.SelectDifficulty - 1] = true;
                    clearDifficulty = DifficultyManager.IsClearBossDifficulty1;
                }
                if (GameManager.SelectChara == 2)
                {
                    if (GameManager.IsBossClear2[GameManager.SelectDifficulty - 1]) return;
                    GameManager.IsBossClear2[GameManager.SelectDifficulty - 1] = true;
                    clearDifficulty = DifficultyManager.IsClearBossDifficulty2;
                }

                AddRankPoint();

                DifficultyManager.SetBossClearDifficulty(GameManager.SelectDifficulty);
                GameManager.SelectDifficulty++;
                PlayerDataManager.Save();
            }
        }));
        
        resultWindowController.Open();
    }

    private void ResultExit()
    {
        resultWindowController.Close();
        dropController.Initialize();
    }

    /// <summary>
    /// ポイント加算
    /// </summary>
    void AddRankPoint()
    {
        // ポイント加算前のランクを保存
        for (int i = 0; i < System.Enum.GetValues(typeof(StatusType)).Length; i++)
        {
            StatusType type = (StatusType)System.Enum.ToObject(typeof(StatusType), i);
            lastRank[type] = PlayerDataManager.player.GetRank(type);
        }
        for (int i = 0; i < System.Enum.GetValues(typeof(CombiType)).Length - 1; i++)
        {
            CombiType type = (CombiType)System.Enum.ToObject(typeof(CombiType), i);
            lastCombiRank[type] = PlayerDataManager.player.GetCombiRank(type);
        }

        // ポイント加算前のランクを設定
        for (int i = 0; i < resultGuages.Length; i++)
        {
            resultGuages[i].LastRank = PlayerDataManager.player.GetRank(resultGuages[i].Type);
        }
        for (int i = 0; i < resultCombiGuages.Length; i++)
        {
            resultCombiGuages[i].LastRank = PlayerDataManager.player.GetCombiRank(resultCombiGuages[i].CombiType);
        }

        for (int i = 0; i < dropController.DropedItems.Count; i++)
        {
            for (int j = 0; j < resultGuages.Length; j++)
            {
                int amount = dropController.DropedItems[i].dropAmount;

                if (GameManager.isHyperTraningMode)
                {
                    // デバッグモードならドロップ量1000倍
                    amount *= 1000;
                }

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

        // いずれかのステータスがランクアップしたかどうか確認
        for (int i = 0; i < System.Enum.GetValues(typeof(StatusType)).Length; i++)
        {
            StatusType type = (StatusType)System.Enum.ToObject(typeof(StatusType), i);
            if (lastRank[type] < currentRank[type])
            {
                didRankUp = true;
                continue;
            }
        }

        if (!didRankUp) didSkillReleaseComplete = true;
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

        for (int i = 0; i < resultCombiGuages.Length; i++)
        {
            resultCombiGuages[i].Initialize();
        }
    }

    /// <summary>
    /// 特殊技能解放演出
    /// </summary>
    void ReleaseSpecialTecnique()
    {
        if ((int)checkType >= System.Enum.GetValues(typeof(StatusType)).Length - 1 && lastRank[checkType] >= PlayerDataManager.player.GetRank(checkType))
        {
            didSkillReleaseComplete = true;
            CheckFirstDirectionCompleted();
            return;
        }

        if ((int)checkType >= 0 && (int)checkType < System.Enum.GetValues(typeof(StatusType)).Length)
        {
            currentRank[checkType] = PlayerDataManager.player.GetRank(checkType);
        }

        if (!specialTecniqueManager) return;

        SpecialTecnique st = null;

        if (currentRank[checkType] != lastRank[checkType])
        {
            // 特殊技能の詳細を取得する
            st = specialTecniqueManager.ReleaseSpecialTecniqueAndGetData((Rank)(lastRank[checkType] + 1), checkType);
        }
        else if (currentRank[checkType] <= lastRank[checkType])
        {
            // 次のステータスを確認する
            checkType++;
            ReleaseSpecialTecnique();
        }

        if (st == null) return;

        // 解放した特殊技能の詳細を表示
        if (st.m_skillType == 1)
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
    }

    /// <summary>
    /// 特殊技能解放ウィンドウを閉じる
    /// </summary>
    public void BackSpecialTecnique()
    {
        window_st_Skill.SetActive(false);
        window_st_Passive.SetActive(false);

        lastRank[checkType]++;

        // 次に解放した特殊技能を表示
        Invoke("ReleaseSpecialTecnique", 0.1f);
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

        yield return new WaitForSeconds(0.5f);
        
        ReleaseSpecialTecnique();
    }

    /// <summary>
    /// ゲージ増加演出スキップ
    /// </summary>
    public void Skip()
    {
        if (!didSkillReleaseComplete || window_st_Skill.activeSelf || window_st_Passive.activeSelf) return;

        if (!didSkipDirection1)
        {
            // 通常ステータス 演出スキップ
            for (int i = 0; i < resultGuages.Length; i++)
            {
                resultGuages[i].Skip();
            }

            didSkipDirection1 = true;

            Invoke("DisplayCombiRankGauge", 0.5f);
        }
    }

    /// <summary>
    /// 複合ステータス　ゲージ増加演出スキップ
    /// </summary>
    public void SkipCombiGuageDirection()
    {
        if (!didSkillReleaseComplete || window_st_Skill.activeSelf || window_st_Passive.activeSelf) return;

        if (!didSkipDirection2)
        {
            // 複合ステータス 演出スキップ
            for (int i = 0; i < resultCombiGuages.Length; i++)
            {
                resultCombiGuages[i].Skip();
            }

            didSkipDirection2 = true;

            // 衣装解放
            PlayerDataManager.Evolution();
        }
    }

    /// <summary>
    /// 通常ステータスのゲージ増加演出が完了したかを確認する
    /// </summary>
    public void CheckFirstDirectionCompleted()
    {
        if (!didSkillReleaseComplete || window_st_Skill.activeSelf || window_st_Passive.activeSelf) return;

        for (int i = 0; i < resultGuages.Length; i++)
        {
            if (!resultGuages[i].IncreaseCompleted) return;
        }

        // 演出完了していたら複合ステータスの表示へ
        Invoke("DisplayCombiRankGauge", 0.5f);
        didSkipDirection1 = true;
    }

    /// <summary>
    /// 複合ステータスのゲージ増加演出が完了したかを確認する
    /// </summary>
    public void CheckSecondDirectionCompleted()
    {
        if (!didSkillReleaseComplete || window_st_Skill.activeSelf || window_st_Passive.activeSelf) return;

        for (int i = 0; i < resultCombiGuages.Length; i++)
        {
            if (!resultCombiGuages[i].IncreaseCompleted) return;
        }

        // 衣装解放
        PlayerDataManager.Evolution();
        didSkipDirection2 = true;
    }

    /// <summary>
    /// 合算ポイントのゲージ増加演出
    /// </summary>
    public void DisplayCombiRankGauge()
    {
        if (!tutorial.CompleteTutorial || didCombiRankDisp) return;

        // ポイント加算後のランクを設定
        for (int i = 0; i < resultCombiGuages.Length; i++)
        {
            resultCombiGuages[i].CurrentRank = PlayerDataManager.player.GetCombiRank(resultCombiGuages[i].CombiType);
        }

        window1.SetActive(false);
        window2.SetActive(true);

        int amount = 0;

        for (int i = 0; i < resultCombiGuages.Length; i++)
        {
            var cType = resultCombiGuages[i].CombiType;

            for (int j = 0; j < dropController.DropedItems.Count; j++)
            {
                var itemType = dropController.DropedItems[j].itemType;

                if (cType == CombiType.DEF)
                {
                    if (itemType == StatusType.HP || itemType == StatusType.DEF)
                    {
                        int a = dropController.DropedItems[j].dropAmount;
                        // 最大までポイントがたまっていたら0Pt表記
                        if (PlayerDataManager.player.GetRankPt(itemType) >= PlayerDataManager.player.GetRankPtMax(itemType) ||
                            PlayerDataManager.player.GetCombiRankPt(cType) >= PlayerDataManager.player.GetCombiRankPtMax(cType))
                        {
                            a = 0;
                        }

                        amount += a;
                    }
                }
                if (cType == CombiType.ATK)
                {
                    if (itemType == StatusType.ATK || itemType == StatusType.MP)
                    {
                        int a = dropController.DropedItems[j].dropAmount;
                        // 最大までポイントがたまっていたら0Pt表記
                        if (PlayerDataManager.player.GetRankPt(itemType) >= PlayerDataManager.player.GetRankPtMax(itemType) ||
                            PlayerDataManager.player.GetCombiRankPt(cType) >= PlayerDataManager.player.GetCombiRankPtMax(cType))
                        {
                            a = 0;
                        }

                        amount += a;
                    }
                }
                if (cType == CombiType.TEC)
                {
                    if (itemType == StatusType.AGI || itemType == StatusType.DEX)
                    {
                        int a = dropController.DropedItems[j].dropAmount;
                        // 最大までポイントがたまっていたら0Pt表記
                        if (PlayerDataManager.player.GetRankPt(itemType) >= PlayerDataManager.player.GetRankPtMax(itemType) ||
                            PlayerDataManager.player.GetCombiRankPt(cType) >= PlayerDataManager.player.GetCombiRankPtMax(cType))
                        {
                            a = 0;
                        }

                        amount += a;
                    }
                }
            }

            if (GameManager.isHyperTraningMode)
            {
                // デバッグモードならドロップ量1000倍
                amount *= 1000;
            }

            if (amount > 0)
            {
                resultCombiGuages[i].SetPointText(amount);
            }
            
            amount = 0;
        }

        StartCoroutine(AddCombiRankPtDirection());

        didCombiRankDisp = true;
    }

    /// <summary>
    /// Pt加算演出
    /// </summary>
    /// <returns></returns>
    IEnumerator AddCombiRankPtDirection()
    {
        yield return new WaitForSeconds(0.5f);

        for (int i = 0; i < resultCombiGuages.Length; i++)
        {
            resultCombiGuages[i].IncreaseAmount();
        }
    }

    IEnumerator WaitTutorial(System.Action _action)
    {
        yield return new WaitUntil(() => tutorial.CompleteTutorial);

        _action?.Invoke();
    }

    /// <summary>
    /// 選択画面へ移行
    /// </summary>
    public void ToSelect()
    {
        ResultExit();

        if (GameManager.SelectArea == 1)
        {
            SceneLoader.LoadFade("SelectScene_Traning");
        }
        else if (GameManager.SelectArea == 2)
        {
            SceneLoader.LoadFade("SelectScene_Boss");
        }
        else SceneLoader.LoadFade("SelectScene_Traning");
    }

    /// <summary>
    /// 再戦
    /// </summary>
    public void Retry()
    {
        if ((GameManager.SelectArea == 1 && staminaManager.Traning()) ||
            (GameManager.SelectArea == 2 && staminaManager.Boss()))
        {
            ResultExit();
            SceneLoader.LoadFade("MainTest");
        }
    }
}