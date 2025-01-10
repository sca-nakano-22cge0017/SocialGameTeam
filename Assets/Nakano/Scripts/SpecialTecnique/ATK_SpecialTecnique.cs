using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ATK_SpecialTecnique : SpecialTecniqueMethod
{
    GameObject[] enemies;

    int effectAmount_A = 0; // 現在の効果量

    public override void GameStart()
    {
        enemies = GameObject.FindGameObjectsWithTag("Enemy");
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

        player.AddState(true, rankC.m_id, rankC.m_continuationTurn, () => { Cancel_RankC(); }, true);

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
    void Cancel_RankC()
    {
        for (int i = 0; i < enemies.Length; i++)
        {
            enemies[i].GetComponent<Enemy>().isIgnoreDeffence = false;
        }

        Debug.Log("「ピアス」解除");
    }

    /// <summary>
    /// 背水の陣　パッシブ
    /// HPが減少するごとに攻撃力が上がる
    /// </summary>
    public void RankB()
    {
        // 未解放なら処理しない
        if(!rankB.m_released) return;

        float lostHp = (float)(player.HP - player.currentHp) / player.HP;
        float amount = ((float)rankB.m_value2 / 100.0f) / ((float)rankB.m_value1 / 100.0f) * lostHp;
        player.AddBuff(StatusType.ATK, amount);

        Debug.Log("「背水の陣」発動 攻撃力 " + (amount * 100) + "% 上昇");
    }

    /// <summary>
    /// ガードブレイカー　パッシブ
    /// 通常攻撃時、V%の確率で敵の防御力をW％下げる　最大80％ダウン
    /// </summary>
    public void RankA(Enemy _enemy)
    {
        // 未解放なら処理しない
        if(!rankA.m_released) return;

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
                _enemy.AddDebuff(StatusType.DEF, ((float)amount / 100.0f));
            }

            Debug.Log("「ガードブレイカー」発動 敵の防御力" + amount + " %ダウン 合計" + effectAmount_A + " %ダウン");
        }
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

        player.AddState(true, rankS.m_id, 1, () => { Cancel_RankS(); }, false);

        player.AddBuff(StatusType.ATK, amount);
        Debug.Log("「全身全霊」発動 攻撃力 " + (amount * 100) + "%アップ");

        _specialMove?.Invoke();
    }

    void Cancel_RankS()
    {
        float amount = (float)rankS.m_value1 / 100.0f;
        player.AddBuff(StatusType.ATK, -amount);

        Debug.Log("「全身全霊」解除");
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
        });
    }
}
