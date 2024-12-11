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

    [SerializeField] private Text difficultyText;
    [SerializeField] private WindowController wc;
    [SerializeField] private Button[] selectButtons;

    [SerializeField] private bool isDebug = false;

    private void Awake()
    {
        if (GameManager.lastSelectDifficulty > 0)
        {
            GameManager.SelectDifficulty = GameManager.lastSelectDifficulty;
        }
        else
        {
            if (isClearBossDifficulty < 5)
            {
                GameManager.SelectDifficulty = isClearBossDifficulty + 1;
            }
            else GameManager.SelectDifficulty = isClearBossDifficulty;
        }
        
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

            if (isDebug) selectButtons[i].interactable = true;
        }

        difficultyText.text = "Lv" + (GameManager.SelectDifficulty * 10).ToString();
    }

    public void SetDifficulty_Button(int _difficulty)
    {
        if (isDebug)
        {
            GameManager.SelectDifficulty = _difficulty;
            difficultyText.text = "Lv" + (GameManager.SelectDifficulty * 10).ToString();
            wc.Close();
        }
        else _SetDifficulty(_difficulty);

        StageSelect ss = FindObjectOfType<StageSelect>();
        if (ss)
        {
            ss.DropDetailDisplay();
        }
    }

    /// <summary>
    /// ��Փx�ݒ�
    /// </summary>
    /// <param name="_difficulty">�ݒ��̓�Փx</param>
    void _SetDifficulty(int _difficulty)
    {
        if (_difficulty < initDifficulty || _difficulty > maxDifficulty)
            return; // �͈͊O

        // �{�X����N���A������Փx�{�P�܂ł�I���ł���
        if (_difficulty <= isClearBossDifficulty + 1)
        {
            GameManager.SelectDifficulty = _difficulty;
            difficultyText.text = "Lv" + (GameManager.SelectDifficulty * 10).ToString();
            wc.Close();
        }
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
