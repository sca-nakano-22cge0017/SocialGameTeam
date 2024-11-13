using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AGI_SpecialTecnique : SpecialTecniqueMethod
{
    public override void GameStart() { }

    public override void TurnStart() { }

    public override void PlayerTurnStart() { }

    public override void TurnEnd()
    {
    }

    /// <summary>
    /// 加速　スキル
    /// Nターンの間、速度をV%上げる
    /// </summary>
    public  void RankC()
    {
        // 未解放なら処理しない
        //if(!rankC.m_released) return;
    }

    /// <summary>
    /// スロウ　スキル
    /// Nターンの間、敵の速度をV％上げる
    /// </summary>
    public  void RankB()
    {
        // 未解放なら処理しない
        //if(!rankB.m_released) return;
    }

    /// <summary>
    /// 再行動　パッシブ
    /// 通常攻撃時、V%の確率でもう一度攻撃する
    /// </summary>
    public  void RankA()
    {
        // 未解放なら処理しない
        //if(!rankA.m_released) return;

    }

    /// <summary>
    /// ステップ　パッシブ
    /// 被ダメ時、V%の確率で回避
    /// </summary>
    public  void RankS()
    {
        // 未解放なら処理しない
        //if(!rankS.m_released) return;
    }

    /// <summary>
    /// 神速の業　スキル
    /// 必殺技ゲージをマックスにする
    /// </summary>
    public  void RankSS()
    {
        // 未解放なら処理しない
        //if(!rankSS.m_released) return;

    }
}
