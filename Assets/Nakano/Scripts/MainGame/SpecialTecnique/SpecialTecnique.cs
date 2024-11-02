using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpecialTecnique : MonoBehaviour
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

    // 継続ターン数
    private int m_continuationTurn;

    // 効果量
    private int m_value;

    // 効果内容（プレイヤー向け）
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
