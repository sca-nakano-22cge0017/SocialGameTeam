using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 初期スキル
/// </summary>
public class InitialSkill : SpecialTecniqueMethod
{
    [SerializeField] private SpecialTecnique heal;
    [SerializeField] private SpecialTecnique fire;

    public void Heal()
    {
        float amount = (float)player.HP * ((float)heal.m_value1 / 100.0f);
        player.HealHP((int)amount);

        Debug.Log("「ヒール」発動 HP " + amount + " 回復");

        player.BuffMotion();
    }

    public void Fire()
    {
        Enemy target = mainGameSystem.Target;
        if (target == null || target.currentHp <= 0) return;

        Debug.Log("「ファイア」発動");

        player.CriticalLottery();

        float amount = (float)player.ATK * ((float)fire.m_value1 / 100.0f) * player.power_Skill * player.critical;
        
        player.AttackMotion();
        target.Damage(amount);
    }
}
