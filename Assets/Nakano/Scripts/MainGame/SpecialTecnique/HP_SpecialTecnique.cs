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
        // –¢‰ğ•ú‚È‚çˆ—‚µ‚È‚¢
        if(!rankC.m_released) return;
    }

    public void LoseLose()
    {
        // –¢‰ğ•ú‚È‚çˆ—‚µ‚È‚¢
        if (!rankB.m_released) return;
    }

    public void AutoHeal()
    {
        // –¢‰ğ•ú‚È‚çˆ—‚µ‚È‚¢
        if (!rankA.m_released) return;
    }

    public void Undefeated()
    {
        // –¢‰ğ•ú‚È‚çˆ—‚µ‚È‚¢
        if (!rankS.m_released) return;
    }

    public void GoddessBlessing()
    {
        // –¢‰ğ•ú‚È‚çˆ—‚µ‚È‚¢
        if (!rankSS.m_released) return;
    }
}
