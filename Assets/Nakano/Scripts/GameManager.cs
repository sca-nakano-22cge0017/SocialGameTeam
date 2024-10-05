using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ゲーム全体で使用する変数等を管理
/// </summary>
public class GameManager : MonoBehaviour
{
    private static int charaID = -1;

    /// <summary>
    /// 選択中キャラクター / 1:シスター , 2:剣士 , -1;エラー
    /// </summary>
    public static int CharaID
    {
        get { return charaID; }
        set
        {
            if (value != 1 && value != 2) return;
            charaID = value;
        }
    }
}
