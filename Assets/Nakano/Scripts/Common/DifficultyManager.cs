using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// �Q�[���S�̂̓�Փx
/// </summary>
public class DifficultyManager : MonoBehaviour
{
    /// <summary>
    /// ��Փx
    /// </summary>
    private static int difficulty = 1;
    private const int initDifficulty = 1;
    
    private const int maxDifficulty = 5;
    /// <summary>
    /// ��Փx
    /// </summary>
    public static int Difficulty
    {
        get
        {
            if (difficulty < initDifficulty || difficulty > maxDifficulty)
            {
                Debug.Log("��Փx�F�w��O�̓�Փx���ݒ肳��Ă��܂�");
            }

            return difficulty;
        }
        set
        {
            if (value < initDifficulty || value > maxDifficulty)
            {
                Debug.Log("��Փx�F�w��O�̓�Փx�ł�");
                return;
            }

            difficulty = value;
        }
    }

    /// <summary>
    /// ��Փx�ݒ�
    /// </summary>
    /// <param name="_difficulty">�ݒ��̓�Փx</param>
    public static void SetDifficulty(int _difficulty)
    {
        if (_difficulty >= initDifficulty && _difficulty <= maxDifficulty)
        {
            difficulty = _difficulty;
        }
    }

    /// <summary>
    /// ��Փx�㏸
    /// </summary>
    /// <param name="_amount">�㏸��</param>
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
        Debug.Log("��Փx��" + _amount + "�㏸, ���݂̓�Փx" + difficulty);
    }

    /// <summary>
    /// ��Փx����
    /// </summary>
    /// <param name="_amount">������</param>
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

        Debug.Log("��Փx��" + _amount + "����, ���݂̓�Փx" + difficulty);
    }

    /// <summary>
    /// ��Փx���Z�b�g
    /// </summary>
    public static void DifficultyReset()
    {
        difficulty = initDifficulty;
        Debug.Log("��Փx���Z�b�g");
    }
}
