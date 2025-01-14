using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AGI_SpecialTecnique : SpecialTecniqueMethod
{
    int effectAmount_A = 0;

    public override void TurnEnd()
    {
        effectAmount_A = 0;
    }

    /// <summary>
    /// �����@�X�L��
    /// N�^�[���̊ԁA���x��V%�グ��
    /// </summary>
    public  void RankC()
    {
        // ������Ȃ珈�����Ȃ�
        if(!rankC.m_released) return;

        if (!player.CostMP(rankC.m_cost)) return;

        float amount = (float)rankC.m_value1 / 100.0f;

        player.AddState(true, rankC.m_id, rankC.m_continuationTurn, rankC.m_value1, () => { Cancel_RankC(); }, false);

        player.BuffMotion(() => 
        {
            player.AddBuff(StatusType.AGI, amount);

            Debug.Log("�u�����v���� ���x" + (amount * 100) + "%�A�b�v");
        });
    }

    /// <summary>
    /// �u�����v����
    /// </summary>
    void Cancel_RankC()
    {
        float amount = (float)rankC.m_value1 / 100.0f;
        player.AddBuff(StatusType.AGI, -amount);

        Debug.Log("�u�����v����");
    }

    /// <summary>
    /// �X���E�@�X�L��
    /// N�^�[���̊ԁA�G�̑��x��V��������
    /// </summary>
    public void RankB()
    {
        // ������Ȃ珈�����Ȃ�
        if(!rankB.m_released) return;

        if (!player.CostMP(rankB.m_cost)) return;

        // ���b�N�I�������G�Ƀf�o�t
        Enemy enemy = mainGameSystem.Target;

        float amount = (float)rankB.m_value1 / 100.0f;

        enemy.AddState(false, rankB.m_id, rankB.m_continuationTurn, rankB.m_value1, () => { Cancel_RankB(enemy); }, false);

        player.BuffMotion(() => 
        {
            enemy.AddDebuff(StatusType.AGI, amount);

            Debug.Log("�u�X���E�v���� �G�̑��x" + (amount * 100) + "%�_�E��");
        });
    }

    /// <summary>
    /// �u�X���E�v����
    /// </summary>
    void Cancel_RankB(Enemy _enemy)
    {
        float amount = (float)rankB.m_value1 / 100.0f;
        _enemy.AddDebuff(StatusType.AGI, -amount);

        Debug.Log("�u�X���E�v����");
    }

    /// <summary>
    /// �čs���@�p�b�V�u
    /// �ʏ�U�����AV%�̊m���ł�����x�U������
    /// </summary>
    public bool RankA()
    {
        // ������Ȃ珈�����Ȃ�
        if(!rankA.m_released) return false;

        // �čs�����ɍX�ɍčs�����Ȃ��悤�ɉ񐔐���
        if (effectAmount_A > 0) return false;

        int result = Random.Range(1, 100);
        if (result <= rankA.m_value1)
        {
            effectAmount_A++;
            Debug.Log("�u�čs���v����");
            return true;
        }

        return false;
    }

    /// <summary>
    /// �X�e�b�v�@�p�b�V�u
    /// ��_�����AV%�̊m���ŉ��
    /// </summary>
    public bool RankS()
    {
        // ������Ȃ珈�����Ȃ�
        if(!rankS.m_released) return false;

        int result = Random.Range(1, 100);
        if (result <= rankS.m_value1)
        {
            Debug.Log("�u�X�e�b�v�v����");
            return true;
        }

        return false;
    }

    /// <summary>
    /// �_���̋Ɓ@�X�L��
    /// �K�E�Z�Q�[�W���}�b�N�X�ɂ���
    /// </summary>
    public  void RankSS()
    {
        // ������Ȃ珈�����Ȃ�
        if(!rankSS.m_released) return;

        if (!player.CostMP(rankSS.m_cost)) return;

        player.BuffMotion(() => 
        {
            player.UpSpecialMoveGuage();

            Debug.Log("�u�_���̋Ɓv����");
        });
    }
}
