using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

/// <summary>
/// ���C���Q�[����œ����L�����N�^�[�@PlayerData��Enemy�Ɍp��
/// </summary>
public class Character : MonoBehaviour
{
    protected SoundController soundController;
    [SerializeField] protected MainGameSystem mainGameSystem;
    [SerializeField] protected MainDirection mainDirection;

    public MeshRenderer meshRenderer;

    [SerializeField] public MainGameGuage hpGuage;
    [SerializeField] protected Text criticalText;
    [SerializeField] protected Text damageText;
    [SerializeField] protected Text healText;
    [SerializeField] protected Text buffText;

    [SerializeField, Header("�e�L�X�g�\������")] protected float textDispTime;

    // �X�e�[�^�X
    [HideInInspector] public int ATK; // �U��
    [HideInInspector] public int MP;  // ����
    [HideInInspector] public int HP;  // �̗�
    [HideInInspector] public int DEF; // �h��
    [HideInInspector] public int AGI; // ���x
    [HideInInspector] public int DEX; // ��p

    // �v�Z�p ���ݒl
    [HideInInspector] public int currentMp;
    [HideInInspector] public int currentHp;

    // �X�e�[�^�X�{��
    [HideInInspector] public float powerMp ;
    [HideInInspector] public float powerAtk;
    [HideInInspector] public float powerHp ;
    [HideInInspector] public float powerDef;
    [HideInInspector] public float powerDex;
    [HideInInspector] public float powerAgi;

    // �o�t���� ����
    [HideInInspector] public float buffMp ;
    [HideInInspector] public float buffAtk;
    [HideInInspector] public float buffHp ;
    [HideInInspector] public float buffDef;
    [HideInInspector] public float buffDex;
    [HideInInspector] public float buffAgi;

    // �f�o�t���� ����
    [HideInInspector] public float debuffMp ;
    [HideInInspector] public float debuffAtk;
    [HideInInspector] public float debuffHp ;
    [HideInInspector] public float debuffDef;
    [HideInInspector] public float debuffDex;
    [HideInInspector] public float debuffAgi;

    // ��S��
    public float criticalProbabilityInitial;  // �����l
    public float _criticalProbability;        // �v�Z�p

    public float power_CriticalInit;    // ��{��S���{��
    public float buffCriticalPower;     // ��S���{���o�t
    public float critical;    // ��S���{���@�v�Z�p

    // ���[�V�����֌W
    public Animator motion;
    public SpineAnim spineAnim;

    /// <summary>
    /// ������
    /// </summary>
    public virtual void Initialize()
    {
        critical = 1.0f;

        currentMp = MP;
        currentHp = HP;

        powerMp = 1;
        powerAtk = 1;
        powerHp = 1;
        powerDef = 1;
        powerDex = 1;
        powerAgi = 1;

        buffMp = 0;
        buffAtk = 0;
        buffHp = 0;
        buffDef = 0;
        buffDex = 0;
        buffAgi = 0;

        debuffMp = 0;
        debuffAtk = 0;
        debuffHp = 0;
        debuffDef = 0;
        debuffDex = 0;
        debuffAgi = 0;

        hpGuage.Initialize(HP);
        if (damageText) damageText.enabled = false;
        if (healText) healText.enabled = false;
        if (buffText) buffText.enabled = false;
        if (criticalText) criticalText.enabled = false;

        _criticalProbability = criticalProbabilityInitial;
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
    /// �^�[���I��
    /// </summary>
    public virtual void TurnEnd() { }

    /// <summary>
    /// �ʏ�U��
    /// </summary>
    public virtual void NormalAttack() { }

    /// <summary>
    /// �_���[�W
    /// </summary>
    /// <param name="_amount">�_���[�W��</param>
    /// <returns>�h��͕������������_���[�W��</returns>
    public virtual int Damage(float _amount)
    {
        // ��_�� - �h��� �����ۂ̔�_���[�W�ɂ���
        int damage = (int)Mathf.Ceil(_amount - (DEF * powerDef));
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
    public virtual int Damage(float _amount, bool cantGuard)
    {
        // ��_�� - �h��� �����ۂ̔�_���[�W�ɂ���
        // �h�䖳���̂Ƃ��͔�_������h��͕����������Ȃ�
        int damage = cantGuard ? (int)Mathf.Ceil(_amount) : (int)Mathf.Ceil(_amount - (DEF * powerDef));
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
    /// �Œ�_���[�W
    /// </summary>
    /// <param name="_amount">�_���[�W��</param>
    /// <returns></returns>
    public virtual int Damage(int _amount)
    {
        int damage = _amount;
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
    public virtual void AddBuff(StatusType _type, float _amount)
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

        if (_amount > 0)
        {
            string str = _type.ToString() + " " + (int)(_amount * 100) + " %UP";
            Color orange = new Color(1.0f, 0.56f, 0.0f, 1.0f);
            StartCoroutine(DispText(buffText, str, orange));
        }
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

        CalcPower();
    }

    /// <summary>
    /// �f�o�t�ǉ�
    /// </summary>
    /// <param name="_type">�X�e�[�^�X�̎��</param>
    /// <param name="_amount">�f�o�t��</param>
    public virtual void AddDebuff(StatusType _type, float _amount)
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

        if (_amount > 0)
        {
            string str = _type.ToString() + " " + (int)(_amount * 100) + " %DOWN";
            StartCoroutine(DispText(buffText, str, Color.blue));
        }
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

        CalcPower();
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

        Debug.Log($"�{�� HP:{powerHp}, MP:{powerMp}, ATK:{powerAtk}, DEF:{powerDef}, AGI:{powerAgi}, DEX:{powerDex}");
    }

    /// <summary>
    /// ��S���I
    /// </summary>
    public bool CriticalLottery()
    {
        int c = UnityEngine.Random.Range(0, 100);

        if (c < _criticalProbability)
        {
            critical = power_CriticalInit + buffCriticalPower;
            Debug.Log("��S����");
            return true;
        }
        else
        {
            critical = 1.0f;
            return false;
        }
    }

    /// <summary>
    /// ��S���I
    /// </summary>
    protected bool CriticalLottery(float criticalProbability)
    {
        int c = UnityEngine.Random.Range(0, 100);

        if (c < criticalProbability)
        {
            critical = power_CriticalInit + buffCriticalPower;
            Debug.Log("��S����");
            return true;
        }
        else
        {
            critical = 1.0f;
            return false;
        }
    }

    /// <summary>
    /// �uCritical!�v��\������
    /// </summary>
    public void CriticalDamage()
    {
        StartCoroutine(DispText(criticalText, "Critical!"));
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

    protected IEnumerator DispText(Text _text, string _str, Color _color)
    {
        _text.text = _str;
        _text.enabled = true;
        _text.color = _color;

        yield return new WaitForSeconds(textDispTime);

        _text.enabled = false;
    }

    protected IEnumerator HPGuageDirectionCompleteWait(Action _action)
    {
        yield return new WaitUntil(() => hpGuage.isDirectionCompleted);

        _action?.Invoke();
    }
}
