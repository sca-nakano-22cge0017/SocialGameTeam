using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MP_SpecialTecnique : SpecialTecniqueMethod
{
    public override void GameStart() { }

    public override void TurnStart() { }

    public override void PlayerTurnStart() { }

    public override void TurnEnd()
    {
    }

    /// <summary>
    /// オーラ　スキル
    /// 攻撃力と防御力をV％上げる
    /// </summary>
    public void RankC()
    {
        // 未解放なら処理しない
        //if(!rankC.m_released) return;
    }

    /// <summary>
    /// ドレイン　パッシブ
    /// 通常攻撃時MPをV%回復
    /// </summary>
    public void RankB()
    {
        // 未解放なら処理しない
        //if(!rankB.m_released) return;
    }

    /// <summary>
    /// 魔女の特権　パッシブ
    /// 毎ターンMPをV％回復
    /// </summary>
    public void RankA()
    {
        // 未解放なら処理しない
        //if(!rankA.m_released) return;

    }

    /// <summary>
    /// 呪い　パッシブ
    /// 被ダメ時、V％の確率で敵に呪い状態を付与
    /// 呪い状態：毎ターンHP減少
    /// </summary>
    public void RankS()
    {
        // 未解放なら処理しない
        //if(!rankS.m_released) return;
    }

    /// <summary>
    /// 魔術師の結界　スキル
    /// Nターンの間、MP消費量をV％ダウン　必殺技ゲージを1ターン毎にW％回復
    /// </summary>
    public void RankSS()
    {
        // 未解放なら処理しない
        //if(!rankSS.m_released) return;

    }
}

