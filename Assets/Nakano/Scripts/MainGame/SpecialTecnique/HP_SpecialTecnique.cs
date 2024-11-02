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

    /// <summary>
    /// クリアヒール　スキル
    /// </summary>
    public void ClearHeal()
    {
        // 未解放なら処理しない
        if(!rankC.m_released) return;
    }

    /// <summary>
    /// 痛み分け　スキル
    /// </summary>
    public void LoseLose()
    {
        // 未解放なら処理しない
        if (!rankB.m_released) return;
    }

    /// <summary>
    /// オートヒール　常時発動
    /// </summary>
    public void AutoHeal()
    {
        // 未解放なら処理しない
        if (!rankA.m_released) return;
    }

    /// <summary>
    /// 不倒の構え　常時発動
    /// </summary>
    public void Undefeated()
    {
        // 未解放なら処理しない
        if (!rankS.m_released) return;
    }

    /// <summary>
    /// 女神の加護　常時発動
    /// </summary>
    public void GoddessBlessing()
    {
        // 未解放なら処理しない
        if (!rankSS.m_released) return;
    }
}
