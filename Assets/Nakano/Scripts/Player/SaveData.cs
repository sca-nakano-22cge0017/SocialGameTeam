using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// プレイヤーのセーブデータ用クラス
[System.Serializable]
public class SaveData
{
    public PlayerSaveData chara1 = new();
    public PlayerSaveData chara2 = new();
}

[System.Serializable]
public class PlayerSaveData
{
    /// <summary>
    /// キャラクターID　1：剣士　2：シスター
    /// </summary>
    public int id = -1;

    /// <summary>
    /// 現在のステータス
    /// </summary>
    public int hp = 0;
    public int mp = 0;
    public int atk = 0;
    public int def = 0;
    public int spd = 0;
    public int dex = 0;

    /// <summary>
    /// 現在の累積ランクPt
    /// </summary>
    public int  hp_rankPt = 0;
    public int  mp_rankPt = 0;
    public int atk_rankPt = 0;
    public int def_rankPt = 0;
    public int spd_rankPt = 0;
    public int dex_rankPt = 0;

    /// <summary>
    /// プラスステータス
    /// </summary>
    public int  hp_plusStatus = 0;
    public int  mp_plusStatus = 0;
    public int atk_plusStatus = 0;
    public int def_plusStatus = 0;
    public int spd_plusStatus = 0;
    public int dex_plusStatus = 0;
}