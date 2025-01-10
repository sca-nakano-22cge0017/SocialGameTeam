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

    private StageSelectButton selectingButton;                    // �I�𒆂̃{�^��
    [SerializeField] private StageSelectButton firstSelectButton; // �ŏ��ɑI�����Ă����{�^��
    [SerializeField] private GameObject selectingFrame;           // �I�𒆂̃{�^���ɕ\������g

    [SerializeField] private Text costStamina;

    SelectButton pressedButton = null;

    bool isCoolTime_Select = false;
    const float coolTime = 0.01f;

    StaminaManager sm = null;
    StageDataManager sdm = null;
    TutorialWindow tutorial = null;
    SoundController soundController;

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
        int last = GameManager.lastSelectButton;
        if (GameManager.lastScene == "MainTest" && last >= 0 && last < selectButtons.Length)
        {
            selectingButton = selectButtons[last].button;
            pressedButton = selectButtons[last];
            stageImage.sprite = selectButtons[last].sprite;

            selectingFrame.transform.SetParent(selectButtons[last].button.gameObject.transform);
            selectingFrame.transform.localPosition = Vector3.zero;

            // �h���b�v���e�\��
            DropDetailFirstDisplay(last);
        }
        else
        {
            selectingButton = firstSelectButton;
            pressedButton = selectButtons[0];

            stageImage.sprite = selectButtons[0].sprite;

            selectingFrame.transform.SetParent(selectButtons[0].button.gameObject.transform);
            selectingFrame.transform.localPosition = Vector3.zero;

            // �h���b�v���e�\��
            DropDetailFirstDisplay(0);
        }
    }

    /// <summary>
    /// �{�^�������ɂ��X�e�[�W�I��
    /// </summary>
    public void Select(StageSelectButton _selectingButton)
    {
        int select = 0;

        // �N�[���^�C�����Ȃ�I��
        if (isCoolTime_Select) return;
        isCoolTime_Select = true;

        // ���������{�^���ɉ����ď��擾
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

        // ��������
        if (selectingButton != _selectingButton)
        {
            stageImage.sprite = pressedButton.sprite;
            selectingButton = _selectingButton;

            if (SceneManager.GetActiveScene().name == "SelectScene_Boss")
            {
                GameManager.SelectDifficulty = _selectingButton.Difficluty;
                
                if (GameManager.SelectChara == 1) GameManager.lastSelectDifficulty1 = GameManager.SelectDifficulty;
                if (GameManager.SelectChara == 2) GameManager.lastSelectDifficulty2 = GameManager.SelectDifficulty;
            }

            GameManager.lastSelectButton = select;
            
            DropDetailDisplay();
        }

        // ������A��莞�ԉ�����������Ȃ�
        StartCoroutine(DelayCoroutine(coolTime, () => 
        {
            isCoolTime_Select = false;
        }));
    }

    /// <summary>
    /// �h���b�v���e�\��
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
                    information += PlayerDataManager.StutasTypeToString(data.dropItem[i].itemType) + "�����N�|�C���g �{" + data.dropItem[i].dropAmount + "Pt\n";
                }
            }
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
                    information += PlayerDataManager.StutasTypeToString(data.dropItem[i].itemType) + "�����N�|�C���g �{" + data.dropItem[i].dropAmount + "Pt\n";
                }
            }
        }

        stageInformationText.text = information;
    }

    /// <summary>
    /// �{�^�������ɂ��X�e�[�W�J��
    /// </summary>
    public void Transition()
    {
        // �N�[���^�C�����Ȃ�I��
        if (isCoolTime_Select) return;
        isCoolTime_Select = true;

        // �X�^�~�i�����邩�ǂ����m�F
        ConsumeStamina(selectingButton.AreaID);
    }
    
    /// <summary>
    /// �X�^�~�i����
    /// </summary>
    /// <param name="_stageId"></param>
    private void ConsumeStamina(int _areaId)
    {
        if (sm == null) return;

        // ��������
        GameManager.SelectArea = selectingButton.AreaID;
        GameManager.SelectStage = selectingButton.StageID;

        int difficulty = -1;
        if (selectingButton.AreaID == 1) difficulty = GameManager.SelectDifficulty;
        if (selectingButton.AreaID == 2) difficulty = selectingButton.Difficluty;
        GameManager.SelectDifficulty = difficulty;

        // �X�e�[�W�f�[�^�ǂݍ��݊������̏���
        sdm.LoadCompleteProcess = () =>
        {
            if ((_areaId == 1 && sm.Cost(sm.GetCost_Traning)) ||
            (_areaId == 2 && sm.Cost(sm.GetCost_Boss)))
            {
                SceneLoader.LoadFade("MainTest");
            }
        };
        sdm.LoadData(GameManager.SelectDifficulty, selectingButton.AreaID, selectingButton.StageID);

        // ������A��莞�ԉ�����������Ȃ�
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
    /// �X�^�~�i�S��
    /// </summary>
    public void StaminaRecovery()
    {
        sm.DebugRecovery();
    }

    public void TapSE()
    {
        soundController.PlayTap1SE();
    }
}
