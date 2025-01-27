using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ATK_SpecialTecnique : SpecialTecniqueMethod
{
    [SerializeField] Animator explosionEffect;

    GameObject[] enemies;

    int effectAmount_A = 0; // ���݂̌��ʗ�

    public override void GameStart()
    {
        enemies = GameObject.FindGameObjectsWithTag("Enemy");

        RankB();
    }

    /// <summary>
    /// �s�A�X�@�X�L��
    /// N�^�[���̊ԁA�G�̖h�䖳��
    /// </summary>
    public  void RankC()
    {
        // ������Ȃ珈�����Ȃ�
        if (!rankC.m_released) return;

        if (!player.CostMP(rankC.m_cost)) return;

        player.AddState(true, rankC.m_id, rankC.m_continuationTurn, 0, () => { Cancel_RankC(); }, true);

        player.BuffMotion(() => 
        {
            for (int i = 0; i < enemies.Length; i++)
            {
                // �S�Ă̓G��h�䖳����ԂɕύX
                enemies[i].GetComponent<Enemy>().isIgnoreDeffence = true;
            }

            Debug.Log("�u�s�A�X�v����");
        });
    }

    /// <summary>
    /// �s�A�X����
    /// </summary>
    public void Cancel_RankC()
    {
        for (int i = 0; i < enemies.Length; i++)
        {
            enemies[i].GetComponent<Enemy>().isIgnoreDeffence = false;
        }

        Debug.Log("�u�s�A�X�v����");
    }

    public void RankC_Restart()
    {
        for (int i = 0; i < enemies.Length; i++)
        {
            // �S�Ă̓G��h�䖳����ԂɕύX
            enemies[i].GetComponent<Enemy>().isIgnoreDeffence = true;
        }
    }

    float lastAmount = 0;
    float lastLostHp = 0;
    /// <summary>
    /// �w���̐w�@�p�b�V�u
    /// HP���������邲�ƂɍU���͂��オ��
    /// </summary>
    public void RankB()
    {
        // ������Ȃ珈�����Ȃ�
        if(!rankB.m_released) return;

        float lostHp = (float)(player.HP - player.currentHp) / player.HP * 100.0f; // ������HP�̊���(%)

        player.AddBuff(StatusType.ATK, -lastAmount, false);
        player.RemoveState(rankB.m_id);

        int value = (int)(lostHp / rankB.m_value1);
        float amount = rankB.m_value2 / 100.0f * value;

        if (lostHp == 0 || value == 0)
        {
            lastAmount = 0;
            lastLostHp = 0;
            return;
        }

        // ���񎸂���HP���O�񎸂���HP�ʂ�菭�Ȃ��Ƃ����񕜂����Ƃ�
        if (lostHp <= lastLostHp)
        {
            // ���o�Ȃ�
            player.AddBuff(StatusType.ATK, amount, false);
        }
        else player.AddBuff(StatusType.ATK, amount, true);
        player.AddState(true, rankB.m_id, 999, (amount * 100.0f), null, true);

        lastAmount = amount;
        lastLostHp = lostHp;

        Debug.Log("�u�w���̐w�v���� �U���� " + (amount * 100) + "% �㏸");
    }

    Enemy enemy_RankA = new();
    /// <summary>
    /// �K�[�h�u���C�J�[�@�p�b�V�u
    /// �ʏ�U�����AV%�̊m���œG�̖h��͂�W��������@�ő�80���_�E��
    /// </summary>
    public void RankA(Enemy _enemy)
    {
        // ������Ȃ珈�����Ȃ�
        if(!rankA.m_released) return;

        enemy_RankA = _enemy;
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
                _enemy.AddDebuff(StatusType.DEF, ((float)amount / 100.0f), true);

                _enemy.AddState(false, rankA.m_id, rankA.m_continuationTurn, amount, () => { Cancel_RankA(); }, false);
            }

            Debug.Log("�u�K�[�h�u���C�J�[�v���� �G�̖h���" + amount + " %�_�E�� ���v" + effectAmount_A + " %�_�E��");
        }
    }

    public void Cancel_RankA()
    {
        float amount = rankA.m_value2 / 100.0f;
        enemy_RankA.AddDebuff(StatusType.DEF, -amount, false);

        Debug.Log("�u�K�[�h�u���C�J�[�v����");
    }

    public void RankA_Restart(Enemy _enemy)
    {
        int amount = rankA.m_value2;
        effectAmount_A += amount;
        _enemy.AddDebuff(StatusType.DEF, ((float)amount / 100.0f), false);

        enemy_RankA = _enemy;
    }

    /// <summary>
    /// �S�g�S��@�p�b�V�u
    /// �K�E�Z��łO�ɍU���͂�V%�㏸������
    /// </summary>
    public void RankS(System.Action _specialMove)
    {
        // ������Ȃ珈�����Ȃ�
        if (!rankS.m_released)
        {
            _specialMove?.Invoke();
            return;
        }

        float amount = (float)rankS.m_value1 / 100.0f;

        player.AddState(true, rankS.m_id, 1, rankS.m_value1, () => { Cancel_RankS(); }, false);

        player.AddBuff(StatusType.ATK, amount, true);
        Debug.Log("�u�S�g�S��v���� �U���� " + (amount * 100) + "%�A�b�v");

        _specialMove?.Invoke();
    }

    public void Cancel_RankS()
    {
        float amount = (float)rankS.m_value1 / 100.0f;
        player.AddBuff(StatusType.ATK, -amount, false);

        Debug.Log("�u�S�g�S��v����");
    }

    public void RankS_Restart()
    {
        float amount = (float)rankS.m_value1 / 100.0f;
        player.AddBuff(StatusType.ATK, amount, false);
    }

    /// <summary>
    /// �G�N�X�v���[�W�����@�X�L��
    /// �U����V���̑S�̍U��
    /// </summary>
    public  void RankSS()
    {
        // ������Ȃ珈�����Ȃ�
        if(!rankSS.m_released) return;

        if (!player.CostMP(rankSS.m_cost)) return;

        Debug.Log("�u�G�N�X�v���[�W�����v����");

        // ��S���I
        var cri = player.CriticalLottery();

        float amount = (float)rankSS.m_value1 / 100.0f * (float)player.ATK * player.power_Skill * player.critical;
        
        player.AttackMotion(() => 
        {
            for (int i = 0; i < enemies.Length; i++)
            {
                if (enemies[i] == null || enemies[i].activeSelf == false) continue;

                var ene = enemies[i].GetComponent<Enemy>();
                ene.Damage(amount);
                if (cri) ene.CriticalDamage();
            }

            explosionEffect.SetTrigger("Play");
        });
    }
}
