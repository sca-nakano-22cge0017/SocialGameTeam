using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HP_SpecialTecnique : SpecialTecniqueMethod
{
    bool isActive_B = false; // �X�L�����������ǂ���

    bool isActive_S = false;

    int elapsedTurn_SS = 1;

    public override void GameStart()
    {
        RankS(); // �s�|�̍\��
    }

    public override void TurnEnd()
    {
        RankA();
        RankSS();

        elapsedTurn_SS++;
    }

    /// <summary>
    /// �N���A�q�[���@�X�L��
    /// ��Ԉُ���񕜁AHP��V����
    /// �{�^���������ɏ���
    /// </summary>
    public void RankC()
    {
        // ������Ȃ珈�����Ȃ�
        if(!rankC.m_released) return;

        if (!player.CostMP(rankC.m_cost)) return;

        // �񕜗ʌv�Z
        float amount = player.HP * (rankC.m_value1 / 100.0f);
        
        player.BuffMotion(() => 
        {
            player.HealHP((int)amount);

            // �f�o�t����
            player.ResetDebuff();

            Debug.Log("�u�N���A�q�[���v���� HP " + amount + "��");
        });
    }

    /// <summary>
    /// �ɂݕ����@�X�L��
    /// </summary>
    public  void RankB()
    {
        // ������Ȃ珈�����Ȃ�
        if (!rankB.m_released) return;

        if (!player.CostMP(rankB.m_cost)) return;
        
        isActive_B = true;

        player.AddState(true, rankB.m_id, rankB.m_continuationTurn, 0, () => { Cancel_RankB(); }, true);

        player.BuffMotion(() => { Debug.Log("�u�ɂݕ����v����"); });
    }

    /// <summary>
    /// ��_���[�W������
    /// �{�^����������w��^�[���o�߂���܂ŏ���
    /// </summary>
    /// <param name="_damage">�_���[�W��</param>
    /// <param name="_enmey">�Ώۂ̓G</param>
    public void _RankB(int _damage, Enemy _enemy)
    {
        if (!isActive_B) return;

        // �J�E���^�[�̃_���[�W�ʎZ�o
        float d = (float)_damage * (float)(rankB.m_value1 / 100.0f);

        // �h�䖳���J�E���^�[
        _enemy.Damage((int)d, true);
        Debug.Log("�u�ɂݕ����v �J�E���^�[�_���[�W " + d);
    }

    /// <summary>
    /// �u�ɂݕ����v ����
    /// </summary>
    public void Cancel_RankB()
    {
        isActive_B = false;

        Debug.Log("�ɂݕ����@����");
    }

    /// <summary>
    /// �ɂݕ��� �o�g���ĊJ���̏���
    /// </summary>
    public void RankB_Restart()
    {
        isActive_B = true;
    }

    /// <summary>
    /// �I�[�g�q�[���@�p�b�V�u
    /// ���^�[��HP��V����
    /// ���^�[���I�����ɌĂ�
    /// </summary>
    public  void RankA()
    {
        // ������Ȃ珈�����Ȃ�
        if (!rankA.m_released) return;

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
    public  void RankS()
    {
        // ������Ȃ珈�����Ȃ�
        if (!rankS.m_released) return;

        float hpPer = (float)player.currentHp / (float)player.HP * 100.0f;
        float amount = (float)rankS.m_value2 / 100;

        // HP���w��l�ȏ�Ȃ�
        if (hpPer >= rankS.m_value1)
        {
            // �o�t���|�����Ă��Ȃ��ꍇ�̂݃o�t���|����
            if (!isActive_S)
            {
                player.AddBuff(StatusType.ATK, amount, true);
                isActive_S = true;
                Debug.Log("�u�s�|�̍\���v���� �U���� " + amount + "�㏸");

                player.AddState(true, rankS.m_id, 999, amount, () => 
                {
                    Cancel_RankS();
                }, true);
            }
        }

        else
        {
            // �o�t���|�����Ă���ꍇ�A�o�t�𖳂���
            if (isActive_S)
            {
                Cancel_RankS();

                player.RemoveState(rankS.m_id);
            }
        }
    }

    void Cancel_RankS()
    {
        float amount = (float)rankS.m_value2 / 100;
        isActive_S = false;
        player.AddBuff(StatusType.ATK, -amount, false);
    }

    /// <summary>
    /// ���_�̉���@�p�b�V�u
    /// n�^�[������HP��V����
    /// ���^�[������/����
    /// </summary>
    public  void RankSS()
    {
        // ������Ȃ珈�����Ȃ�
        if (!rankSS.m_released) return;

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
