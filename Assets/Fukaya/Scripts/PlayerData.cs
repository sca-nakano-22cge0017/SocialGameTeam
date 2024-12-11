using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Master;
using System;

/// <summary>
/// プレイヤーのステータスやコマンド管理 特殊技能・スキルは別スクリプト
/// </summary>
public class PlayerData : Character
{
    [SerializeField] private MainGameGuage mpGuage;
    [SerializeField] private MainGameGuage specialMoveGuage;
    [SerializeField] private Button[] commands;

    // 攻撃倍率
    public float power_NormalAttack;  // 通常攻撃
    public float power_Skill;         // スキル
    public float power_SpecialMove;   // 必殺技

    // ガード
    private bool isGuard;
    public float power_Guard = 0.2f; // ガード時防御倍率

    // 必殺技ゲージ
    public int specialMoveGuageAmount;
    public int specialMoveGuageMax; // 最大量

    // 必殺ゲージ設定
    // 1：通常攻撃 2：防御状態で被ダメ 3：非防御状態で被ダメ 4：経過ターン 5：アップ用スキル
    public SpecialMoveGuageSetting sm_NormalAttack;
    public SpecialMoveGuageSetting sm_Guard;
    public SpecialMoveGuageSetting sm_Damage;
    public SpecialMoveGuageSetting sm_Turn;
    public SpecialMoveGuageSetting sm_Skill;

    /// <summary>
    /// MP消費量倍率
    /// </summary>
    public float power_CostMp = 1;

    [SerializeField] HP_SpecialTecnique hp_st;
    [SerializeField] DEF_SpecialTecnique def_st;
    [SerializeField] ATK_SpecialTecnique atk_st;
    [SerializeField] MP_SpecialTecnique mp_st;
    [SerializeField] AGI_SpecialTecnique agi_st;
    [SerializeField] DEX_SpecialTecnique dex_st;

    /// <summary>
    /// 無敵状態かどうか
    /// </summary>
    public bool isInvincible = false;

    /// <summary>
    /// 必殺技解放済みかどうか
    /// </summary>
    public bool canSpecialMove = false;

    void Start()
    {
        soundController = FindObjectOfType<SoundController>();
        atk_st.GameStart();
    }

    /// <summary>
    /// ステータス初期化
    /// </summary>
    public override void Initialize()
    {
        base.Initialize();

        specialMoveGuageAmount = 0;
        mpGuage.Initialize(MP);

        SetCommandsButton(false);

        specialMoveGuage.Initialize(specialMoveGuageMax, 0);
    }

    public override void Move()
    {
        // ガードしてればガード解除
        if (isGuard)
        {
            AddBuff(StatusType.DEF, -power_Guard);
            isGuard = false;
        }

        if (currentHp <= 0)
        {
            MoveEnd();
            return;
        }

        Debug.Log("プレイヤーの行動");

        SetCommandsButton(true);
        hp_st.PlayerTurnStart();
    }

    public override void MoveEnd()
    {
        Debug.Log("プレイヤーの行動終了");

        SetCommandsButton(false);

        mainGameSystem.ActionEnd();
    }
    IEnumerator EndWait()
    {
        yield return new WaitForSeconds(0.1f);
        MoveEnd();
    }

    public override void TurnEnd()
    {
        // ターン経過によるゲージ上昇
        UpSpecialMoveGuage(sm_Turn.guageUpAmount);
    }

    public override void NormalAttack()
    {
        SetCommandsButton(false);

        AttackMotion(() =>
        { 
            // 会心抽選
            var cri = CriticalLottery();

            // ダメージ量 = 攻撃力 * 通常攻撃倍率 * 攻撃力倍率 * 会心倍率
            float damage = ATK * power_NormalAttack * powerAtk * critical;

            // ロックオンした敵にダメージ
            var target = mainGameSystem.Target;
            target.Damage(damage);
            if (cri) target.CriticalDamage();

            // 通常攻撃時に処理される特殊技能
            atk_st.RankA(target);        // ガードブレイカー
            mp_st.RankB();               // ドレイン
            dex_st.RankA(target);        // 小手先のテクニック
            if (agi_st.RankA()) NormalAttack(); // 再行動

            UpSpecialMoveGuage(sm_NormalAttack.guageUpAmount);
        });
    }

    /// <summary>
    /// ガード
    /// </summary>
    public void Guard()
    {
        SetCommandsButton(false);

        // 防御力倍率上昇
        AddBuff(StatusType.DEF, power_Guard);

        isGuard = true;

        // 防御時に処理される特殊技能
        def_st.RankC();
        def_st.RankB();

        StartCoroutine(EndWait());
    }

    /// <summary>
    /// 必殺技
    /// </summary>
    public void SpecialMove()
    {
        if (specialMoveGuageAmount < specialMoveGuageMax) return;
        if (!canSpecialMove) return;

        SetCommandsButton(false);
        specialMoveGuage.SetCurrent(0);

        AttackMotion(() => 
        {
            atk_st.RankS(); // 全身全霊
            specialMoveGuageAmount = 0;

            // 会心抽選
            var cri = CriticalLottery();

            // ダメージ量 = 攻撃力 * 必殺技倍率 * 攻撃力倍率 * 会心倍率
            float damage = ATK * power_SpecialMove * powerAtk * critical;

            // ロックオンした敵にダメージ
            var target = mainGameSystem.Target;
            target.Damage(damage);
            if (cri) target.CriticalDamage();
        });
    }

    public int Damage(float _damageAmount, Enemy _enemy)
    {
        if (agi_st.RankS())
        {
            UpSpecialMoveGuage(sm_Guard.guageUpAmount);
            return 0;     // ステップ
        }

        // 被ダメ - 防御力 を実際の被ダメージにする
        int damage = (int)Mathf.Ceil(_damageAmount - (DEF * powerDef));
        damage = damage < 0 ? 0 : damage; // 0未満なら0にする

        // 被ダメージ時に処理される特殊技能
        if (isGuard) def_st.RankS(damage, _enemy); // 攻防一体
        mp_st.RankS(_enemy);

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
        soundController.PlayDamageSE();

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

        // 回復量表示
        StartCoroutine(DispText(healText, _amount.ToString()));
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

    public override void AddBuff(StatusType _type, float _amount)
    {
        base.AddBuff(_type, _amount);
        if (_amount > 0) soundController.PlayBuffSE();
    }

    public override void AddDebuff(StatusType _type, float _amount)
    {
        base.AddDebuff(_type, _amount);
        if (_amount > 0) soundController.PlayDebuffSE();
    }

    /// <summary>
    /// 必殺技ゲージ上昇
    /// </summary>
    public void UpSpecialMoveGuage(int _amount)
    {
        specialMoveGuageAmount += _amount;

        if (specialMoveGuageAmount > specialMoveGuageMax)
            specialMoveGuageAmount = specialMoveGuageMax;

        specialMoveGuage.Add(_amount);
    }

    /// <summary>
    /// 必殺技ゲージを最大まで上昇
    /// </summary>
    public void UpSpecialMoveGuage()
    {
        int amount = specialMoveGuageMax - specialMoveGuageAmount;

        specialMoveGuageAmount = specialMoveGuageMax;

        specialMoveGuage.Add(amount);
    }

    /// <summary>
    /// 死亡
    /// </summary>
    public override void Dead()
    {
        mainGameSystem.Judge();

        // Todo 敗北演出・モーション再生

        StartCoroutine(DirectionCompleteWait(() =>
        {
            meshRenderer.enabled = false;
        }));
    }

    IEnumerator DirectionCompleteWait(Action _action)
    {
        yield return new WaitUntil(() => hpGuage.isDirectionCompleted);

        yield return new WaitForSeconds(0.5f);

        _action?.Invoke();
    }

    public void AttackMotion(Action _action)
    {
        SetCommandsButton(false);

        spineAnim.callBack = () => 
        {
            soundController.PlayAttackSE(GameManager.SelectChara);

            _action?.Invoke();
            StartCoroutine(EndWait());
        };
        spineAnim.PlayAttackMotion();
    }

    public void BuffMotion(Action _action)
    {
        SetCommandsButton(false);

        spineAnim.callBack = () => { _action?.Invoke(); StartCoroutine(EndWait()); };
        spineAnim.PlaybuffMotion();
    }

    public void WinMotion()
    {
        SetCommandsButton(false);

        motion.SetTrigger("win");
    }

    /// <summary>
    /// コマンド　押せるかどうかを設定
    /// </summary>
    /// <param name="_canPut">falseのとき押せない</param>
    void SetCommandsButton(bool _canPut)
    {
        if (_canPut)
        {
            for (int i = 0; i < commands.Length; i++)
            {
                commands[i].interactable = true;
                commands[i].image.color = new Color(1.0f, 1.0f, 1.0f, 1.0f);
            }
        }
        else
        {
            for (int i = 0; i < commands.Length; i++)
            {
                commands[i].interactable = false;
                commands[i].image.color = new Color(0.8f, 0.8f, 0.8f, 1.0f);
            }
        }

        if (specialMoveGuageAmount < specialMoveGuageMax || !canSpecialMove)
        {
            commands[0].interactable = false;
            commands[0].image.color = new Color(0.8f, 0.8f, 0.8f, 1.0f);
        }
    }
}
