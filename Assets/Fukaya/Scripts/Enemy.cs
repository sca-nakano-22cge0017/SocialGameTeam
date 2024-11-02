using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Enemy : MonoBehaviour
{
    public Image image; // �C���X�g

    [SerializeField] private MainGameGuage hpGuage;

    public int POSITION; // �G�̈ʒu

    public int ATK; // �U����
    public int MP; // ����
    public int HP; // �̗�
    public int DEF; // �h���
    public int AGI; // ���x
    public int DEX; // ��p

    // �v�Z�p
    private int currentMp;
    private int currentAtk;
    private int currentHp;
    private int currentDef;
    private int currentDex;
    private int currentAgi;

    // �A�^�b�N�p�^�[��
    public List<Master.EnemyAttackPattern> attackPattern = new();

    void Start()
    {
        
    }
    void Update()
    {
        
    }

    /// <summary>
    /// ������ԂɈڍs
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
        // �U�����@�𒊑I
        Master.EnemyAttackPattern move = MoveLottely();

        // �s��
        if (move.attackId == 1) Attack();
    }

    void Attack()
    {

    }

    /// <summary>
    /// �_���[�W
    /// </summary>
    /// <param name="_amount">�_���[�W��</param>
    public void Damage(int _amount)
    {
        currentHp -= _amount;
        hpGuage.Sub(_amount); // �Q�[�W�������o

        if (currentHp < 0)
        {
            currentHp = 0;
            // ���S����H
        }

        // Todo �_���[�W���o�E���[�V�����Đ�
    }

    /// <summary>
    /// HP��
    /// </summary>
    /// <param name="_amount">�񕜗�</param>
    public void HealHP(int _amount)
    {
        currentHp += _amount;

        if (currentHp > HP) currentHp = HP;

        // �񕜉��o
    }

    /// <summary>
    /// MP�g�p
    /// </summary>
    /// <param name="_amount">�g�p��</param>
    /// <returns>�����s�Ȃ�false��Ԃ�</returns>
    public bool CostMP(int _amount)
    {
        // MP������Ȃ���Δ����s��
        if (currentMp < _amount)
            return false;

        currentMp -= _amount;

        if (currentMp < 0) currentMp = 0;

        return true;
    }

    /// <summary>
    /// MP��
    /// </summary>
    /// <param name="_amount">�񕜗�</param>
    public void HealMP(int _amount)
    {
        currentMp += _amount;

        if (currentMp > MP) currentMp = MP;
        // �񕜉��o
    }

    /// <summary>
    /// ���S
    /// </summary>
    public void Dead()
    {
        // �s�k���o�E���[�V�����Đ�
    }

    /// <summary>
    /// �A�^�b�N�p�^�[�����I
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
