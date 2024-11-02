using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerData : MonoBehaviour
{

    public GameObject player; // プレイヤーオブジェクト

    // キャラクター
    public int ID; // キャラID
    public int RANKID; // ランクID

    // 攻撃
    public int ATK_m; // 攻撃力最低値
    public int ATK_M; // 攻撃力最大値
    public int ATKUP; // アタックランクアップボーナス

    // 魔力
    public int MP_m; // 魔力最低値
    public int MP_M; // 魔力最大値
    public int MPUP; // 魔力ランクアップボーナス

    // 体力
    public int HP_m; // 体力最低値
    public int HP_M; // 体力最大値
    public int HPUP; // HPランクアップボーナス

    // 防御
    public int DEF_m; // 防御力最低値
    public int DEF_M; // 防御力最大値
    public int DEFUP; // 防御力ランクアップボーナス

    // 速度
    public int AGI_m; // 速度最低値
    public int AGI_M; // 速度最大値
    public int AGIUP; // 速度ランクアップボーナス

    // 器用
    public int DEX_m; // 器用最低値
    public int DEX_M; // 器用最大値
    public int DEXUP; // 器用ランクアップボーナス

    // 特殊技能ID
    public int AttackRankSkill; // アタック特殊技能ID
    public int DefenceRankSkill; // ディフェンス特殊技能ID
    public int TechnicalRankSkill; // テクニカル特殊技能ID





    void Start()
    {
        
    }

    void Update()
    {
        
    }
}
