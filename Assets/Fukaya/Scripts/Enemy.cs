using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Master;

public class Enemy : Character
{
    private DropController dropController;

    public string enemyId;
    public int POSITION; // 敵の位置
    private int drop;

    // アタックパターン
    public List<EnemyAttackPattern> attackPattern = new();

    float critical_NormalAttack = 1;

    int turn_Debuff1 = 0;          // 継続ターン
    float value_Debuff1 = 0;       // 効果量
    bool isActive_Debuff1 = false; // 発動中かどうか

    int turn_Debuff2 = 0;
    float value_Debuff2 = 0;
    bool isActive_Debuff2 = false;

    int turn_Buff = 0;
    float value_Buff = 0;
    bool isActive_Buff = false;

    float value_DoubleAttack = 0;
    float critical_DoubleAttack = 1;

    bool hasAbsolutelyKill = false;
    int turn_AbsolutelyKill = 1;          // 確殺攻撃までのターン
    int elapsedTurn_AbsolutelyKill = 1;
    int value_AbsolutelyKill = 999999999;

    [SerializeField] private PlayerData player;
    [SerializeField] public GameObject hpGuage_Obj;
    [SerializeField] private Button targetChangeButton;
    [SerializeField, Header("ドロップ")] private Text dropText;

    /// <summary>
    /// 防御無視状態
    /// </summary>
    public bool isIgnoreDeffence = false;

    void Start()
    {
        soundController = FindObjectOfType<SoundController>();
        dropController = FindObjectOfType<DropController>();
    }

    /// <summary>
    /// 初期状態に移行
    /// </summary>
    public override void Initialize()
    {
        base.Initialize();

        for (int i = 0; i < attackPattern.Count; i++)
        {
            switch(attackPattern[i].attackId)
            {
                case 1:
                    critical_NormalAttack = attackPattern[i].criticalProbability;
                    break;
                case 2:
                    turn_Debuff1 = attackPattern[i].turn;
                    value_Debuff1 = attackPattern[i].value;
                    break;
                case 3:
                    turn_Debuff2 = attackPattern[i].turn;
                    value_Debuff2= attackPattern[i].value;
                    break;
                case 4:
                    value_DoubleAttack = attackPattern[i].value;
                    critical_DoubleAttack = attackPattern[i].criticalProbability;
                    break;
                case 5:
                    turn_Buff = attackPattern[i].turn;
                    value_Buff = attackPattern[i].value;
                    break;
                case 6:
                    turn_AbsolutelyKill = attackPattern[i].turn;
                    hasAbsolutelyKill = true;
                    break;
            }
        }

        defaultAnimSpd_Wait = motion.GetFloat("taikiSpeed");
        defaultAnimSpd_Attack = motion.GetFloat("attackSpeed");
    }

    public override void Move()
    {
        if (currentHp <= 0 || player.currentHp <= 0)
        {
            MoveEnd();
            return;
        }

        Debug.Log("敵" + POSITION + "の行動");

        // 確殺ターンなら確殺攻撃をする
        if (elapsedTurn_AbsolutelyKill >= turn_AbsolutelyKill && hasAbsolutelyKill)
        {
            AbsolutelyKill();
            return;
        }

        // 攻撃方法を抽選
        Master.EnemyAttackPattern move = MoveLottery();
        if (move == null) return;

        switch(move.attackId)
        {
            case 1:
                NormalAttack();
                break;
            case 2:
                Debuff1();
                break;
            case 3:
                Debuff2();
                break;
            case 4:
                DoubleAttack();
                break;
            case 5:
                Buff();
                break;
            default:
                break;
        }
    }

    public override void MoveEnd()
    {
        Debug.Log("敵" + POSITION + "の行動終了");

        mainGameSystem.ActionEnd();
    }

    IEnumerator EndWait()
    {
        yield return new WaitForSeconds(0.5f);
        MoveEnd();
    }

    public override void TurnEnd()
    {
        base.TurnEnd();
        elapsedTurn_AbsolutelyKill++;
    }

    public override void NormalAttack()
    {
        var cri = CriticalLottery(critical_NormalAttack);

        float damage = ATK * powerAtk * critical;

        AttackMotion(() => 
        {
            mainDirection.DamageImpulse();

            player.Damage(damage, this);
            if (cri) player.CriticalDamage();

            Debug.Log("敵 " + POSITION + " 通常攻撃" + damage);
        });
    }

    /// <summary>
    /// デバフ１　プレイヤーにTターンの防御力V％ダウン付与
    /// </summary>
    void Debuff1()
    {
        isActive_Debuff1 = true;

        float amount = value_Debuff1 / 100.0f;

        player.AddState(false, 101, turn_Debuff1, value_Debuff1, () => { Cancel_Debuff1(); }, false);
        
        AttackMotion(() => 
        {
            player.AddDebuff(StatusType.DEF, amount, true);

            Debug.Log("敵 " + POSITION + " デバフ１発動 プレイヤー 防御力" + (amount * 100) + "%ダウン付与");
        });
    }
    void Cancel_Debuff1()
    {
        if (!isActive_Debuff1) return;

        float amount = value_Debuff1 / 100.0f;
        player.AddDebuff(StatusType.DEF, -amount, false);

        isActive_Debuff1 = false;

        Debug.Log("敵 " + POSITION + " デバフ１解除");
    }

    /// <summary>
    /// デバフ２　プレイヤーにTターンの攻撃力V％ダウン付与
    /// </summary>
    void Debuff2()
    {
        isActive_Debuff2 = true;

        float amount = value_Debuff2 / 100.0f;

        player.AddState(false, 102, turn_Debuff2, value_Debuff2, () => { Cancel_Debuff2(); }, false);

        AttackMotion(() => 
        {
            player.AddDebuff(StatusType.ATK, amount, true);

            Debug.Log("敵 " + POSITION + " デバフ２発動 プレイヤー 攻撃力" + (amount * 100) + "%ダウン付与");
        });
    }
    void Cancel_Debuff2()
    {
        if (!isActive_Debuff2) return;

        float amount = value_Debuff2 / 100.0f;
        player.AddDebuff(StatusType.ATK, -amount, false);

        isActive_Debuff2 = false;

        Debug.Log("敵 " + POSITION + " デバフ２解除");
    }

    /// <summary>
    /// バフ　自身にTターンの攻撃力V％アップ付与
    /// </summary>
    void Buff()
    {
        isActive_Buff = true;

        float amount = value_Buff / 100.0f;

        AddState(true, 103, turn_Buff, value_Buff, () => { Cancel_Buff(); }, false);

        AttackMotion(() => 
        {
            AddBuff(StatusType.ATK, amount, true);

            Debug.Log("敵 " + POSITION + " バフ発動 攻撃力" + (amount * 100) + "%アップ");
        });
    }
    void Cancel_Buff()
    {
        if (!isActive_Buff) return;

        float amount = (float)value_Buff / 100.0f;
        AddBuff(StatusType.ATK, -amount, false);

        isActive_Buff = false;

        Debug.Log("敵 " + POSITION + " バフ解除");
    }

    /// <summary>
    /// ２連撃　攻撃力 * Vの攻撃を二回行う
    /// </summary>
    void DoubleAttack()
    {
        Debug.Log("敵 " + POSITION + " ダブルアタック発動");

        var cri = CriticalLottery(critical_DoubleAttack);

        float damage = ATK * powerAtk * (value_DoubleAttack / 100.0f) * critical;

        AttackMotion(() =>
        {
            mainDirection.DamageImpulse();
            player.Damage(damage);
            if (cri) player.CriticalDamage();

            StartCoroutine(SecondAttack(damage));
        });
    }

    IEnumerator SecondAttack(float damage)
    {
        yield return new WaitForSeconds(0.5f);
        player.Damage(damage);
    }

    /// <summary>
    /// 確殺　プレイヤーに999999999の固定ダメージ
    /// </summary>
    void AbsolutelyKill()
    {
        AttackMotion(() => 
        {
            Debug.Log("敵 " + POSITION + " 確殺攻撃発動");
            player.Damage((int)value_AbsolutelyKill);

            mainDirection.AbsolutelyImpulse();
        });
    }

    /// <summary>
    /// ダメージ
    /// </summary>
    /// <param name="_amount">ダメージ量</param>
    /// <returns>防御力分減少させたダメージ量</returns>
    public override int Damage(float _amount)
    {
        if (currentHp <= 0) return 0;

        int damage = 0;

        if (isIgnoreDeffence) damage = base.Damage(_amount, true);
        else damage = base.Damage(_amount);

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
    /// 死亡
    /// </summary>
    public override void Dead()
    {
        if (currentHp > 0) return;

        Debug.Log("敵" + POSITION + "を倒した");

        // ドロップ抽選
        drop = dropController.DropLottery();
        mainGameSystem.Judge();

        // イラスト・HPゲージを非表示にする
        StartCoroutine(HPGuageDirectionCompleteWait(() =>
        {
            // 死亡演出
            StartCoroutine(DeadMotion(() => 
            {
                hpGuage_Obj.SetActive(false);
                targetChangeButton.interactable = false;

                if (GameManager.SelectArea == 1)
                {
                    StartCoroutine(DropDirection()); 
                    
                    // レア敵ドロップ
                    if (enemyId.Substring(0, 1) == "R")
                    {
                        dropController.RareDropLottery();
                    }
                }
                if (GameManager.SelectArea == 2)
                {
                    dropController.BossDrop();
                }
            }));
        }));
    }

    /// <summary>
    /// ドロップ抽選・演出
    /// </summary>
    /// <returns></returns>
    IEnumerator DropDirection()
    {
        yield return new WaitForSeconds(1.0f);

        // ドロップ量表示
        string str = "+" + drop.ToString() + "p";
        StartCoroutine(DispText(dropText, str));
    }

    /// <summary>
    /// アタックパターン抽選
    /// </summary>
    EnemyAttackPattern MoveLottery()
    {
        List<float> range = new();
        float t = 0;
        range.Add(0);
        for (int i = 0; i < attackPattern.Count; i++)
        {
            t += attackPattern[i].probability;
            range.Add(t);
        }

        int rnd = UnityEngine.Random.Range(0, 100);
        for (int i = 0; i < range.Count - 1; i++)
        {
            if (range[i] <= rnd && rnd < range[i + 1])
            {
                return attackPattern[i];
            }
        }

        range.Clear();
        return attackPattern[0];
    }

    public void TargetChange()
    {
        mainGameSystem.TargetChange(this);
    }

    void AttackMotion(System.Action _action)
    {
        spineAnim.callBack = () => 
        { 
            _action?.Invoke(); 
            StartCoroutine(EndWait()); 
        };
        spineAnim.PlayAttackMotion();
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
            case 13: // ガードブレイカー
                s.isBuff = false;
                atk_st.RankA_Restart(this);
                s.werasOffAction = atk_st.Cancel_RankA;
                break;
            case 19: // 呪い
                s.isBuff = false;
                mp_st.RankS_Restart(this);
                s.werasOffAction = mp_st.Cancel_RankS;
                s.lastingEffects = mp_st._RankS;
                break;
            case 22: // スロウ
                s.isBuff = false;
                agi_st.RankB_Restart(this);
                s.werasOffAction = agi_st.Cancel_RankB;
                break;
            case 26: // ガードクラッシュ
                s.isBuff = false;
                dex_st.RankC_Restart(this);
                s.werasOffAction = dex_st.Cancel_RankC;
                break;
            case 103: // 攻撃力アップ
                s.isBuff = true;
                AddBuff(StatusType.ATK, _value, false);
                s.werasOffAction = Cancel_Buff;
                break;
        }

        state.Add(s);
    }
}
