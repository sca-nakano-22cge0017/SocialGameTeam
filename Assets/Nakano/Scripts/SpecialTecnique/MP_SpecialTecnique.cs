using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MP_SpecialTecnique : SpecialTecniqueMethod
{
    bool isActive_SS = false;

    public override void TurnEnd()
    {
        RankA();
        _RankSS();
    }

    /// <summary>
    /// �I�[���@�X�L��
    /// �U���́E�h��͂���V%�グ��
    /// </summary>
    public void RankC()
    {
        // ������Ȃ珈�����Ȃ�
        if (!rankC.m_released) return;

        if (!player.CostMP(rankC.m_cost)) return;

        float amount = (float)rankC.m_value1 / 100.0f;

        player.AddState(true, rankC.m_id, rankC.m_continuationTurn, () => { Cancel_RankC(); }, false);

        player.BuffMotion(() =>
        {
            player.AddBuff(StatusType.ATK, amount);
            player.AddBuff(StatusType.DEF, amount);

            Debug.Log("�u�I�[���v���� �U����/�h��� " + (amount * 100) + "%�A�b�v");
        });
    }

    /// <summary>
    /// �I�[������
    /// </summary>
    void Cancel_RankC()
    {
        float amount = (float)rankC.m_value1 / 100.0f;
        player.AddBuff(StatusType.ATK, -amount);
        player.AddBuff(StatusType.DEF, -amount);
        Debug.Log("�u�I�[���v����");
    }

    /// <summary>
    /// �h���C���@�p�b�V�u
    /// �ʏ�U����MP��V%��
    /// </summary>
    public void RankB()
    {
        // ������Ȃ珈�����Ȃ�
        if(!rankB.m_released) return;

        float amount = (float)rankB.m_value1 / 100.0f * player.MP;
        player.HealMP((int)amount);

        Debug.Log("�u�h���C���v���� MP "+ (int)amount + "��");
    }

    /// <summary>
    /// �����̓����@�p�b�V�u
    /// ���^�[��MP��V����
    /// </summary>
    public void RankA()
    {
        // ������Ȃ珈�����Ȃ�
        if(!rankA.m_released) return;

        float amount = (float)rankA.m_value1 / 100.0f * player.MP;
        player.HealMP((int)amount);

        Debug.Log("�u�����̓����v���� MP " + (int)amount + "��");
    }

    /// <summary>
    /// �􂢁@�p�b�V�u
    /// ��_�����AV���̊m���œG�Ɏ􂢏�Ԃ�t�^
    /// �􂢏�ԁF���^�[��W%HP����
    /// </summary>
    public void RankS(Enemy _enemy)
    {
        // ������Ȃ珈�����Ȃ�
        if(!rankS.m_released) return;

        int result = Random.Range(1, 100);
        if (result <= rankS.m_value1)
        {
            _enemy.AddState(false, rankS.m_id, rankS.m_continuationTurn, () => { Cancel_RankS(); }, () => { _RankS(_enemy); }, true);

            Debug.Log("�u�􂢁v�t�^");
        }
    }

    void _RankS(Enemy _enemy)
    {
        float amount = (float)rankS.m_value2 / 100.0f * _enemy.HP;
        _enemy.Damage((int)amount, true);
    }

    /// <summary>
    /// �􂢉���
    /// </summary>
    void Cancel_RankS()
    {
        Debug.Log("�u�􂢁v����");
    }

    /// <summary>
    /// ���p�t�̌��E�@�X�L��
    /// N�^�[���̊ԁAMP����ʂ�V���_�E���@�K�E�Z�Q�[�W��1�^�[������W����
    /// </summary>
    public void RankSS()
    {
        // ������Ȃ珈�����Ȃ�
        if (!rankSS.m_released) return;
        if (!player.CostMP(rankSS.m_cost)) return;

        isActive_SS = true;

        float mpAmount = (float)rankSS.m_value1 / 100.0f;
        player.AddState(true, rankSS.m_id, rankSS.m_continuationTurn, () => { Cancel_RankSS(); }, true);

        player.BuffMotion(() => 
        {
            player.power_CostMp = (1 - mpAmount);

            Debug.Log("�u���p�t�̌��E�v���� MP����� " + player.power_CostMp + "�{");
        });
    }

    void _RankSS()
    {
        if (!isActive_SS) return;

        float amount = (float)rankSS.m_value2 / 100.0f * player.specialMoveGuageMax;
        player.UpSpecialMoveGuage((int)amount);

        Debug.Log("�u���p�t�̌��E�v���� �K�E�Z�Q�[�W " + (int)amount + "��");
    }

    /// <summary>
    /// ���p�t�̌��E�@����
    /// </summary>
    void Cancel_RankSS()
    {
        isActive_SS = false;
        Debug.Log("�u���p�t�̌��E�v����");
    }
}

