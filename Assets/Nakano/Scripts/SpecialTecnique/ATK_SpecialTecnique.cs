using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ATK_SpecialTecnique : SpecialTecniqueMethod
{
    GameObject[] enemies;

    int effectAmount_A = 0; // ���݂̌��ʗ�

    public override void GameStart()
    {
        enemies = GameObject.FindGameObjectsWithTag("Enemy");
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

        player.AddState(true, rankC.m_id, rankC.m_continuationTurn, () => { Cancel_RankC(); }, true);

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
    void Cancel_RankC()
    {
        for (int i = 0; i < enemies.Length; i++)
        {
            enemies[i].GetComponent<Enemy>().isIgnoreDeffence = false;
        }

        Debug.Log("�u�s�A�X�v����");
    }

    /// <summary>
    /// �w���̐w�@�p�b�V�u
    /// HP���������邲�ƂɍU���͂��オ��
    /// </summary>
    public void RankB()
    {
        // ������Ȃ珈�����Ȃ�
        if(!rankB.m_released) return;

        float lostHp = (float)(player.HP - player.currentHp) / player.HP;
        float amount = ((float)rankB.m_value2 / 100.0f) / ((float)rankB.m_value1 / 100.0f) * lostHp;
        player.AddBuff(StatusType.ATK, amount);

        Debug.Log("�u�w���̐w�v���� �U���� " + (amount * 100) + "% �㏸");
    }

    /// <summary>
    /// �K�[�h�u���C�J�[�@�p�b�V�u
    /// �ʏ�U�����AV%�̊m���œG�̖h��͂�W��������@�ő�80���_�E��
    /// </summary>
    public void RankA(Enemy _enemy)
    {
        // ������Ȃ珈�����Ȃ�
        if(!rankA.m_released) return;

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
    public void RankS(System.Action _specialMove)
    {
        // ������Ȃ珈�����Ȃ�
        if (!rankS.m_released)
        {
            _specialMove?.Invoke();
            return;
        }

        float amount = (float)rankS.m_value1 / 100.0f;

        player.AddState(true, rankS.m_id, 1, () => { Cancel_RankS(); }, false);

        player.AddBuff(StatusType.ATK, amount);
        Debug.Log("�u�S�g�S��v���� �U���� " + (amount * 100) + "%�A�b�v");

        _specialMove?.Invoke();
    }

    void Cancel_RankS()
    {
        float amount = (float)rankS.m_value1 / 100.0f;
        player.AddBuff(StatusType.ATK, -amount);

        Debug.Log("�u�S�g�S��v����");
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
        });
    }
}
