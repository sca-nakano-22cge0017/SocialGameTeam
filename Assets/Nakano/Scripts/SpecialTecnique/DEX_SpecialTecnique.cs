using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DEX_SpecialTecnique : SpecialTecniqueMethod
{
    bool isActive_C = false;
    List<EnemyBuffTurn> elapsedTurn_C = new();

    bool isActive_S = false;
    int elapsedTurn_S = 0;

    bool isActive_SS = false;
    int elapsedTurn_SS = 0;

    public override void GameStart() { }

    public override void TurnStart() { }

    public override void PlayerTurnStart() { }

    public override void TurnEnd()
    {
        // �o�߃^�[�����Z
        if (isActive_C) elapsedTurn_C = TurnPass(elapsedTurn_C);
        if (isActive_S) elapsedTurn_S++;
        if (isActive_SS) elapsedTurn_SS++;

        Cancel_RankC();
        Cancel_RankS();
        Cancel_RankSS();
    }

    /// <summary>
    /// �K�[�h�N���b�V���@�X�L��
    /// �G�P�̂ɍU����V%�̍U���@�G�̖h��͂�W%���Ƃ�
    /// </summary>
    public  void RankC()
    {
        // ������Ȃ珈�����Ȃ�
        if(!rankC.m_released) return;

        // ���b�N�I�������G�ɍU���E�f�o�t
        Enemy enemy = mainGameSystem.Target;

        EnemyBuffTurn e = new();
        e.enemy = enemy;
        e.elapsedTurn = 1;

        elapsedTurn_C.Add(e);
        isActive_C = true;

        if (enemy == null || enemy.gameObject.activeSelf == false) return;

        float damage = (float)rankC.m_value1 / 100.0f * player.ATK * player.power_Skill;
        float debuff = (float)rankC.m_value2 / 100.0f;

        Debug.Log("�u�K�[�h�N���b�V���v���� �G�̖h��� " + (debuff * 100) + "%�_�E��");

        player.AttackMotion();

        enemy.Damage(damage);
        enemy.AddDebuff(StatusType.DEF, debuff);
    }

    void Cancel_RankC()
    {
        if (!isActive_C) return;

        for (int i = 0; i < elapsedTurn_C.Count; i++)
        {
            if (elapsedTurn_C[i].elapsedTurn > rankC.m_continuationTurn)
            {
                elapsedTurn_C.Remove(elapsedTurn_C[i]);

                Debug.Log("�u�K�[�h�N���b�V���v����");
            }
        }

        for (int i = 0; i < elapsedTurn_C.Count; i++)
        {
            if (elapsedTurn_C[i].elapsedTurn <= rankC.m_continuationTurn)
            {
                return;
            }
        }

        isActive_C = false;
    }

    /// <summary>
    /// �����̓��@�p�b�V�u
    /// �Ⴆ�郉���N�|�C���gV%�A�b�v
    /// </summary>
    public float RankB()
    {
        // ������Ȃ珈�����Ȃ�
        if(!rankB.m_released) return 0;

        float amount = (float)rankB.m_value1 / 100.0f;
        Debug.Log("�u�����̓��v���� �h���b�v��" + (amount * 100) + "%�A�b�v");

        return amount;
    }

    /// <summary>
    /// �����̃e�N�j�b�N�@�p�b�V�u
    /// �U�����AV%�̊m���œG�𑦎�������
    /// </summary>
    public  void RankA(Enemy _enemy)
    {
        // ������Ȃ珈�����Ȃ�
        if(!rankA.m_released) return;

        int result = Random.Range(1, 100);
        if (result <= rankA.m_value1)
        {
            Debug.Log("�u�����̃e�N�j�b�N�v����");
            _enemy.Dead();
        }
    }

    /// <summary>
    /// �o�[�X�g�@�X�L��
    /// �N���e�B�J���З͂�V���A�b�v
    /// </summary>
    public  void RankS()
    {
        // ������Ȃ珈�����Ȃ�
        if(!rankS.m_released) return;

        elapsedTurn_S = 1;
        isActive_S = true;

        float amount = (float)rankS.m_value1 / 100.0f;
        player.buffCriticalPower = amount;

        Debug.Log("�u�o�[�X�g�v���� ��S���{��" + (amount * 100) + "%�A�b�v");

        player.BuffMotion();
    }
    
    /// <summary>
    /// �o�[�X�g����
    /// </summary>
    void Cancel_RankS()
    {
        if (!isActive_S) return;

        if (elapsedTurn_S > rankS.m_continuationTurn)
        {
            elapsedTurn_S = 0;
            isActive_S = false;

            player.buffCriticalPower = 0;

            Debug.Log("�u�o�[�X�g�v����");
        }
    }

    /// <summary>
    /// �񑩂��ꂽ�����@�X�L��
    /// N�^�[���̊ԁA�m��N���e�B�J���@�N���e�B�J�����̃_���[�W��V%�グ��
    /// </summary>
    public  void RankSS()
    {
        // ������Ȃ珈�����Ȃ�
        if(!rankSS.m_released) return;

        elapsedTurn_SS = 1;
        isActive_SS = true;

        float amount = (float)rankS.m_value1 / 100.0f;
        player.buffCriticalPower = amount;

        player._criticalProbability = 100;

        Debug.Log("�u�񑩂��ꂽ�����v���� ��S���{��" + (amount * 100) + "%�A�b�v, �N���e�B�J���m��");

        player.BuffMotion();
    }
    
    /// <summary>
    /// �񑩂��ꂽ�����@����
    /// </summary>
    void Cancel_RankSS()
    {
        if (!isActive_SS) return;

        if (elapsedTurn_SS > rankSS.m_continuationTurn)
        {
            elapsedTurn_SS = 0;
            isActive_SS = false;

            player.buffCriticalPower = 0;
            player._criticalProbability = player.criticalProbabilityInitial;

            Debug.Log("�u�񑩂��ꂽ�����v����");
        }
    }
}