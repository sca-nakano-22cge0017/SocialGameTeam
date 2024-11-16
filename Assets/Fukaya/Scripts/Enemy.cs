using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Master;

public class Enemy : Character
{
    private DropController dropController;

    public int POSITION; // 敵の位置

    // アタックパターン
    public List<EnemyAttackPattern> attackPattern = new();

    int elapsedTurn_Debuff1 = 0;
    bool isActive_Debuff1 = false;

    int elapsedTurn_Debuff2 = 0;
    bool isActive_Debuff2 = false;

    int elapsedTurn_Buff = 0;
    bool isActive_Buff = false;

    int turn_AbsolutelyKill = 0;          // 確殺攻撃までのターン
    int value_AbsolutelyKill = 999999999;

    [SerializeField] private PlayerData player;
    [SerializeField] protected GameObject hpGuage_Obj;
    [SerializeField, Header("ドロップ")] private Text dropText;

    /// <summary>
    /// 防御無視状態
    /// </summary>
    public bool isIgnoreDeffence = false;

    void Start()
    {
        dropController = FindObjectOfType<DropController>();
    }

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.A))
        {
            //NormalAttack();
            AbsolutelyKill();
        }
    }

    /// <summary>
    /// 初期状態に移行
    /// </summary>
    public override void Initialize()
    {
        base.Initialize();

        dropText.enabled = false;
        damageText.enabled = false;
    }

    public override void Move()
    {
        Debug.Log("敵" + POSITION + "の行動");

        // 攻撃方法を抽選
        Master.EnemyAttackPattern move = MoveLottery();
        Debug.Log(move.criticalProbability);
    }

    public override void TurnEnd()
    {
        elapsedTurn_Debuff1++;
        elapsedTurn_Debuff2++;
        elapsedTurn_Buff++;
    }

    public override void NormalAttack()
    {
        float damage = ATK * powerAtk;
        player.Damage(damage, this);
    }

    /// <summary>
    /// デバフ１　プレイヤーにTターンの防御力V％ダウン付与
    /// </summary>
    void Debuff1()
    {
        elapsedTurn_Debuff1 = 1;
        isActive_Debuff1 = true;


    }

    void Cancel_Debuff1()
    {
        if (!isActive_Debuff1) return;

        if (elapsedTurn_Debuff1 >= 2)
        {
            
        }
    }

    /// <summary>
    /// デバフ２　プレイヤーにTターンの攻撃力V％ダウン付与
    /// </summary>
    void Debuff2()
    {
        elapsedTurn_Debuff2 = 1;
        isActive_Debuff2 = true;
    }

    /// <summary>
    /// バフ　自身にTターンの攻撃力V％アップ付与
    /// </summary>
    void Buff()
    {
        elapsedTurn_Buff = 1;
        isActive_Buff = true;
    }

    /// <summary>
    /// ２連撃　攻撃力 * Vの攻撃を二回行う
    /// </summary>
    void DoubleAttack()
    {

    }

    /// <summary>
    /// 確殺　プレイヤーに999999999の固定ダメージ
    /// </summary>
    void AbsolutelyKill()
    {
        player.Damage((int)value_AbsolutelyKill);
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
        Debug.Log("敵" + POSITION + "を倒した");

        // Todo モーション再生

        // イラスト・HPゲージを非表示にする
        image.enabled = false;
        hpGuage_Obj.SetActive(false);

        StartCoroutine(DropDirection());
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

        return null;
    }
}
