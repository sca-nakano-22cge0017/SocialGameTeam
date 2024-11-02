using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "SpecialTecnique")]
public class SpecialTecnique : ScriptableObject
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

    // �^�C�v
    private int m_type;

    // �p���^�[����
    private int m_continuationTurn;

    // ���ʗ�
    private int m_value1;
    private int m_value2;

    // ���ʓ��e�i�v���C���[�����j
    private string m_effects;

    public void Setting(string _name, bool _isSkill, int _type, int _continuationTurn, int _value1, int _value2, string _effects)
    {
        m_name = _name;
        m_isSkill = _isSkill;
        m_type = _type;
        m_continuationTurn = _continuationTurn;
        m_value1 = _value1;
        m_value2 = _value2;
        
        if (_effects.Contains("V"))
        {
            _effects.Replace("V", m_value1.ToString());
        }
        if (_effects.Contains("W"))
        {
            _effects.Replace("W", m_value2.ToString());
        }

        if (_effects.Contains("{"))
        {
            int start = _effects.IndexOf('{');
            int end = _effects.IndexOf('}');
            _effects.Remove(start, (end - start + 1));
        }

        m_effects = _effects;
    }
}
