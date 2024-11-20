using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AGI_SpecialTecnique : SpecialTecniqueMethod
{
    bool isActive_C = false;
    List<int> elapsedTurn_C = new();

    bool isActive_B = false;
    List<EnemyBuffTurn> elapsedTurn_B = new();

    int effectAmount_A = 0;

    public override void TurnEnd()
    {
        // �o�߃^�[�������Z
        elapsedTurn_C = TurnPass(elapsedTurn_C);
        elapsedTurn_B = TurnPass(elapsedTurn_B);
        effectAmount_A = 0;

        Cancel_RankC();
        Cancel_RankB();
    }

    /// <summary>
    /// �����@�X�L��
    /// N�^�[���̊ԁA���x��V%�グ��
    /// </summary>
    public  void RankC()
    {
        // ������Ȃ珈�����Ȃ�
        //if(!rankC.m_released) return;

        elapsedTurn_C.Add(1);
        isActive_C = true;

        float amount = (float)rankC.m_value1 / 100.0f;
        player.AddBuff(StatusType.AGI, amount);

        Debug.Log("�u�����v���� ���x" + (amount * 100) + "%�A�b�v");

        player.BuffMotion();
    }

    /// <summary>
    /// �u�����v����
    /// </summary>
    void Cancel_RankC()
    {
        for (int i = 0; i < elapsedTurn_C.Count; i++)
        {
            if (elapsedTurn_C[i] > rankC.m_continuationTurn)
            {
                elapsedTurn_C.Remove(elapsedTurn_C[i]);

                float amount = (float)rankC.m_value1 / 100.0f;
                player.AddBuff(StatusType.AGI, -amount);

                Debug.Log("�u�����v����");
            }
        }

        for (int i = 0; i < elapsedTurn_C.Count; i++)
        {
            if (elapsedTurn_C[i] <= rankC.m_continuationTurn)
            {
                return;
            }
        }

        isActive_C = false;
    }

    /// <summary>
    /// �X���E�@�X�L��
    /// N�^�[���̊ԁA�G�̑��x��V��������
    /// </summary>
    public void RankB()
    {
        // ������Ȃ珈�����Ȃ�
        //if(!rankB.m_released) return;

        // Todo ���b�N�I�������G�Ƀf�o�t
        Enemy enemy = new();

        EnemyBuffTurn e = new();
        e.enemy = enemy;
        e.elapsedTurn = 1;

        elapsedTurn_B.Add(e);
        isActive_B = true;

        float amount = (float)rankB.m_value1 / 100.0f;
        enemy.AddDebuff(StatusType.AGI, amount);

        Debug.Log("�u�X���E�v���� �G�̑��x" + (amount * 100) + "%�_�E��");

        player.BuffMotion();
    }

    /// <summary>
    /// �u�X���E�v����
    /// </summary>
    void Cancel_RankB()
    {
        for (int i = 0; i < elapsedTurn_B.Count; i++)
        {
            if (elapsedTurn_B[i].elapsedTurn > rankB.m_continuationTurn)
            {
                float amount = (float)rankB.m_value1 / 100.0f;
                elapsedTurn_B[i].enemy.AddDebuff(StatusType.AGI, -amount);

                elapsedTurn_B.Remove(elapsedTurn_B[i]);

                Debug.Log("�u�X���E�v����");
            }
        }

        for (int i = 0; i < elapsedTurn_B.Count; i++)
        {
            if (elapsedTurn_B[i].elapsedTurn <= rankB.m_continuationTurn)
            {
                return;
            }
        }

        isActive_B = false;
    }

    /// <summary>
    /// �čs���@�p�b�V�u
    /// �ʏ�U�����AV%�̊m���ł�����x�U������
    /// </summary>
    public bool RankA()
    {
        // ������Ȃ珈�����Ȃ�
        //if(!rankA.m_released) return false;

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
        //if(!rankS.m_released) return false;

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
        //if(!rankSS.m_released) return;

        player.UpSpecialMoveGuage();

        Debug.Log("�u�_���̋Ɓv����");

        player.BuffMotion();
    }
}
