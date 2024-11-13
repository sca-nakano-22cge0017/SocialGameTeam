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
    [SerializeField] DEF_SpecialTecnique def_st;
    [SerializeField] ATK_SpecialTecnique atk_st;
    [SerializeField] MP_SpecialTecnique mp_st;
    [SerializeField] AGI_SpecialTecnique agi_st;
    [SerializeField] DEX_SpecialTecnique dex_st;

    [SerializeField] Enemy enemy_forDebug;

    /// <summary>
    /// 無敵状態かどうか
    /// </summary>
    public bool isInvincible = false;

    void Start()
    {
        
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.S))
        {
            //enemies[0].Damage(100);

            Move();

            atk_st.GameStart();
            hp_st.PlayerTurnStart();
        }

        if (Input.GetKeyDown(KeyCode.E))
        {
            MoveEnd();

            hp_st.TurnEnd();
            def_st.TurnEnd();
            atk_st.TurnEnd();
        }
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

    public override void MoveEnd()
    {
        Debug.Log("プレイヤーの行動終了");

        isGuard = false;
    }

    public override void NormalAttack()
    {
        // 会心倍率
        float critical = CriticalLottery() == true ? power_Critical : 1.0f;

        // ダメージ量 = 攻撃力 * 通常攻撃倍率 * 攻撃力倍率 * 会心倍率
        int damage = (int)(ATK * power_NormalAttack * powerAtk * critical);

        // Todo ロックオンした敵にダメージ
        //enemy_forDebug.Damage(damage);

        // 通常攻撃時に処理される特殊技能
        atk_st.RankA(enemy_forDebug);

        UpSpecialMoveGuage(sm_NormalAttack.guageUpAmount);
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

        // 防御時に処理される特殊技能
        def_st.RankC();
        def_st.RankB();
    }

    /// <summary>
    /// 必殺技
    /// </summary>
    public void SpecialMove()
    {
        specialMoveGuageAmount = 0;
    }

    public int Damage(int _damageAmount, Enemy _enemy)
    {
        // Todo 回避判定

        // 被ダメ - 防御力 を実際の被ダメージにする
        int damage = (_damageAmount - (int)(DEF * powerDef));
        damage = damage < 0 ? 0 : damage; // 0未満なら0にする

        // 被ダメージ時に処理される特殊技能
        if (isGuard) def_st.RankS(damage, _enemy); // 攻防一体

        def_st._RankA();                   // 無敵
        if (isInvincible) damage = 0;      // 無敵状態なら被ダメ0

        damage -= def_st._RankSS(damage); // 守護神の権能
        hp_st._RankB(damage, _enemy);     // 痛み分け

        // HP減少
        currentHp -= damage;
        if (currentHp < 0)
        {
            currentHp = 0;
            Dead();
        }

        atk_st.RankB();                   // 背水の陣

        // HPゲージ減少演出
        hpGuage.Sub(damage);

        // ダメージ表示
        StartCoroutine(DispText(damageText, damage.ToString()));

        // Todo ダメージ演出・モーション再生

        // 必殺ゲージ回復
        if (isGuard) UpSpecialMoveGuage(sm_Guard.guageUpAmount);
        else UpSpecialMoveGuage(sm_Damage.guageUpAmount);

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
        image.enabled = false;
    }
}
