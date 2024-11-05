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
    [SerializeField] protected MainGameGuage hpGuage;
    [SerializeField] protected Text damageText;
    [SerializeField, Header("テキスト表示時間")] protected float textDispTime = 3.0f;

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

    // バフ総量 割合
    public float buffMp = 0;
    public float buffAtk = 0;
    public float buffHp = 0;
    public float buffDef = 0;
    public float buffDex = 0;
    public float buffAgi = 0;

    // デバフ総量 割合
    public float debuffMp = 0;
    public float debuffAtk = 0;
    public float debuffHp = 0;
    public float debuffDef = 0;
    public float debuffDex = 0;
    public float debuffAgi = 0;

    // 会心率
    public float criticalProbability;

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

        damageText.enabled = false;
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
        int damage = _amount - (int)(DEF * powerDef);
        currentHp -= damage;

        if (currentHp < 0)
        {
            currentHp = 0;
            // 死亡判定
        }

        // Todo ダメージ演出・モーション再生
        StartCoroutine(DispText(damageText, damage.ToString()));
    }

    /// <summary>
    /// HP回復
    /// </summary>
    /// <param name="_amount">回復量</param>
    public virtual void HealHP(int _amount)
    {
        currentHp += _amount;

        if (currentHp > HP) currentHp = HP;

        // Todo 回復演出
    }

    /// <summary>
    /// 死亡
    /// </summary>
    public virtual void Dead()
    {

    }

    public void AddBuff(StatusType _type, float _amount)
    {
        switch(_type)
        {
            case StatusType.HP:
                buffHp += _amount;
                break;
            case StatusType.MP:
                buffMp += _amount;
                break;
            case StatusType.ATK:
                buffAtk += _amount;
                break;
            case StatusType.DEF:
                buffDef += _amount;
                break;
            case StatusType.AGI:
                buffAgi += _amount;
                break;
            case StatusType.DEX:
                buffDex += _amount;
                break;
        }

        CalcPower();
    }

    /// <summary>
    /// ステータス倍率計算
    /// </summary>
    void CalcPower()
    {
        powerHp = 1 + buffHp - debuffHp;
        powerMp = 1 + buffMp - debuffMp;
        powerAtk = 1 + buffAtk - debuffAtk;
        powerDef = 1 + buffDef - debuffDef;
        powerAgi = 1 + buffAgi - debuffAgi;
        powerDex = 1 + buffDex - debuffDex;
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

    /// <summary>
    /// テキスト表示　ダメージ等々
    /// </summary>
    /// <returns></returns>
    protected IEnumerator DispText(Text _text, string _str)
    {
        _text.text = _str;
        _text.enabled = true;

        yield return new WaitForSeconds(textDispTime);

        _text.enabled = false;
    }
}
