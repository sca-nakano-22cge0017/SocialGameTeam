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
            NormalAttack();
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
    }

    public override void NormalAttack()
    {
        int damage = 500;
        player.Damage(damage, this);
    }

    /// <summary>
    /// ダメージ
    /// </summary>
    /// <param name="_amount">ダメージ量</param>
    /// <returns>防御力分減少させたダメージ量</returns>
    public override int Damage(int _amount)
    {
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
