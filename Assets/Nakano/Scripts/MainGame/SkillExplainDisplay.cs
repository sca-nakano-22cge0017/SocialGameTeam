using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System;

/// <summary>
/// スキルの詳細表示
/// </summary>
public class SkillExplainDisplay : MonoBehaviour
{
    private float longTapTime = 0.5f;

    [SerializeField] InitialSkill initialSkill;
    [SerializeField] HP_SpecialTecnique hp_st;
    [SerializeField] DEF_SpecialTecnique def_st;
    [SerializeField] ATK_SpecialTecnique atk_st;
    [SerializeField] MP_SpecialTecnique mp_st;
    [SerializeField] AGI_SpecialTecnique agi_st;
    [SerializeField] DEX_SpecialTecnique dex_st;

    [SerializeField] private WindowController wc;
    [SerializeField] private GameObject skillDetailWindow;
    [SerializeField] private Image skillIcon;
    [SerializeField] private Text skillName;
    [SerializeField] private Text skillCost;
    [SerializeField] private Text skillExplain;

    private bool isTapping = false;
    private float tapTime = 0;

    private SpecialTecnique st;

    public bool isSkillDetailDisp = false;

    private void Start()
    {
        skillDetailWindow.SetActive(false);
    }

    private void OnEnable()
    {
        skillDetailWindow.SetActive(false);
    }

    private void Update()
    {
        if (isTapping)
        {
            tapTime += Time.deltaTime;
        }

        if (tapTime >= longTapTime)
        {
            skillIcon.sprite = st.m_illust;
            skillName.text = st.m_name;
            skillCost.text = $"消費MP : {st.m_cost}";
            skillExplain.text = st.m_effects;

            skillDetailWindow.SetActive(true);

            isSkillDetailDisp = true;
        }
        else isSkillDetailDisp = false;
    }

    public void PointerDown(SpecialTecnique _st)
    {
        st = _st;

        isTapping = true;
        tapTime = 0;
    }

    public void PointerUp(int _skillId)
    {
        isTapping = false;
        tapTime = 0;

        if (isSkillDetailDisp)
        {
            skillDetailWindow.SetActive(false);
            isSkillDetailDisp = false;
        }
        else
        {
            skillDetailWindow.SetActive(false);
            
            StartCoroutine(SkillAct(_skillId));
        }
    }

    IEnumerator SkillAct(int _skillId)
    {
        yield return new WaitForSeconds(0.2f);

        wc.Close();

        yield return new WaitForSeconds(1.0f);

        // スキル発動
        switch (_skillId)
        {
            case -1:
                initialSkill.Heal();
                break;
            case 0:
                initialSkill.Fire();
                break;
            case 1:
                hp_st.RankC();
                break;
            case 2:
                hp_st.RankB();
                break;
            case 8:
                def_st.RankA();
                break;
            case 10:
                def_st.RankSS();
                break;
            case 11:
                atk_st.RankC();
                break;
            case 15:
                atk_st.RankSS();
                break;
            case 16:
                mp_st.RankC();
                break;
            case 20:
                mp_st.RankSS();
                break;
            case 21:
                agi_st.RankC();
                break;
            case 22:
                agi_st.RankB();
                break;
            case 25:
                agi_st.RankSS();
                break;
            case 26:
                dex_st.RankC();
                break;
            case 29:
                dex_st.RankS();
                break;
            case 30:
                dex_st.RankSS();
                break;
        }
    }
}
