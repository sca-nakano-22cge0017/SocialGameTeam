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
    private static int isClearBossDifficulty1 = 0;
    public static int IsClearBossDifficulty1
    {
        get => isClearBossDifficulty1;
        set => isClearBossDifficulty1 = value;
    }

    private static int isClearBossDifficulty2 = 0;
    public static int IsClearBossDifficulty2
    {
        get => isClearBossDifficulty2;
        set => isClearBossDifficulty2 = value;
    }

    public static bool isReset1 = false;
    public static bool isReset2 = false;

    private static bool isChangeClearDifficulty = false;
    public static bool isCharaChange = false;

    [SerializeField] private Text difficultyText;
    [SerializeField] private WindowController wc;
    [SerializeField] private Button[] selectButtons;

    private void Awake()
    {
        SetSelectButtons();
    }

    void SetSelectButtons()
    {
        var difficulty = GameManager.SelectChara == 1 ? isClearBossDifficulty1 : isClearBossDifficulty2;

        // 育成ステージの難易度ボタン解放
        for (int i = 0; i < selectButtons.Length; i++)
        {
            if (i <= difficulty)
            {
                selectButtons[i].interactable = true;
            }
            else selectButtons[i].interactable = false;
        }

        // 育成リセット後は難易度を一番低いものに変えておく
        if (isReset1)
        {
            GameManager.SelectDifficulty = 1;
            isReset1 = false;
        }
        if (isReset2)
        {
            GameManager.SelectDifficulty = 1;
            isReset2 = false;
        }

        // クリア状況が変更されていた場合
        if (isChangeClearDifficulty || isCharaChange)
        {
            if (GameManager.SelectChara == 1)
            {
                if (isClearBossDifficulty1 < 5)
                {
                    // 難易度をクリア済みの難易度 + 1のものに変更する
                    GameManager.SelectDifficulty = isClearBossDifficulty1 + 1;
                }
                else GameManager.SelectDifficulty = isClearBossDifficulty1;
            }

            if (GameManager.SelectChara == 2)
            {
                if (isClearBossDifficulty2 < 5)
                {
                    // 難易度をクリア済みの難易度 + 1のものに変更する
                    GameManager.SelectDifficulty = isClearBossDifficulty2 + 1;
                }
                else GameManager.SelectDifficulty = isClearBossDifficulty2;
            }

            if (isChangeClearDifficulty) isChangeClearDifficulty = false;
            if (isCharaChange) isCharaChange = false;
        }

        difficultyText.text = "Lv" + (GameManager.SelectDifficulty * 10).ToString();
    }

    /// <summary>
    /// 難易度設定
    /// </summary>
    /// <param name="_difficulty">変更後の難易度</param>
    public void SetDifficulty_Button(int _difficulty)
    {
        _SetDifficulty(_difficulty);

        StageSelect ss = FindObjectOfType<StageSelect>();
        if (ss)
        {
            ss.DropDetailDisplay();
        }
    }

    /// <summary>
    /// 難易度設定
    /// </summary>
    /// <param name="_difficulty">設定後の難易度</param>
    void _SetDifficulty(int _difficulty)
    {
        if (_difficulty < initDifficulty || _difficulty > maxDifficulty)
            return; // 範囲外

        // ボス戦をクリアした難易度＋１までを選択できる
        if (GameManager.SelectChara == 1)
        {
            if (_difficulty <= isClearBossDifficulty1 + 1)
            {
                GameManager.SelectDifficulty = _difficulty;

                difficultyText.text = "Lv" + (GameManager.SelectDifficulty * 10).ToString();
                wc.Close();
            }
        }
        if (GameManager.SelectChara == 2)
        {
            if (_difficulty <= isClearBossDifficulty2 + 1)
            {
                GameManager.SelectDifficulty = _difficulty;

                difficultyText.text = "Lv" + (GameManager.SelectDifficulty * 10).ToString();
                wc.Close();
            }
        }
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
        if (GameManager.SelectChara == 1)
        {
            if (_difficulty <= isClearBossDifficulty1 + 1)
            {
                GameManager.SelectDifficulty = _difficulty;
            }
        }
        if (GameManager.SelectChara == 2)
        {
            if (_difficulty <= isClearBossDifficulty2 + 1)
            {
                GameManager.SelectDifficulty = _difficulty;
            }
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

        if (GameManager.SelectChara == 1)
        {
            if (_difficulty > isClearBossDifficulty1)
            {
                // クリア済み最高難易度を変更
                isClearBossDifficulty1 = _difficulty;
                isChangeClearDifficulty = true;

                Debug.Log("test " + isChangeClearDifficulty + " / isClearBossDifficulty1 : " + isClearBossDifficulty1);
            }
        }
        if (GameManager.SelectChara == 2)
        {
            if (_difficulty > isClearBossDifficulty2)
            {
                // クリア済み最高難易度を変更
                isClearBossDifficulty2 = _difficulty;
                isChangeClearDifficulty = true;

                Debug.Log("test " + isChangeClearDifficulty + " / isClearBossDifficulty2 : " + isClearBossDifficulty2);
            }
        }
    }
}
