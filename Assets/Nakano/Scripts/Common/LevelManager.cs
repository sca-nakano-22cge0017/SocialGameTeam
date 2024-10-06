using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ゲーム全体の難易度
/// </summary>
public class LevelManager : MonoBehaviour
{
    /// <summary>
    /// 難易度
    /// </summary>
    private static int level = 1;
    private static string levelId = "01";
    private const int initLevel = 1;
    
    private const int maxLevel = 70;
    /// <summary>
    /// 難易度
    /// </summary>
    public static int Level
    {
        get
        {
            if (level < initLevel || level > maxLevel)
            {
                Debug.Log("難易度：指定外の難易度が設定されています");
            }

            return level;
        }
        set
        {
            if (value < initLevel || value > maxLevel)
            {
                Debug.Log("難易度：指定外の難易度です");
                return;
            }

            level = value;
        }
    }

    /// <summary>
    /// 難易度のID
    /// </summary>
    public static string LevelId
    {
        get
        {
            if (level < initLevel || level > maxLevel)
            {
                Debug.Log("難易度：指定外の難易度が設定されています");
            }
            else
            {
                levelId = level.ToString("d2");
            }

            return levelId;
        }
        private set { }
    }

    /// <summary>
    /// 難易度上昇
    /// </summary>
    /// <param name="_amount">上昇量</param>
    public static void LevelUp(int _amount)
    {
        int lv = level + _amount;
        if (lv <= maxLevel)
        {
            level += _amount;
        }
        else if (lv > maxLevel)
        {
            level = maxLevel;
        }
        Debug.Log("難易度が" + _amount + "上昇, 現在の難易度" + level);
    }

    /// <summary>
    /// 難易度減少
    /// </summary>
    /// <param name="_amount">減少量</param>
    public static void LevelDown(int _amount)
    {
        int lv = level - _amount;
        if (lv >= initLevel)
        {
            level -= _amount;
        }
        else if (lv < initLevel)
        {
            level = initLevel;
        }

        Debug.Log("難易度が" + _amount + "減少, 現在の難易度" + level);
    }

    /// <summary>
    /// 難易度リセット
    /// </summary>
    public static void LevelReset()
    {
        level = initLevel;
        Debug.Log("難易度リセット");
    }
}
