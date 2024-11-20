using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AGI_SpecialTecnique : SpecialTecniqueMethod
{
    bool isActive_C = false;
    List<int> elapsedTurn_C = new();

    bool isActive_B = false;
    List<EnemyBuffTurn> elapsedTurn_B = new();

    int effectAmount_A = 0;

    public override void TurnEnd()
    {
        // 経過ターンを加算
        elapsedTurn_C = TurnPass(elapsedTurn_C);
        elapsedTurn_B = TurnPass(elapsedTurn_B);
        effectAmount_A = 0;

        Cancel_RankC();
        Cancel_RankB();
    }

    /// <summary>
    /// 加速　スキル
    /// Nターンの間、速度をV%上げる
    /// </summary>
    public  void RankC()
    {
        // 未解放なら処理しない
        //if(!rankC.m_released) return;

        elapsedTurn_C.Add(1);
        isActive_C = true;

        float amount = (float)rankC.m_value1 / 100.0f;
        player.AddBuff(StatusType.AGI, amount);

        Debug.Log("「加速」発動 速度" + (amount * 100) + "%アップ");

        player.BuffMotion();
    }

    /// <summary>
    /// 「加速」解除
    /// </summary>
    void Cancel_RankC()
    {
        for (int i = 0; i < elapsedTurn_C.Count; i++)
        {
            if (elapsedTurn_C[i] > rankC.m_continuationTurn)
            {
                elapsedTurn_C.Remove(elapsedTurn_C[i]);

                float amount = (float)rankC.m_value1 / 100.0f;
                player.AddBuff(StatusType.AGI, -amount);

                Debug.Log("「加速」解除");
            }
        }

        for (int i = 0; i < elapsedTurn_C.Count; i++)
        {
            if (elapsedTurn_C[i] <= rankC.m_continuationTurn)
            {
                return;
            }
        }

        isActive_C = false;
    }

    /// <summary>
    /// スロウ　スキル
    /// Nターンの間、敵の速度をV％下げる
    /// </summary>
    public void RankB()
    {
        // 未解放なら処理しない
        //if(!rankB.m_released) return;

        // Todo ロックオンした敵にデバフ
        Enemy enemy = new();

        EnemyBuffTurn e = new();
        e.enemy = enemy;
        e.elapsedTurn = 1;

        elapsedTurn_B.Add(e);
        isActive_B = true;

        float amount = (float)rankB.m_value1 / 100.0f;
        enemy.AddDebuff(StatusType.AGI, amount);

        Debug.Log("「スロウ」発動 敵の速度" + (amount * 100) + "%ダウン");

        player.BuffMotion();
    }

    /// <summary>
    /// 「スロウ」解除
    /// </summary>
    void Cancel_RankB()
    {
        for (int i = 0; i < elapsedTurn_B.Count; i++)
        {
            if (elapsedTurn_B[i].elapsedTurn > rankB.m_continuationTurn)
            {
                float amount = (float)rankB.m_value1 / 100.0f;
                elapsedTurn_B[i].enemy.AddDebuff(StatusType.AGI, -amount);

                elapsedTurn_B.Remove(elapsedTurn_B[i]);

                Debug.Log("「スロウ」解除");
            }
        }

        for (int i = 0; i < elapsedTurn_B.Count; i++)
        {
            if (elapsedTurn_B[i].elapsedTurn <= rankB.m_continuationTurn)
            {
                return;
            }
        }

        isActive_B = false;
    }

    /// <summary>
    /// 再行動　パッシブ
    /// 通常攻撃時、V%の確率でもう一度攻撃する
    /// </summary>
    public bool RankA()
    {
        // 未解放なら処理しない
        //if(!rankA.m_released) return false;

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
        //if(!rankS.m_released) return false;

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
        //if(!rankSS.m_released) return;

        player.UpSpecialMoveGuage();

        Debug.Log("「神速の業」発動");

        player.BuffMotion();
    }
}
