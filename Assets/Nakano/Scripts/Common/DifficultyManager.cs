using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ゲーム全体の難易度
/// </summary>
public class DifficultyManager : MonoBehaviour
{
    /// <summary>
    /// 難易度
    /// </summary>
    private static int difficulty = 1;
    private const int initDifficulty = 1;
    
    private const int maxDifficulty = 5;
    /// <summary>
    /// 難易度
    /// </summary>
    public static int Difficulty
    {
        get
        {
            if (difficulty < initDifficulty || difficulty > maxDifficulty)
            {
                Debug.Log("難易度：指定外の難易度が設定されています");
            }

            return difficulty;
        }
        set
        {
            if (value < initDifficulty || value > maxDifficulty)
            {
                Debug.Log("難易度：指定外の難易度です");
                return;
            }

            difficulty = value;
        }
    }

    /// <summary>
    /// 難易度設定
    /// </summary>
    /// <param name="_difficulty">設定後の難易度</param>
    public static void SetDifficulty(int _difficulty)
    {
        if (_difficulty >= initDifficulty && _difficulty <= maxDifficulty)
        {
            difficulty = _difficulty;
        }
    }

    /// <summary>
    /// 難易度上昇
    /// </summary>
    /// <param name="_amount">上昇量</param>
    public static void DifficultyUp(int _amount)
    {
        int lv = difficulty + _amount;
        if (lv <= maxDifficulty)
        {
            difficulty += _amount;
        }
        else if (lv > maxDifficulty)
        {
            difficulty = maxDifficulty;
        }
        Debug.Log("難易度が" + _amount + "上昇, 現在の難易度" + difficulty);
    }

    /// <summary>
    /// 難易度減少
    /// </summary>
    /// <param name="_amount">減少量</param>
    public static void DifficultyDown(int _amount)
    {
        int lv = difficulty - _amount;
        if (lv >= initDifficulty)
        {
            difficulty -= _amount;
        }
        else if (lv < initDifficulty)
        {
            difficulty = initDifficulty;
        }

        Debug.Log("難易度が" + _amount + "減少, 現在の難易度" + difficulty);
    }

    /// <summary>
    /// 難易度リセット
    /// </summary>
    public static void DifficultyReset()
    {
        difficulty = initDifficulty;
        Debug.Log("難易度リセット");
    }
}
