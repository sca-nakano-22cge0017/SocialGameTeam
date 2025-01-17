using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PassiveWindow : MonoBehaviour
{
    [SerializeField] private GameObject window;
    [SerializeField] private WindowController wc;
    [SerializeField] private Text rank;
    private StatusType type;

    SpecialTecniqueManager stm;
    [SerializeField] private Button[] allSkill;
    [SerializeField] private Button[] atkSkill;
    [SerializeField] private Button[] mpSkill;
    [SerializeField] private Button[] hpSkill;
    [SerializeField] private Button[] defSkill;
    [SerializeField] private Button[] agiSkill;
    [SerializeField] private Button[] dexSkill;


    void Start()
    {
        stm = FindObjectOfType<SpecialTecniqueManager>();
    }

    /// <summary>
    /// �p�b�V�u�X�L���\��
    /// </summary>
    /// <param name="_statusName">�ΏۃX�e�[�^�X ATK / MP / HP / DEF / AGI / DEX </param>
    public void Open(string _statusName)
    {
        wc.Open();

        type = PlayerDataManager.StringToStutasType(_statusName);

        rank.text = "Rank " + PlayerDataManager.player.GetRank(type).ToString();
        SkillRelease();
    }

    void SkillRelease()
    {
        var buttons = atkSkill;

        switch (type)
        {
            case StatusType.HP:
                buttons = hpSkill;
                break;

            case StatusType.MP:
                buttons = mpSkill;
                break;

            case StatusType.ATK:
                buttons = atkSkill;
                break;

            case StatusType.DEF:
                buttons = defSkill;
                break;

            case StatusType.AGI:
                buttons = agiSkill;
                break;

            case StatusType.DEX:
                buttons = dexSkill;
                break;

            default:
                buttons = atkSkill;
                break;
        }

        // �S�Ĕ�\���ɂ���
        for (int j = 0; j < allSkill.Length; j++)
        {
            allSkill[j].gameObject.SetActive(false);
        }

        for (int j = 0; j < buttons.Length; j++)
        {
            buttons[j].gameObject.SetActive(false);
        }

        for (int i = 0; i < stm.specialTecniques.Length; i++)
        {
            for (int j = 0; j < buttons.Length; j++)
            {
                // ScriptableObject�ƃQ�[���I�u�W�F�N�g(�{�^��)�̖��O�������Ȃ�
                // ������ς݂Ȃ�
                if (stm.specialTecniques[i].name == buttons[j].name &&
                    stm.specialTecniques[i].m_released)
                {
                    buttons[j].gameObject.SetActive(true);
                }
            }
        }
    }
}
