using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MP_SpecialTecnique : SpecialTecniqueMethod
{
    List<int> elapsedTurn_C = new();
    bool isActive_C = false;

    List<EnemyBuffTurn> elapsedTurn_S = new();
    bool isActive_S = false;

    int elapsedTurn_SS = 0;
    bool isActive_SS = false;

    public override void GameStart() { }

    public override void PlayerTurnStart() { }

    public override void TurnEnd()
    {
        RankA();
        _RankS();
        _RankSS();

        // �o�߃^�[�����Z
        elapsedTurn_C = TurnPass(elapsedTurn_C);
        elapsedTurn_S = TurnPass(elapsedTurn_S);
        elapsedTurn_SS++;

        Cancel_RankC();
        Cancel_RankS();
        Cancel_RankSS();
    }

    /// <summary>
    /// �I�[���@�X�L��
    /// �U���́E�h��͂���V%�グ��
    /// </summary>
    public void RankC()
    {
        // ������Ȃ珈�����Ȃ�
        //if(!rankC.m_released) return;

        elapsedTurn_C.Add(1);
        isActive_C = true;

        float amount = (float)rankC.m_value1 / 100.0f;
        player.AddBuff(StatusType.ATK, amount);
        player.AddBuff(StatusType.DEF, amount);

        Debug.Log("�u�I�[���v���� �U����/�h��� " + (amount * 100) + "%�A�b�v");
    }

    /// <summary>
    /// �I�[������
    /// </summary>
    void Cancel_RankC()
    {
        if (!isActive_C) return;

        for (int i = 0; i < elapsedTurn_C.Count; i++)
        {
            if (elapsedTurn_C[i] > rankC.m_continuationTurn)
            {
                elapsedTurn_C.Remove(elapsedTurn_C[i]);

                float amount = (float)rankC.m_value1 / 100.0f;
                player.AddBuff(StatusType.ATK, -amount);
                player.AddBuff(StatusType.DEF, -amount);
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
        Debug.Log("�u�I�[���v����");
    }

    /// <summary>
    /// �h���C���@�p�b�V�u
    /// �ʏ�U����MP��V%��
    /// </summary>
    public void RankB()
    {
        // ������Ȃ珈�����Ȃ�
        //if(!rankB.m_released) return;

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
        //if(!rankA.m_released) return;

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
        //if(!rankS.m_released) return;

        int result = Random.Range(1, 100);
        if (result <= rankS.m_value1)
        {
            EnemyBuffTurn e = new();
            e.enemy = _enemy;
            e.elapsedTurn = 1;

            elapsedTurn_S.Add(e);
            isActive_S = true;

            Debug.Log("�u�􂢁v����");
        }
    }

    void _RankS()
    {
        for (int i = 0; i < elapsedTurn_S.Count; i++)
        {
            if (elapsedTurn_S[i].elapsedTurn <= rankS.m_continuationTurn)
            {
                Enemy enemy = elapsedTurn_S[i].enemy;

                float amount = (float)rankS.m_value2 / 100.0f * enemy.HP;
                enemy.Damage((int)amount, true);
            }
        }
    }

    /// <summary>
    /// �􂢉���
    /// </summary>
    void Cancel_RankS()
    {
        if (!isActive_S) return;

        for (int i = 0; i < elapsedTurn_S.Count; i++)
        {
            if (elapsedTurn_S[i].elapsedTurn > rankS.m_continuationTurn)
            {
                elapsedTurn_S.Remove(elapsedTurn_S[i]);

                Debug.Log("�u�􂢁v����");
            }
        }

        for (int i = 0; i < elapsedTurn_S.Count; i++)
        {
            if (elapsedTurn_S[i].elapsedTurn <= rankS.m_continuationTurn)
            {
                return;
            }
        }

        isActive_S = false;
    }

    /// <summary>
    /// ���p�t�̌��E�@�X�L��
    /// N�^�[���̊ԁAMP����ʂ�V���_�E���@�K�E�Z�Q�[�W��1�^�[������W����
    /// </summary>
    public void RankSS()
    {
        // ������Ȃ珈�����Ȃ�
        //if(!rankSS.m_released) return;

        elapsedTurn_SS = 1;
        isActive_SS = true;

        float mpAmount = (float)rankSS.m_value1 / 100.0f;
        player.power_CostMp = (1 - mpAmount);

        Debug.Log("�u���p�t�̌��E�v���� MP����� " + player.power_CostMp + "�{");
    }

    void _RankSS()
    {
        if (!isActive_SS) return;

        if (elapsedTurn_SS <= rankSS.m_continuationTurn)
        {
            float amount = (float)rankSS.m_value2 / 100.0f * player.specialMoveGuageMax;
            player.UpSpecialMoveGuage((int)amount);

            Debug.Log("�u���p�t�̌��E�v���� �K�E�Z�Q�[�W " + (int)amount + "��");
        }
    }

    /// <summary>
    /// ���p�t�̌��E�@����
    /// </summary>
    void Cancel_RankSS()
    {
        if (!isActive_SS) return;

        if (elapsedTurn_SS > rankSS.m_continuationTurn)
        {
            elapsedTurn_SS = 0;

            Debug.Log("�u���p�t�̌��E�v����");
        }
    }
}

