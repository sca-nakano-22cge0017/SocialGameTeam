using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MP_SpecialTecnique : SpecialTecniqueMethod
{
    List<int> elapsedTurn_C = new();
    bool isActive_C = false;

    List<EnemyBuffTurn> elapsedTurn_S = new();
    bool isActive_S = false;

    int elapsedTurn_SS = 0;
    bool isActive_SS = false;

    public override void GameStart() { }

    public override void PlayerTurnStart() { }

    public override void TurnEnd()
    {
        RankA();
        _RankS();
        _RankSS();

        // 経過ターン加算
        elapsedTurn_C = TurnPass(elapsedTurn_C);
        elapsedTurn_S = TurnPass(elapsedTurn_S);
        elapsedTurn_SS++;

        Cancel_RankC();
        Cancel_RankS();
        Cancel_RankSS();
    }

    /// <summary>
    /// オーラ　スキル
    /// 攻撃力・防御力ををV%上げる
    /// </summary>
    public void RankC()
    {
        // 未解放なら処理しない
        //if(!rankC.m_released) return;

        elapsedTurn_C.Add(1);
        isActive_C = true;

        float amount = (float)rankC.m_value1 / 100.0f;
        player.AddBuff(StatusType.ATK, amount);
        player.AddBuff(StatusType.DEF, amount);

        Debug.Log("「オーラ」発動 攻撃力/防御力 " + (amount * 100) + "%アップ");
    }

    /// <summary>
    /// オーラ解除
    /// </summary>
    void Cancel_RankC()
    {
        if (!isActive_C) return;

        for (int i = 0; i < elapsedTurn_C.Count; i++)
        {
            if (elapsedTurn_C[i] > rankC.m_continuationTurn)
            {
                elapsedTurn_C.Remove(elapsedTurn_C[i]);

                float amount = (float)rankC.m_value1 / 100.0f;
                player.AddBuff(StatusType.ATK, -amount);
                player.AddBuff(StatusType.DEF, -amount);
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
        Debug.Log("「オーラ」解除");
    }

    /// <summary>
    /// ドレイン　パッシブ
    /// 通常攻撃時MPをV%回復
    /// </summary>
    public void RankB()
    {
        // 未解放なら処理しない
        //if(!rankB.m_released) return;

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
        //if(!rankA.m_released) return;

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
        //if(!rankS.m_released) return;

        int result = Random.Range(1, 100);
        if (result <= rankS.m_value1)
        {
            EnemyBuffTurn e = new();
            e.enemy = _enemy;
            e.elapsedTurn = 1;

            elapsedTurn_S.Add(e);
            isActive_S = true;

            Debug.Log("「呪い」発動");
        }
    }

    void _RankS()
    {
        for (int i = 0; i < elapsedTurn_S.Count; i++)
        {
            if (elapsedTurn_S[i].elapsedTurn <= rankS.m_continuationTurn)
            {
                Enemy enemy = elapsedTurn_S[i].enemy;

                float amount = (float)rankS.m_value2 / 100.0f * enemy.HP;
                enemy.Damage((int)amount, true);
            }
        }
    }

    /// <summary>
    /// 呪い解除
    /// </summary>
    void Cancel_RankS()
    {
        if (!isActive_S) return;

        for (int i = 0; i < elapsedTurn_S.Count; i++)
        {
            if (elapsedTurn_S[i].elapsedTurn > rankS.m_continuationTurn)
            {
                elapsedTurn_S.Remove(elapsedTurn_S[i]);

                Debug.Log("「呪い」解除");
            }
        }

        for (int i = 0; i < elapsedTurn_S.Count; i++)
        {
            if (elapsedTurn_S[i].elapsedTurn <= rankS.m_continuationTurn)
            {
                return;
            }
        }

        isActive_S = false;
    }

    /// <summary>
    /// 魔術師の結界　スキル
    /// Nターンの間、MP消費量をV％ダウン　必殺技ゲージを1ターン毎にW％回復
    /// </summary>
    public void RankSS()
    {
        // 未解放なら処理しない
        //if(!rankSS.m_released) return;

        elapsedTurn_SS = 1;
        isActive_SS = true;

        float mpAmount = (float)rankSS.m_value1 / 100.0f;
        player.power_CostMp = (1 - mpAmount);

        Debug.Log("「魔術師の結界」発動 MP消費量 " + player.power_CostMp + "倍");
    }

    void _RankSS()
    {
        if (!isActive_SS) return;

        if (elapsedTurn_SS <= rankSS.m_continuationTurn)
        {
            float amount = (float)rankSS.m_value2 / 100.0f * player.specialMoveGuageMax;
            player.UpSpecialMoveGuage((int)amount);

            Debug.Log("「魔術師の結界」発動 必殺技ゲージ " + (int)amount + "回復");
        }
    }

    /// <summary>
    /// 魔術師の結界　解除
    /// </summary>
    void Cancel_RankSS()
    {
        if (!isActive_SS) return;

        if (elapsedTurn_SS > rankSS.m_continuationTurn)
        {
            elapsedTurn_SS = 0;

            Debug.Log("「魔術師の結界」解除");
        }
    }
}

