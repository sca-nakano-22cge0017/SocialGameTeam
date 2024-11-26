using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HP_SpecialTecnique : SpecialTecniqueMethod
{
    bool isActive_B = false; // スキル発動中かどうか
    int elapsedTurn_B = 0;       // スキル発動からの経過ターン

    bool isActive_SS = false;
    int elapsedTurn_SS = 1;

    public override void TurnStart()
    {

    }

    public override void PlayerTurnStart()
    {
        RankS();
    }

    public override void TurnEnd()
    {
        RankA();
        RankSS();

        // 経過ターンを加算
        elapsedTurn_B++;
        elapsedTurn_SS++;

        Cancel_RankB();
    }

    /// <summary>
    /// クリアヒール　スキル
    /// 状態異常を回復、HPをV％回復
    /// ボタン押下時に処理
    /// </summary>
    public  void RankC()
    {
        // 未解放なら処理しない
        if(!rankC.m_released) return;

        // 回復量計算
        float amount = player.HP * (rankC.m_value1 / 100.0f);
        player.HealHP((int)amount);

        // デバフ解除
        player.ResetDebuff();

        Debug.Log("「クリアヒール」発動 HP " + amount + "回復");

        player.BuffMotion();
    }

    /// <summary>
    /// 痛み分け　スキル
    /// </summary>
    public  void RankB()
    {
        // 未解放なら処理しない
        if (!rankB.m_released) return;

        elapsedTurn_B = 1;
        isActive_B = true; // スキル発動

        Debug.Log("「痛み分け」発動");

        player.BuffMotion();
    }

    /// <summary>
    /// 被ダメージ時処理
    /// nターン　ダメージのV％を敵に返す n = rankB.m_continuationTurn
    /// ボタン押下から指定ターン経過するまで処理
    /// </summary>
    /// <param name="_damage">ダメージ量</param>
    /// <param name="_enmey">対象の敵</param>
    public void _RankB(int _damage, Enemy _enemy)
    {
        // スキル発動中でなければ処理しない
        if (!isActive_B) return;

        // スキル発動からの経過ターンが指定ターン以下　＝　スキル持続中なら
        if (elapsedTurn_B <= rankB.m_continuationTurn)
        {
            // カウンターのダメージ量算出
            float d = (float)_damage * (float)(rankB.m_value1 / 100.0f);

            // 防御無視カウンター
            _enemy.Damage((int)d, true);
            Debug.Log("「痛み分け」 カウンターダメージ " + d);
        }
    }

    /// <summary>
    /// 「痛み分け」 解除
    /// </summary>
    void Cancel_RankB()
    {
        if (elapsedTurn_B > rankB.m_continuationTurn)
        {
            elapsedTurn_B = 0;
            isActive_B = false;
        }
    }

    /// <summary>
    /// オートヒール　パッシブ
    /// 毎ターンHPをV％回復
    /// 毎ターン終了時に呼ぶ
    /// </summary>
    public  void RankA()
    {
        // 未解放なら処理しない
        if (!rankA.m_released) return;

        // 回復量計算
        float amount = (float)player.HP * (float)(rankA.m_value1 / 100.0f);
        player.HealHP((int)amount);

        Debug.Log("「オートヒール」発動 HP " + amount + "回復");
    }

    /// <summary>
    /// 不倒の構え　パッシブ
    /// 体力がV％以上のとき、攻撃力W%アップ
    /// 毎ターン プレイヤーの行動時判定/処理
    /// </summary>
    public  void RankS()
    {
        // 未解放なら処理しない
        if (!rankS.m_released) return;

        float hpPer = (float)player.currentHp / (float)player.HP * 100.0f;
        float amount = (float)rankS.m_value2 / 100;

        // HPが指定値以下なら
        if (hpPer >= rankS.m_value1)
        {
            // バフが掛かっていない場合のみバフを掛ける
            if (!isActive_SS)
            {
                player.AddBuff(StatusType.ATK, amount);
                isActive_SS = true;
                Debug.Log("「不倒の構え」発動 攻撃力 " + amount + "上昇");
            }
        }
        else
        {
            // バフが掛かっている場合、バフを無くす
            if (isActive_SS)
            {
                player.AddBuff(StatusType.ATK, -amount);
                isActive_SS = false;
            }
        }
    }

    /// <summary>
    /// 女神の加護　パッシブ
    /// nターン毎にHPをV％回復　n = rankSS.m_continuationTurn
    /// 毎ターン判定/処理
    /// </summary>
    public  void RankSS()
    {
        // 未解放なら処理しない
        if (!rankSS.m_released) return;

        // 指定ターン経過したら回復
        if (elapsedTurn_SS >= rankSS.m_continuationTurn)
        {
            float heal = (float)player.HP * (float)(rankSS.m_value1 / 100.0f);
            player.HealHP((int)heal);
            elapsedTurn_SS = 0;

            Debug.Log("「女神の加護」発動 HP " + heal + " 回復");
        }
    }
}
