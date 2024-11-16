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

    int elapsedTurn_Debuff1 = 0;
    bool isActive_Debuff1 = false;

    int elapsedTurn_Debuff2 = 0;
    bool isActive_Debuff2 = false;

    int elapsedTurn_Buff = 0;
    bool isActive_Buff = false;

    int turn_AbsolutelyKill = 0;          // �m�E�U���܂ł̃^�[��
    int value_AbsolutelyKill = 999999999;

    [SerializeField] private PlayerData player;
    [SerializeField] protected GameObject hpGuage_Obj;
    [SerializeField, Header("�h���b�v")] private Text dropText;

    /// <summary>
    /// �h�䖳�����
    /// </summary>
    public bool isIgnoreDeffence = false;

    void Start()
    {
        dropController = FindObjectOfType<DropController>();
    }

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.A))
        {
            //NormalAttack();
            AbsolutelyKill();
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
        Debug.Log(move.criticalProbability);
    }

    public override void TurnEnd()
    {
        elapsedTurn_Debuff1++;
        elapsedTurn_Debuff2++;
        elapsedTurn_Buff++;
    }

    public override void NormalAttack()
    {
        float damage = ATK * powerAtk;
        player.Damage(damage, this);
    }

    /// <summary>
    /// �f�o�t�P�@�v���C���[��T�^�[���̖h���V���_�E���t�^
    /// </summary>
    void Debuff1()
    {
        elapsedTurn_Debuff1 = 1;
        isActive_Debuff1 = true;


    }

    void Cancel_Debuff1()
    {
        if (!isActive_Debuff1) return;

        if (elapsedTurn_Debuff1 >= 2)
        {
            
        }
    }

    /// <summary>
    /// �f�o�t�Q�@�v���C���[��T�^�[���̍U����V���_�E���t�^
    /// </summary>
    void Debuff2()
    {
        elapsedTurn_Debuff2 = 1;
        isActive_Debuff2 = true;
    }

    /// <summary>
    /// �o�t�@���g��T�^�[���̍U����V���A�b�v�t�^
    /// </summary>
    void Buff()
    {
        elapsedTurn_Buff = 1;
        isActive_Buff = true;
    }

    /// <summary>
    /// �Q�A���@�U���� * V�̍U������s��
    /// </summary>
    void DoubleAttack()
    {

    }

    /// <summary>
    /// �m�E�@�v���C���[��999999999�̌Œ�_���[�W
    /// </summary>
    void AbsolutelyKill()
    {
        player.Damage((int)value_AbsolutelyKill);
    }

    /// <summary>
    /// �_���[�W
    /// </summary>
    /// <param name="_amount">�_���[�W��</param>
    /// <returns>�h��͕������������_���[�W��</returns>
    public override int Damage(float _amount)
    {
        if (currentHp <= 0) return 0;

        int damage = 0;

        if (isIgnoreDeffence) damage = base.Damage(_amount, true);
        else damage = base.Damage(_amount);

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

        // �C���X�g�EHP�Q�[�W���\���ɂ���
        image.enabled = false;
        hpGuage_Obj.SetActive(false);

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
