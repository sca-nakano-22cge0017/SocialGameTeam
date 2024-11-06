using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HP_SpecialTecnique : MonoBehaviour, SpecialTecniqueMethod
{
    [SerializeField] PlayerData player;
    [SerializeField] BattleSystem battleSystem;

    [SerializeField] SpecialTecnique rankC;
    [SerializeField] SpecialTecnique rankB;
    [SerializeField] SpecialTecnique rankA;
    [SerializeField] SpecialTecnique rankS;
    [SerializeField] SpecialTecnique rankSS;

    void Awake()
    {
        
    }

    /// <summary>
    /// �o�߃^�[�������閈�ɌĂяo��
    /// </summary>
    public void Turn()
    {
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

        // �񕜗ʌv�Z
        float amount = player.HP * (rankC.m_value1 / 100);
        player.HealHP((int)amount);

        // �f�o�t����
        player.ResetDebuff();
    }

    /// <summary>
    /// �ɂݕ����@�X�L��
    /// ���^�[���@�_���[�W��V����G�ɕԂ�
    /// �{�^����������w��^�[���o�߂���܂ŏ���
    /// </summary>
    public void RankB()
    {
        // ������Ȃ珈�����Ȃ�
        if (!rankB.m_released) return;
    }

    /// <summary>
    /// �I�[�g�q�[���@�p�b�V�u
    /// ���^�[��HP��V����
    /// ���^�[���I�����ɌĂ�
    /// </summary>
    public void RankA()
    {
        // ������Ȃ珈�����Ȃ�
        if (!rankA.m_released) return;

        // �񕜗ʌv�Z
        float amount = player.HP * (rankA.m_value1 / 100);
        player.HealHP((int)amount);
    }

    bool isAtkUp_S = false;

    /// <summary>
    /// �s�|�̍\���@�p�b�V�u
    /// �̗͂�V���ȏ�̂Ƃ��A�U����W%�A�b�v
    /// ���^�[�� �v���C���[�̍s��������/����
    /// </summary>
    public void RankS()
    {
        // ������Ȃ珈�����Ȃ�
        if (!rankS.m_released) return;

        float hpPer = player.currentHp / player.HP * 100;
        float amount = rankS.m_value1 / 100;

        // HP���w��l�ȉ��Ȃ�
        if (hpPer >= rankS.m_value1)
        {
            // �o�t���|�����Ă��Ȃ��ꍇ�̂݃o�t���|����
            if (!isAtkUp_S)
            {
                isAtkUp_S = true;
                player.AddBuff(StatusType.ATK, amount);
            }
        }
        else
        {
            // �o�t���|�����Ă���ꍇ�A�o�t�𖳂���
            if (isAtkUp_S)
            {
                player.AddBuff(StatusType.ATK, -amount);
                isAtkUp_S = false;
            }
        }
    }

    /// <summary>
    /// ���_�̉���@�p�b�V�u
    /// 3�^�[������HP��V����
    /// ���^�[������/����
    /// </summary>
    public void RankSS()
    {
        // ������Ȃ珈�����Ȃ�
        if (!rankSS.m_released) return;
    }
}
