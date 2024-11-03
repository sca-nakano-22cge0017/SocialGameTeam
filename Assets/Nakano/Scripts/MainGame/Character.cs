using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// メインゲーム上で動くキャラクター　PlayerDataとEnemyに継承
/// </summary>
public class Character : MonoBehaviour
{
    public Image image; // イラスト

    // ステータス
    public int ATK; // 攻撃
    public int MP;  // 魔力
    public int HP;  // 体力
    public int DEF; // 防御
    public int AGI; // 速度
    public int DEX; // 器用

    // 計算用 現在値
    public int currentMp;
    public int currentAtk;
    public int currentHp;
    public int currentDef;
    public int currentDex;
    public int currentAgi;

    // ステータス倍率
    public float powerMp = 1;
    public float powerAtk = 1;
    public float powerHp = 1;
    public float powerDef = 1;
    public float powerDex = 1;
    public float powerAgi = 1;

    // バフ総量
    public float buffMp = 0;
    public float buffAtk = 0;
    public float buffHp = 0;
    public float buffDef = 0;
    public float buffDex = 0;
    public float buffAgi = 0;

    // 会心率
    public float criticalProbability;

    // 行動終了後に呼び出す
    public delegate void NextMove();
    public NextMove nextTurn;

    /// <summary>
    /// 初期化
    /// </summary>
    public virtual void Initialize()
    {
        currentMp = MP;
        currentAtk = ATK;
        currentHp = HP;
        currentDef = DEF;
        currentDex = DEX;
        currentAgi = AGI;
    }

    /// <summary>
    /// 行動
    /// </summary>
    public virtual void Move()
    {
        
    }

    /// <summary>
    /// 通常攻撃
    /// </summary>
    /// <returns>与ダメージ量</returns>
    public virtual int NormalAttack()
    {
        return -1;
    }

    /// <summary>
    /// ダメージ
    /// </summary>
    /// <param name="_amount">ダメージ量</param>
    public virtual void Damage(int _amount)
    {
        // 被ダメ - 防御力 を実際の被ダメージにする
        currentHp -= (_amount - (int)(DEF * powerDef));

        if (currentHp < 0)
        {
            currentHp = 0;
            // 死亡判定？
        }

        // Todo ダメージ演出・モーション再生
    }

    /// <summary>
    /// 死亡
    /// </summary>
    public virtual void Dead()
    {

    }

    /// <summary>
    /// 会心抽選
    /// </summary>
    protected bool CriticalLottery()
    {
        int c = Random.Range(0, 100);

        if (c < criticalProbability) return true;
        else return false;
    }
}
