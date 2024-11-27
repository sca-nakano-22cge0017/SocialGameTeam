using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DEX_SpecialTecnique : SpecialTecniqueMethod
{
    bool isActive_C = false;
    List<EnemyBuffTurn> elapsedTurn_C = new();

    bool isActive_S = false;
    int elapsedTurn_S = 0;

    bool isActive_SS = false;
    int elapsedTurn_SS = 0;

    public override void GameStart() { }

    public override void TurnStart() { }

    public override void PlayerTurnStart() { }

    public override void TurnEnd()
    {
        // 経過ターン加算
        if (isActive_C) elapsedTurn_C = TurnPass(elapsedTurn_C);
        if (isActive_S) elapsedTurn_S++;
        if (isActive_SS) elapsedTurn_SS++;

        Cancel_RankC();
        Cancel_RankS();
        Cancel_RankSS();
    }

    /// <summary>
    /// ガードクラッシュ　スキル
    /// 敵単体に攻撃力V%の攻撃　敵の防御力をW%落とす
    /// </summary>
    public  void RankC()
    {
        // 未解放なら処理しない
        if(!rankC.m_released) return;

        // ロックオンした敵に攻撃・デバフ
        Enemy enemy = mainGameSystem.Target;

        EnemyBuffTurn e = new();
        e.enemy = enemy;
        e.elapsedTurn = 1;

        elapsedTurn_C.Add(e);
        isActive_C = true;

        if (enemy == null || enemy.gameObject.activeSelf == false) return;

        float damage = (float)rankC.m_value1 / 100.0f * player.ATK * player.power_Skill;
        float debuff = (float)rankC.m_value2 / 100.0f;

        Debug.Log("「ガードクラッシュ」発動 敵の防御力 " + (debuff * 100) + "%ダウン");

        player.AttackMotion();

        enemy.Damage(damage);
        enemy.AddDebuff(StatusType.DEF, debuff);
    }

    void Cancel_RankC()
    {
        if (!isActive_C) return;

        for (int i = 0; i < elapsedTurn_C.Count; i++)
        {
            if (elapsedTurn_C[i].elapsedTurn > rankC.m_continuationTurn)
            {
                elapsedTurn_C.Remove(elapsedTurn_C[i]);

                Debug.Log("「ガードクラッシュ」解除");
            }
        }

        for (int i = 0; i < elapsedTurn_C.Count; i++)
        {
            if (elapsedTurn_C[i].elapsedTurn <= rankC.m_continuationTurn)
            {
                return;
            }
        }

        isActive_C = false;
    }

    /// <summary>
    /// 成長の道　パッシブ
    /// 貰えるランクポイントV%アップ
    /// </summary>
    public float RankB()
    {
        // 未解放なら処理しない
        if(!rankB.m_released) return 0;

        float amount = (float)rankB.m_value1 / 100.0f;
        Debug.Log("「成長の道」発動 ドロップ量" + (amount * 100) + "%アップ");

        return amount;
    }

    /// <summary>
    /// 小手先のテクニック　パッシブ
    /// 攻撃時、V%の確率で敵を即死させる
    /// </summary>
    public  void RankA(Enemy _enemy)
    {
        // 未解放なら処理しない
        if(!rankA.m_released) return;

        int result = Random.Range(1, 100);
        if (result <= rankA.m_value1)
        {
            Debug.Log("「小手先のテクニック」発動");
            _enemy.Dead();
        }
    }

    /// <summary>
    /// バースト　スキル
    /// クリティカル威力をV％アップ
    /// </summary>
    public  void RankS()
    {
        // 未解放なら処理しない
        if(!rankS.m_released) return;

        elapsedTurn_S = 1;
        isActive_S = true;

        float amount = (float)rankS.m_value1 / 100.0f;
        player.buffCriticalPower = amount;

        Debug.Log("「バースト」発動 会心時倍率" + (amount * 100) + "%アップ");

        player.BuffMotion();
    }
    
    /// <summary>
    /// バースト解除
    /// </summary>
    void Cancel_RankS()
    {
        if (!isActive_S) return;

        if (elapsedTurn_S > rankS.m_continuationTurn)
        {
            elapsedTurn_S = 0;
            isActive_S = false;

            player.buffCriticalPower = 0;

            Debug.Log("「バースト」解除");
        }
    }

    /// <summary>
    /// 約束された勝利　スキル
    /// Nターンの間、確定クリティカル　クリティカル時のダメージをV%上げる
    /// </summary>
    public  void RankSS()
    {
        // 未解放なら処理しない
        if(!rankSS.m_released) return;

        elapsedTurn_SS = 1;
        isActive_SS = true;

        float amount = (float)rankS.m_value1 / 100.0f;
        player.buffCriticalPower = amount;

        player._criticalProbability = 100;

        Debug.Log("「約束された勝利」発動 会心時倍率" + (amount * 100) + "%アップ, クリティカル確定");

        player.BuffMotion();
    }
    
    /// <summary>
    /// 約束された勝利　解除
    /// </summary>
    void Cancel_RankSS()
    {
        if (!isActive_SS) return;

        if (elapsedTurn_SS > rankSS.m_continuationTurn)
        {
            elapsedTurn_SS = 0;
            isActive_SS = false;

            player.buffCriticalPower = 0;
            player._criticalProbability = player.criticalProbabilityInitial;

            Debug.Log("「約束された勝利」解除");
        }
    }
}