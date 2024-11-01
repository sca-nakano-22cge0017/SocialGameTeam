using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpecialTecnique : MonoBehaviour
{
    // 特殊技能ID
    private int m_id;

    // 以下はマスターデータで変更あり
    // 名前
    private string m_name;

    // 効果内容（プレイヤー向け）
    private string m_effects;

    // 使用アイコン
    private Sprite m_illust;

    // スキルかどうか
    private bool m_isSkill = false;

    // 継続ターン数
    private int m_continuationTurn;

    // 効果量
    private int m_value;

    void Start()
    {
        
    }

    void Update()
    {
        
    }
}
