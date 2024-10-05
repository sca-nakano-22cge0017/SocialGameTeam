using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using UnityEngine.SceneManagement;

[System.Serializable]
public class SelectButton
{
    public StageSelectButton button;
    public string information;
    public Sprite sprite;
    public string name;
}

public class StageSelect : MonoBehaviour
{
    [SerializeField] private bool isBossSelect = false;
    [SerializeField] private Image stageImage;
    [SerializeField] private Text stageInformationText;

    [SerializeField] private SelectButton[] selectButtons;

    private StageSelectButton selectingButton;                    // 選択中のボタン
    [SerializeField] private StageSelectButton firstSelectButton; // 最初に選択しておくボタン
    [SerializeField] private GameObject selectingFrame;           // 選択中のボタンに表示する枠

    // ターゲットロック
    // Todo ターゲットしたステージを保存・取得する
    private StageSelectButton targetButton;            // ターゲットされているボタン
    [SerializeField] private GameObject targetingMark; // ターゲットマーク
    [SerializeField] private Vector2 targetingMarkPosition;

    [SerializeField, Header("ターゲット変更に必要な長押し時間")] 
    private float longTapTime_sec = 0.5f;
    public float GetLongTapTime_sec { get { return longTapTime_sec; } private set { } }


    bool isCoolTime_Select = false;
    const float coolTime = 0.01f;

    void Start()
    {
        FirstSelect();
        
        if (isBossSelect) FirstTarget();
    }

    void Update()
    {
    }

    private void FirstSelect()
    {
        selectingButton = firstSelectButton;

        stageImage.sprite = selectButtons[0].sprite;
        stageInformationText.text = selectButtons[0].information;

        selectingFrame.transform.SetParent(selectButtons[0].button.gameObject.transform);
        selectingFrame.transform.localPosition = Vector3.zero;

        selectButtons[0].button.FirstSelected();
    }

    private void FirstTarget()
    {
        for (int i = 0; i < selectButtons.Length; i++)
        {
            // 最高レベルをターゲット
            if (i == selectButtons.Length - 1)
            {
                targetButton = selectButtons[i].button;

                targetingMark.transform.SetParent(selectButtons[i].button.gameObject.transform);
                targetingMark.transform.localPosition = targetingMarkPosition;
            }
        }
    }

    /// <summary>
    /// ボタン押下によるステージ選択
    /// </summary>
    public void Select(StageSelectButton _selectingButton)
    {
        // クールタイム中なら終了
        if (isCoolTime_Select) return;
        isCoolTime_Select = true;

        // 押下したボタンに応じて情報取得
        SelectButton pressedButton = null;
        for (int i = 0; i < selectButtons.Length; i++)
        {
            if (selectButtons[i].button == _selectingButton)
            {
                pressedButton = selectButtons[i];

                selectingFrame.transform.SetParent(selectButtons[i].button.gameObject.transform);
                selectingFrame.transform.localPosition = Vector3.zero;
                selectingFrame.transform.localScale = Vector3.one;

                // ターゲットロックのマークが上になるように描画順調整
                if (selectingFrame.transform.GetSiblingIndex() >= 3)
                    selectingFrame.transform.SetSiblingIndex(2);
            }
            else
            {
                selectButtons[i].button.TapCountReset();
            }
        }
        if (pressedButton == null) return;

        // 押下処理
        if (selectingButton != _selectingButton)
        {
            stageImage.sprite = pressedButton.sprite;
            stageInformationText.text = pressedButton.information;
            selectingButton = _selectingButton;
        }

        // 押下後、一定時間押下判定を取らない
        StartCoroutine(DelayCoroutine(coolTime, () => 
        {
            isCoolTime_Select = false;
        }));
    }

    /// <summary>
    /// ボタン押下によるステージ遷移
    /// </summary>
    public void Transition(StageSelectButton _selectingButton)
    {
        // クールタイム中なら終了
        if (isCoolTime_Select) return;
        isCoolTime_Select = true;

        // 押下したボタンに応じて情報取得
        SelectButton pressedButton = null;
        for (int i = 0; i < selectButtons.Length; i++)
        {
            if (selectButtons[i].button == _selectingButton)
            {
                pressedButton = selectButtons[i];
                break;
            }
        }
        if (pressedButton == null) return;

        // 押下処理
        if (selectingButton == _selectingButton)
        {
            // バトル画面への遷移
            Debug.Log(pressedButton.name + "へ遷移します");
            SceneManager.LoadScene("MainGame");
        }

        // 押下後、一定時間押下判定を取らない
        StartCoroutine(DelayCoroutine(coolTime, () =>
        {
            isCoolTime_Select = false;
        }));
    }

    /// <summary>
    /// ボタン長押しによるターゲットロック変更
    /// </summary>
    public void TargetChange(StageSelectButton _targetingButton)
    {
        for (int i = 0; i < selectButtons.Length; i++)
        {
            if (selectButtons[i].button == _targetingButton)
            {
                if (targetButton != _targetingButton)
                {
                    targetingMark.transform.SetParent(selectButtons[i].button.gameObject.transform);
                    targetingMark.transform.localPosition = targetingMarkPosition;
                    targetingMark.transform.localScale = Vector3.one;

                    selectButtons[i].button.TargetChange();
                    targetButton = _targetingButton;
                }
            }
        }
    }

    IEnumerator DelayCoroutine(float _time, Action _action)
    {
        yield return new WaitForSeconds(_time);
        _action?.Invoke();
    }
}
