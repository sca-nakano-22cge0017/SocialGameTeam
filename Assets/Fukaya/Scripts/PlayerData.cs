using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Master;
using System;

/// <summary>
/// �v���C���[�̃X�e�[�^�X��R�}���h�Ǘ�
/// </summary>
public class PlayerData : Character
{
    // ����Z�\�E�X�L���͕ʃX�N���v�g

    [SerializeField] private MainGameGuage mpGuage;
    [SerializeField] private Button[] commands;
    
    // �U���{��
    public float power_NormalAttack;  // �ʏ�U��
    public float power_Skill;         // �X�L��
    public float power_SpecialMove;   // �K�E�Z

    // �K�[�h
    private bool isGuard;
    public float power_Guard = 1.2f; // �K�[�h���h��{��

    // �K�E�Z�Q�[�W
    public int specialMoveGuageAmount;
    public int specialMoveGuageMax; // �ő��

    // �K�E�Q�[�W�ݒ�
    // 1�F�ʏ�U�� 2�F�h���ԂŔ�_�� 3�F��h���ԂŔ�_�� 4�F�o�߃^�[�� 5�F�A�b�v�p�X�L��
    public SpecialMoveGuageSetting sm_NormalAttack;
    public SpecialMoveGuageSetting sm_Guard;
    public SpecialMoveGuageSetting sm_Damage;
    public SpecialMoveGuageSetting sm_Turn;
    public SpecialMoveGuageSetting sm_Skill;

    /// <summary>
    /// MP����ʔ{��
    /// </summary>
    public float power_CostMp = 1;

    [SerializeField] HP_SpecialTecnique hp_st;
    [SerializeField] DEF_SpecialTecnique def_st;
    [SerializeField] ATK_SpecialTecnique atk_st;
    [SerializeField] MP_SpecialTecnique mp_st;
    [SerializeField] AGI_SpecialTecnique agi_st;
    [SerializeField] DEX_SpecialTecnique dex_st;

    /// <summary>
    /// ���G��Ԃ��ǂ���
    /// </summary>
    public bool isInvincible = false;

    // ���[�V�����֌W
    public Animator motion;

    void Start()
    {
        atk_st.GameStart();
    }

    /// <summary>
    /// �X�e�[�^�X������
    /// </summary>
    public override void Initialize()
    {
        base.Initialize();

        specialMoveGuageAmount = 0;
        mpGuage.Initialize(MP);

        SetCommandsButton(false);
    }

    public override void Move()
    {
        if (currentHp <= 0)
        {
            MoveEnd();
            return;
        }

        Debug.Log("�v���C���[�̍s��");

        SetCommandsButton(true);
        hp_st.PlayerTurnStart();
    }

    public override void MoveEnd()
    {
        Debug.Log("�v���C���[�̍s���I��");

        isGuard = false;
        SetCommandsButton(false);

        mainGameSystem.ActionEnd();
    }
    IEnumerator EndWait()
    {
        yield return new WaitForSeconds(2.0f);
        MoveEnd();
    }

    public override void NormalAttack()
    {
        // ��S���I
        CriticalLottery();

        // �_���[�W�� = �U���� * �ʏ�U���{�� * �U���͔{�� * ��S�{��
        float damage = ATK * power_NormalAttack * powerAtk * critical;

        AttackMotion();

        // Todo ���b�N�I�������G�Ƀ_���[�W
        var target = mainGameSystem.Target;
        target.Damage(damage);

        // �ʏ�U�����ɏ�����������Z�\
        atk_st.RankA(target);        // �K�[�h�u���C�J�[
        mp_st.RankB();               // �h���C��
        dex_st.RankA(target);        // �����̃e�N�j�b�N
        if (agi_st.RankA()) NormalAttack(); // �čs��

        UpSpecialMoveGuage(sm_NormalAttack.guageUpAmount);

        StartCoroutine(EndWait());
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

        StartCoroutine(EndWait());
    }

    /// <summary>
    /// �K�E�Z
    /// </summary>
    public void SpecialMove()
    {
        atk_st.RankS(); // �S�g�S��
        specialMoveGuageAmount = 0;

        AttackMotion();
        // �K�E�Z����

        StartCoroutine(EndWait());
    }

    public int Damage(float _damageAmount, Enemy _enemy)
    {
        if (agi_st.RankS())
        {
            UpSpecialMoveGuage(sm_Guard.guageUpAmount);
            return 0;     // �X�e�b�v
        }

        // ��_�� - �h��� �����ۂ̔�_���[�W�ɂ���
        int damage = (int)Mathf.Ceil(_damageAmount - (DEF * powerDef));
        damage = damage < 0 ? 0 : damage; // 0�����Ȃ�0�ɂ���

        // ��_���[�W���ɏ�����������Z�\
        if (isGuard) def_st.RankS(damage, _enemy); // �U�h���
        mp_st.RankS(_enemy);

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
    /// �K�E�Z�Q�[�W���ő�܂ŏ㏸
    /// </summary>
    public void UpSpecialMoveGuage()
    {
        specialMoveGuageAmount = specialMoveGuageMax;
        // Todo �㏸���o
    }

    /// <summary>
    /// ���S
    /// </summary>
    public override void Dead()
    {
        // Todo �s�k���o�E���[�V�����Đ�

        meshRenderer.enabled = false;
    }

    public void AttackMotion()
    {
        motion.SetTrigger("attack");
    }

    public void BuffMotion()
    {
        motion.SetTrigger("buff");
    }

    public void WinMotion()
    {
        motion.SetTrigger("win");
    }

    /// <summary>
    /// �R�}���h�@�����邩�ǂ�����ݒ�
    /// </summary>
    /// <param name="_canPut">false�̂Ƃ������Ȃ�</param>
    void SetCommandsButton(bool _canPut)
    {
        if (_canPut)
        {
            for (int i = 0; i < commands.Length; i++)
            {
                commands[i].interactable = true;
            }
        }
        else
        {
            for (int i = 0; i < commands.Length; i++)
            {
                commands[i].interactable = false;
            }
        }
    }
}
