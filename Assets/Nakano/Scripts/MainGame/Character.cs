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
    [HideInInspector] public int ATK; // �U��
    [HideInInspector] public int MP;  // ����
    [HideInInspector] public int HP;  // �̗�
    [HideInInspector] public int DEF; // �h��
    [HideInInspector] public int AGI; // ���x
    [HideInInspector] public int DEX; // ��p

    // �v�Z�p ���ݒl
    [HideInInspector] public int currentMp;
    [HideInInspector] public int currentAtk;
    [HideInInspector] public int currentHp;
    [HideInInspector] public int currentDef;
    [HideInInspector] public int currentDex;
    [HideInInspector] public int currentAgi;

    // �X�e�[�^�X�{��
    [HideInInspector] public float powerMp = 1;
    [HideInInspector] public float powerAtk = 1;
    [HideInInspector] public float powerHp = 1;
    [HideInInspector] public float powerDef = 1;
    [HideInInspector] public float powerDex = 1;
    [HideInInspector] public float powerAgi = 1;

    // �o�t���� ����
    [HideInInspector] public float buffMp = 0;
    [HideInInspector] public float buffAtk = 0;
    [HideInInspector] public float buffHp = 0;
    [HideInInspector] public float buffDef = 0;
    [HideInInspector] public float buffDex = 0;
    [HideInInspector] public float buffAgi = 0;

    // �f�o�t���� ����
    [HideInInspector] public float debuffMp = 0;
    [HideInInspector] public float debuffAtk = 0;
    [HideInInspector] public float debuffHp = 0;
    [HideInInspector] public float debuffDef = 0;
    [HideInInspector] public float debuffDex = 0;
    [HideInInspector] public float debuffAgi = 0;

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
    /// �s���I��
    /// </summary>
    public virtual void MoveEnd() { }

    /// <summary>
    /// �ʏ�U��
    /// </summary>
    public virtual void NormalAttack() { }

    /// <summary>
    /// �_���[�W
    /// </summary>
    /// <param name="_amount">�_���[�W��</param>
    /// <returns>�h��͕������������_���[�W��</returns>
    public virtual int Damage(int _amount)
    {
        // ��_�� - �h��� �����ۂ̔�_���[�W�ɂ���
        int damage = (_amount - (int)(DEF * powerDef));
        damage = damage < 0 ? 0 : damage; // 0�����Ȃ�0�ɂ���

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
    /// <param name="cantGuard">�h�䖳�����ǂ����@true�Ȃ�h�䖳��</param>
    /// <returns>�h��͕������������_���[�W��</returns>
    public virtual int Damage(int _amount, bool cantGuard)
    {
        // ��_�� - �h��� �����ۂ̔�_���[�W�ɂ���
        // �h�䖳���̂Ƃ��͔�_������h��͕����������Ȃ�
        int damage = cantGuard ? _amount : (_amount - (int)(DEF * powerDef));
        damage = damage < 0 ? 0 : damage; // 0�����Ȃ�0�ɂ���

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
