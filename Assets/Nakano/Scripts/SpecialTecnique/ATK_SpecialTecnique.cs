using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ATK_SpecialTecnique : SpecialTecniqueMethod
{
    [SerializeField] Animator explosionEffect;

    GameObject[] enemies;

    int effectAmount_A = 0; // 現在の効果量

    public override void GameStart()
    {
        enemies = GameObject.FindGameObjectsWithTag("Enemy");

        RankB();
    }

    /// <summary>
    /// ピアス　スキル
    /// Nターンの間、敵の防御無視
    /// </summary>
    public  void RankC()
    {
        // 未解放なら処理しない
        if (!rankC.m_released) return;

        if (!player.CostMP(rankC.m_cost)) return;

        player.AddState(true, rankC.m_id, rankC.m_continuationTurn, 0, () => { Cancel_RankC(); }, true);

        player.BuffMotion(() => 
        {
            for (int i = 0; i < enemies.Length; i++)
            {
                // 全ての敵を防御無視状態に変更
                enemies[i].GetComponent<Enemy>().isIgnoreDeffence = true;
            }

            Debug.Log("「ピアス」発動");
        });
    }

    /// <summary>
    /// ピアス解除
    /// </summary>
    public void Cancel_RankC()
    {
        for (int i = 0; i < enemies.Length; i++)
        {
            enemies[i].GetComponent<Enemy>().isIgnoreDeffence = false;
        }

        Debug.Log("「ピアス」解除");
    }

    public void RankC_Restart()
    {
        for (int i = 0; i < enemies.Length; i++)
        {
            // 全ての敵を防御無視状態に変更
            enemies[i].GetComponent<Enemy>().isIgnoreDeffence = true;
        }
    }

    float lastAmount = 0;
    float lastLostHp = 0;
    /// <summary>
    /// 背水の陣　パッシブ
    /// HPが減少するごとに攻撃力が上がる
    /// </summary>
    public void RankB()
    {
        // 未解放なら処理しない
        if(!rankB.m_released) return;

        float lostHp = (float)(player.HP - player.currentHp) / player.HP * 100.0f; // 失ったHPの割合(%)

        player.AddBuff(StatusType.ATK, -lastAmount, false);
        player.RemoveState(rankB.m_id);

        int value = (int)(lostHp / rankB.m_value1);
        float amount = rankB.m_value2 / 100.0f * value;

        if (lostHp == 0 || value == 0)
        {
            lastAmount = 0;
            lastLostHp = 0;
            return;
        }

        // 今回失ったHPが前回失ったHP量より少ないとき＝回復したとき
        if (lostHp <= lastLostHp)
        {
            // 演出なし
            player.AddBuff(StatusType.ATK, amount, false);
        }
        else player.AddBuff(StatusType.ATK, amount, true);
        player.AddState(true, rankB.m_id, 999, (amount * 100.0f), null, true);

        lastAmount = amount;
        lastLostHp = lostHp;

        Debug.Log("「背水の陣」発動 攻撃力 " + (amount * 100) + "% 上昇");
    }

    Enemy enemy_RankA = new();
    /// <summary>
    /// ガードブレイカー　パッシブ
    /// 通常攻撃時、V%の確率で敵の防御力をW％下げる　最大80％ダウン
    /// </summary>
    public void RankA(Enemy _enemy)
    {
        // 未解放なら処理しない
        if(!rankA.m_released) return;

        enemy_RankA = _enemy;
        int max = 80;
        int amount = 0;

        // 効果量最大値まで行っていたら処理しない
        if (effectAmount_A >= max) return;

        int result = Random.Range(1, 100);
        if (result <= rankA.m_value1)
        {
            amount = rankA.m_value2;
            if (effectAmount_A + amount <= max)
            {
                effectAmount_A += amount;
                _enemy.AddDebuff(StatusType.DEF, ((float)amount / 100.0f), true);

                _enemy.AddState(false, rankA.m_id, rankA.m_continuationTurn, amount, () => { Cancel_RankA(); }, false);
            }

            Debug.Log("「ガードブレイカー」発動 敵の防御力" + amount + " %ダウン 合計" + effectAmount_A + " %ダウン");
        }
    }

    public void Cancel_RankA()
    {
        float amount = rankA.m_value2 / 100.0f;
        enemy_RankA.AddDebuff(StatusType.DEF, -amount, false);

        Debug.Log("「ガードブレイカー」解除");
    }

    public void RankA_Restart(Enemy _enemy)
    {
        int amount = rankA.m_value2;
        effectAmount_A += amount;
        _enemy.AddDebuff(StatusType.DEF, ((float)amount / 100.0f), false);

        enemy_RankA = _enemy;
    }

    /// <summary>
    /// 全身全霊　パッシブ
    /// 必殺技を打つ前に攻撃力をV%上昇させる
    /// </summary>
    public void RankS(System.Action _specialMove)
    {
        // 未解放なら処理しない
        if (!rankS.m_released)
        {
            _specialMove?.Invoke();
            return;
        }

        float amount = (float)rankS.m_value1 / 100.0f;

        player.AddState(true, rankS.m_id, 1, rankS.m_value1, () => { Cancel_RankS(); }, false);

        player.AddBuff(StatusType.ATK, amount, true);
        Debug.Log("「全身全霊」発動 攻撃力 " + (amount * 100) + "%アップ");

        _specialMove?.Invoke();
    }

    public void Cancel_RankS()
    {
        float amount = (float)rankS.m_value1 / 100.0f;
        player.AddBuff(StatusType.ATK, -amount, false);

        Debug.Log("「全身全霊」解除");
    }

    public void RankS_Restart()
    {
        float amount = (float)rankS.m_value1 / 100.0f;
        player.AddBuff(StatusType.ATK, amount, false);
    }

    /// <summary>
    /// エクスプロージョン　スキル
    /// 攻撃力V％の全体攻撃
    /// </summary>
    public  void RankSS()
    {
        // 未解放なら処理しない
        if(!rankSS.m_released) return;

        if (!player.CostMP(rankSS.m_cost)) return;

        Debug.Log("「エクスプロージョン」発動");

        // 会心抽選
        var cri = player.CriticalLottery();

        float amount = (float)rankSS.m_value1 / 100.0f * (float)player.ATK * player.power_Skill * player.critical;
        
        player.AttackMotion(() => 
        {
            for (int i = 0; i < enemies.Length; i++)
            {
                if (enemies[i] == null || enemies[i].activeSelf == false) continue;

                var ene = enemies[i].GetComponent<Enemy>();
                ene.Damage(amount);
                if (cri) ene.CriticalDamage();
            }

            explosionEffect.SetTrigger("Play");
        });
    }
}
