using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HP_SpecialTecnique : MonoBehaviour
{
    [SerializeField] SpecialTecnique rankC;
    [SerializeField] SpecialTecnique rankB;
    [SerializeField] SpecialTecnique rankA;
    [SerializeField] SpecialTecnique rankS;
    [SerializeField] SpecialTecnique rankSS;

    void Start()
    {
        
    }

    void Update()
    {
        
    }

    /// <summary>
    /// �N���A�q�[���@�X�L��
    /// </summary>
    public void ClearHeal()
    {
        // ������Ȃ珈�����Ȃ�
        if(!rankC.m_released) return;
    }

    /// <summary>
    /// �ɂݕ����@�X�L��
    /// </summary>
    public void LoseLose()
    {
        // ������Ȃ珈�����Ȃ�
        if (!rankB.m_released) return;
    }

    /// <summary>
    /// �I�[�g�q�[���@�펞����
    /// </summary>
    public void AutoHeal()
    {
        // ������Ȃ珈�����Ȃ�
        if (!rankA.m_released) return;
    }

    /// <summary>
    /// �s�|�̍\���@�펞����
    /// </summary>
    public void Undefeated()
    {
        // ������Ȃ珈�����Ȃ�
        if (!rankS.m_released) return;
    }

    /// <summary>
    /// ���_�̉���@�펞����
    /// </summary>
    public void GoddessBlessing()
    {
        // ������Ȃ珈�����Ȃ�
        if (!rankSS.m_released) return;
    }
}
