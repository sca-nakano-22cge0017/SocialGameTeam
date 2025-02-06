using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class HomeButtonController : MonoBehaviour
{

    [SerializeField, Header("���Z�b�gwindow")] private GameObject resetWindow = null;
    [SerializeField, Header("�m�Fwindow")] private GameObject checkWindow = null;
    [SerializeField, Header("�x��window")] private GameObject warningWindow = null;
    [SerializeField,Header("�ŏI�m�Fwindow")] private GameObject decisionWindow = null;

    [SerializeField,Header("������window")] private GameObject nmWindow = null;

    private TutorialWindow tutorialWindow;

    bool effectCheck;

    private SoundController soundController;

    [SerializeField, Header("�m�FWindow�̊J����")] private WindowController checkWC;
    [SerializeField, Header("�x��Window�̊J����")] private WindowController warningWC;
    [SerializeField, Header("�ŏI�m�FWindow�̊J����")] private WindowController decisionWC;
    [SerializeField, Header("������Window�̊J����")] private WindowController nmWC;

    // Start is called before the first frame update
    void Start()
    {
        resetWindow.SetActive(false);
        checkWindow.SetActive(false);
        warningWindow.SetActive(false);
        decisionWindow.SetActive(false);
        nmWindow.SetActive(false); 

        tutorialWindow = FindObjectOfType<TutorialWindow>();
        soundController = FindObjectOfType<SoundController>();
    }

    // Update is called once per frame
    void Update()
    {
        //�ǉ����ʂ����邩�𔻒�
        if (checkWindow.activeSelf)
        {
            effectCheck = PlayerDataManager.IsAddPlusStatus();
        }

        if (Input.GetMouseButtonDown(0))
        {
            if (warningWindow.activeSelf)
            {
                //soundController.PlayTap1SE();
                warningWindowChack();
            }
            if (nmWindow.activeSelf)
            {
                soundController.PlayTap1SE();
                OnNMButton();
            }
        }
    }

    //�z�[����ʂ̃{�^���ݒ�
    public void SelectOnClick(string str)
    {
        switch (str)
        {
            case "Traning": //�琬
                SceneLoader.LoadFade("SelectScene_Traning");
                break;
            case "Boss": //�{�X
                SceneLoader.LoadFade("SelectScene_Boss");
                break;
            case "Select": //�L�����N�^�[�I�����
                SceneLoader.LoadFade("CharaSelect");
                break;
            case "CheckInWindow": //�m�FWindow
                checkWC.Open();
                //checkWindow.SetActive(true);
                tutorialWindow.TraningReset();
                break;
            case "HomeWindow": //�z�[����ʂɖ߂�
                SceneLoader.Load("HomeScene");
                break;
            case "NextWindow": //�ǉ����ʂ����邩����
                effectInCheck();
                break;
            case "NMWindow": //������window
                nmWC.Open();
                //nmWindow.SetActive(true);
                break;
            case "Title":
                SceneManager.LoadScene("TitleScene");
                break;
            default:
                SceneLoader.LoadFade("TitleScene");
                break;
        }
    }

    //�ǉ����ʂ����邩����
    private void effectInCheck()
    {
        checkWC.Close(false);
        //checkWindow.SetActive(false);
        //����->���Z�b�g�E�B���h�E�\��
        if (effectCheck)
        {
            resetWindow.SetActive(true);
        }
        else //�Ȃ�->�x���E�B���h�E�\��
        {
            //warningWindow.SetActive(true);
            warningWC.Open();
        }
    }

    //�x�����o���čŏI�m�F������
    private void warningWindowChack()
    {
        //warningWindow.SetActive(false);
        //decisionWindow.SetActive(true);

        warningWC.Close();
        decisionWC.Open();
    }

    private void OnNMButton()
    {
        nmWC.Close(false);
        //nmWindow.SetActive(false);
    }
}
