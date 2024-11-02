using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// �v���C���[�̃X�e�[�^�X��R�}���h�Ǘ�
/// </summary>
public class PlayerData : MonoBehaviour
{
    // ����Z�\�E�X�L���͕ʃX�N���v�g

    public Image image; // �C���X�g

    [SerializeField] private MainGameGuage hpGuage;
    [SerializeField] private MainGameGuage mpGuage;

    // �X�e�[�^�X
    public int ATK; // �U��
    public int MP;  // ����
    public int HP;  // �̗�
    public int DEF; // �h��
    public int AGI; // ���x
    public int DEX; // ��p

    // �v�Z�p
    private int currentMp;
    private int currentAtk;
    private int currentHp;
    private int currentDef;
    private int currentDex;
    private int currentAgi;

    // �X�e�[�^�X�{��
    public float powerMp = 1;
    public float powerAtk = 1;
    public float powerHp = 1;
    public float powerDef = 1;
    public float powerDex = 1;
    public float powerAgi = 1;

    // �U���{��
    public float power_NormalAttack;  // �ʏ�U��
    public float power_Skill;         // �X�L��
    public float power_Critical;      // ��S���{��
    public float power_SpecialMove;   // �K�E�Z

    public float criticalProbability; // ��S��

    // �K�[�h
    private bool isGuard;
    public float power_Guard = 1.2f; // �K�[�h���h��{��

    // �K�E�Z�Q�[�W
    [SerializeField] private Image specialMoveGuage;
    private int specialMoveGuageAmount;
    public int specialMoveGuageMax; // �ő��

    // �K�E�Q�[�W�ݒ� 1�F�ʏ�U�� 2�F�h���ԂŔ�_�� 3�F��h���ԂŔ�_�� 4�F�o�߃^�[�� 5�F�A�b�v�p�X�L��
    public Master.SpecialMoveGuageSetting sm_NormalAttack;
    public Master.SpecialMoveGuageSetting sm_Guard;
    public Master.SpecialMoveGuageSetting sm_Damage;
    public Master.SpecialMoveGuageSetting sm_Turn;
    public Master.SpecialMoveGuageSetting sm_Skill;

    void Start()
    {
        
    }

    void Update()
    {
        
    }

    /// <summary>
    /// �X�e�[�^�X������
    /// </summary>
    private void Initialize()
    {
        currentMp = MP;
        currentAtk = ATK;
        currentHp = HP;
        currentDef = DEF;
        currentDex = DEX;
        currentAgi = AGI;

        specialMoveGuageAmount = 0;

        hpGuage.Initialize(HP);
        mpGuage.Initialize(MP);
    }

    /// <summary>
    /// �ʏ�U��
    /// </summary>
    /// <returns>�^�_���[�W��</returns>
    public int NormalAttack()
    {
        // ��S�{��
        float critical = CriticalLottely() == true ? power_Critical : 1.0f;

        // �_���[�W�� = �U���� * �ʏ�U���{�� * �U���͔{�� * ��S�{��
        int damage = (int)(ATK * power_NormalAttack * powerAtk * critical);

        UpSpecialMoveGuage(sm_NormalAttack.guageUpAmount);

        return damage;
    }

    /// <summary>
    /// �K�[�h
    /// </summary>
    public void Guard()
    {
        // Todo 1�^�[���o�߂ŃK�[�h�����i�h��͔{����1�ɖ߂��j

        // �h��͔{���㏸
        powerDef = power_Guard;

        isGuard = true;
    }

    /// <summary>
    /// �K�E�Z
    /// </summary>
    public void SpecialMove()
    {
        specialMoveGuageAmount = 0;
    }

    /// <summary>
    /// �_���[�W
    /// </summary>
    /// <param name="_amount">�_���[�W��</param>
    public void Damage(int _amount)
    {
        // Todo ��𔻒�

        // ��_�� - �h��� �����ۂ̔�_���[�W�ɂ���
        currentHp -= (_amount - (int)(DEF * powerDef));

        // �Q�[�W�������o
        hpGuage.Sub(_amount);

        if (currentHp < 0)
        {
            currentHp = 0;
            // ���S����H
        }

        // �K�E�Q�[�W��
        if (isGuard) UpSpecialMoveGuage(sm_Guard.guageUpAmount);
        else UpSpecialMoveGuage(sm_Damage.guageUpAmount);

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

        // Todo �񕜉��o
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
        mpGuage.Sub(_amount); // �Q�[�W�������o

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
        // Todo �񕜉��o
    }

    /// <summary>
    /// �K�E�Z�Q�[�W�㏸
    /// </summary>
    public void UpSpecialMoveGuage(int _amount)
    {
        specialMoveGuageAmount += _amount;

        if (specialMoveGuageAmount > specialMoveGuageMax)
            specialMoveGuageAmount = specialMoveGuageMax;

        // Todo �㏸���o
    }

    /// <summary>
    /// ���S
    /// </summary>
    public void Dead()
    {
        // Todo �s�k���o�E���[�V�����Đ�
    }

    /// <summary>
    /// ��S���I
    /// </summary>
    bool CriticalLottely()
    {
        int c = Random.Range(0, 100);

        if (c < criticalProbability) return true;
        else return false;
    }
}
