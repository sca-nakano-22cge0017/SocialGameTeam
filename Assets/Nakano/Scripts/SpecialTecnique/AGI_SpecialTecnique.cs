using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AGI_SpecialTecnique : SpecialTecniqueMethod
{
    int effectAmount_A = 0;

    public override void TurnEnd()
    {
        effectAmount_A = 0;
    }

    /// <summary>
    /// 加速　スキル
    /// Nターンの間、速度をV%上げる
    /// </summary>
    public  void RankC()
    {
        // 未解放なら処理しない
        if(!rankC.m_released) return;

        if (!player.CostMP(rankC.m_cost)) return;

        float amount = (float)rankC.m_value1 / 100.0f;

        player.AddState(true, rankC.m_id, rankC.m_continuationTurn, rankC.m_value1, () => { Cancel_RankC(); }, false);

        player.BuffMotion(() => 
        {
            player.AddBuff(StatusType.AGI, amount);

            Debug.Log("「加速」発動 速度" + (amount * 100) + "%アップ");
        });
    }

    /// <summary>
    /// 「加速」解除
    /// </summary>
    void Cancel_RankC()
    {
        float amount = (float)rankC.m_value1 / 100.0f;
        player.AddBuff(StatusType.AGI, -amount);

        Debug.Log("「加速」解除");
    }

    /// <summary>
    /// スロウ　スキル
    /// Nターンの間、敵の速度をV％下げる
    /// </summary>
    public void RankB()
    {
        // 未解放なら処理しない
        if(!rankB.m_released) return;

        if (!player.CostMP(rankB.m_cost)) return;

        // ロックオンした敵にデバフ
        Enemy enemy = mainGameSystem.Target;

        float amount = (float)rankB.m_value1 / 100.0f;

        enemy.AddState(false, rankB.m_id, rankB.m_continuationTurn, rankB.m_value1, () => { Cancel_RankB(enemy); }, false);

        player.BuffMotion(() => 
        {
            enemy.AddDebuff(StatusType.AGI, amount);

            Debug.Log("「スロウ」発動 敵の速度" + (amount * 100) + "%ダウン");
        });
    }

    /// <summary>
    /// 「スロウ」解除
    /// </summary>
    void Cancel_RankB(Enemy _enemy)
    {
        float amount = (float)rankB.m_value1 / 100.0f;
        _enemy.AddDebuff(StatusType.AGI, -amount);

        Debug.Log("「スロウ」解除");
    }

    /// <summary>
    /// 再行動　パッシブ
    /// 通常攻撃時、V%の確率でもう一度攻撃する
    /// </summary>
    public bool RankA()
    {
        // 未解放なら処理しない
        if(!rankA.m_released) return false;

        // 再行動時に更に再行動しないように回数制限
        if (effectAmount_A > 0) return false;

        int result = Random.Range(1, 100);
        if (result <= rankA.m_value1)
        {
            effectAmount_A++;
            Debug.Log("「再行動」発動");
            return true;
        }

        return false;
    }

    /// <summary>
    /// ステップ　パッシブ
    /// 被ダメ時、V%の確率で回避
    /// </summary>
    public bool RankS()
    {
        // 未解放なら処理しない
        if(!rankS.m_released) return false;

        int result = Random.Range(1, 100);
        if (result <= rankS.m_value1)
        {
            Debug.Log("「ステップ」発動");
            return true;
        }

        return false;
    }

    /// <summary>
    /// 神速の業　スキル
    /// 必殺技ゲージをマックスにする
    /// </summary>
    public  void RankSS()
    {
        // 未解放なら処理しない
        if(!rankSS.m_released) return;

        if (!player.CostMP(rankSS.m_cost)) return;

        player.BuffMotion(() => 
        {
            player.UpSpecialMoveGuage();

            Debug.Log("「神速の業」発動");
        });
    }
}
