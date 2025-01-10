using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DEF_SpecialTecnique : SpecialTecniqueMethod
{
    bool isActive_A = false; // �X�L�����������ǂ���

    float cutPercent = 0; // ���_�̌��\ �_���[�W�J�b�g�� %

    /// <summary>
    /// MP�z���@�p�b�V�u
    /// �h�䎞��MP��V����
    /// �h�䎞�ɏ���
    /// </summary>
    public  void RankC()
    {
        // ������Ȃ珈�����Ȃ�
        if(!rankC.m_released) return;

        float amount = (float)player.MP * ((float)rankC.m_value1 / 100.0f);
        player.HealMP((int)amount);

        Debug.Log("�uMP�z���v���� MP" + amount + " ��");
    }

    /// <summary>
    /// HP�z���@�p�b�V�u
    /// �h�䎞��HP��V����
    /// �h�䎞�ɏ���
    /// </summary>
    public  void RankB()
    {
        // ������Ȃ珈�����Ȃ�
        if(!rankB.m_released) return;

        float amount = (float)player.HP * ((float)rankB.m_value1 / 100.0f);
        player.HealHP((int)amount);

        Debug.Log("�uHP�z���v���� HP" + (int)amount + " ��");
    }

    /// <summary>
    /// ���G�@�X�L��
    /// 1�^�[���̊Ԗ��G���
    /// </summary>
    public  void RankA()
    {
        // ������Ȃ珈�����Ȃ�
        if(!rankA.m_released) return;

        if(!player.CostMP(rankA.m_cost)) return;

        isActive_A = true;

        player.AddState(true, rankA.m_id, rankA.m_continuationTurn, () => { Cancel_RankA(); }, true);

        player.BuffMotion(() => 
        { Debug.Log("�u���G�v����"); });
    }

    /// <summary>
    /// ��_���\�W������
    /// </summary>
    public void _RankA()
    {
        // �X�L���������łȂ���Ώ������Ȃ�
        if (!isActive_A) return;

        player.isInvincible = true;
    }

    /// <summary>
    /// �u���G�v����
    /// </summary>
    void Cancel_RankA()
    {
        player.isInvincible = false;
        isActive_A = false;

        Debug.Log("�u���G�v����");
    }

    /// <summary>
    /// �U�h��́@�p�b�V�u
    /// �h�䎞�G�Ƀ_���[�W��V������
    /// �h���ԂŔ�_�����ɏ���
    /// </summary>
    public void RankS(int _damage, Enemy _enemy)
    {
        // ������Ȃ珈�����Ȃ�
        if(!rankS.m_released) return;

        // �_���[�W�ʌv�Z
        float amount = (float)_damage * ((float)rankS.m_value1 / 100.0f);

        if (_enemy == null || _enemy.gameObject.activeSelf == false) return;
        _enemy.Damage(amount);

        Debug.Log("�u�U�h��́v���� �J�E���^�[�_���[�W " + amount);
    }

    /// <summary>
    /// ���_�̌��\�@�X�L��
    /// </summary>
    public  void RankSS()
    {
        // ������Ȃ珈�����Ȃ�
        //if(!rankSS.m_released) return;

        if (!player.CostMP(rankSS.m_cost)) return;

        cutPercent += (float)rankSS.m_value1;

        player.AddState(true, rankSS.m_id, rankSS.m_continuationTurn, () => { Cancel_RankSS(); }, false);

        player.BuffMotion(() => 
        { Debug.Log("�u���_�̌��\�v����"); });
    }

    /// <summary>
    /// n�^�[���̊ԁA��_���[�W��V���J�b�g
    /// ��_���[�W�ɏ���
    /// </summary>
    /// <param name="_damage">��_����</param>
    /// <returns>�_���[�W�J�b�g��</returns>
    public int _RankSS(int _damage)
    {
        if (cutPercent <= 0) return 0;

        float amount = (float)_damage * (cutPercent / 100.0f);
        Debug.Log("�u���_�̌��\ �_���[�W " + amount + " �J�b�g");
        return (int)amount;
    }

    /// <summary>
    /// �u���_�̌��\�v����
    /// </summary>
    void Cancel_RankSS()
    {
        cutPercent -= (float)rankSS.m_value1;

        Debug.Log("�u���_�̌��\�v����");
    }
}
