using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DEX_SpecialTecnique : SpecialTecniqueMethod
{
    Enemy enemy_RankC = new();
    /// <summary>
    /// �K�[�h�N���b�V���@�X�L��
    /// �G�P�̂ɍU����V%�̍U���@N�^�[���̊ԓG�̖h��͂�W%���Ƃ�
    /// </summary>
    public  void RankC()
    {
        // ������Ȃ珈�����Ȃ�
        if(!rankC.m_released) return;

        if (!player.CostMP(rankC.m_cost)) return;

        // ���b�N�I�������G�ɍU���E�f�o�t
        Enemy enemy = mainGameSystem.Target;
        enemy_RankC = enemy;

        if (enemy == null || enemy.gameObject.activeSelf == false) return;

        float damage = (float)rankC.m_value1 / 100.0f * player.ATK * player.power_Skill;
        float debuff = (float)rankC.m_value2 / 100.0f;

        Debug.Log("�u�K�[�h�N���b�V���v���� �G�̖h��� " + (debuff * 100) + "%�_�E��");

        enemy.AddState(false, rankC.m_id, rankC.m_continuationTurn, rankC.m_value2, () => { Cancel_RankC(); }, false);

        player.AttackMotion(() => 
        {
            enemy.Damage(damage);
            enemy.AddDebuff(StatusType.DEF, debuff, true);
        });
    }

    public void Cancel_RankC()
    {
        float debuff = (float)rankC.m_value2 / 100.0f;
        enemy_RankC.AddDebuff(StatusType.DEF, -debuff, false);

        Debug.Log("�u�K�[�h�N���b�V���v����");
    }

    public void RankC_Restart(Enemy _enemy)
    {
        enemy_RankC = _enemy;

        float debuff = (float)rankC.m_value2 / 100.0f;
        enemy_RankC.AddDebuff(StatusType.DEF, debuff, false);
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
            _enemy.Dead();
            Debug.Log("�u�����̃e�N�j�b�N�v����");
        }
    }

    /// <summary>
    /// �o�[�X�g�@�X�L��
    /// �N���e�B�J���З͂�V���A�b�v
    /// </summary>
    public  void RankS()
    {
        // ������Ȃ珈�����Ȃ�
        if (!rankS.m_released) return;

        if (!player.CostMP(rankS.m_cost)) return;

        player.AddState(true, rankS.m_id, rankS.m_continuationTurn, 0, () => { Cancel_RankS(); }, false);

        float amount = (float)rankS.m_value1 / 100.0f;
        
        player.BuffMotion(() => 
        {
            player.buffCriticalPower += amount;

            Debug.Log("�u�o�[�X�g�v���� ��S���{��" + (amount * 100) + "%�A�b�v");
        });
    }
    
    /// <summary>
    /// �o�[�X�g����
    /// </summary>
    public void Cancel_RankS()
    {
        float amount = (float)rankS.m_value1 / 100.0f;
        player.buffCriticalPower -= amount;

        Debug.Log("�u�o�[�X�g�v����");
    }

    public void RankS_Restart()
    {
        float amount = (float)rankS.m_value1 / 100.0f;
        player.buffCriticalPower += amount;
    }

    /// <summary>
    /// �񑩂��ꂽ�����@�X�L��
    /// N�^�[���̊ԁA�m��N���e�B�J���@�N���e�B�J�����̃_���[�W��V%�グ��
    /// </summary>
    public  void RankSS()
    {
        // ������Ȃ珈�����Ȃ�
        if (!rankSS.m_released) return;
        if (!player.CostMP(rankSS.m_cost)) return;

        float amount = (float)rankS.m_value1 / 100.0f;

        player.AddState(true, rankSS.m_id, rankSS.m_continuationTurn, rankS.m_value1, () => { Cancel_RankSS(); }, false);

        player.BuffMotion(() => 
        {
            player.buffCriticalPower += amount;
            player._criticalProbability = 100;

            Debug.Log("�u�񑩂��ꂽ�����v���� ��S���{��" + (amount * 100) + "%�A�b�v, �N���e�B�J���m��");
        });
    }
    
    /// <summary>
    /// �񑩂��ꂽ�����@����
    /// </summary>
    public void Cancel_RankSS()
    {
        float amount = (float)rankS.m_value1 / 100.0f;

        player.buffCriticalPower -= amount;
        player._criticalProbability = player.criticalProbabilityInitial;

        Debug.Log("�u�񑩂��ꂽ�����v����");
    }

    public void RankSS_Restart()
    {
        float amount = (float)rankS.m_value1 / 100.0f;
        player.buffCriticalPower += amount;
        player._criticalProbability = 100;
    }
}