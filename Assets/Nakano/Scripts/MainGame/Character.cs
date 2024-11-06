using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// ���C���Q�[����œ����L�����N�^�[�@PlayerData��Enemy�Ɍp��
/// </summary>
public class Character : MonoBehaviour
{
    public Image image; // �C���X�g
    [SerializeField] protected MainGameGuage hpGuage;
    [SerializeField] protected Text damageText;

    [SerializeField, Header("�e�L�X�g�\������")] protected float textDispTime = 3.0f;

    // �X�e�[�^�X
    public int ATK; // �U��
    public int MP;  // ����
    public int HP;  // �̗�
    public int DEF; // �h��
    public int AGI; // ���x
    public int DEX; // ��p

    // �v�Z�p ���ݒl
    public int currentMp;
    public int currentAtk;
    public int currentHp;
    public int currentDef;
    public int currentDex;
    public int currentAgi;

    // �X�e�[�^�X�{��
    public float powerMp = 1;
    public float powerAtk = 1;
    public float powerHp = 1;
    public float powerDef = 1;
    public float powerDex = 1;
    public float powerAgi = 1;

    // �o�t���� ����
    public float buffMp = 0;
    public float buffAtk = 0;
    public float buffHp = 0;
    public float buffDef = 0;
    public float buffDex = 0;
    public float buffAgi = 0;

    // �f�o�t���� ����
    public float debuffMp = 0;
    public float debuffAtk = 0;
    public float debuffHp = 0;
    public float debuffDef = 0;
    public float debuffDex = 0;
    public float debuffAgi = 0;

    // ��S��
    public float criticalProbability;

    /// <summary>
    /// ������
    /// </summary>
    public virtual void Initialize()
    {
        currentMp = MP;
        currentAtk = ATK;
        currentHp = HP;
        currentDef = DEF;
        currentDex = DEX;
        currentAgi = AGI;

        hpGuage.Initialize(HP);
        if (damageText) damageText.enabled = false;
    }

    /// <summary>
    /// �s��
    /// </summary>
    public virtual void Move() { }

    /// <summary>
    /// �ʏ�U��
    /// </summary>
    /// <returns>�^�_���[�W��</returns>
    public virtual int NormalAttack()
    {
        return -1;
    }

    /// <summary>
    /// �_���[�W
    /// </summary>
    /// <param name="_amount">�_���[�W��</param>
    public virtual int Damage(int _amount)
    {
        // ��_�� - �h��� �����ۂ̔�_���[�W�ɂ���
        int damage = (_amount - (int)(DEF * powerDef));
        currentHp -= damage;

        if (currentHp < 0)
        {
            currentHp = 0;
            Dead();
        }

        // �Q�[�W�������o
        hpGuage.Sub(damage);

        // �_���[�W�\��
        StartCoroutine(DispText(damageText, damage.ToString()));

        return damage;
    }

    /// <summary>
    /// �_���[�W
    /// </summary>
    /// <param name="_amount">�_���[�W��</param>
    /// <param name="_enemy">�_���[�W��^�����G</param>
    public virtual int Damage(int _amount, Enemy _enemy)
    {
        return Damage(_amount);
    }

    /// <summary>
    /// HP��
    /// </summary>
    /// <param name="_amount">�񕜗�</param>
    public virtual void HealHP(int _amount)
    {
        currentHp += _amount;
        if (currentHp > HP) currentHp = HP;
        hpGuage.Add(_amount);
    }

    /// <summary>
    /// ���S
    /// </summary>
    public virtual void Dead() { }

    /// <summary>
    /// �o�t�ǉ�
    /// </summary>
    /// <param name="_type">�X�e�[�^�X�̎��</param>
    /// <param name="_amount">�o�t�ʁ@�����\�L</param>
    public void AddBuff(StatusType _type, float _amount)
    {
        switch(_type)
        {
            case StatusType.HP:
                buffHp += _amount;
                break;
            case StatusType.MP:
                buffMp += _amount;
                break;
            case StatusType.ATK:
                buffAtk += _amount;
                break;
            case StatusType.DEF:
                buffDef += _amount;
                break;
            case StatusType.AGI:
                buffAgi += _amount;
                break;
            case StatusType.DEX:
                buffDex += _amount;
                break;
        }

        CalcPower();
    }

    /// <summary>
    /// �o�t��ԃ��Z�b�g
    /// </summary>
    public void ResetBuff()
    {
        buffMp = 0;
        buffAtk = 0;
        buffHp = 0;
        buffDef = 0;
        buffDex = 0;
        buffAgi = 0;
    }

    /// <summary>
    /// �f�o�t�ǉ�
    /// </summary>
    /// <param name="_type">�X�e�[�^�X�̎��</param>
    /// <param name="_amount">�f�o�t��</param>
    public void AddDebuff(StatusType _type, float _amount)
    {
        switch (_type)
        {
            case StatusType.HP:
                debuffHp += _amount;
                break;
            case StatusType.MP:
                debuffMp += _amount;
                break;
            case StatusType.ATK:
                debuffAtk += _amount;
                break;
            case StatusType.DEF:
                debuffDef += _amount;
                break;
            case StatusType.AGI:
                debuffAgi += _amount;
                break;
            case StatusType.DEX:
                debuffDex += _amount;
                break;
        }

        CalcPower();
    }

    /// <summary>
    /// �f�o�t��ԃ��Z�b�g
    /// </summary>
    public void ResetDebuff()
    {
        debuffMp = 0;
        debuffAtk = 0;
        debuffHp = 0;
        debuffDef = 0;
        debuffDex = 0;
        debuffAgi = 0;
    }

    /// <summary>
    /// �X�e�[�^�X�{���v�Z
    /// </summary>
    void CalcPower()
    {
        powerHp = 1 + buffHp - debuffHp;
        powerMp = 1 + buffMp - debuffMp;
        powerAtk = 1 + buffAtk - debuffAtk;
        powerDef = 1 + buffDef - debuffDef;
        powerAgi = 1 + buffAgi - debuffAgi;
        powerDex = 1 + buffDex - debuffDex;
    }

    /// <summary>
    /// ��S���I
    /// </summary>
    protected bool CriticalLottery()
    {
        int c = Random.Range(0, 100);

        if (c < criticalProbability) return true;
        else return false;
    }

    /// <summary>
    /// �e�L�X�g�\���@�_���[�W�ʓ��X
    /// </summary>
    /// <returns></returns>
    protected IEnumerator DispText(Text _text, string _str)
    {
        _text.text = _str;
        _text.enabled = true;

        yield return new WaitForSeconds(textDispTime);

        _text.enabled = false;
    }
}
