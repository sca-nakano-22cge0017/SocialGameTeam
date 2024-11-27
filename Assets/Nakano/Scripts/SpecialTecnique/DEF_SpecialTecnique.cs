using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DEF_SpecialTecnique : SpecialTecniqueMethod
{
    bool isActive_A = false; // スキル発動中かどうか
    int elapsedTurn_A = 0;       // スキル発動からの経過ターン

    bool isActive_SS = false;     // スキル発動中かどうか
    List<int> elapsedTurn_SS = new(); // スキル発動からの経過ターン

    public override void TurnEnd()
    {
        // 経過ターンを加算
        elapsedTurn_A++;
        elapsedTurn_SS = TurnPass(elapsedTurn_SS);

        Cancel_RankA();
        Cancel_RankSS();
    }

    /// <summary>
    /// MP吸収　パッシブ
    /// 防御時にMPをV％回復
    /// 防御時に処理
    /// </summary>
    public  void RankC()
    {
        // 未解放なら処理しない
        if(!rankC.m_released) return;

        float amount = (float)player.MP * ((float)rankC.m_value1 / 100.0f);
        player.HealMP((int)amount);

        Debug.Log("「MP吸収」発動 MP" + amount + " 回復");
    }

    /// <summary>
    /// HP吸収　パッシブ
    /// 防御時にHPをV％回復
    /// 防御時に処理
    /// </summary>
    public  void RankB()
    {
        // 未解放なら処理しない
        if(!rankB.m_released) return;

        float amount = (float)player.HP * ((float)rankB.m_value1 / 100.0f);
        player.HealHP((int)amount);

        Debug.Log("「HP吸収」発動 HP" + (int)amount + " 回復");
    }

    /// <summary>
    /// 無敵　スキル
    /// 1ターンの間無敵状態
    /// </summary>
    public  void RankA()
    {
        // 未解放なら処理しない
        //if(!rankA.m_released) return;

        elapsedTurn_A = 1;
        isActive_A = true;

        Debug.Log("「無敵」発動");

        player.BuffMotion();
    }

    /// <summary>
    /// 被ダメ―ジ時処理
    /// </summary>
    public void _RankA()
    {
        // スキル発動中でなければ処理しない
        if (!isActive_A) return;

        // スキル発動からの経過ターンが指定ターン以下　＝　スキル持続中なら
        if (elapsedTurn_A <= rankA.m_continuationTurn)
        {
            player.isInvincible = true;
        }
    }

    /// <summary>
    /// 「無敵」解除
    /// </summary>
    void Cancel_RankA()
    {
        if (elapsedTurn_A > rankA.m_continuationTurn)
        {
            player.isInvincible = false;
        }
    }

    /// <summary>
    /// 攻防一体　パッシブ
    /// 防御時敵にダメージをV％反射
    /// 防御状態で被ダメ時に処理
    /// </summary>
    public void RankS(int _damage, Enemy _enemy)
    {
        // 未解放なら処理しない
        if(!rankS.m_released) return;

        // ダメージ量計算
        float amount = (float)_damage * ((float)rankS.m_value1 / 100.0f);

        if (_enemy == null || _enemy.gameObject.activeSelf == false) return;
        _enemy.Damage(amount);

        Debug.Log("「攻防一体」発動 カウンターダメージ " + amount);
    }

    /// <summary>
    /// 守護神の権能　スキル
    /// 重ね掛け可
    /// </summary>
    public  void RankSS()
    {
        // 未解放なら処理しない
        if(!rankSS.m_released) return;

        elapsedTurn_SS.Add(1);
        isActive_SS = true;

        Debug.Log("「守護神の権能」発動");

        player.BuffMotion();
    }

    /// <summary>
    /// nターンの間、被ダメージをV％カット
    /// 被ダメージに処理
    /// </summary>
    /// <param name="_damage">被ダメ量</param>
    /// <returns>ダメージカット量</returns>
    public int _RankSS(int _damage)
    {
        // スキル発動中でなければ処理しない
        if (!isActive_SS) return 0;

        float cutPercent = 0; // ダメージカット量 %

        for (int i = 0; i < elapsedTurn_SS.Count; i++)
        {
            // スキル発動からの経過ターンが指定ターン以下　＝　スキル持続中なら
            if (elapsedTurn_SS[i] <= rankSS.m_continuationTurn)
            {
                cutPercent += (float)rankSS.m_value1;
            }
        }

        float amount = (float)_damage * (cutPercent / 100.0f);
        Debug.Log("「守護神の権能 ダメージ " + amount + " カット");
        return (int)amount;
    }

    /// <summary>
    /// 「守護神の権能」解除
    /// </summary>
    void Cancel_RankSS()
    {
        if (!isActive_SS) return;

        for (int i = 0; i < elapsedTurn_SS.Count; i++)
        {
            if (elapsedTurn_SS[i] > rankSS.m_continuationTurn)
            {
                elapsedTurn_SS.Remove(elapsedTurn_SS[i]);
            }
        }

        for (int i = 0; i < elapsedTurn_SS.Count; i++)
        {
            if (elapsedTurn_SS[i] <= rankSS.m_continuationTurn)
            {
                return;
            }
        }

        isActive_SS = false;
        Debug.Log("「守護神の権能」解除");
    }
}
