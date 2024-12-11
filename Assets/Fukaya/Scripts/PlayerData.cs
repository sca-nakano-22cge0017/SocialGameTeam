using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Master;
using System;

/// <summary>
/// �v���C���[�̃X�e�[�^�X��R�}���h�Ǘ� ����Z�\�E�X�L���͕ʃX�N���v�g
/// </summary>
public class PlayerData : Character
{
    [SerializeField] private MainGameGuage mpGuage;
    [SerializeField] private MainGameGuage specialMoveGuage;
    [SerializeField] private Button[] commands;

    // �U���{��
    public float power_NormalAttack;  // �ʏ�U��
    public float power_Skill;         // �X�L��
    public float power_SpecialMove;   // �K�E�Z

    // �K�[�h
    private bool isGuard;
    public float power_Guard = 0.2f; // �K�[�h���h��{��

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

    /// <summary>
    /// �K�E�Z����ς݂��ǂ���
    /// </summary>
    public bool canSpecialMove = false;

    void Start()
    {
        soundController = FindObjectOfType<SoundController>();
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

        specialMoveGuage.Initialize(specialMoveGuageMax, 0);
    }

    public override void Move()
    {
        // �K�[�h���Ă�΃K�[�h����
        if (isGuard)
        {
            AddBuff(StatusType.DEF, -power_Guard);
            isGuard = false;
        }

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

        SetCommandsButton(false);

        mainGameSystem.ActionEnd();
    }
    IEnumerator EndWait()
    {
        yield return new WaitForSeconds(0.1f);
        MoveEnd();
    }

    public override void TurnEnd()
    {
        // �^�[���o�߂ɂ��Q�[�W�㏸
        UpSpecialMoveGuage(sm_Turn.guageUpAmount);
    }

    public override void NormalAttack()
    {
        SetCommandsButton(false);

        AttackMotion(() =>
        { 
            // ��S���I
            var cri = CriticalLottery();

            // �_���[�W�� = �U���� * �ʏ�U���{�� * �U���͔{�� * ��S�{��
            float damage = ATK * power_NormalAttack * powerAtk * critical;

            // ���b�N�I�������G�Ƀ_���[�W
            var target = mainGameSystem.Target;
            target.Damage(damage);
            if (cri) target.CriticalDamage();

            // �ʏ�U�����ɏ�����������Z�\
            atk_st.RankA(target);        // �K�[�h�u���C�J�[
            mp_st.RankB();               // �h���C��
            dex_st.RankA(target);        // �����̃e�N�j�b�N
            if (agi_st.RankA()) NormalAttack(); // �čs��

            UpSpecialMoveGuage(sm_NormalAttack.guageUpAmount);
        });
    }

    /// <summary>
    /// �K�[�h
    /// </summary>
    public void Guard()
    {
        SetCommandsButton(false);

        // �h��͔{���㏸
        AddBuff(StatusType.DEF, power_Guard);

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
        if (specialMoveGuageAmount < specialMoveGuageMax) return;
        if (!canSpecialMove) return;

        SetCommandsButton(false);
        specialMoveGuage.SetCurrent(0);

        AttackMotion(() => 
        {
            atk_st.RankS(); // �S�g�S��
            specialMoveGuageAmount = 0;

            // ��S���I
            var cri = CriticalLottery();

            // �_���[�W�� = �U���� * �K�E�Z�{�� * �U���͔{�� * ��S�{��
            float damage = ATK * power_SpecialMove * powerAtk * critical;

            // ���b�N�I�������G�Ƀ_���[�W
            var target = mainGameSystem.Target;
            target.Damage(damage);
            if (cri) target.CriticalDamage();
        });
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
        soundController.PlayDamageSE();

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

        // �񕜗ʕ\��
        StartCoroutine(DispText(healText, _amount.ToString()));
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

    public override void AddBuff(StatusType _type, float _amount)
    {
        base.AddBuff(_type, _amount);
        if (_amount > 0) soundController.PlayBuffSE();
    }

    public override void AddDebuff(StatusType _type, float _amount)
    {
        base.AddDebuff(_type, _amount);
        if (_amount > 0) soundController.PlayDebuffSE();
    }

    /// <summary>
    /// �K�E�Z�Q�[�W�㏸
    /// </summary>
    public void UpSpecialMoveGuage(int _amount)
    {
        specialMoveGuageAmount += _amount;

        if (specialMoveGuageAmount > specialMoveGuageMax)
            specialMoveGuageAmount = specialMoveGuageMax;

        specialMoveGuage.Add(_amount);
    }

    /// <summary>
    /// �K�E�Z�Q�[�W���ő�܂ŏ㏸
    /// </summary>
    public void UpSpecialMoveGuage()
    {
        int amount = specialMoveGuageMax - specialMoveGuageAmount;

        specialMoveGuageAmount = specialMoveGuageMax;

        specialMoveGuage.Add(amount);
    }

    /// <summary>
    /// ���S
    /// </summary>
    public override void Dead()
    {
        mainGameSystem.Judge();

        // Todo �s�k���o�E���[�V�����Đ�

        StartCoroutine(DirectionCompleteWait(() =>
        {
            meshRenderer.enabled = false;
        }));
    }

    IEnumerator DirectionCompleteWait(Action _action)
    {
        yield return new WaitUntil(() => hpGuage.isDirectionCompleted);

        yield return new WaitForSeconds(0.5f);

        _action?.Invoke();
    }

    public void AttackMotion(Action _action)
    {
        SetCommandsButton(false);

        spineAnim.callBack = () => 
        {
            soundController.PlayAttackSE(GameManager.SelectChara);

            _action?.Invoke();
            StartCoroutine(EndWait());
        };
        spineAnim.PlayAttackMotion();
    }

    public void BuffMotion(Action _action)
    {
        SetCommandsButton(false);

        spineAnim.callBack = () => { _action?.Invoke(); StartCoroutine(EndWait()); };
        spineAnim.PlaybuffMotion();
    }

    public void WinMotion()
    {
        SetCommandsButton(false);

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
                commands[i].image.color = new Color(1.0f, 1.0f, 1.0f, 1.0f);
            }
        }
        else
        {
            for (int i = 0; i < commands.Length; i++)
            {
                commands[i].interactable = false;
                commands[i].image.color = new Color(0.8f, 0.8f, 0.8f, 1.0f);
            }
        }

        if (specialMoveGuageAmount < specialMoveGuageMax || !canSpecialMove)
        {
            commands[0].interactable = false;
            commands[0].image.color = new Color(0.8f, 0.8f, 0.8f, 1.0f);
        }
    }
}
