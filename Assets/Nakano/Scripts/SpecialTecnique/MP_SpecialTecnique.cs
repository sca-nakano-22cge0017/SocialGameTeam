using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MP_SpecialTecnique : SpecialTecniqueMethod
{
    public override void GameStart() { }

    public override void TurnStart() { }

    public override void PlayerTurnStart() { }

    public override void TurnEnd()
    {
    }

    /// <summary>
    /// �I�[���@�X�L��
    /// �U���͂Ɩh��͂�V���グ��
    /// </summary>
    public void RankC()
    {
        // ������Ȃ珈�����Ȃ�
        //if(!rankC.m_released) return;
    }

    /// <summary>
    /// �h���C���@�p�b�V�u
    /// �ʏ�U����MP��V%��
    /// </summary>
    public void RankB()
    {
        // ������Ȃ珈�����Ȃ�
        //if(!rankB.m_released) return;
    }

    /// <summary>
    /// �����̓����@�p�b�V�u
    /// ���^�[��MP��V����
    /// </summary>
    public void RankA()
    {
        // ������Ȃ珈�����Ȃ�
        //if(!rankA.m_released) return;

    }

    /// <summary>
    /// �􂢁@�p�b�V�u
    /// ��_�����AV���̊m���œG�Ɏ􂢏�Ԃ�t�^
    /// �􂢏�ԁF���^�[��HP����
    /// </summary>
    public void RankS()
    {
        // ������Ȃ珈�����Ȃ�
        //if(!rankS.m_released) return;
    }

    /// <summary>
    /// ���p�t�̌��E�@�X�L��
    /// N�^�[���̊ԁAMP����ʂ�V���_�E���@�K�E�Z�Q�[�W��1�^�[������W����
    /// </summary>
    public void RankSS()
    {
        // ������Ȃ珈�����Ȃ�
        //if(!rankSS.m_released) return;

    }
}

