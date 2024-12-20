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
    public Image image;

    // アタックパターン
    public List<EnemyAttackPattern> attackPattern = new();

    float critical_NormalAttack = 1;

    int turn_Debuff1 = 0;          // 継続ターン
    int elapsedTurn_Debuff1 = 0;   // 発動からの経過ターン
    float value_Debuff1 = 0;       // 効果量
    bool isActive_Debuff1 = false; // 発動中かどうか

    int turn_Debuff2 = 0;
    int elapsedTurn_Debuff2 = 0;
    float value_Debuff2 = 0;
    bool isActive_Debuff2 = false;

    int turn_Buff = 0;
    int elapsedTurn_Buff = 0;
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
    }

    public override void Move()
    {
        if (currentHp <= 0)
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
        elapsedTurn_Debuff1++;
        elapsedTurn_Debuff2++;
        elapsedTurn_Buff++;
        elapsedTurn_AbsolutelyKill++;

        Cancel_Debuff1();
        Cancel_Debuff2();
        Cancel_Buff();
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
        elapsedTurn_Debuff1 = 1;
        isActive_Debuff1 = true;

        float amount = value_Debuff1 / 100.0f;
        
        AttackMotion(() => 
        {
            player.AddDebuff(StatusType.DEF, amount);

            Debug.Log("敵 " + POSITION + " デバフ１発動 プレイヤー 防御力" + (amount * 100) + "%ダウン付与");
        });
    }
    void Cancel_Debuff1()
    {
        if (!isActive_Debuff1) return;

        if (elapsedTurn_Debuff1 > turn_Debuff1)
        {
            float amount = value_Debuff1 / 100.0f;
            player.AddDebuff(StatusType.DEF, -amount);

            elapsedTurn_Debuff1 = 0;
            isActive_Debuff1 = false;

            Debug.Log("敵 " + POSITION + " デバフ１解除");
        }
    }

    /// <summary>
    /// デバフ２　プレイヤーにTターンの攻撃力V％ダウン付与
    /// </summary>
    void Debuff2()
    {
        elapsedTurn_Debuff2 = 1;
        isActive_Debuff2 = true;

        float amount = value_Debuff2 / 100.0f;
        
        AttackMotion(() => 
        {
            player.AddDebuff(StatusType.ATK, amount);

            Debug.Log("敵 " + POSITION + " デバフ２発動 プレイヤー 攻撃力" + (amount * 100) + "%ダウン付与");
        });
    }
    void Cancel_Debuff2()
    {
        if (!isActive_Debuff2) return;

        if (elapsedTurn_Debuff2 > turn_Debuff2)
        {
            float amount = value_Debuff2 / 100.0f;
            player.AddDebuff(StatusType.ATK, -amount);

            elapsedTurn_Debuff2 = 0;
            isActive_Debuff2 = false;

            Debug.Log("敵 " + POSITION + " デバフ２解除");
        }
    }

    /// <summary>
    /// バフ　自身にTターンの攻撃力V％アップ付与
    /// </summary>
    void Buff()
    {
        elapsedTurn_Buff = 1;
        isActive_Buff = true;

        float amount = value_Buff / 100.0f;
        
        AttackMotion(() => 
        {
            AddBuff(StatusType.ATK, amount);

            Debug.Log("敵 " + POSITION + " バフ発動 攻撃力" + (amount * 100) + "%アップ");
        });
    }
    void Cancel_Buff()
    {
        if (!isActive_Buff) return;

        if (elapsedTurn_Buff > turn_Buff)
        {
            float amount = (float)value_Buff / 100.0f;
            AddBuff(StatusType.ATK, -amount);

            elapsedTurn_Buff = 0;
            isActive_Buff = false;

            Debug.Log("敵 " + POSITION + " バフ解除");
        }
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
        if (currentHp < 0) return;

        Debug.Log("敵" + POSITION + "を倒した");

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
        // ドロップ抽選
        int drop = dropController.DropLottery();

        yield return new WaitForSeconds(1.0f);

        // ドロップ量表示
        string str = "+" + drop.ToString() + "Pt";
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
        spineAnim.callBack = () => { _action?.Invoke(); StartCoroutine(EndWait()); };
        spineAnim.PlayAttackMotion();
    }

    [SerializeField] private float deadMotionTime = 1.0f;
    [SerializeField] private float deadMotionDist = 100;

    /// <summary>
    /// 死亡演出再生
    /// </summary>
    /// <param name="_action"></param>
    /// <returns></returns>
    IEnumerator DeadMotion(System.Action _action)
    {
        yield return new WaitForSeconds(0.5f);

        float time = 0;
        float alpha = 1;

        while (time < deadMotionTime)
        {
            Vector3 pos = image.rectTransform.localPosition;
            pos.y -= (deadMotionDist / deadMotionTime * Time.deltaTime);
            image.rectTransform.localPosition = pos;

            alpha -= 1 / deadMotionTime * Time.deltaTime;
            image.color = new Color(1,1,1,alpha);

            time += Time.deltaTime;

            yield return null;
        }

        _action?.Invoke();
    }
}
