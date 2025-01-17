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

    [Header("����Z�\��� �X�L��")]
    [SerializeField] private GameObject window_st_Skill;
    [SerializeField] private Text name_st_Skill;
    [SerializeField] private Text explain_st_Skill;
    [SerializeField] private Image icon_st_Skill;

    [Header("����Z�\��� �p�b�V�u")]
    [SerializeField] private GameObject window_st_Passive;
    [SerializeField] private Text name_st_Passive;
    [SerializeField] private Text explain_st_Passive;

    private Dictionary<StatusType, int> dropAmounts = new();

    private Dictionary<StatusType, Rank> lastRank = new();
    private Dictionary<StatusType, Rank> currentRank = new();
    private StatusType checkType = 0; // �m�F���̃X�e�[�^�X

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

    private bool isFirstClear = true;

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

            // �X�^�~�i������
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
        isFirstClear = true;
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

            dropAmounts[type] = -1;
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
                // �N���A�󋵂̐ݒ�
                if (GameManager.SelectChara == 1)
                {
                    // ���N���A
                    if (!GameManager.IsBossClear1[GameManager.SelectDifficulty - 1])
                    {
                        GameManager.IsBossClear1[GameManager.SelectDifficulty - 1] = true;
                        DifficultyManager.SetBossClearDifficulty(GameManager.SelectDifficulty);
                        isFirstClear = true;
                    }
                    else isFirstClear = false;
                }
                if (GameManager.SelectChara == 2)
                {
                    // ���N���A
                    if (!GameManager.IsBossClear2[GameManager.SelectDifficulty - 1])
                    {
                        GameManager.IsBossClear2[GameManager.SelectDifficulty - 1] = true;
                        DifficultyManager.SetBossClearDifficulty(GameManager.SelectDifficulty);
                        isFirstClear = true;
                    }
                    else isFirstClear = false;
                }

                AddRankPoint();
                
                PlayerDataManager.Save();
            }
        }));
        
        resultWindowController.Open();
    }

    /// <summary>
    /// ���U���g��ʂ̏�����
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

    // ����Z�\���
    /// <summary>
    /// ����Z�\������o
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
            // ����Z�\�̏ڍׂ��擾����
            st = specialTecniqueManager.ReleaseSpecialTecniqueAndGetData((Rank)(lastRank[checkType] + 1), checkType);
        }
        else if (currentRank[checkType] <= lastRank[checkType])
        {
            // ���̃X�e�[�^�X���m�F����
            checkType++;
            ReleaseSpecialTecnique();
        }

        if (st == null) return;

        // �����������Z�\�̏ڍׂ�\��
        if (st.m_skillType == 1)
        {
            // �X�L���̏ꍇ
            name_st_Skill.text = "�X�L�� " + st.m_name + " ���";
            explain_st_Skill.text = st.m_effects;
            icon_st_Skill.sprite = st.m_illust;
            window_st_Skill.SetActive(true);
        }
        else
        {
            // �p�b�V�u�̏ꍇ
            name_st_Passive.text = "����Z�\ " + st.m_name + " ���";
            explain_st_Passive.text = st.m_effects;
            window_st_Passive.SetActive(true);
        }
    }

    /// <summary>
    /// ����Z�\����E�B���h�E�����
    /// </summary>
    public void BackSpecialTecnique()
    {
        window_st_Skill.SetActive(false);
        window_st_Passive.SetActive(false);

        lastRank[checkType]++;

        // ���ɉ����������Z�\��\��
        Invoke("ReleaseSpecialTecnique", 0.1f);
    }

    // �ʏ�X�e�[�^�X
    /// <summary>
    /// �|�C���g���Z
    /// </summary>
    void AddRankPoint()
    {
        // �|�C���g���Z�O�̃����N��ۑ�
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

        // �|�C���g���Z�O�̃����N��ݒ�
        for (int i = 0; i < resultGuages.Length; i++)
        {
            resultGuages[i].LastRank = PlayerDataManager.player.GetRank(resultGuages[i].Type);
        }
        for (int i = 0; i < resultCombiGuages.Length; i++)
        {
            resultCombiGuages[i].LastRank = PlayerDataManager.player.GetCombiRank(resultCombiGuages[i].CombiType);
        }

        for (int i = 0; i < resultGuages.Length; i++)
        {
            StatusType type = resultGuages[i].Type;
            int amount = dropController.GetDropAmount(type);

            if (amount > 0)
            {
                if (GameManager.isHyperTraningMode)
                {
                    // �f�o�b�O���[�h�Ȃ�h���b�v��1000�{
                    amount *= 1000;
                }

                var cType = PlayerDataManager.NormalStatusToCombiStatus(type);

                // �ő�܂Ń|�C���g�����܂��Ă�����0Pt�\�L
                if (PlayerDataManager.player.GetRankPt(type) >= PlayerDataManager.player.GetRankPtMax(type) ||
                    PlayerDataManager.player.GetCombiRankPt(cType) >= PlayerDataManager.player.GetCombiRankPtMax(cType))
                {
                    dropAmounts[type] = 0;
                    amount = 0;
                }
                else
                {
                    dropAmounts[type] = amount;
                }

                PlayerDataManager.RankPtUp(type, amount);
                resultGuages[i].SetPointText(amount);
                resultGuages[i].CurrentRank = PlayerDataManager.player.GetRank(type);

                if (!isFirstClear)
                {
                    StartCoroutine(AddRankPtDirection());
                }
            }
        }

        // �����ꂩ�̃X�e�[�^�X�������N�A�b�v�������ǂ����m�F
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
    /// Pt���Z���o
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
    /// �Q�[�W�������o�X�L�b�v
    /// </summary>
    public void Skip()
    {
        if (!didSkillReleaseComplete || window_st_Skill.activeSelf || window_st_Passive.activeSelf) return;

        if (!didSkipDirection1)
        {
            // �ʏ�X�e�[�^�X ���o�X�L�b�v
            for (int i = 0; i < resultGuages.Length; i++)
            {
                resultGuages[i].Skip();
            }

            didSkipDirection1 = true;

            Invoke("DisplayCombiRankGauge", 0.5f);
        }
    }

    /// <summary>
    /// �ʏ�X�e�[�^�X�̃Q�[�W�������o���������������m�F����
    /// </summary>
    public void CheckFirstDirectionCompleted()
    {
        if (!didSkillReleaseComplete || window_st_Skill.activeSelf || window_st_Passive.activeSelf) return;

        for (int i = 0; i < resultGuages.Length; i++)
        {
            if (!resultGuages[i].IncreaseCompleted) return;
        }

        // ���o�������Ă����畡���X�e�[�^�X�̕\����
        Invoke("DisplayCombiRankGauge", 0.5f);
        didSkipDirection1 = true;
    }

    // �����X�e�[�^�X
    /// <summary>
    /// �����X�e�[�^�X�̃Q�[�W�������o
    /// </summary>
    public void DisplayCombiRankGauge()
    {
        if (!tutorial.CompleteTutorial || didCombiRankDisp) return;

        // �|�C���g���Z��̃����N��ݒ�
        for (int i = 0; i < resultCombiGuages.Length; i++)
        {
            resultCombiGuages[i].CurrentRank = PlayerDataManager.player.GetCombiRank(resultCombiGuages[i].CombiType);
        }

        if (isFirstClear)
        {
            int amountATK = 0, amountDEF = 0, amountTEC = 0;

            for (int i = 0; i < System.Enum.GetValues(typeof(StatusType)).Length; i++)
            {
                StatusType type = (StatusType)System.Enum.ToObject(typeof(StatusType), i);
                var cType = PlayerDataManager.NormalStatusToCombiStatus(type);

                int a = dropAmounts[type];

                if (cType == CombiType.DEF)
                {
                    if (a >= 0) amountDEF += a;
                }
                if (cType == CombiType.ATK)
                {
                    if (a >= 0) amountATK += a;
                }
                if (cType == CombiType.TEC)
                {
                    if (a >= 0) amountTEC += a;
                }
            }

            for (int i = 0; i < resultCombiGuages.Length; i++)
            {
                int a1 = 0, a2 = 0;
                var type = resultCombiGuages[i].CombiType;
                switch (type)
                {
                    case CombiType.ATK:
                        a1 = dropAmounts[StatusType.MP];
                        a2 = dropAmounts[StatusType.ATK];
                        if (a1 >= 0 || a2 >= 0) resultCombiGuages[i].SetPointText(amountATK);
                        break;
                    case CombiType.DEF:
                        a1 = dropAmounts[StatusType.HP];
                        a2 = dropAmounts[StatusType.DEF];
                        if (a1 >= 0 || a2 >= 0) resultCombiGuages[i].SetPointText(amountDEF);
                        break;
                    case CombiType.TEC:
                        a1 = dropAmounts[StatusType.AGI];
                        a2 = dropAmounts[StatusType.DEX];
                        if (a1 >= 0 || a2 >= 0) resultCombiGuages[i].SetPointText(amountTEC);
                        break;
                }
            }
        }

        StartCoroutine(AddCombiRankPtDirection());
        didCombiRankDisp = true;

        window1.SetActive(false);
        window2.SetActive(true);
    }

    /// <summary>
    /// Pt���Z���o
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

    /// <summary>
    /// �����X�e�[�^�X�@�Q�[�W�������o�X�L�b�v
    /// </summary>
    public void SkipCombiGuageDirection()
    {
        if (!didSkillReleaseComplete || window_st_Skill.activeSelf || window_st_Passive.activeSelf) return;

        if (!didSkipDirection2)
        {
            // �����X�e�[�^�X ���o�X�L�b�v
            for (int i = 0; i < resultCombiGuages.Length; i++)
            {
                resultCombiGuages[i].Skip();
            }

            didSkipDirection2 = true;

            // �ߑ����
            PlayerDataManager.Evolution();
        }
    }

    /// <summary>
    /// �����X�e�[�^�X�̃Q�[�W�������o���������������m�F����
    /// </summary>
    public void CheckSecondDirectionCompleted()
    {
        if (!didSkillReleaseComplete || window_st_Skill.activeSelf || window_st_Passive.activeSelf) return;

        for (int i = 0; i < resultCombiGuages.Length; i++)
        {
            if (!resultCombiGuages[i].IncreaseCompleted) return;
        }

        // �ߑ����
        PlayerDataManager.Evolution();
        didSkipDirection2 = true;
    }


    IEnumerator WaitTutorial(System.Action _action)
    {
        yield return new WaitUntil(() => tutorial.CompleteTutorial);

        _action?.Invoke();
    }

    /// <summary>
    /// �I����ʂֈڍs
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
    /// �Đ�
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

    private void ResultExit()
    {
        dropController.Initialize();
    }
}