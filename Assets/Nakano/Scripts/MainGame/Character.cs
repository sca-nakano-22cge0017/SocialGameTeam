using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

/// <summary>
/// メインゲーム上で動くキャラクター　PlayerDataとEnemyに継承
/// </summary>
public class Character : MonoBehaviour
{
    protected SoundController soundController;
    [SerializeField] protected MainGameSystem mainGameSystem;
    [SerializeField] protected MainDirection mainDirection;

    public MeshRenderer meshRenderer;

    [SerializeField] public MainGameGuage hpGuage;
    [SerializeField] protected Text criticalText;
    [SerializeField] protected Text damageText;
    [SerializeField] protected Text healText;
    [SerializeField] protected Text buffText;

    [SerializeField, Header("テキスト表示時間")] protected float textDispTime;

    // ステータス
    [HideInInspector] public int ATK; // 攻撃
    [HideInInspector] public int MP;  // 魔力
    [HideInInspector] public int HP;  // 体力
    [HideInInspector] public int DEF; // 防御
    [HideInInspector] public int AGI; // 速度
    [HideInInspector] public int DEX; // 器用

    // 計算用 現在値
    [HideInInspector] public int currentMp;
    [HideInInspector] public int currentHp;

    // ステータス倍率
    [HideInInspector] public float powerMp ;
    [HideInInspector] public float powerAtk;
    [HideInInspector] public float powerHp ;
    [HideInInspector] public float powerDef;
    [HideInInspector] public float powerDex;
    [HideInInspector] public float powerAgi;

    // バフ総量 割合
    [HideInInspector] public float buffMp ;
    [HideInInspector] public float buffAtk;
    [HideInInspector] public float buffHp ;
    [HideInInspector] public float buffDef;
    [HideInInspector] public float buffDex;
    [HideInInspector] public float buffAgi;

    // デバフ総量 割合
    [HideInInspector] public float debuffMp ;
    [HideInInspector] public float debuffAtk;
    [HideInInspector] public float debuffHp ;
    [HideInInspector] public float debuffDef;
    [HideInInspector] public float debuffDex;
    [HideInInspector] public float debuffAgi;

    // 会心率
    public float criticalProbabilityInitial;  // 初期値
    public float _criticalProbability;        // 計算用

    public float power_CriticalInit;    // 基本会心時倍率
    public float buffCriticalPower;     // 会心時倍率バフ
    public float critical;    // 会心時倍率　計算用

    // モーション関係
    public Animator motion;
    public SpineAnim spineAnim;

    /// <summary>
    /// 初期化
    /// </summary>
    public virtual void Initialize()
    {
        critical = 1.0f;

        currentMp = MP;
        currentHp = HP;

        powerMp = 1;
        powerAtk = 1;
        powerHp = 1;
        powerDef = 1;
        powerDex = 1;
        powerAgi = 1;

        buffMp = 0;
        buffAtk = 0;
        buffHp = 0;
        buffDef = 0;
        buffDex = 0;
        buffAgi = 0;

        debuffMp = 0;
        debuffAtk = 0;
        debuffHp = 0;
        debuffDef = 0;
        debuffDex = 0;
        debuffAgi = 0;

        hpGuage.Initialize(HP);
        if (damageText) damageText.enabled = false;
        if (healText) healText.enabled = false;
        if (buffText) buffText.enabled = false;
        if (criticalText) criticalText.enabled = false;

        _criticalProbability = criticalProbabilityInitial;
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
    /// ターン終了
    /// </summary>
    public virtual void TurnEnd() { }

    /// <summary>
    /// 通常攻撃
    /// </summary>
    public virtual void NormalAttack() { }

    /// <summary>
    /// ダメージ
    /// </summary>
    /// <param name="_amount">ダメージ量</param>
    /// <returns>防御力分減少させたダメージ量</returns>
    public virtual int Damage(float _amount)
    {
        // 被ダメ - 防御力 を実際の被ダメージにする
        int damage = (int)Mathf.Ceil(_amount - (DEF * powerDef));
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
    public virtual int Damage(float _amount, bool cantGuard)
    {
        // 被ダメ - 防御力 を実際の被ダメージにする
        // 防御無視のときは被ダメから防御力分減少させない
        int damage = cantGuard ? (int)Mathf.Ceil(_amount) : (int)Mathf.Ceil(_amount - (DEF * powerDef));
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
    /// 固定ダメージ
    /// </summary>
    /// <param name="_amount">ダメージ量</param>
    /// <returns></returns>
    public virtual int Damage(int _amount)
    {
        int damage = _amount;
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
    public virtual void AddBuff(StatusType _type, float _amount)
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

        if (_amount > 0)
        {
            string str = _type.ToString() + " " + (int)(_amount * 100) + " %UP";
            Color orange = new Color(1.0f, 0.56f, 0.0f, 1.0f);
            StartCoroutine(DispText(buffText, str, orange));
        }
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

        CalcPower();
    }

    /// <summary>
    /// デバフ追加
    /// </summary>
    /// <param name="_type">ステータスの種類</param>
    /// <param name="_amount">デバフ量</param>
    public virtual void AddDebuff(StatusType _type, float _amount)
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

        if (_amount > 0)
        {
            string str = _type.ToString() + " " + (int)(_amount * 100) + " %DOWN";
            StartCoroutine(DispText(buffText, str, Color.blue));
        }
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

        Debug.Log($"倍率 HP:{powerHp}, MP:{powerMp}, ATK:{powerAtk}, DEF:{powerDef}, AGI:{powerAgi}, DEX:{powerDex}");
    }

    /// <summary>
    /// 会心抽選
    /// </summary>
    public bool CriticalLottery()
    {
        int c = UnityEngine.Random.Range(0, 100);

        if (c < _criticalProbability)
        {
            critical = power_CriticalInit + buffCriticalPower;
            Debug.Log("会心発動");
            return true;
        }
        else
        {
            critical = 1.0f;
            return false;
        }
    }

    /// <summary>
    /// 会心抽選
    /// </summary>
    protected bool CriticalLottery(float criticalProbability)
    {
        int c = UnityEngine.Random.Range(0, 100);

        if (c < criticalProbability)
        {
            critical = power_CriticalInit + buffCriticalPower;
            Debug.Log("会心発動");
            return true;
        }
        else
        {
            critical = 1.0f;
            return false;
        }
    }

    /// <summary>
    /// 「Critical!」を表示する
    /// </summary>
    public void CriticalDamage()
    {
        StartCoroutine(DispText(criticalText, "Critical!"));
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

    protected IEnumerator DispText(Text _text, string _str, Color _color)
    {
        _text.text = _str;
        _text.enabled = true;
        _text.color = _color;

        yield return new WaitForSeconds(textDispTime);

        _text.enabled = false;
    }

    protected IEnumerator HPGuageDirectionCompleteWait(Action _action)
    {
        yield return new WaitUntil(() => hpGuage.isDirectionCompleted);

        _action?.Invoke();
    }
}
