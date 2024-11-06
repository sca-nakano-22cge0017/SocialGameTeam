using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Master;

/// <summary>
/// プレイヤーのステータスやコマンド管理
/// </summary>
public class PlayerData : Character
{
    // 特殊技能・スキルは別スクリプト

    [SerializeField] private MainGameGuage mpGuage;
    
    // 攻撃倍率
    public float power_NormalAttack;  // 通常攻撃
    public float power_Skill;         // スキル
    public float power_Critical;      // 会心時倍率
    public float power_SpecialMove;   // 必殺技

    // ガード
    private bool isGuard;
    public float power_Guard = 1.2f; // ガード時防御倍率

    // 必殺技ゲージ
    private int specialMoveGuageAmount;
    public int specialMoveGuageMax; // 最大量

    // 必殺ゲージ設定
    // 1：通常攻撃 2：防御状態で被ダメ 3：非防御状態で被ダメ 4：経過ターン 5：アップ用スキル
    public SpecialMoveGuageSetting sm_NormalAttack;
    public SpecialMoveGuageSetting sm_Guard;
    public SpecialMoveGuageSetting sm_Damage;
    public SpecialMoveGuageSetting sm_Turn;
    public SpecialMoveGuageSetting sm_Skill;

    [SerializeField] HP_SpecialTecnique hp_st;

    void Start()
    {
        
    }

    void Update()
    {
        
    }

    /// <summary>
    /// ステータス初期化
    /// </summary>
    public override void Initialize()
    {
        base.Initialize();

        specialMoveGuageAmount = 0;
        mpGuage.Initialize(MP);
    }

    public override void Move()
    {
        Debug.Log("プレイヤーの行動");
    }

    public override int NormalAttack()
    {
        // 会心倍率
        float critical = CriticalLottery() == true ? power_Critical : 1.0f;

        // ダメージ量 = 攻撃力 * 通常攻撃倍率 * 攻撃力倍率 * 会心倍率
        int damage = (int)(ATK * power_NormalAttack * powerAtk * critical);

        UpSpecialMoveGuage(sm_NormalAttack.guageUpAmount);

        return damage;
    }

    /// <summary>
    /// ガード
    /// </summary>
    public void Guard()
    {
        // Todo 1ターン経過でガード解除（防御力倍率を1に戻す）

        // 防御力倍率上昇
        powerDef += power_Guard;

        isGuard = true;
    }

    /// <summary>
    /// 必殺技
    /// </summary>
    public void SpecialMove()
    {
        specialMoveGuageAmount = 0;
    }

    /// <summary>
    /// ダメージ
    /// </summary>
    /// <param name="_amount">ダメージ量</param>
    public override int Damage(int _amount)
    {
        // Todo 回避判定

        int damage = base.Damage(_amount);

        // 必殺ゲージ回復
        if (isGuard) UpSpecialMoveGuage(sm_Guard.guageUpAmount);
        else UpSpecialMoveGuage(sm_Damage.guageUpAmount);

        // Todo ダメージ演出・モーション再生

        return damage;
    }

    /// <summary>
    /// ダメージ
    /// </summary>
    /// <param name="_amount">ダメージ量</param>
    /// <param name="_enemy">ダメージを与えた敵</param>
    public override int Damage(int _amount, Enemy _enemy)
    {
        int damage = base.Damage(_amount, _enemy);

        // 被ダメージに処理される特殊技能
        hp_st._RankB(damage, _enemy);

        return damage;
    }

    /// <summary>
    /// HP回復
    /// </summary>
    /// <param name="_amount">回復量</param>
    public override void HealHP(int _amount)
    {
        base.HealHP(_amount);

        // Todo 回復演出
    }

    /// <summary>
    /// MP使用
    /// </summary>
    /// <param name="_amount">使用量</param>
    /// <returns>発動不可ならfalseを返す</returns>
    public bool CostMP(int _amount)
    {
        // MPが足りなければ発動不可
        if (currentMp < _amount)
            return false;

        currentMp -= _amount;
        mpGuage.Sub(_amount); // ゲージ減少演出

        if (currentMp < 0) currentMp = 0;

        return true;
    }

    /// <summary>
    /// MP回復
    /// </summary>
    /// <param name="_amount">回復量</param>
    public void HealMP(int _amount)
    {
        currentMp += _amount;
        if (currentMp > MP) currentMp = MP;
        mpGuage.Add(_amount);

        // Todo 回復演出
    }

    /// <summary>
    /// 必殺技ゲージ上昇
    /// </summary>
    public void UpSpecialMoveGuage(int _amount)
    {
        specialMoveGuageAmount += _amount;

        if (specialMoveGuageAmount > specialMoveGuageMax)
            specialMoveGuageAmount = specialMoveGuageMax;

        // Todo 上昇演出
    }

    /// <summary>
    /// 死亡
    /// </summary>
    public override void Dead()
    {
        // Todo 敗北演出・モーション再生
    }
}
