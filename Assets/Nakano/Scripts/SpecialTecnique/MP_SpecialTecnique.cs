using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MP_SpecialTecnique : SpecialTecniqueMethod
{
    bool isActive_SS = false;

    public override void TurnEnd()
    {
        RankA();
        _RankSS();
    }

    /// <summary>
    /// オーラ　スキル
    /// 攻撃力・防御力ををV%上げる
    /// </summary>
    public void RankC()
    {
        // 未解放なら処理しない
        if (!rankC.m_released) return;

        if (!player.CostMP(rankC.m_cost)) return;

        float amount = (float)rankC.m_value1 / 100.0f;

        player.AddState(true, rankC.m_id, rankC.m_continuationTurn, () => { Cancel_RankC(); }, false);

        player.BuffMotion(() =>
        {
            player.AddBuff(StatusType.ATK, amount);
            player.AddBuff(StatusType.DEF, amount);

            Debug.Log("「オーラ」発動 攻撃力/防御力 " + (amount * 100) + "%アップ");
        });
    }

    /// <summary>
    /// オーラ解除
    /// </summary>
    void Cancel_RankC()
    {
        float amount = (float)rankC.m_value1 / 100.0f;
        player.AddBuff(StatusType.ATK, -amount);
        player.AddBuff(StatusType.DEF, -amount);
        Debug.Log("「オーラ」解除");
    }

    /// <summary>
    /// ドレイン　パッシブ
    /// 通常攻撃時MPをV%回復
    /// </summary>
    public void RankB()
    {
        // 未解放なら処理しない
        if(!rankB.m_released) return;

        float amount = (float)rankB.m_value1 / 100.0f * player.MP;
        player.HealMP((int)amount);

        Debug.Log("「ドレイン」発動 MP "+ (int)amount + "回復");
    }

    /// <summary>
    /// 魔女の特権　パッシブ
    /// 毎ターンMPをV％回復
    /// </summary>
    public void RankA()
    {
        // 未解放なら処理しない
        if(!rankA.m_released) return;

        float amount = (float)rankA.m_value1 / 100.0f * player.MP;
        player.HealMP((int)amount);

        Debug.Log("「魔女の特権」発動 MP " + (int)amount + "回復");
    }

    /// <summary>
    /// 呪い　パッシブ
    /// 被ダメ時、V％の確率で敵に呪い状態を付与
    /// 呪い状態：毎ターンW%HP減少
    /// </summary>
    public void RankS(Enemy _enemy)
    {
        // 未解放なら処理しない
        if(!rankS.m_released) return;

        int result = Random.Range(1, 100);
        if (result <= rankS.m_value1)
        {
            _enemy.AddState(false, rankS.m_id, rankS.m_continuationTurn, () => { Cancel_RankS(); }, () => { _RankS(_enemy); }, true);

            Debug.Log("「呪い」付与");
        }
    }

    void _RankS(Enemy _enemy)
    {
        float amount = (float)rankS.m_value2 / 100.0f * _enemy.HP;
        _enemy.Damage((int)amount, true);
    }

    /// <summary>
    /// 呪い解除
    /// </summary>
    void Cancel_RankS()
    {
        Debug.Log("「呪い」解除");
    }

    /// <summary>
    /// 魔術師の結界　スキル
    /// Nターンの間、MP消費量をV％ダウン　必殺技ゲージを1ターン毎にW％回復
    /// </summary>
    public void RankSS()
    {
        // 未解放なら処理しない
        if (!rankSS.m_released) return;
        if (!player.CostMP(rankSS.m_cost)) return;

        isActive_SS = true;

        float mpAmount = (float)rankSS.m_value1 / 100.0f;
        player.AddState(true, rankSS.m_id, rankSS.m_continuationTurn, () => { Cancel_RankSS(); }, true);

        player.BuffMotion(() => 
        {
            player.power_CostMp = (1 - mpAmount);

            Debug.Log("「魔術師の結界」発動 MP消費量 " + player.power_CostMp + "倍");
        });
    }

    void _RankSS()
    {
        if (!isActive_SS) return;

        float amount = (float)rankSS.m_value2 / 100.0f * player.specialMoveGuageMax;
        player.UpSpecialMoveGuage((int)amount);

        Debug.Log("「魔術師の結界」発動 必殺技ゲージ " + (int)amount + "回復");
    }

    /// <summary>
    /// 魔術師の結界　解除
    /// </summary>
    void Cancel_RankSS()
    {
        isActive_SS = false;
        Debug.Log("「魔術師の結界」解除");
    }
}

