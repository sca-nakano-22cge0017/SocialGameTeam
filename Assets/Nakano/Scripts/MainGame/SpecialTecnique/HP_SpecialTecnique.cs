using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HP_SpecialTecnique : MonoBehaviour
{
    [SerializeField] SpecialTecnique rankC;
    [SerializeField] SpecialTecnique rankB;
    [SerializeField] SpecialTecnique rankA;
    [SerializeField] SpecialTecnique rankS;
    [SerializeField] SpecialTecnique rankSS;

    void Start()
    {
        
    }

    void Update()
    {
        
    }

    public void ClearHeal()
    {
        // ������Ȃ珈�����Ȃ�
        if(!rankC.m_released) return;
    }

    public void LoseLose()
    {
        // ������Ȃ珈�����Ȃ�
        if (!rankB.m_released) return;
    }

    public void AutoHeal()
    {
        // ������Ȃ珈�����Ȃ�
        if (!rankA.m_released) return;
    }

    public void Undefeated()
    {
        // ������Ȃ珈�����Ȃ�
        if (!rankS.m_released) return;
    }

    public void GoddessBlessing()
    {
        // ������Ȃ珈�����Ȃ�
        if (!rankSS.m_released) return;
    }
}
