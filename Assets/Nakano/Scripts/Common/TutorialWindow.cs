using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TutorialWindow : MonoBehaviour
{
    [System.Serializable]
    public class TutorialExplains
    {
        public string title;
        public TutorialExplain[] pages;
    }

    [System.Serializable]
    public class TutorialExplain
    {
        public Sprite sprite;
        public string explain;
    }

    [SerializeField] private GameObject tutorialWindow;
    [SerializeField] private Canvas tutorialCanvas;
    [SerializeField] private Image image;
    [SerializeField] private Text explain;
    [SerializeField] private Text title;
    [SerializeField] private Text pageText;
    [SerializeField] private GameObject backButton;
    [SerializeField] private GameObject nextButton;
    [SerializeField] private GameObject skipButton;
    [SerializeField] private GameObject closeButton;

    [SerializeField] private TutorialExplains charaSelect;
    [SerializeField] private TutorialExplains home;
    [SerializeField] private TutorialExplains stageSelect;
    [SerializeField] private TutorialExplains battle;
    [SerializeField] private TutorialExplains result;
    [SerializeField] private TutorialExplains bossBattle;
    [SerializeField] private TutorialExplains stageSelect_BossCleared;
    [SerializeField] private TutorialExplains reset;

    private TutorialExplains displayExplain; // �\���������
    private int page = 1;                    // �\�����̃y�[�W
    private bool completeTutorial = false;
    /// <summary>
    /// �`���[�g���A�������ς݂��ǂ���
    /// </summary>
    public bool CompleteTutorial { get => completeTutorial; private set => completeTutorial = value; }

    private void Awake()
    {
        // ���C���J�����ݒ�
        tutorialCanvas.worldCamera = Camera.main;
    }

    public void CharaSelect()
    {
        if (GameManager.TutorialProgress.checkedCharaSelect)
        {
            completeTutorial = true;
            return;
        }

        ChangeExplain(charaSelect);

        GameManager.TutorialProgress.checkedCharaSelect = true;
    }

    public void Home()
    {
        if (GameManager.TutorialProgress.checkedHome)
        {
            completeTutorial = true;
            return;
        }

        ChangeExplain(home);

        GameManager.TutorialProgress.checkedHome = true;
    }

    public void StageSelect()
    {
        if (GameManager.TutorialProgress.checkedStageSelect)
        {
            completeTutorial = true;
            return;
        }

        ChangeExplain(stageSelect);

        GameManager.TutorialProgress.checkedStageSelect = true;
    }

    public void Battle()
    {
        if (GameManager.TutorialProgress.checkedBattle)
        {
            completeTutorial = true;
            return;
        }

        ChangeExplain(battle);

        GameManager.TutorialProgress.checkedBattle = true;
    }

    public void Result()
    {
        if (GameManager.TutorialProgress.checkedResult)
        {
            completeTutorial = true;
            return;
        }

        ChangeExplain(result);

        GameManager.TutorialProgress.checkedResult = true;
    }

    public void BossBattle()
    {
        if (GameManager.TutorialProgress.checkedBossBattle)
        {
            completeTutorial = true;
            return;
        }

        ChangeExplain(bossBattle);

        GameManager.TutorialProgress.checkedBossBattle = true;
    }

    public void StageSelect_BossCleared()
    {
        if (GameManager.TutorialProgress.checkedStageSelect_BossCleared)
        {
            completeTutorial = true;
            return;
        }

        ChangeExplain(stageSelect_BossCleared);

        GameManager.TutorialProgress.checkedStageSelect_BossCleared = true;
    }

    public void TraningReset()
    {
        if (GameManager.TutorialProgress.checkedReset)
        {
            completeTutorial = true;
            return;
        }

        ChangeExplain(reset);

        GameManager.TutorialProgress.checkedReset = true;
    }

    /// <summary>
    /// �\�����������ύX
    /// </summary>
    void ChangeExplain(TutorialExplains _explains)
    {
        completeTutorial = false;

        displayExplain = _explains;

        title.text = displayExplain.title;

        page = 1;
        ChangePage();

        tutorialWindow.SetActive(true);
    }

    public void NextPage()
    {
        page++;
        ChangePage();
    }

    public void BackPage()
    {
        page--;
        ChangePage();
    }

    public void Close()
    {
        tutorialWindow.SetActive(false);
        completeTutorial = true;
    }

    /// <summary>
    /// �y�[�W�ڍs����
    /// </summary>
    void ChangePage()
    {
        if (page < 0 || page > displayExplain.pages.Length) return;

        pageText.text = page.ToString() + " / " + (displayExplain.pages.Length).ToString();

        // �y�[�W����
        image.sprite = displayExplain.pages[page - 1].sprite;
        explain.text = displayExplain.pages[page - 1].explain;

        // 2�y�[�W�ȍ~�Ȃ�O�֖߂�{�^����\��
        if (page > 1)
        {
            backButton.SetActive(true);
        }
        else backButton.SetActive(false);

        // �ŏI�y�[�W�łȂ���Ύ��֍s���{�^����\��
        // �ŏI�y�[�W�ł���΃`���[�g���A���I���{�^����\��
        if (page < displayExplain.pages.Length)
        {
            nextButton.SetActive(true);
            skipButton.SetActive(true);
            closeButton.SetActive(false);
        }
        else
        {
            nextButton.SetActive(false);
            skipButton.SetActive(false);
            closeButton.SetActive(true);
        }
    }
}
