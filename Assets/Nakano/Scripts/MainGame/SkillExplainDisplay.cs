using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// ƒXƒLƒ‹‚ÌÚ×•\¦
/// </summary>
public class SkillExplainDisplay : MonoBehaviour
{
    private float longTapTime = 0.5f;

    [SerializeField] private GameObject skillDetailWindow;
    [SerializeField] private Image skillIcon;
    [SerializeField] private Text skillName;
    [SerializeField] private Text skillExplain;

    private bool isTapping = false;
    private float tapTime = 0;

    private SpecialTecnique st;

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
        }
    }

    public void PointerDown(SpecialTecnique _st)
    {
        st = _st;

        isTapping = true;
        tapTime = 0;
    }

    public void PointerUp()
    {
        isTapping = false;
        tapTime = 0;

        skillDetailWindow.SetActive(false);
    }
}
