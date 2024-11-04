using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "SpecialTecnique")]
public class SpecialTecnique : ScriptableObject
{
    // ����Z�\ID
    public int m_id;

    // ����ς݂��ǂ���
    [HideInInspector] public bool m_released = false;

    // �g�p�A�C�R��
    public Sprite m_illust;

    // �ȉ��̓}�X�^�[�f�[�^�ŕύX����
    // ���O
    [HideInInspector] public string m_name;

    // �X�L�����ǂ���
    [HideInInspector] public bool m_isSkill = false;

    // �^�C�v
    [HideInInspector] public int m_type;

    // �p���^�[����
    [HideInInspector] public int m_continuationTurn;

    // ���ʗ�
    [HideInInspector] public int m_value1;
    [HideInInspector] public int m_value2;

    // ���ʓ��e�i�v���C���[�����j
    [HideInInspector] public string m_effects;

    public void Setting(string _name, bool _isSkill, int _type, int _continuationTurn, int _value1, int _value2, string _effects)
    {
        m_name = _name;
        m_isSkill = _isSkill;
        m_type = _type;
        m_continuationTurn = _continuationTurn;
        m_value1 = _value1;
        m_value2 = _value2;
        m_effects = _effects;

        if (m_effects.Contains("{"))
        {
            string str = _effects;
            int start = str.IndexOf('{');
            int end = str.IndexOf('}');
            m_effects = str.Remove(start, (end - start + 1));
        }
        if (_effects.Contains("V"))
        {
            string value = m_value1.ToString();
            string str = _effects;
            m_effects = str.Replace("V", value);
        }

        if (m_effects.Contains("W"))
        {
            string value = m_value2.ToString();
            string str = _effects;
            m_effects = str.Replace("W", value);
        }
    }
}
