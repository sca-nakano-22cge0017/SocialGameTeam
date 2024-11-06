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

    [SerializeField] private PlayerData player;
    [SerializeField, Header("�h���b�v")] private Text dropText;

    void Start()
    {
        dropController = FindObjectOfType<DropController>();
    }

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.A))
        {
            player.Damage(NormalAttack(), this);
        }
    }

    /// <summary>
    /// ������ԂɈڍs
    /// </summary>
    public override void Initialize()
    {
        base.Initialize();

        dropText.enabled = false;
        damageText.enabled = false;
    }

    public override void Move()
    {
        Debug.Log("�G" + POSITION + "�̍s��");

        // �U�����@�𒊑I
        Master.EnemyAttackPattern move = MoveLottery();
    }

    public override int NormalAttack()
    {
        return 300;
    }

    /// <summary>
    /// �_���[�W
    /// </summary>
    /// <param name="_amount">�_���[�W��</param>
    public override int Damage(int _amount)
    {
        int damage = base.Damage(_amount);

        return damage;
    }

    /// <summary>
    /// HP��
    /// </summary>
    /// <param name="_amount">�񕜗�</param>
    public override void HealHP(int _amount)
    {
        base.HealHP(_amount);

        // Todo �񕜉��o
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
