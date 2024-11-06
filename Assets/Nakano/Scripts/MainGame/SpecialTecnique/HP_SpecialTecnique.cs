using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HP_SpecialTecnique : MonoBehaviour, SpecialTecniqueMethod
{
    [SerializeField] PlayerData player;
    [SerializeField] BattleSystem battleSystem;

    [SerializeField] SpecialTecnique rankC;
    [SerializeField] SpecialTecnique rankB;
    [SerializeField] SpecialTecnique rankA;
    [SerializeField] SpecialTecnique rankS;
    [SerializeField] SpecialTecnique rankSS;

    void Awake()
    {
        
    }

    /// <summary>
    /// 経過ターン増える毎に呼び出し
    /// </summary>
    public void Turn()
    {
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

        // 回復量計算
        float amount = player.HP * (rankC.m_value1 / 100);
        player.HealHP((int)amount);

        // デバフ解除
        player.ResetDebuff();
    }

    /// <summary>
    /// 痛み分け　スキル
    /// 一定ターン　ダメージのV％を敵に返す
    /// ボタン押下から指定ターン経過するまで処理
    /// </summary>
    public void RankB()
    {
        // 未解放なら処理しない
        if (!rankB.m_released) return;
    }

    /// <summary>
    /// オートヒール　パッシブ
    /// 毎ターンHPをV％回復
    /// 毎ターン終了時に呼ぶ
    /// </summary>
    public void RankA()
    {
        // 未解放なら処理しない
        if (!rankA.m_released) return;

        // 回復量計算
        float amount = player.HP * (rankA.m_value1 / 100);
        player.HealHP((int)amount);
    }

    bool isAtkUp_S = false;

    /// <summary>
    /// 不倒の構え　パッシブ
    /// 体力がV％以上のとき、攻撃力W%アップ
    /// 毎ターン プレイヤーの行動時判定/処理
    /// </summary>
    public void RankS()
    {
        // 未解放なら処理しない
        if (!rankS.m_released) return;

        float hpPer = player.currentHp / player.HP * 100;
        float amount = rankS.m_value1 / 100;

        // HPが指定値以下なら
        if (hpPer >= rankS.m_value1)
        {
            // バフが掛かっていない場合のみバフを掛ける
            if (!isAtkUp_S)
            {
                isAtkUp_S = true;
                player.AddBuff(StatusType.ATK, amount);
            }
        }
        else
        {
            // バフが掛かっている場合、バフを無くす
            if (isAtkUp_S)
            {
                player.AddBuff(StatusType.ATK, -amount);
                isAtkUp_S = false;
            }
        }
    }

    /// <summary>
    /// 女神の加護　パッシブ
    /// 3ターン毎にHPをV％回復
    /// 毎ターン判定/処理
    /// </summary>
    public void RankSS()
    {
        // 未解放なら処理しない
        if (!rankSS.m_released) return;
    }
}
