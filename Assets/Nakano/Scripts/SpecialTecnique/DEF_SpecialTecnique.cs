using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DEF_SpecialTecnique : SpecialTecniqueMethod
{
    bool isActive_A = false; // �X�L�����������ǂ���
    int elapsedTurn_A = 0;       // �X�L����������̌o�߃^�[��

    bool isActive_SS = false;     // �X�L�����������ǂ���
    List<int> elapsedTurn_SS = new(); // �X�L����������̌o�߃^�[��

    public override void TurnEnd()
    {
        // �o�߃^�[�������Z
        elapsedTurn_A++;
        elapsedTurn_SS = TurnPass(elapsedTurn_SS);

        Cancel_RankA();
        Cancel_RankSS();
    }

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
        //if(!rankA.m_released) return;

        elapsedTurn_A = 1;
        isActive_A = true;

        Debug.Log("�u���G�v����");

        player.BuffMotion();
    }

    /// <summary>
    /// ��_���\�W������
    /// </summary>
    public void _RankA()
    {
        // �X�L���������łȂ���Ώ������Ȃ�
        if (!isActive_A) return;

        // �X�L����������̌o�߃^�[�����w��^�[���ȉ��@���@�X�L���������Ȃ�
        if (elapsedTurn_A <= rankA.m_continuationTurn)
        {
            player.isInvincible = true;
        }
    }

    /// <summary>
    /// �u���G�v����
    /// </summary>
    void Cancel_RankA()
    {
        if (elapsedTurn_A > rankA.m_continuationTurn)
        {
            player.isInvincible = false;
        }
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
    /// �d�ˊ|����
    /// </summary>
    public  void RankSS()
    {
        // ������Ȃ珈�����Ȃ�
        if(!rankSS.m_released) return;

        elapsedTurn_SS.Add(1);
        isActive_SS = true;

        Debug.Log("�u���_�̌��\�v����");

        player.BuffMotion();
    }

    /// <summary>
    /// n�^�[���̊ԁA��_���[�W��V���J�b�g
    /// ��_���[�W�ɏ���
    /// </summary>
    /// <param name="_damage">��_����</param>
    /// <returns>�_���[�W�J�b�g��</returns>
    public int _RankSS(int _damage)
    {
        // �X�L���������łȂ���Ώ������Ȃ�
        if (!isActive_SS) return 0;

        float cutPercent = 0; // �_���[�W�J�b�g�� %

        for (int i = 0; i < elapsedTurn_SS.Count; i++)
        {
            // �X�L����������̌o�߃^�[�����w��^�[���ȉ��@���@�X�L���������Ȃ�
            if (elapsedTurn_SS[i] <= rankSS.m_continuationTurn)
            {
                cutPercent += (float)rankSS.m_value1;
            }
        }

        float amount = (float)_damage * (cutPercent / 100.0f);
        Debug.Log("�u���_�̌��\ �_���[�W " + amount + " �J�b�g");
        return (int)amount;
    }

    /// <summary>
    /// �u���_�̌��\�v����
    /// </summary>
    void Cancel_RankSS()
    {
        if (!isActive_SS) return;

        for (int i = 0; i < elapsedTurn_SS.Count; i++)
        {
            if (elapsedTurn_SS[i] > rankSS.m_continuationTurn)
            {
                elapsedTurn_SS.Remove(elapsedTurn_SS[i]);
            }
        }

        for (int i = 0; i < elapsedTurn_SS.Count; i++)
        {
            if (elapsedTurn_SS[i] <= rankSS.m_continuationTurn)
            {
                return;
            }
        }

        isActive_SS = false;
        Debug.Log("�u���_�̌��\�v����");
    }
}
