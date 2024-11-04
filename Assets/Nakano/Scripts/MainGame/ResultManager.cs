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

    [Header("����Z�\��� �X�L��")]
    [SerializeField] private GameObject window_st_Skill;
    [SerializeField] private Text name_st_Skill;
    [SerializeField] private Text explain_st_Skill;
    [SerializeField] private Image icon_st_Skill;

    [Header("����Z�\��� �p�b�V�u")]
    [SerializeField] private GameObject window_st_Passive;
    [SerializeField] private Text name_st_Passive;
    [SerializeField] private Text explain_st_Passive;

    private Rank lastRank;
    private Rank currentRank;

    private bool resultDispCompleted = false; // �\������

    void Start()
    {
        specialTecniqueManager = FindObjectOfType<SpecialTecniqueManager>();
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
        window_st_Skill.SetActive(false);
        window_st_Passive.SetActive(false);

        StartCoroutine(DispDirection());

        if (GameManager.SelectArea == 1)
        {
            if (dropController.DropedItems.Count == 0) return;
            AddRankPoint();
        }
        else
        {
            DifficultyManager.SetBossClearDifficulty(GameManager.SelectDifficulty);
            PlayerDataManager.Save();
        }
    }

    /// <summary>
    /// �|�C���g���Z
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
                amount *= 50;
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
    }

    /// <summary>
    /// ����Z�\������o
    /// </summary>
    void ReleaseSpecialTecnique()
    {
        // �����̓���Z�\�������ɉ�������ꍇ��x�O�����Ă���̂Œ��Ӂ@�d�l�ύX����ΏC��

        StatusType type = (StatusType)(GameManager.SelectStage - 1);
        currentRank = PlayerDataManager.player.GetRank(type);

        if (!specialTecniqueManager) return;

        SpecialTecnique st = null;
        
        if (currentRank != lastRank)
        {
            st = specialTecniqueManager.ReleaseSpecialTecniqueAndGetData(currentRank, type);
        }

        if (st == null) return;
        
        if (st.m_isSkill)
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
    /// �I����ʂֈڍs
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
        yield return new WaitForSeconds(0.5f);

        for (int i = 0; i < resultGuages.Length; i++)
        {
            resultGuages[i].IncreaseAmount();
        }

        yield return new WaitForSeconds(1f);
        ReleaseSpecialTecnique();
    }
}
