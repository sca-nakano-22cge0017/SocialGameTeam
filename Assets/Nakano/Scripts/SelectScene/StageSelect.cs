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

    private StageSelectButton selectingButton;                    // �I�𒆂̃{�^��
    [SerializeField] private StageSelectButton firstSelectButton; // �ŏ��ɑI�����Ă����{�^��
    [SerializeField] private GameObject selectingFrame;           // �I�𒆂̃{�^���ɕ\������g

    // �^�[�Q�b�g���b�N
    // Todo �^�[�Q�b�g�����X�e�[�W��ۑ��E�擾����
    private StageSelectButton targetButton;            // �^�[�Q�b�g����Ă���{�^��
    [SerializeField] private GameObject targetingMark; // �^�[�Q�b�g�}�[�N
    [SerializeField] private Vector2 targetingMarkPosition;

    [SerializeField, Header("�^�[�Q�b�g�ύX�ɕK�v�Ȓ���������")] 
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
            // �ō����x�����^�[�Q�b�g
            if (i == selectButtons.Length - 1)
            {
                targetButton = selectButtons[i].button;

                targetingMark.transform.SetParent(selectButtons[i].button.gameObject.transform);
                targetingMark.transform.localPosition = targetingMarkPosition;
            }
        }
    }

    /// <summary>
    /// �{�^�������ɂ��X�e�[�W�I��
    /// </summary>
    public void Select(StageSelectButton _selectingButton)
    {
        // �N�[���^�C�����Ȃ�I��
        if (isCoolTime_Select) return;
        isCoolTime_Select = true;

        // ���������{�^���ɉ����ď��擾
        SelectButton pressedButton = null;
        for (int i = 0; i < selectButtons.Length; i++)
        {
            if (selectButtons[i].button == _selectingButton)
            {
                pressedButton = selectButtons[i];

                selectingFrame.transform.SetParent(selectButtons[i].button.gameObject.transform);
                selectingFrame.transform.localPosition = Vector3.zero;
                selectingFrame.transform.localScale = Vector3.one;

                // �^�[�Q�b�g���b�N�̃}�[�N����ɂȂ�悤�ɕ`�揇����
                if (selectingFrame.transform.GetSiblingIndex() >= 3)
                    selectingFrame.transform.SetSiblingIndex(2);
            }
            else
            {
                selectButtons[i].button.TapCountReset();
            }
        }
        if (pressedButton == null) return;

        // ��������
        if (selectingButton != _selectingButton)
        {
            stageImage.sprite = pressedButton.sprite;
            stageInformationText.text = pressedButton.information;
            selectingButton = _selectingButton;
        }

        // ������A��莞�ԉ�����������Ȃ�
        StartCoroutine(DelayCoroutine(coolTime, () => 
        {
            isCoolTime_Select = false;
        }));
    }

    /// <summary>
    /// �{�^�������ɂ��X�e�[�W�J��
    /// </summary>
    public void Transition(StageSelectButton _selectingButton)
    {
        // �N�[���^�C�����Ȃ�I��
        if (isCoolTime_Select) return;
        isCoolTime_Select = true;

        // ���������{�^���ɉ����ď��擾
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

        // ��������
        if (selectingButton == _selectingButton)
        {
            // �o�g����ʂւ̑J��
            Debug.Log(pressedButton.name + "�֑J�ڂ��܂�");
            SceneManager.LoadScene("MainGame");
        }

        // ������A��莞�ԉ�����������Ȃ�
        StartCoroutine(DelayCoroutine(coolTime, () =>
        {
            isCoolTime_Select = false;
        }));
    }

    /// <summary>
    /// �{�^���������ɂ��^�[�Q�b�g���b�N�ύX
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
