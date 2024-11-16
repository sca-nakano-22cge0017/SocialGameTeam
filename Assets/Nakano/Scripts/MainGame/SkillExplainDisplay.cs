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

    [SerializeField] UnityEvent[] skill;
    [SerializeField] private WindowController wc;
    [SerializeField] private GameObject skillDetailWindow;
    [SerializeField] private Image skillIcon;
    [SerializeField] private Text skillName;
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
                skill[0].Invoke();
                break;
            case 0:
                skill[1].Invoke();
                break;
            case 1:
                skill[2].Invoke();
                break;
            case 2:
                skill[3].Invoke();
                break;
            case 8:
                skill[4].Invoke();
                break;
            case 10:
                skill[5].Invoke();
                break;
            case 11:
                skill[6].Invoke();
                break;
            case 15:
                skill[7].Invoke();
                break;
            case 16:
                skill[8].Invoke();
                break;
            case 20:
                skill[9].Invoke();
                break;
            case 21:
                skill[10].Invoke();
                break;
            case 22:
                skill[11].Invoke();
                break;
            case 25:
                skill[12].Invoke();
                break;
            case 26:
                skill[13].Invoke();
                break;
            case 29:
                skill[14].Invoke();
                break;
            case 30:
                skill[15].Invoke();
                break;
        }
    }
}
