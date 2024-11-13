using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DEX_SpecialTecnique : SpecialTecniqueMethod
{
    public override void GameStart() { }

    public override void TurnStart() { }

    public override void PlayerTurnStart() { }

    public override void TurnEnd()
    {
    }

    /// <summary>
    /// ガードクラッシュ　スキル
    /// 敵単体に攻撃力V%の攻撃　敵の防御力をW%落とす
    /// </summary>
    public  void RankC()
    {
        // 未解放なら処理しない
        //if(!rankC.m_released) return;
    }

    /// <summary>
    /// 成長の道　パッシブ
    /// 貰えるランクポイントV%アップ
    /// </summary>
    public  void RankB()
    {
        // 未解放なら処理しない
        //if(!rankB.m_released) return;
    }

    /// <summary>
    /// 小手先のテクニック　パッシブ
    /// 攻撃時、V%の確率で敵を即死させる
    /// </summary>
    public  void RankA()
    {
        // 未解放なら処理しない
        //if(!rankA.m_released) return;

    }

    /// <summary>
    /// バースト　スキル
    /// クリティカル威力をV％アップ
    /// </summary>
    public  void RankS()
    {
        // 未解放なら処理しない
        //if(!rankS.m_released) return;
    }

    /// <summary>
    /// 約束された勝利　スキル
    /// Nターンの間、確定クリティカル　クリティカル時のダメージをV%上げる
    /// </summary>
    public  void RankSS()
    {
        // 未解放なら処理しない
        //if(!rankSS.m_released) return;

    }
}