using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Master;
using System;

public class Enemy : Character
{
    [SerializeField] private MainGameGuage hpGuage;

    public int POSITION; // �G�̈ʒu

    // �A�^�b�N�p�^�[��
    public List<EnemyAttackPattern> attackPattern = new();

    void Start()
    {

    }

    void Update()
    {
        
    }

    /// <summary>
    /// ������ԂɈڍs
    /// </summary>
    public override void Initialize()
    {
        currentMp = MP;
        currentAtk = ATK;
        currentHp = HP;
        currentDef = DEF;
        currentDex = DEX;
        currentAgi = AGI;

        hpGuage.Initialize(HP);
    }

    public override void Move()
    {
        Debug.Log("�G" + POSITION + "�̍s��");

        // �U�����@�𒊑I
        Master.EnemyAttackPattern move = MoveLottery();
    }

    public override int NormalAttack()
    {
        return -1;
    }

    /// <summary>
    /// �_���[�W
    /// </summary>
    /// <param name="_amount">�_���[�W��</param>
    public override void Damage(int _amount)
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
