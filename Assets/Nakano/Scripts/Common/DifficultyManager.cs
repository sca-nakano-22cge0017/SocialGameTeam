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

    [SerializeField] private Text difficultyText;
    [SerializeField] private WindowController wc;
    [SerializeField] private Button[] selectButtons;

    [SerializeField] private bool isDebug = false;

    private void Awake()
    {
        if (GameManager.SelectChara == 1)
        {
            if (GameManager.lastSelectDifficulty1 > 0)
            {
                GameManager.SelectDifficulty = GameManager.lastSelectDifficulty1;
            }
            else
            {
                if (isClearBossDifficulty1 < 5)
                {
                    GameManager.SelectDifficulty = isClearBossDifficulty1 + 1;
                }
                else GameManager.SelectDifficulty = isClearBossDifficulty1;
            }
        }
        if (GameManager.SelectChara == 2)
        {
            if (GameManager.lastSelectDifficulty2 > 0)
            {
                GameManager.SelectDifficulty = GameManager.lastSelectDifficulty2;
            }
            else
            {
                if (isClearBossDifficulty1 < 5)
                {
                    GameManager.SelectDifficulty = isClearBossDifficulty2 + 1;
                }
                else GameManager.SelectDifficulty = isClearBossDifficulty2;
            }
        }
        
        SetSelectButtons();
    }

    void SetSelectButtons()
    {
        var difficulty = GameManager.SelectChara == 1 ? isClearBossDifficulty1 : isClearBossDifficulty2;

        for (int i = 0; i < selectButtons.Length; i++)
        {
            if (i <= difficulty)
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
        if (GameManager.SelectChara == 1)
        {
            if (_difficulty <= isClearBossDifficulty1 + 1)
            {
                GameManager.SelectDifficulty = _difficulty;
                GameManager.lastSelectDifficulty1 = GameManager.SelectDifficulty;

                difficultyText.text = "Lv" + (GameManager.SelectDifficulty * 10).ToString();
                wc.Close();
            }
        }
        if (GameManager.SelectChara == 2)
        {
            if (_difficulty <= isClearBossDifficulty2 + 1)
            {
                GameManager.SelectDifficulty = _difficulty;
                GameManager.lastSelectDifficulty2 = GameManager.SelectDifficulty;

                difficultyText.text = "Lv" + (GameManager.SelectDifficulty * 10).ToString();
                wc.Close();
            }
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
        if (GameManager.SelectChara == 1)
        {
            if (_difficulty <= isClearBossDifficulty1 + 1)
            {
                GameManager.SelectDifficulty = _difficulty;
                GameManager.lastSelectDifficulty1 = GameManager.SelectDifficulty;
            }
        }
        if (GameManager.SelectChara == 2)
        {
            if (_difficulty <= isClearBossDifficulty2 + 1)
            {
                GameManager.SelectDifficulty = _difficulty;
                GameManager.lastSelectDifficulty2 = GameManager.SelectDifficulty;
            }
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

        if (GameManager.SelectChara == 1)
        {
            if (_difficulty > isClearBossDifficulty1)
            {
                isClearBossDifficulty1 = _difficulty;
            }
        }
        if (GameManager.SelectChara == 2)
        {
            if (_difficulty > isClearBossDifficulty2)
            {
                isClearBossDifficulty2 = _difficulty;
            }
        }
    }
}
