using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ATK_SpecialTecnique : SpecialTecniqueMethod
{
    GameObject[] enemies;

    int elapsedTurn_C = 0;
    bool isActive_C = false;

    int effectAmount_A = 0; // ���݂̌��ʗ�

    public override void GameStart()
    {
        enemies = GameObject.FindGameObjectsWithTag("Enemy");
    }

    public override void TurnEnd()
    {
        // �o�߃^�[�������Z
        elapsedTurn_C++;

        Cancel_RankC();
    }

    /// <summary>
    /// �s�A�X�@�X�L��
    /// N�^�[���̊ԁA�G�̖h�䖳��
    /// </summary>
    public  void RankC()
    {
        // ������Ȃ珈�����Ȃ�
        //if(!rankC.m_released) return;

        elapsedTurn_C = 1;
        isActive_C = true;

        for (int i = 0; i < enemies.Length; i++)
        {
            enemies[i].GetComponent<Enemy>().isIgnoreDeffence = true;
        }

        Debug.Log("�u�s�A�X�v����");
    }

    /// <summary>
    /// �s�A�X����
    /// </summary>
    void Cancel_RankC()
    {
        if (!isActive_C) return;

        if (elapsedTurn_C >= rankC.m_continuationTurn)
        {
            for (int i = 0; i < enemies.Length; i++)
            {
                enemies[i].GetComponent<Enemy>().isIgnoreDeffence = false;
            }

            elapsedTurn_C = 0;
            isActive_C = false;

            Debug.Log("�u�s�A�X�v����");
        }
    }

    /// <summary>
    /// �w���̐w�@�p�b�V�u
    /// HP���������邲�ƂɍU���͂��オ��
    /// </summary>
    public void RankB()
    {
        // ������Ȃ珈�����Ȃ�
        //if(!rankB.m_released) return;

        float lostHp = (float)(player.HP - player.currentHp) / player.HP;
        float amount = ((float)rankB.m_value2 / 100.0f) / ((float)rankB.m_value1 / 100.0f) * lostHp;
        player.AddBuff(StatusType.ATK, amount);

        Debug.Log("�u�w���̐w�v���� �U���� " + (amount * 100) + "% �㏸");
    }

    /// <summary>
    /// �K�[�h�u���C�J�[�@�p�b�V�u
    /// �ʏ�U�����AV%�̊m���œG�̖h��͂�10��������@�ő�80���_�E��
    /// </summary>
    public void RankA(Enemy _enemy)
    {
        // ������Ȃ珈�����Ȃ�
        //if(!rankA.m_released) return;

        int max = 80;
        int amount = 0;
        // ���ʗʍő�l�܂ōs���Ă����珈�����Ȃ�
        if (effectAmount_A >= max) return;

        int result = Random.Range(1, 100);
        if (result <= rankA.m_value1)
        {
            amount = rankA.m_value2;
            if (effectAmount_A + amount <= max)
            {
                effectAmount_A += amount;
                _enemy.AddDebuff(StatusType.DEF, ((float)amount / 100.0f));
            }

            Debug.Log("�u�K�[�h�u���C�J�[�v���� �G�̖h���" + amount + " %�_�E�� ���v" + effectAmount_A + " %�_�E��");
        }
    }

    /// <summary>
    /// �S�g�S��@�p�b�V�u
    /// �K�E�Z��łO�ɍU���͂�V%�㏸������
    /// </summary>
    public  void RankS()
    {
        // ������Ȃ珈�����Ȃ�
        //if(!rankS.m_released) return;

        float amount = (float)rankS.m_value1 / 100.0f;
        player.AddBuff(StatusType.ATK, amount);

        Debug.Log("�u�S�g�S��v���� �U���� " + (amount * 100) + "%�A�b�v");
    }

    /// <summary>
    /// �G�N�X�v���[�W�����@�X�L��
    /// �U����V���̑S�̍U��
    /// </summary>
    public  void RankSS()
    {
        // ������Ȃ珈�����Ȃ�
        //if(!rankSS.m_released) return;

        float amount = (float)rankSS.m_value1 / 100.0f * (float)player.ATK;

        for (int i = 0; i < enemies.Length; i++)
        {
            enemies[i].GetComponent<Enemy>().Damage((int)amount);
        }

        Debug.Log("�u�G�N�X�v���[�W�����v����");
    }
}