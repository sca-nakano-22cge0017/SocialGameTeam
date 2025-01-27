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

    [SerializeField] private Image specialMoveForground;

    // 攻撃倍率
    [HideInInspector] public float power_NormalAttack;  // 通常攻撃
    [HideInInspector] public float power_Skill;         // スキル
    [HideInInspector] public float power_SpecialMove;   // 必殺技

    // ガード
    private bool isGuard;
    [HideInInspector] public float power_Guard = 0.2f; // ガード時防御倍率

    // 必殺技ゲージ
    [HideInInspector] public int specialMoveGuageAmount;
    [HideInInspector] public int specialMoveGuageMax; // 最大量

    // 必殺ゲージ設定
    // 1：通常攻撃 2：防御状態で被ダメ 3：非防御状態で被ダメ 4：経過ターン 5：アップ用スキル
    [HideInInspector] public SpecialMoveGuageSetting sm_NormalAttack;
    [HideInInspector] public SpecialMoveGuageSetting sm_Guard;
    [HideInInspector] public SpecialMoveGuageSetting sm_Damage;
    [HideInInspector] public SpecialMoveGuageSetting sm_Turn;
    [HideInInspector] public SpecialMoveGuageSetting sm_Skill;

    /// <summary>
    /// MP消費量倍率
    /// </summary>
    [HideInInspector] public float power_CostMp = 1;

    // エフェクト
    [SerializeField] Animator sisterAttack;
    [SerializeField] Animator swordAttack;
    
    // 再行動するか
    private bool isRemove = false;

    /// <summary>
    /// 無敵状態かどうか
    /// </summary>
    public bool isInvincible = false;

    /// <summary>
    /// 必殺技解放済みかどうか
    /// </summary>
    public bool canSpecialMove = false;

    /// <summary>
    /// コマンド入力待ちか
    /// </summary>
    public bool isInputWaiting = false;

    void Start()
    {
        soundController = FindObjectOfType<SoundController>();
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

    public override void RestartInitialize()
    {
        base.RestartInitialize();

        mpGuage.Initialize(MP, currentMp);
        specialMoveGuage.Initialize(specialMoveGuageMax, specialMoveGuageAmount);

        SetCommandsButton(false);
    }

    public override void Move()
    {
        // ガードしてればガード解除
        if (isGuard)
        {
            AddBuff(StatusType.DEF, -power_Guard, false);
            isGuard = false;
        }

        if (currentHp <= 0)
        {
            MoveEnd();
            return;
        }

        Debug.Log("プレイヤーの行動");

        if (mainGameSystem.IsAutoMode)
        {
            SetCommandsButton(false);
            NormalAttack();
        }
        else
        {
            SetCommandsButton(true);
        }

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
        
        if (!isRemove)
        {
            MoveEnd();
        }
        else
        {
            isRemove = false;
            NormalAttack();
        }
    }

    public override void TurnEnd()
    {
        base.TurnEnd();

        // ターン経過によるゲージ上昇
        UpSpecialMoveGuage(sm_Turn.guageUpAmount);
    }

    // 基本動作
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

            Debug.Log($"プレイヤーの通常攻撃 ダメージ:{damage} 攻撃力:{ATK} 通常攻撃倍率:{power_NormalAttack} バフ:{powerAtk} 会心倍率:{critical}");

            // 再行動
            if (agi_st.RankA())
            {
                if (target.currentHp > 0) isRemove = true;
            }

            if (!isRemove)
            {
                // 通常攻撃時に処理される特殊技能
                atk_st.RankA(target);        // ガードブレイカー
                mp_st.RankB();               // ドレイン
                dex_st.RankA(target);        // 小手先のテクニック

                UpSpecialMoveGuage(sm_NormalAttack.guageUpAmount);
            }

            PlayNormalAttackEffect();
        });
    }

    /// <summary>
    /// ガード
    /// </summary>
    public void Guard()
    {
        SetCommandsButton(false);

        // 防御力倍率上昇
        AddBuff(StatusType.DEF, power_Guard, true);

        var amount = power_Guard * 100.0f;
        AddState(true, 201, 1, amount, null, false);

        isGuard = true;

        // 防御時に処理される特殊技能
        def_st.RankC();
        def_st.RankB();

        StartCoroutine(EndWait());
    }

    /// <summary>
    /// スキル発動
    /// </summary>
    public void SkillAct()
    {
        SetCommandsButton(false);
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

        StartCoroutine(mainDirection.CutIn(() =>
        {
            // 全身全霊
            atk_st.RankS(() => 
            {
                AttackMotion(() =>
                {
                    specialMoveGuageAmount = 0;

                    // 会心抽選
                    var cri = CriticalLottery();

                    // ダメージ量 = 攻撃力 * 必殺技倍率 * 攻撃力倍率 * 会心倍率
                    float damage = ATK * power_SpecialMove * powerAtk * critical;

                    // ロックオンした敵にダメージ
                    var target = mainGameSystem.Target;
                    target.Damage(damage);
                    if (cri) target.CriticalDamage();

                    PlayNormalAttackEffect();
                });
            });
        }));
    }

    // 基本処理
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
        if (currentHp <= 0)
        {
            currentHp = 0;
            Dead();
        }

        atk_st.RankB(); // 背水の陣
        hp_st.RankS(); // 不倒の構え

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

        atk_st.RankB(); // 背水の陣
        hp_st.RankS(); // 不倒の構え
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

        StartCoroutine(DirectionCompleteWait(() =>
        {
            // 死亡演出
            StartCoroutine(DeadMotion(() =>
            {
                meshRenderer.enabled = false;
            }));
        }));
    }

    IEnumerator DirectionCompleteWait(Action _action)
    {
        yield return new WaitUntil(() => hpGuage.isDirectionCompleted);

        yield return new WaitForSeconds(0.5f);

        _action?.Invoke();
    }

    // モーション
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
    /// 通常攻撃のエフェクト表示
    /// </summary>
    void PlayNormalAttackEffect()
    {
        if (GameManager.SelectChara == 1)
        {
            swordAttack.SetTrigger("Play");
        }

        if (GameManager.SelectChara == 2)
        {
            var type = PlayerDataManager.player.GetSelectEvolutionType();
            switch (type)
            {
                case CombiType.ATK:
                    sisterAttack.SetTrigger("ATK");
                    break;
                case CombiType.DEF:
                    sisterAttack.SetTrigger("DEF");
                    break;
                case CombiType.TEC:
                    sisterAttack.SetTrigger("TEC");
                    break;
                case CombiType.NORMAL:
                    sisterAttack.SetTrigger("Normal");
                    break;
            }
        }
    }

    /// <summary>
    /// コマンド　押せるかどうかを設定
    /// </summary>
    /// <param name="_canPut">falseのとき押せない</param>
    void SetCommandsButton(bool _canPut)
    {
        // 押せる
        if (_canPut)
        {
            for (int i = 0; i < commands.Length; i++)
            {
                commands[i].interactable = true;

                if (i == 0)
                {
                    specialMoveForground.color = new Color(0.0f, 0.0f, 0.0f, 0.0f);
                }
                else commands[i].image.color = new Color(1.0f, 1.0f, 1.0f, 1.0f);
            }
        }

        // 押せない
        else
        {
            for (int i = 0; i < commands.Length; i++)
            {
                commands[i].interactable = false;

                if (i == 0)
                {
                    specialMoveForground.color = new Color(0.0f, 0.0f, 0.0f, 0.8f);
                }
                else commands[i].image.color = new Color(0.8f, 0.8f, 0.8f, 1.0f);
            }
        }

        if (specialMoveGuageAmount < specialMoveGuageMax || !canSpecialMove)
        {
            commands[0].interactable = false;
            specialMoveForground.color = new Color(0.0f, 0.0f, 0.0f, 0.8f);
        }

        isInputWaiting = _canPut;
    }

    public override void SetState(int _stateNumber, float _value, int _elapsedTurn, int _continuationTurn)
    {
        State s = new();
        s.stateId = _stateNumber;
        s.elapsedTurn = _elapsedTurn;
        s.continuationTurn = _continuationTurn;
        s.value = _value;
        s.lastingEffects = null;

        switch (_stateNumber)
        {
            case 2: // 痛み分け
                s.isBuff = true;
                hp_st.RankB_Restart();
                s.werasOffAction = hp_st.Cancel_RankB;
                break;
            case 8: // 無敵
                s.isBuff = true;
                def_st.RankA_Restart();
                s.werasOffAction = def_st.Cancel_RankA;
                break;
            case 10: // 守護神の権能
                s.isBuff = true;
                def_st.RankSS_Restart(s.value);
                s.werasOffAction = def_st.Cancel_RankSS;
                break;
            case 11: // ピアス
                s.isBuff = true;
                atk_st.RankC_Restart();
                s.werasOffAction = atk_st.Cancel_RankC;
                break;
            case 14: // 全身全霊
                s.isBuff = true;
                atk_st.RankS_Restart();
                s.werasOffAction = atk_st.Cancel_RankS;
                break;
            case 16: // オーラ
                s.isBuff = true;
                mp_st.RankC_Restart();
                s.werasOffAction = mp_st.Cancel_RankC;
                break;
            case 20: // 魔術師の結界
                s.isBuff = true;
                mp_st.RankSS_Restart();
                s.werasOffAction = mp_st.Cancel_RankSS;
                break;
            case 21: // 加速
                s.isBuff = true;
                agi_st.RankC_Restart();
                s.werasOffAction = agi_st.Cancel_RankC;
                break;
            case 29: // バースト
                s.isBuff = true;
                dex_st.RankS_Restart();
                s.werasOffAction = dex_st.Cancel_RankS;
                break;
            case 30: // 約束された勝利
                s.isBuff = true;
                dex_st.RankSS_Restart();
                s.werasOffAction = dex_st.Cancel_RankSS;
                break;
            case 201: // 防御
                s.isBuff = true;
                AddBuff(StatusType.DEF, _value, false);
                isGuard = true;
                break;
            case 101: // 敵 防御力ダウン
                s.isBuff = false;
                AddDebuff(StatusType.DEF, _value / 100.0f, false);
                s.werasOffAction = () => {
                    AddDebuff(StatusType.DEF, -_value / 100.0f, false);
                };
                break;
            case 102: // 敵 攻撃力ダウン
                s.isBuff = false;
                AddDebuff(StatusType.ATK, _value / 100.0f, false);
                s.werasOffAction = () => {
                    AddDebuff(StatusType.ATK, -_value / 100.0f, false);
                };
                break;
        }

        state.Add(s);
    }
}
