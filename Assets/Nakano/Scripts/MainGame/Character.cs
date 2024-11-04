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
    [SerializeField, Header("�e�L�X�g�\������")] protected float textDispTime = 1.0f;

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

    // �o�t����
    public float buffMp = 0;
    public float buffAtk = 0;
    public float buffHp = 0;
    public float buffDef = 0;
    public float buffDex = 0;
    public float buffAgi = 0;

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
    }

    /// <summary>
    /// �s��
    /// </summary>
    public virtual void Move()
    {
        
    }

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
    public virtual void Damage(int _amount)
    {
        // ��_�� - �h��� �����ۂ̔�_���[�W�ɂ���
        currentHp -= (_amount - (int)(DEF * powerDef));

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
    public virtual void HealHP(int _amount)
    {
        currentHp += _amount;

        if (currentHp > HP) currentHp = HP;

        // Todo �񕜉��o
    }

    /// <summary>
    /// ���S
    /// </summary>
    public virtual void Dead()
    {

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
    /// �e�L�X�g�\���@�_���[�W���X
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
