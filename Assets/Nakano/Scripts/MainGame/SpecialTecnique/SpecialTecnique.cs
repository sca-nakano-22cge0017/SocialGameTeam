using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpecialTecnique : MonoBehaviour
{
    // ����Z�\ID
    public int m_id;

    // ����ς݂��ǂ���
    public bool m_released = false;

    // �g�p�A�C�R��
    public Sprite m_illust;

    // �ȉ��̓}�X�^�[�f�[�^�ŕύX����
    // ���O
    private string m_name;

    // �X�L�����ǂ���
    private bool m_isSkill = false;

    // �p���^�[����
    private int m_continuationTurn;

    // ���ʗ�
    private int m_value;

    // ���ʓ��e�i�v���C���[�����j
    private string m_effects;

    public void Setting(string _name, bool _isSkill, int _continuationTurn, int _value, string _effects)
    {
        m_name = _name;
        m_isSkill = _isSkill;
        m_continuationTurn = _continuationTurn;
        m_value = _value;
        m_effects = _effects;
    }
}
