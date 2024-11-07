using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HP_SpecialTecnique : SpecialTecniqueMethod
{
    bool isRankB_Active = false; // �X�L�����������ǂ���
    int elapsedTurn_B = 0;       // �X�L����������̌o�߃^�[��

    bool isRankS_Active = false;

    int elapsedTurn_SS = 1;

    public override void TurnStart()
    {

    }

    public override void PlayerTurnStart()
    {
        RankS();
    }

    public override void TurnEnd()
    {
        RankA();
        RankSS();

        // �o�߃^�[�������Z
        elapsedTurn_B++;
        elapsedTurn_SS++;

        Cancell_RankB();
    }

    /// <summary>
    /// �N���A�q�[���@�X�L��
    /// ��Ԉُ���񕜁AHP��V����
    /// �{�^���������ɏ���
    /// </summary>
    public override void RankC()
    {
        // ������Ȃ珈�����Ȃ�
        //if(!rankC.m_released) return;

        // �񕜗ʌv�Z
        float amount = player.HP * (rankC.m_value1 / 100.0f);
        player.HealHP((int)amount);

        // �f�o�t����
        player.ResetDebuff();

        Debug.Log("�u�N���A�q�[���v���� HP " + amount + "��");
    }

    /// <summary>
    /// �ɂݕ����@�X�L��
    /// </summary>
    public override void RankB()
    {
        // ������Ȃ珈�����Ȃ�
        //if (!rankB.m_released) return;

        elapsedTurn_B = 1;
        isRankB_Active = true; // �X�L������

        Debug.Log("�u�ɂݕ����v����");
    }

    /// <summary>
    /// ��_���[�W������
    /// n�^�[���@�_���[�W��V����G�ɕԂ� n = rankB.m_continuationTurn
    /// �{�^����������w��^�[���o�߂���܂ŏ���
    /// </summary>
    /// <param name="_damage">�_���[�W��</param>
    /// <param name="_enmey">�Ώۂ̓G</param>
    public void _RankB(int _damage, Enemy _enemy)
    {
        // �X�L���������łȂ���Ώ������Ȃ�
        if (!isRankB_Active) return;

        // �X�L����������̌o�߃^�[�����w��^�[���ȉ��@���@�X�L���������Ȃ�
        if (elapsedTurn_B <= rankB.m_continuationTurn)
        {
            // �J�E���^�[�̃_���[�W�ʎZ�o
            float d = (float)_damage * (float)(rankB.m_value1 / 100.0f);

            // �J�E���^�[
            _enemy.Damage((int)d);
            Debug.Log("�u�ɂݕ����v �J�E���^�[�_���[�W " + d);
        }
    }

    /// <summary>
    /// �u�ɂݕ����v ����
    /// </summary>
    void Cancell_RankB()
    {
        if (elapsedTurn_B >= rankB.m_continuationTurn)
        {
            isRankB_Active = false;
        }
    }

    /// <summary>
    /// �I�[�g�q�[���@�p�b�V�u
    /// ���^�[��HP��V����
    /// ���^�[���I�����ɌĂ�
    /// </summary>
    public override void RankA()
    {
        // ������Ȃ珈�����Ȃ�
        //if (!rankA.m_released) return;

        // �񕜗ʌv�Z
        float amount = (float)player.HP * (float)(rankA.m_value1 / 100.0f);
        player.HealHP((int)amount);

        Debug.Log("�u�I�[�g�q�[���v���� HP " + amount + "��");
    }

    /// <summary>
    /// �s�|�̍\���@�p�b�V�u
    /// �̗͂�V���ȏ�̂Ƃ��A�U����W%�A�b�v
    /// ���^�[�� �v���C���[�̍s��������/����
    /// </summary>
    public override void RankS()
    {
        // ������Ȃ珈�����Ȃ�
        //if (!rankS.m_released) return;

        float hpPer = (float)player.currentHp / (float)player.HP * 100.0f;
        float amount = (float)rankS.m_value2 / 100;

        // HP���w��l�ȉ��Ȃ�
        if (hpPer >= rankS.m_value1)
        {
            // �o�t���|�����Ă��Ȃ��ꍇ�̂݃o�t���|����
            if (!isRankS_Active)
            {
                player.AddBuff(StatusType.ATK, amount);
                isRankS_Active = true;
                Debug.Log("�u�s�|�̍\���v���� �U���� " + amount + "�㏸");
            }
        }
        else
        {
            // �o�t���|�����Ă���ꍇ�A�o�t�𖳂���
            if (isRankS_Active)
            {
                player.AddBuff(StatusType.ATK, -amount);
                isRankS_Active = false;
            }
        }
    }

    /// <summary>
    /// ���_�̉���@�p�b�V�u
    /// n�^�[������HP��V���񕜁@n = rankSS.m_continuationTurn
    /// ���^�[������/����
    /// </summary>
    public override void RankSS()
    {
        // ������Ȃ珈�����Ȃ�
        //if (!rankSS.m_released) return;

        // �w��^�[���o�߂������
        if (elapsedTurn_SS >= rankSS.m_continuationTurn)
        {
            float heal = (float)player.HP * (float)(rankSS.m_value1 / 100.0f);
            player.HealHP((int)heal);
            elapsedTurn_SS = 0;

            Debug.Log("�u���_�̉���v���� HP " + heal + " ��");
        }
    }
}
