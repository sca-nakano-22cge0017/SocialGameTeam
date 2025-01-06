using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 掛かっているバフ、デバフを表示
/// </summary>
public class BaffDisplay : MonoBehaviour
{
    [SerializeField] WindowController baffWindow;
    [SerializeField] GameObject prefab;
    [SerializeField] GameObject parent;

    SpecialTecniqueManager specialTecniqueManager;

    private int effectsAmount = 0;

    [SerializeField] private float longTapTime = 0.5f;
    private bool isTapping = false;
    private float tapTime = 0;

    void Start()
    {
        specialTecniqueManager = GameObject.FindObjectOfType<SpecialTecniqueManager>();
        PassiveSkill();
    }

    void Update()
    {
        if (isTapping)
        {
            tapTime += Time.deltaTime;
        }

        if (tapTime >= longTapTime)
        {
            baffWindow.Open();   
        }
    }

    void PassiveSkill()
    {
        for (int i = 0; i < specialTecniqueManager.specialTecniques.Length; i++)
        {
            var skill = specialTecniqueManager.specialTecniques[i];
            
            if (skill.m_released && skill.m_skillType == 2)
            {
                ContentSizeChange();
                var obj = Instantiate(prefab, parent.transform);

                var icon = obj.transform.GetChild(0).gameObject.GetComponent<Image>();
                var explain = obj.transform.GetChild(1).gameObject.GetComponent<Text>();

                icon.sprite = skill.m_illust;
                explain.text = skill.m_effects;
            }
        }
    }

    void ContentSizeChange()
    {
        effectsAmount++;
        var rect = parent.gameObject.GetComponent<RectTransform>();
        var size = rect.sizeDelta;
        size.y = effectsAmount * 140.0f;
        rect.sizeDelta = size;
    }

    public void PointerDown()
    {
        isTapping = true;
        tapTime = 0;
    }

    public void PointerUp()
    {
        isTapping = false;
        tapTime = 0;
    }
}
