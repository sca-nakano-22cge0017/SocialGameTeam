using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// �����X�L��
/// </summary>
public class InitialSkill : SpecialTecniqueMethod
{
    [SerializeField] private SpecialTecnique heal;
    [SerializeField] private SpecialTecnique fire;

    [SerializeField] private Enemy enemy;

    public void Heal()
    {
        float amount = (float)player.HP * ((float)heal.m_value1 / 100.0f);
        player.HealHP((int)amount);

        Debug.Log("�u�q�[���v���� HP " + amount + " ��");

        player.BuffMotion();
    }

    public void Fire()
    {
        // Todo ���b�N�I�������G���擾

        if (enemy == null || enemy.gameObject.activeSelf == false) return;

        Debug.Log("�u�t�@�C�A�v����");

        float amount = (float)player.ATK * ((float)fire.m_value1 / 100.0f);
        
        player.AttackMotion();
        enemy.Damage(amount);
    }
}
