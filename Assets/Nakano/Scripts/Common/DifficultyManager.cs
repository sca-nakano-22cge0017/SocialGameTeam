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

        // �琬�X�e�[�W�̓�Փx�{�^�����
        for (int i = 0; i < selectButtons.Length; i++)
        {
            if (i <= difficulty)
            {
                selectButtons[i].interactable = true;
            }
            else selectButtons[i].interactable = false;
        }

        // �琬���Z�b�g��͓�Փx����ԒႢ���̂ɕς��Ă���
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

        // �N���A�󋵂��ύX����Ă����ꍇ
        if (isChangeClearDifficulty || isCharaChange)
        {
            if (GameManager.SelectChara == 1)
            {
                if (isClearBossDifficulty1 < 5)
                {
                    // ��Փx���N���A�ς݂̓�Փx + 1�̂��̂ɕύX����
                    GameManager.SelectDifficulty = isClearBossDifficulty1 + 1;
                }
                else GameManager.SelectDifficulty = isClearBossDifficulty1;
            }

            if (GameManager.SelectChara == 2)
            {
                if (isClearBossDifficulty2 < 5)
                {
                    // ��Փx���N���A�ς݂̓�Փx + 1�̂��̂ɕύX����
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
    /// ��Փx�ݒ�
    /// </summary>
    /// <param name="_difficulty">�ύX��̓�Փx</param>
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
                // �N���A�ςݍō���Փx��ύX
                isClearBossDifficulty1 = _difficulty;
                isChangeClearDifficulty = true;

                Debug.Log("test " + isChangeClearDifficulty + " / isClearBossDifficulty1 : " + isClearBossDifficulty1);
            }
        }
        if (GameManager.SelectChara == 2)
        {
            if (_difficulty > isClearBossDifficulty2)
            {
                // �N���A�ςݍō���Փx��ύX
                isClearBossDifficulty2 = _difficulty;
                isChangeClearDifficulty = true;

                Debug.Log("test " + isChangeClearDifficulty + " / isClearBossDifficulty2 : " + isClearBossDifficulty2);
            }
        }
    }
}
