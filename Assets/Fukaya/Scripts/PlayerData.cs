using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Master;

/// <summary>
/// �v���C���[�̃X�e�[�^�X��R�}���h�Ǘ�
/// </summary>
public class PlayerData : Character
{
    // ����Z�\�E�X�L���͕ʃX�N���v�g

    [SerializeField] private MainGameGuage mpGuage;
    
    // �U���{��
    public float power_NormalAttack;  // �ʏ�U��
    public float power_Skill;         // �X�L��
    public float power_Critical;      // ��S���{��
    public float power_SpecialMove;   // �K�E�Z

    // �K�[�h
    private bool isGuard;
    public float power_Guard = 1.2f; // �K�[�h���h��{��

    // �K�E�Z�Q�[�W
    private int specialMoveGuageAmount;
    public int specialMoveGuageMax; // �ő��

    // �K�E�Q�[�W�ݒ�
    // 1�F�ʏ�U�� 2�F�h���ԂŔ�_�� 3�F��h���ԂŔ�_�� 4�F�o�߃^�[�� 5�F�A�b�v�p�X�L��
    public SpecialMoveGuageSetting sm_NormalAttack;
    public SpecialMoveGuageSetting sm_Guard;
    public SpecialMoveGuageSetting sm_Damage;
    public SpecialMoveGuageSetting sm_Turn;
    public SpecialMoveGuageSetting sm_Skill;

    [SerializeField] HP_SpecialTecnique hp_st;
    [SerializeField] DEF_SpecialTecnique def_st;
    [SerializeField] ATK_SpecialTecnique atk_st;
    [SerializeField] MP_SpecialTecnique mp_st;
    [SerializeField] AGI_SpecialTecnique agi_st;
    [SerializeField] DEX_SpecialTecnique dex_st;

    [SerializeField] Enemy enemy_forDebug;

    /// <summary>
    /// ���G��Ԃ��ǂ���
    /// </summary>
    public bool isInvincible = false;

    void Start()
    {
        
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.S))
        {
            //enemies[0].Damage(100);

            Move();

            atk_st.GameStart();
            hp_st.PlayerTurnStart();
        }

        if (Input.GetKeyDown(KeyCode.E))
        {
            MoveEnd();

            hp_st.TurnEnd();
            def_st.TurnEnd();
            atk_st.TurnEnd();
        }
    }

    /// <summary>
    /// �X�e�[�^�X������
    /// </summary>
    public override void Initialize()
    {
        base.Initialize();

        specialMoveGuageAmount = 0;
        mpGuage.Initialize(MP);
    }

    public override void Move()
    {
        Debug.Log("�v���C���[�̍s��");
    }

    public override void MoveEnd()
    {
        Debug.Log("�v���C���[�̍s���I��");

        isGuard = false;
    }

    public override void NormalAttack()
    {
        // ��S�{��
        float critical = CriticalLottery() == true ? power_Critical : 1.0f;

        // �_���[�W�� = �U���� * �ʏ�U���{�� * �U���͔{�� * ��S�{��
        int damage = (int)(ATK * power_NormalAttack * powerAtk * critical);

        // Todo ���b�N�I�������G�Ƀ_���[�W
        //enemy_forDebug.Damage(damage);

        // �ʏ�U�����ɏ�����������Z�\
        atk_st.RankA(enemy_forDebug);

        UpSpecialMoveGuage(sm_NormalAttack.guageUpAmount);
    }

    /// <summary>
    /// �K�[�h
    /// </summary>
    public void Guard()
    {
        // Todo 1�^�[���o�߂ŃK�[�h�����i�h��͔{����1�ɖ߂��j

        // �h��͔{���㏸
        powerDef += power_Guard;

        isGuard = true;

        // �h�䎞�ɏ�����������Z�\
        def_st.RankC();
        def_st.RankB();
    }

    /// <summary>
    /// �K�E�Z
    /// </summary>
    public void SpecialMove()
    {
        specialMoveGuageAmount = 0;
    }

    public int Damage(int _damageAmount, Enemy _enemy)
    {
        // Todo ��𔻒�

        // ��_�� - �h��� �����ۂ̔�_���[�W�ɂ���
        int damage = (_damageAmount - (int)(DEF * powerDef));
        damage = damage < 0 ? 0 : damage; // 0�����Ȃ�0�ɂ���

        // ��_���[�W���ɏ�����������Z�\
        if (isGuard) def_st.RankS(damage, _enemy); // �U�h���

        def_st._RankA();                   // ���G
        if (isInvincible) damage = 0;      // ���G��ԂȂ��_��0

        damage -= def_st._RankSS(damage); // ���_�̌��\
        hp_st._RankB(damage, _enemy);     // �ɂݕ���

        // HP����
        currentHp -= damage;
        if (currentHp < 0)
        {
            currentHp = 0;
            Dead();
        }

        atk_st.RankB();                   // �w���̐w

        // HP�Q�[�W�������o
        hpGuage.Sub(damage);

        // �_���[�W�\��
        StartCoroutine(DispText(damageText, damage.ToString()));

        // Todo �_���[�W���o�E���[�V�����Đ�

        // �K�E�Q�[�W��
        if (isGuard) UpSpecialMoveGuage(sm_Guard.guageUpAmount);
        else UpSpecialMoveGuage(sm_Damage.guageUpAmount);

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
        mpGuage.Add(_amount);

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
    public override void Dead()
    {
        // Todo �s�k���o�E���[�V�����Đ�
        image.enabled = false;
    }
}
