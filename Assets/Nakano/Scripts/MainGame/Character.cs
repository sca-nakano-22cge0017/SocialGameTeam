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
    [HideInInspector] public int ATK; // 攻撃
    [HideInInspector] public int MP;  // 魔力
    [HideInInspector] public int HP;  // 体力
    [HideInInspector] public int DEF; // 防御
    [HideInInspector] public int AGI; // 速度
    [HideInInspector] public int DEX; // 器用

    // 計算用 現在値
    [HideInInspector] public int currentMp;
    [HideInInspector] public int currentAtk;
    [HideInInspector] public int currentHp;
    [HideInInspector] public int currentDef;
    [HideInInspector] public int currentDex;
    [HideInInspector] public int currentAgi;

    // ステータス倍率
    [HideInInspector] public float powerMp = 1;
    [HideInInspector] public float powerAtk = 1;
    [HideInInspector] public float powerHp = 1;
    [HideInInspector] public float powerDef = 1;
    [HideInInspector] public float powerDex = 1;
    [HideInInspector] public float powerAgi = 1;

    // バフ総量 割合
    [HideInInspector] public float buffMp = 0;
    [HideInInspector] public float buffAtk = 0;
    [HideInInspector] public float buffHp = 0;
    [HideInInspector] public float buffDef = 0;
    [HideInInspector] public float buffDex = 0;
    [HideInInspector] public float buffAgi = 0;

    // デバフ総量 割合
    [HideInInspector] public float debuffMp = 0;
    [HideInInspector] public float debuffAtk = 0;
    [HideInInspector] public float debuffHp = 0;
    [HideInInspector] public float debuffDef = 0;
    [HideInInspector] public float debuffDex = 0;
    [HideInInspector] public float debuffAgi = 0;

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

        hpGuage.Initialize(HP);
        if (damageText) damageText.enabled = false;
    }

    /// <summary>
    /// 行動
    /// </summary>
    public virtual void Move() { }

    /// <summary>
    /// 行動終了
    /// </summary>
    public virtual void MoveEnd() { }

    /// <summary>
    /// 通常攻撃
    /// </summary>
    public virtual void NormalAttack() { }

    /// <summary>
    /// ダメージ
    /// </summary>
    /// <param name="_amount">ダメージ量</param>
    /// <returns>防御力分減少させたダメージ量</returns>
    public virtual int Damage(int _amount)
    {
        // 被ダメ - 防御力 を実際の被ダメージにする
        int damage = (_amount - (int)(DEF * powerDef));
        damage = damage < 0 ? 0 : damage; // 0未満なら0にする

        currentHp -= damage;

        if (currentHp < 0)
        {
            currentHp = 0;
            Dead();
        }

        // ゲージ減少演出
        hpGuage.Sub(damage);

        // ダメージ表示
        StartCoroutine(DispText(damageText, damage.ToString()));

        return damage;
    }

    /// <summary>
    /// ダメージ
    /// </summary>
    /// <param name="_amount">ダメージ量</param>
    /// <param name="cantGuard">防御無視かどうか　trueなら防御無視</param>
    /// <returns>防御力分減少させたダメージ量</returns>
    public virtual int Damage(int _amount, bool cantGuard)
    {
        // 被ダメ - 防御力 を実際の被ダメージにする
        // 防御無視のときは被ダメから防御力分減少させない
        int damage = cantGuard ? _amount : (_amount - (int)(DEF * powerDef));
        damage = damage < 0 ? 0 : damage; // 0未満なら0にする

        currentHp -= damage;

        if (currentHp < 0)
        {
            currentHp = 0;
            Dead();
        }

        // ゲージ減少演出
        hpGuage.Sub(damage);

        // ダメージ表示
        StartCoroutine(DispText(damageText, damage.ToString()));

        return damage;
    }

    /// <summary>
    /// HP回復
    /// </summary>
    /// <param name="_amount">回復量</param>
    public virtual void HealHP(int _amount)
    {
        currentHp += _amount;
        if (currentHp > HP) currentHp = HP;
        hpGuage.Add(_amount);
    }

    /// <summary>
    /// 死亡
    /// </summary>
    public virtual void Dead() { }

    /// <summary>
    /// バフ追加
    /// </summary>
    /// <param name="_type">ステータスの種類</param>
    /// <param name="_amount">バフ量　割合表記</param>
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
    /// バフ状態リセット
    /// </summary>
    public void ResetBuff()
    {
        buffMp = 0;
        buffAtk = 0;
        buffHp = 0;
        buffDef = 0;
        buffDex = 0;
        buffAgi = 0;
    }

    /// <summary>
    /// デバフ追加
    /// </summary>
    /// <param name="_type">ステータスの種類</param>
    /// <param name="_amount">デバフ量</param>
    public void AddDebuff(StatusType _type, float _amount)
    {
        switch (_type)
        {
            case StatusType.HP:
                debuffHp += _amount;
                break;
            case StatusType.MP:
                debuffMp += _amount;
                break;
            case StatusType.ATK:
                debuffAtk += _amount;
                break;
            case StatusType.DEF:
                debuffDef += _amount;
                break;
            case StatusType.AGI:
                debuffAgi += _amount;
                break;
            case StatusType.DEX:
                debuffDex += _amount;
                break;
        }

        CalcPower();
    }

    /// <summary>
    /// デバフ状態リセット
    /// </summary>
    public void ResetDebuff()
    {
        debuffMp = 0;
        debuffAtk = 0;
        debuffHp = 0;
        debuffDef = 0;
        debuffDex = 0;
        debuffAgi = 0;
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
    /// テキスト表示　ダメージ量等々
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
