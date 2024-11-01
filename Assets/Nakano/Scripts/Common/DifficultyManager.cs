using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// ���C���Q�[���̓�Փx�ݒ�
/// </summary>
public class DifficultyManager : MonoBehaviour
{
    private const int initDifficulty = 1;
    
    private const int maxDifficulty = 5;

    // �{�X��N���A�ςݓ�Փx �N���A�������̂̒��ōł���������
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
    /// ��Փx�ݒ�
    /// </summary>
    /// <param name="_difficulty">�ݒ��̓�Փx</param>
    public static void SetDifficulty(int _difficulty)
    {
        if (_difficulty < initDifficulty || _difficulty > maxDifficulty)
            return; // �͈͊O

        // �{�X����N���A������Փx�{�P�܂ł�I���ł���
        if (_difficulty <= isClearBossDifficulty + 1)
        {
            GameManager.SelectDifficulty = _difficulty;
            Debug.Log(GameManager.SelectDifficulty);
        }
    }

    /// <summary>
    /// �{�X��N���A�ςݓ�Փx��ݒ�
    /// </summary>
    /// <param name="_difficulty"></param>
    public static void SetBossClearDifficulty(int _difficulty)
    {
        if (_difficulty < initDifficulty || _difficulty > maxDifficulty)
            return; // �͈͊O

        if (_difficulty > isClearBossDifficulty)
        {
            isClearBossDifficulty = _difficulty;
        }
    }
}
