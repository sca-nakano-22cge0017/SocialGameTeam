using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Enemy : MonoBehaviour
{
    public Image image; // イラスト

    [SerializeField] private MainGameGuage hpGuage;

    public int POSITION; // 敵の位置

    public int ATK; // 攻撃力
    public int MP; // 魔力
    public int HP; // 体力
    public int DEF; // 防御力
    public int AGI; // 速度
    public int DEX; // 器用

    // 計算用
    private int currentMp;
    private int currentAtk;
    private int currentHp;
    private int currentDef;
    private int currentDex;
    private int currentAgi;

    // アタックパターン
    public List<Master.EnemyAttackPattern> attackPattern = new();

    void Start()
    {
        
    }
    void Update()
    {
        
    }

    /// <summary>
    /// 初期状態に移行
    /// </summary>
    public void Initialize()
    {
        currentMp = MP;
        currentAtk = ATK;
        currentHp = HP;
        currentDef = DEF;
        currentDex = DEX;
        currentAgi = AGI;

        hpGuage.Initialize(HP);
    }

    public void Move()
    {
        // 攻撃方法を抽選
        Master.EnemyAttackPattern move = MoveLottely();

        // 行動
        if (move.attackId == 1) Attack();
    }

    void Attack()
    {

    }

    /// <summary>
    /// ダメージ
    /// </summary>
    /// <param name="_amount">ダメージ量</param>
    public void Damage(int _amount)
    {
        currentHp -= _amount;
        hpGuage.Sub(_amount); // ゲージ減少演出

        if (currentHp < 0)
        {
            currentHp = 0;
            // 死亡判定？
        }

        // Todo ダメージ演出・モーション再生
    }

    /// <summary>
    /// HP回復
    /// </summary>
    /// <param name="_amount">回復量</param>
    public void HealHP(int _amount)
    {
        currentHp += _amount;

        if (currentHp > HP) currentHp = HP;

        // 回復演出
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
        // 回復演出
    }

    /// <summary>
    /// 死亡
    /// </summary>
    public void Dead()
    {
        // 敗北演出・モーション再生
    }

    /// <summary>
    /// アタックパターン抽選
    /// </summary>
    Master.EnemyAttackPattern MoveLottely()
    {
        List<float> range = new();
        float t = 0;
        range.Add(0);
        for (int i = 0; i < attackPattern.Count; i++)
        {
            t += attackPattern[i].probability;
            range.Add(t);
        }

        int rnd = Random.Range(0, 100);
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
