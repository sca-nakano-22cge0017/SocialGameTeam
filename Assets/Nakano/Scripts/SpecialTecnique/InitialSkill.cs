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

    [SerializeField] private Enemy enemy;

    public void Heal()
    {
        float amount = (float)player.HP * ((float)heal.m_value1 / 100.0f);
        player.HealHP((int)amount);

        Debug.Log("「ヒール」発動 HP " + amount + " 回復");

        player.BuffMotion();
    }

    public void Fire()
    {
        // Todo ロックオンした敵を取得

        if (enemy == null || enemy.gameObject.activeSelf == false) return;

        Debug.Log("「ファイア」発動");

        float amount = (float)player.ATK * ((float)fire.m_value1 / 100.0f);
        
        player.AttackMotion();
        enemy.Damage(amount);
    }
}
