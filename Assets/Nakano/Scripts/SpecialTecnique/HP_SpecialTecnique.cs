using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HP_SpecialTecnique : SpecialTecniqueMethod
{
    bool isActive_B = false; // スキル発動中かどうか

    bool isActive_S = false;

    int elapsedTurn_SS = 1;

    public override void GameStart()
    {
        RankS(); // 不倒の構え
    }

    public override void TurnEnd()
    {
        RankA();
        RankSS();

        elapsedTurn_SS++;
    }

    /// <summary>
    /// クリアヒール　スキル
    /// 状態異常を回復、HPをV％回復
    /// ボタン押下時に処理
    /// </summary>
    public void RankC()
    {
        // 未解放なら処理しない
        if(!rankC.m_released) return;

        if (!player.CostMP(rankC.m_cost)) return;

        // 回復量計算
        float amount = player.HP * (rankC.m_value1 / 100.0f);
        
        player.BuffMotion(() => 
        {
            player.HealHP((int)amount);

            // デバフ解除
            player.ResetDebuff();

            Debug.Log("「クリアヒール」発動 HP " + amount + "回復");
        });
    }

    /// <summary>
    /// 痛み分け　スキル
    /// </summary>
    public  void RankB()
    {
        // 未解放なら処理しない
        if (!rankB.m_released) return;

        if (!player.CostMP(rankB.m_cost)) return;
        
        isActive_B = true;

        player.AddState(true, rankB.m_id, rankB.m_continuationTurn, 0, () => { Cancel_RankB(); }, true);

        player.BuffMotion(() => { Debug.Log("「痛み分け」発動"); });
    }

    /// <summary>
    /// 被ダメージ時処理
    /// ボタン押下から指定ターン経過するまで処理
    /// </summary>
    /// <param name="_damage">ダメージ量</param>
    /// <param name="_enmey">対象の敵</param>
    public void _RankB(int _damage, Enemy _enemy)
    {
        if (!isActive_B) return;

        // カウンターのダメージ量算出
        float d = (float)_damage * (float)(rankB.m_value1 / 100.0f);

        // 防御無視カウンター
        _enemy.Damage((int)d, true);
        Debug.Log("「痛み分け」 カウンターダメージ " + d);
    }

    /// <summary>
    /// 「痛み分け」 解除
    /// </summary>
    public void Cancel_RankB()
    {
        isActive_B = false;

        Debug.Log("痛み分け　解除");
    }

    /// <summary>
    /// 痛み分け バトル再開時の処理
    /// </summary>
    public void RankB_Restart()
    {
        isActive_B = true;
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

        // HPが指定値以上なら
        if (hpPer >= rankS.m_value1)
        {
            // バフが掛かっていない場合のみバフを掛ける
            if (!isActive_S)
            {
                player.AddBuff(StatusType.ATK, amount, true);
                isActive_S = true;
                Debug.Log("「不倒の構え」発動 攻撃力 " + amount + "上昇");

                player.AddState(true, rankS.m_id, 999, amount, () => 
                {
                    Cancel_RankS();
                }, true);
            }
        }

        else
        {
            // バフが掛かっている場合、バフを無くす
            if (isActive_S)
            {
                Cancel_RankS();

                player.RemoveState(rankS.m_id);
            }
        }
    }

    void Cancel_RankS()
    {
        float amount = (float)rankS.m_value2 / 100;
        isActive_S = false;
        player.AddBuff(StatusType.ATK, -amount, false);
    }

    /// <summary>
    /// 女神の加護　パッシブ
    /// nターン毎にHPをV％回復
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
