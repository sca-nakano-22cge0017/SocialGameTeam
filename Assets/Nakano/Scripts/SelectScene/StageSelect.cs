using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using UnityEngine.SceneManagement;
using Master;

[System.Serializable]
public class SelectButton
{
    public StageSelectButton button;
    public string information;
    public Sprite sprite;
    public string name;
    public int stageId;
}

public class StageSelect : MonoBehaviour
{
    [SerializeField] private Image stageImage;
    [SerializeField] private Text stageInformationText;

    [SerializeField] private SelectButton[] selectButtons;

    private StageSelectButton selectingButton;                    // 選択中のボタン
    [SerializeField] private StageSelectButton firstSelectButton; // 最初に選択しておくボタン
    [SerializeField] private GameObject selectingFrame;           // 選択中のボタンに表示する枠

    bool isCoolTime_Select = false;
    const float coolTime = 0.01f;

    StaminaManager sm = null;
    StageDataManager sdm = null;

    void Start()
    {
        sm = FindObjectOfType<StaminaManager>();
        sdm = FindObjectOfType<StageDataManager>();

        FirstSelect();
    }

    private void Update()
    {
    }

    private void FirstSelect()
    {
        selectingButton = firstSelectButton;

        stageImage.sprite = selectButtons[0].sprite;

        // ドロップ内容表示
        if (SceneManager.GetActiveScene().name == "SelectScene_Traning")
        {
            string information = "";

            for (int d = 0; d < MasterData.StageDatas.Count; d++)
            {
                StageData data = MasterData.StageDatas[d];
                if (data.difficulty == GameManager.SelectDifficulty && data.areaId == 1 && data.stageId == selectButtons[0].stageId)
                {
                    for (int i = 0; i < data.dropItem.Count; i++)
                    {
                        information += selectButtons[0].information + "ランクポイント ×" + data.dropItem[i].dropAmount + "\n";
                    }
                }
            }

            stageInformationText.text = information;
        }

        selectingFrame.transform.SetParent(selectButtons[0].button.gameObject.transform);
        selectingFrame.transform.localPosition = Vector3.zero;

        selectButtons[0].button.FirstSelected();
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
            selectingButton = _selectingButton;

            // ドロップ内容表示
            if (SceneManager.GetActiveScene().name == "SelectScene_Traning")
            {
                string information = "";

                for (int d = 0; d < MasterData.StageDatas.Count; d++)
                {
                    StageData data = MasterData.StageDatas[d];
                    if (data.difficulty == GameManager.SelectDifficulty && data.areaId == 1 && data.stageId == pressedButton.stageId)
                    {
                        for (int i = 0; i < data.dropItem.Count; i++)
                        {
                            information += pressedButton.information + "ランクポイント ×" + data.dropItem[i].dropAmount + "\n";
                        }
                    }
                }

                stageInformationText.text = information;
            }
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
    public void Transition(StageSelectButton _selectingButton, int _difficulty,  int _areaId, int _stageId)
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
            GameManager.SelectArea = _areaId;
            GameManager.SelectStage = _stageId;

            int difficulty = -1;
            if (_areaId == 1) difficulty = GameManager.SelectDifficulty;
            if (_areaId == 2) difficulty = _difficulty;
            GameManager.SelectDifficulty = difficulty;

            ConsumeStamina(_areaId);

            // ステージデータ読み込み完了時の処理
            sdm.LoadCompleteProcess += () =>
            {
                // バトル画面への遷移
                //SceneManager.LoadScene("Main");
                SceneManager.LoadScene("MainTest");
            };
            sdm.LoadData(GameManager.SelectDifficulty, _areaId, _stageId);
        }

        // 押下後、一定時間押下判定を取らない
        StartCoroutine(DelayCoroutine(coolTime, () =>
        {
            isCoolTime_Select = false;
        }));
    }
    
    /// <summary>
    /// スタミナ消費
    /// </summary>
    /// <param name="_stageId"></param>
    private void ConsumeStamina(int _areaId)
    {
        if (sm == null) return;

        switch (_areaId)
        {
            case 1:
                sm.Traning();
                break;

            case 2:
                sm.Boss();
                break;
        }
    }

    IEnumerator DelayCoroutine(float _time, Action _action)
    {
        yield return new WaitForSeconds(_time);
        _action?.Invoke();
    }
}
