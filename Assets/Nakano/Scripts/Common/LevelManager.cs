using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// �Q�[���S�̂̓�Փx
/// </summary>
public class LevelManager : MonoBehaviour
{
    /// <summary>
    /// ��Փx
    /// </summary>
    private static int level = 1;
    private static string levelId = "01";
    private const int initLevel = 1;
    
    private const int maxLevel = 70;
    /// <summary>
    /// ��Փx
    /// </summary>
    public static int Level
    {
        get
        {
            if (level < initLevel || level > maxLevel)
            {
                Debug.Log("��Փx�F�w��O�̓�Փx���ݒ肳��Ă��܂�");
            }

            return level;
        }
        set
        {
            if (value < initLevel || value > maxLevel)
            {
                Debug.Log("��Փx�F�w��O�̓�Փx�ł�");
                return;
            }

            level = value;
        }
    }

    /// <summary>
    /// ��Փx��ID
    /// </summary>
    public static string LevelId
    {
        get
        {
            if (level < initLevel || level > maxLevel)
            {
                Debug.Log("��Փx�F�w��O�̓�Փx���ݒ肳��Ă��܂�");
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
    /// ��Փx�㏸
    /// </summary>
    /// <param name="_amount">�㏸��</param>
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
        Debug.Log("��Փx��" + _amount + "�㏸, ���݂̓�Փx" + level);
    }

    /// <summary>
    /// ��Փx����
    /// </summary>
    /// <param name="_amount">������</param>
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

        Debug.Log("��Փx��" + _amount + "����, ���݂̓�Փx" + level);
    }

    /// <summary>
    /// ��Փx���Z�b�g
    /// </summary>
    public static void LevelReset()
    {
        level = initLevel;
        Debug.Log("��Փx���Z�b�g");
    }
}
