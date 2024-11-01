using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// メインゲームの難易度設定
/// </summary>
public class DifficultyManager : MonoBehaviour
{
    private const int initDifficulty = 1;
    
    private const int maxDifficulty = 5;

    // ボス戦クリア済み難易度 クリアしたものの中で最も高いもの
    private static int isClearBossDifficulty = 0;
    public static int IsClearBossDifficulty
    {
        get => isClearBossDifficulty;
        set => isClearBossDifficulty = value;
    }

    [SerializeField] private Button[] selectButtons;

    private void Awake()
    {
        SetSelectButtons();
    }

    void SetSelectButtons()
    {
        for (int i = 0; i < selectButtons.Length; i++)
        {
            if (i <= isClearBossDifficulty)
            {
                selectButtons[i].interactable = true;
            }
            else selectButtons[i].interactable = false;
        }
    }

    public void SetDifficulty_Button(int _difficulty)
    {
        SetDifficulty(_difficulty);
    }

    /// <summary>
    /// 難易度設定
    /// </summary>
    /// <param name="_difficulty">設定後の難易度</param>
    public static void SetDifficulty(int _difficulty)
    {
        if (_difficulty < initDifficulty || _difficulty > maxDifficulty)
            return; // 範囲外

        // ボス戦をクリアした難易度＋１までを選択できる
        if (_difficulty <= isClearBossDifficulty + 1)
        {
            GameManager.SelectDifficulty = _difficulty;
            Debug.Log(GameManager.SelectDifficulty);
        }
    }

    /// <summary>
    /// ボス戦クリア済み難易度を設定
    /// </summary>
    /// <param name="_difficulty"></param>
    public static void SetBossClearDifficulty(int _difficulty)
    {
        if (_difficulty < initDifficulty || _difficulty > maxDifficulty)
            return; // 範囲外

        if (_difficulty > isClearBossDifficulty)
        {
            isClearBossDifficulty = _difficulty;
        }
    }
}
