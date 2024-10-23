using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// プレイヤーのセーブデータ用クラス
[System.Serializable]
public class PlayerSaveData
{
    /// <summary>
    /// キャラクターID　1：剣士　2：シスター
    /// </summary>
    public int id;

    /// <summary>
    /// 現在のステータス
    /// </summary>
    public Status status = new(0,0,0,0,0,0);

    /// <summary>
    /// 現在の累積ランクPt
    /// </summary>
    public Status rankPoint = new(0, 0, 0, 0, 0, 0);

    /// <summary>
    /// プラスステータス
    /// </summary>
    public Status plusStatus = new(0, 0, 0, 0, 0, 0);
}
