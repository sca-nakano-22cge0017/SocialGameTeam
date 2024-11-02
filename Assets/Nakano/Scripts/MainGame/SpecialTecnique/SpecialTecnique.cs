using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "SpecialTecnique")]
public class SpecialTecnique : ScriptableObject
{
    // 特殊技能ID
    public int m_id;

    // 解放済みかどうか
    public bool m_released = false;

    // 使用アイコン
    public Sprite m_illust;

    // 以下はマスターデータで変更あり
    // 名前
    private string m_name;

    // スキルかどうか
    private bool m_isSkill = false;

    // タイプ
    private int m_type;

    // 継続ターン数
    private int m_continuationTurn;

    // 効果量
    private int m_value1;
    private int m_value2;

    // 効果内容（プレイヤー向け）
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
