using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DEX_SpecialTecnique : SpecialTecniqueMethod
{
    Enemy enemy_RankC = new();
    /// <summary>
    /// ガードクラッシュ　スキル
    /// 敵単体に攻撃力V%の攻撃　Nターンの間敵の防御力をW%落とす
    /// </summary>
    public  void RankC()
    {
        // 未解放なら処理しない
        if(!rankC.m_released) return;

        if (!player.CostMP(rankC.m_cost)) return;

        // ロックオンした敵に攻撃・デバフ
        Enemy enemy = mainGameSystem.Target;
        enemy_RankC = enemy;

        if (enemy == null || enemy.gameObject.activeSelf == false) return;

        float damage = (float)rankC.m_value1 / 100.0f * player.ATK * player.power_Skill;
        float debuff = (float)rankC.m_value2 / 100.0f;

        Debug.Log("「ガードクラッシュ」発動 敵の防御力 " + (debuff * 100) + "%ダウン");

        enemy.AddState(false, rankC.m_id, rankC.m_continuationTurn, rankC.m_value2, () => { Cancel_RankC(); }, false);

        player.AttackMotion(() => 
        {
            enemy.Damage(damage);
            enemy.AddDebuff(StatusType.DEF, debuff, true);
        });
    }

    public void Cancel_RankC()
    {
        float debuff = (float)rankC.m_value2 / 100.0f;
        enemy_RankC.AddDebuff(StatusType.DEF, -debuff, false);

        Debug.Log("「ガードクラッシュ」解除");
    }

    public void RankC_Restart(Enemy _enemy)
    {
        enemy_RankC = _enemy;

        float debuff = (float)rankC.m_value2 / 100.0f;
        enemy_RankC.AddDebuff(StatusType.DEF, debuff, false);
    }

    /// <summary>
    /// 成長の道　パッシブ
    /// 貰えるランクポイントV%アップ
    /// </summary>
    public float RankB()
    {
        // 未解放なら処理しない
        if(!rankB.m_released) return 0;

        float amount = (float)rankB.m_value1 / 100.0f;
        Debug.Log("「成長の道」発動 ドロップ量" + (amount * 100) + "%アップ");

        return amount;
    }

    /// <summary>
    /// 小手先のテクニック　パッシブ
    /// 攻撃時、V%の確率で敵を即死させる
    /// </summary>
    public  void RankA(Enemy _enemy)
    {
        // 未解放なら処理しない
        if(!rankA.m_released) return;

        int result = Random.Range(1, 100);
        if (result <= rankA.m_value1)
        {
            _enemy.Dead();
            Debug.Log("「小手先のテクニック」発動");
        }
    }

    /// <summary>
    /// バースト　スキル
    /// クリティカル威力をV％アップ
    /// </summary>
    public  void RankS()
    {
        // 未解放なら処理しない
        if (!rankS.m_released) return;

        if (!player.CostMP(rankS.m_cost)) return;

        player.AddState(true, rankS.m_id, rankS.m_continuationTurn, 0, () => { Cancel_RankS(); }, false);

        float amount = (float)rankS.m_value1 / 100.0f;
        
        player.BuffMotion(() => 
        {
            player.buffCriticalPower += amount;

            Debug.Log("「バースト」発動 会心時倍率" + (amount * 100) + "%アップ");
        });
    }
    
    /// <summary>
    /// バースト解除
    /// </summary>
    public void Cancel_RankS()
    {
        float amount = (float)rankS.m_value1 / 100.0f;
        player.buffCriticalPower -= amount;

        Debug.Log("「バースト」解除");
    }

    public void RankS_Restart()
    {
        float amount = (float)rankS.m_value1 / 100.0f;
        player.buffCriticalPower += amount;
    }

    /// <summary>
    /// 約束された勝利　スキル
    /// Nターンの間、確定クリティカル　クリティカル時のダメージをV%上げる
    /// </summary>
    public  void RankSS()
    {
        // 未解放なら処理しない
        if (!rankSS.m_released) return;
        if (!player.CostMP(rankSS.m_cost)) return;

        float amount = (float)rankS.m_value1 / 100.0f;

        player.AddState(true, rankSS.m_id, rankSS.m_continuationTurn, rankS.m_value1, () => { Cancel_RankSS(); }, false);

        player.BuffMotion(() => 
        {
            player.buffCriticalPower += amount;
            player._criticalProbability = 100;

            Debug.Log("「約束された勝利」発動 会心時倍率" + (amount * 100) + "%アップ, クリティカル確定");
        });
    }
    
    /// <summary>
    /// 約束された勝利　解除
    /// </summary>
    public void Cancel_RankSS()
    {
        float amount = (float)rankS.m_value1 / 100.0f;

        player.buffCriticalPower -= amount;
        player._criticalProbability = player.criticalProbabilityInitial;

        Debug.Log("「約束された勝利」解除");
    }

    public void RankSS_Restart()
    {
        float amount = (float)rankS.m_value1 / 100.0f;
        player.buffCriticalPower += amount;
        player._criticalProbability = 100;
    }
}