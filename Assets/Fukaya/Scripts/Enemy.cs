using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Master;

public class Enemy : Character
{
    private DropController dropController;

    public int POSITION; // �G�̈ʒu

    // �A�^�b�N�p�^�[��
    public List<EnemyAttackPattern> attackPattern = new();

    [SerializeField, Header("�h���b�v")] private Text dropText;

    void Start()
    {
        dropController = FindObjectOfType<DropController>();
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

        damageText.enabled = false;
        dropText.enabled = false;
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
        // ��_�� - �h��� �����ۂ̔�_���[�W�ɂ���
        int damage = _amount - (int)(DEF * powerDef);

        currentHp -= damage;
        hpGuage.Sub(damage); // �Q�[�W�������o

        if (currentHp < 0)
        {
            currentHp = 0;

            // ���S����
            Dead();
        }

        // Todo �_���[�W���o�E���[�V�����Đ�
        StartCoroutine(DispText(damageText, damage.ToString()));
    }

    /// <summary>
    /// HP��
    /// </summary>
    /// <param name="_amount">�񕜗�</param>
    public override void HealHP(int _amount)
    {
        currentHp += _amount;

        if (currentHp > HP) currentHp = HP;

        hpGuage.Add(_amount);

        // �񕜉��o
    }

    /// <summary>
    /// ���S
    /// </summary>
    public override void Dead()
    {
        Debug.Log("�G" + POSITION + "��|����");

        // Todo ���[�V�����Đ�

        StartCoroutine(DropDirection());
    }

    /// <summary>
    /// �h���b�v���I�E���o
    /// </summary>
    /// <returns></returns>
    IEnumerator DropDirection()
    {
        // �h���b�v���I
        int drop = dropController.DropLottery();

        yield return new WaitForSeconds(1.0f);

        // �h���b�v�ʕ\��
        string str = "+" + drop.ToString() + "Pt";
        StartCoroutine(DispText(dropText, str));
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
