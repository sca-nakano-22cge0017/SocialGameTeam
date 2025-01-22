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

    [SerializeField] private Text costStamina;
    [SerializeField] private Image stamp;

    SelectButton pressedButton = null;

    bool isCoolTime_Select = false;
    const float coolTime = 0.01f;

    StaminaManager sm = null;
    StageDataManager sdm = null;
    TutorialWindow tutorial = null;
    SoundController soundController;

    // ステージイメージ画像
    [SerializeField] private Sprite[] sprites_Lv1;
    [SerializeField] private Sprite[] sprites_Lv2;
    [SerializeField] private Sprite[] sprites_Lv3;
    [SerializeField] private Sprite[] sprites_Boss;

    void Start()
    {
        sm = FindObjectOfType<StaminaManager>();
        sdm = FindObjectOfType<StageDataManager>();
        tutorial = FindObjectOfType<TutorialWindow>();
        soundController = FindObjectOfType<SoundController>();

        if (SceneManager.GetActiveScene().name == "SelectScene_Traning")
        {
            costStamina.text = "-" + sm.GetCost_Traning;

            var clearBossDifficulty = GameManager.SelectChara == 1 ? DifficultyManager.IsClearBossDifficulty1 : DifficultyManager.IsClearBossDifficulty2;

            if (clearBossDifficulty > 0)
            {
                tutorial.StageSelect_BossCleared();
            }
            else tutorial.StageSelect();
        }
        if (SceneManager.GetActiveScene().name == "SelectScene_Boss")
        {
            costStamina.text = "-" + sm.GetCost_Boss;
        }

        FirstSelect();
    }

    private void FirstSelect()
    {
        int last = GameManager.lastSelectButton_Traning;

        if (SceneManager.GetActiveScene().name == "SelectScene_Traning")
        {
            last = GameManager.lastSelectButton_Traning;
        }
        if (SceneManager.GetActiveScene().name == "SelectScene_Boss")
        {
            last = GameManager.lastSelectButton_Boss;
        }

        // 前回選択したステージを選択状態にしておく
        if (GameManager.lastScene == "MainTest" && last >= 0 && last < selectButtons.Length)
        {
            selectingButton = selectButtons[last].button;
            pressedButton = selectButtons[last];
            //stageImage.sprite = selectButtons[last].sprite;
            stageImage.sprite = GetStageImage(last + 1);

            selectingFrame.transform.SetParent(selectButtons[last].button.gameObject.transform);
            selectingFrame.transform.localPosition = Vector3.zero;

            // ドロップ内容表示
            DropDetailFirstDisplay(last);
        }
        else
        {
            selectingButton = firstSelectButton;
            pressedButton = selectButtons[0];

            //stageImage.sprite = selectButtons[0].sprite;
            stageImage.sprite = GetStageImage(1);

            selectingFrame.transform.SetParent(selectButtons[0].button.gameObject.transform);
            selectingFrame.transform.localPosition = Vector3.zero;

            // ドロップ内容表示
            DropDetailFirstDisplay(0);
        }
    }

    /// <summary>
    /// ボタン押下によるステージ選択
    /// </summary>
    public void Select(StageSelectButton _selectingButton)
    {
        int select = 0;

        // クールタイム中なら終了
        if (isCoolTime_Select) return;
        isCoolTime_Select = true;

        // 押下したボタンに応じて情報取得
        for (int i = 0; i < selectButtons.Length; i++)
        {
            if (selectButtons[i].button == _selectingButton)
            {
                pressedButton = selectButtons[i];

                selectingFrame.transform.SetParent(selectButtons[i].button.gameObject.transform);
                selectingFrame.transform.localPosition = Vector3.zero;
                selectingFrame.transform.localScale = Vector3.one;

                select = i;
            }
        }
        if (pressedButton == null) return;

        // 押下処理
        if (selectingButton != _selectingButton)
        {
            //stageImage.sprite = pressedButton.sprite;
            stageImage.sprite = GetStageImage(select + 1);
            selectingButton = _selectingButton;

            if (SceneManager.GetActiveScene().name == "SelectScene_Boss")
            {
                GameManager.SelectDifficulty = _selectingButton.Difficluty;
                GameManager.lastSelectButton_Boss = select;
            }

            else GameManager.lastSelectButton_Traning = select;
            
            DropDetailDisplay();
        }

        // 押下後、一定時間押下判定を取らない
        StartCoroutine(DelayCoroutine(coolTime, () => 
        {
            isCoolTime_Select = false;
        }));
    }

    /// <summary>
    /// ドロップ内容表示
    /// </summary>
    public void DropDetailDisplay()
    {
        int area = 1;
        if (SceneManager.GetActiveScene().name == "SelectScene_Traning") area = 1;
        if (SceneManager.GetActiveScene().name == "SelectScene_Boss") area = 2;

        string information = "";

        for (int d = 0; d < MasterData.StageDatas.Count; d++)
        {
            StageData data = MasterData.StageDatas[d];
            if (data.difficulty == GameManager.SelectDifficulty && data.areaId == area && data.stageId == pressedButton.stageId)
            {
                for (int i = 0; i < data.dropItem.Count; i++)
                {
                    information += PlayerDataManager.StutasTypeToString(data.dropItem[i].itemType) + "ランクポイント ＋" + data.dropItem[i].dropAmount + "Pt\n";
                }
            }
        }

        if (area == 2)
        {
            int difficulty = GameManager.SelectDifficulty - 1;
            if (difficulty < 0) difficulty = 0;

            var isBossClear = GameManager.SelectChara == 1 ? GameManager.IsBossClear1[difficulty] : GameManager.IsBossClear2[difficulty];
            // クリア済みならスタンプを表示
            if (isBossClear)
            {
                stamp.enabled = true;
            }
            else stamp.enabled = false;
        }

        stageInformationText.text = information;
    }

    void DropDetailFirstDisplay(int _firstSelect)
    {
        int area = 1;
        if (SceneManager.GetActiveScene().name == "SelectScene_Traning") area = 1;
        if (SceneManager.GetActiveScene().name == "SelectScene_Boss") area = 2;

        string information = "";

        for (int d = 0; d < MasterData.StageDatas.Count; d++)
        {
            StageData data = MasterData.StageDatas[d];
            if (data.difficulty == GameManager.SelectDifficulty && data.areaId == area && data.stageId == selectButtons[_firstSelect].stageId)
            {
                for (int i = 0; i < data.dropItem.Count; i++)
                {
                    information += PlayerDataManager.StutasTypeToString(data.dropItem[i].itemType) + "ランクポイント ＋" + data.dropItem[i].dropAmount + "Pt\n";
                }
            }
        }

        if (area == 2)
        {
            var isBossClear = GameManager.SelectChara == 1 ? GameManager.IsBossClear1[0] : GameManager.IsBossClear2[0];

            // クリア済みならスタンプを表示
            if (isBossClear)
            {
                stamp.enabled = true;
            }
            else stamp.enabled = false;
        }

        stageInformationText.text = information;
    }

    /// <summary>
    /// ボタン押下によるステージ遷移
    /// </summary>
    public void Transition()
    {
        // クールタイム中なら終了
        if (isCoolTime_Select) return;
        isCoolTime_Select = true;

        // スタミナがあるかどうか確認
        ConsumeStamina(selectingButton.AreaID);
    }
    
    /// <summary>
    /// スタミナ消費
    /// </summary>
    /// <param name="_stageId"></param>
    private void ConsumeStamina(int _areaId)
    {
        if (sm == null) return;

        // 押下処理
        GameManager.SelectArea = selectingButton.AreaID;
        GameManager.SelectStage = selectingButton.StageID;

        int difficulty = -1;
        if (selectingButton.AreaID == 1) difficulty = GameManager.SelectDifficulty;
        if (selectingButton.AreaID == 2) difficulty = selectingButton.Difficluty;
        GameManager.SelectDifficulty = difficulty;

        // ステージデータ読み込み完了時の処理
        sdm.LoadCompleteProcess = () =>
        {
            if ((_areaId == 1 && sm.Cost(sm.GetCost_Traning)) ||
            (_areaId == 2 && sm.Cost(sm.GetCost_Boss)))
            {
                SceneLoader.LoadScene("MainTest", true);
            }
        };
        sdm.LoadData(GameManager.SelectDifficulty, selectingButton.AreaID, selectingButton.StageID);

        // 押下後、一定時間押下判定を取らない
        StartCoroutine(DelayCoroutine(coolTime, () =>
        {
            isCoolTime_Select = false;
        }));
    }

    IEnumerator DelayCoroutine(float _time, Action _action)
    {
        yield return new WaitForSeconds(_time);
        _action?.Invoke();
    }

    /// <summary>
    /// スタミナ全快
    /// </summary>
    public void StaminaRecovery()
    {
        sm.DebugRecovery();
    }

    public void TapSE()
    {
        soundController.PlayTap1SE();
    }

    /// <summary>
    /// ステージイメージ画像設定
    /// </summary>
    Sprite GetStageImage(int _stageNum)
    {
        Sprite s = null;
        if (SceneManager.GetActiveScene().name == "SelectScene_Traning")
        {
            if (GameManager.SelectDifficulty == 1)
            {
                s = sprites_Lv1[_stageNum - 1];
            }

            if (GameManager.SelectDifficulty == 2)
            {
                s = sprites_Lv2[_stageNum - 1];
            }

            if (GameManager.SelectDifficulty >= 3)
            {
                s = sprites_Lv3[_stageNum - 1];
            }
        }

        if (SceneManager.GetActiveScene().name == "SelectScene_Boss")
        {
            if (_stageNum % 2 == 1)
            {
                s = sprites_Boss[0];
            }

            else
            {
                s = sprites_Boss[1];
            }
        }

        return s;
    }
}
