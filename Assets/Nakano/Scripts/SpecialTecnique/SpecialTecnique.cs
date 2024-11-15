using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "SpecialTecnique")]
public class SpecialTecnique : ScriptableObject
{
    /// <summary>
    /// ����Z�\ID
    /// </summary>
    public int m_id;

    /// <summary>
    /// ����ς݂��ǂ���
    /// </summary>
    [HideInInspector] public bool m_released = false;

    /// <summary>
    /// �g�p�A�C�R��
    /// </summary>
    public Sprite m_illust;

    // �ȉ��̓}�X�^�[�f�[�^�ŕύX����
    /// <summary>
    /// ���O
    /// </summary>
    [HideInInspector] public string m_name;

    /// <summary>
    /// �X�L�����ǂ���
    /// </summary>
    [HideInInspector] public bool m_isSkill = false;

    /// <summary>
    /// �^�C�v
    /// </summary>
    [HideInInspector] public int m_type;

    /// <summary>
    /// �p���^�[����
    /// </summary>
    [HideInInspector] public int m_continuationTurn;

    /// <summary>
    /// ���ʗ�1
    /// </summary>
    [HideInInspector] public int m_value1;

    /// <summary>
    /// ���ʗ�2
    /// </summary>
    [HideInInspector] public int m_value2;

    /// <summary>
    /// ���ʓ��e�i�v���C���[�����j
    /// </summary>
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

        if (_effects.Contains("V"))
        {
            string value = m_value1.ToString();
            string str = m_effects;
            m_effects = str.Replace("V", value);
        }

        if (m_effects.Contains("W"))
        {
            string value = m_value2.ToString();
            string str = m_effects;
            m_effects = str.Replace("W", value);
        }
        if (m_effects.Contains("{"))
        {
            string str = m_effects;
            int start = str.IndexOf('{');
            int end = str.IndexOf('}');
            m_effects = str.Remove(start, (end - start + 1));
        }
    }
}

public class EnemyBuffTurn
{
    // �o�t�f�o�t���|�����Ă���G
    public Enemy enemy;

    // �o�߃^�[��
    public int elapsedTurn;
}

public class SpecialTecniqueMethod : MonoBehaviour
{
    [SerializeField] protected PlayerData player;
    [SerializeField] protected BattleSystem battleSystem;

    [SerializeField] protected SpecialTecnique rankC;
    [SerializeField] protected SpecialTecnique rankB;
    [SerializeField] protected SpecialTecnique rankA;
    [SerializeField] protected SpecialTecnique rankS;
    [SerializeField] protected SpecialTecnique rankSS;

    /// <summary>
    /// �Q�[���J�n���ɌĂяo��
    /// </summary>
    public virtual void GameStart() { }

    /// <summary>
    /// �^�[���J�n���ɌĂяo��
    /// </summary>
    public virtual void TurnStart() { }

    /// <summary>
    /// �v���C���[�s�����ɌĂяo��
    /// </summary>
    public virtual void PlayerTurnStart() { }

    /// <summary>
    /// �^�[���I�����ɌĂяo��
    /// </summary>
    public virtual void TurnEnd() { }

    /// <summary>
    /// �����^�[������̃X�L���̌o�߃^�[�������Z
    /// </summary>
    /// <param name="_turn"></param>
    protected List<int> TurnPass(List<int> _turn)
    {
        List<int> t = _turn;
        for (int i = 0; i < t.Count; i++)
        {
            t[i]++;
        }
        return t;
    }

    protected List<EnemyBuffTurn> TurnPass(List<EnemyBuffTurn> _turn)
    {
        List<EnemyBuffTurn> t = _turn;

        for (int i = 0; i < t.Count; i++)
        {
            t[i].elapsedTurn++;
        }

        return t;
    }
}
