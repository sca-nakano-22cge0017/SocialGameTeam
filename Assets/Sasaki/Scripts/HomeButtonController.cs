using System.Collections;
using System.Collections.Generic;
using UnityEditor.PackageManager.UI;
using UnityEngine;
using UnityEngine.SceneManagement;

public class HomeButtonController : MonoBehaviour
{

    [SerializeField, Header("���Z�b�gwindow")] private GameObject resetWindow = null;
    [SerializeField, Header("�m�Fwindow")] private GameObject checkWindow = null;
    [SerializeField, Header("�x��window")] private GameObject warningWindow = null;
    [SerializeField,Header("�ŏI�m�Fwindow")] private GameObject decisionWindow = null;

    [SerializeField,Header("�A�C�R������������window")] private GameObject iconWindow = null;
    [SerializeField,Header("������window")] private GameObject nmWindow = null;

    private TutorialWindow tutorialWindow;

    bool effectCheck;
    // Start is called before the first frame update
    void Start()
    {
        resetWindow.SetActive(false);
        checkWindow.SetActive(false);
        warningWindow.SetActive(false);
        decisionWindow.SetActive(false);
        nmWindow.SetActive(false); 
        iconWindow.SetActive(false);

        tutorialWindow = FindObjectOfType<TutorialWindow>();
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
                warningWindowChack();
            }
            if (nmWindow.activeSelf)
            {
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
                SceneManager.LoadScene("SelectScene_Traning");
                break;
            case "Boss": //�{�X
                SceneManager.LoadScene("SelectScene_Boss");
                break;
            case "Select": //�L�����N�^�[�I�����
                SceneManager.LoadScene("CharaSelect");
                break;
            case "CheckInWindow": //�m�FWindow
                checkWindow.SetActive(true);
                tutorialWindow.TraningReset();
                break;
            case "IconWindow":
                iconWindow.SetActive(true);
                break;
            case "DetachedButton":
                iconWindow.SetActive(false);
                break;
            case "HomeWindow": //�z�[����ʂɖ߂�
                SceneManager.LoadScene("HomeScene");
                break;
            case "NextWindow": //�ǉ����ʂ����邩����
                effectInCheck();
                break;
            case "NMWindow":
                nmWindow.SetActive(true);
                break;
            default:
                SceneManager.LoadScene("TitleScene");
                break;
        }
    }

    //�ǉ����ʂ����邩����
    private void effectInCheck()
    {
        //����->���Z�b�g�E�B���h�E�\��
        if (effectCheck)
        {
            resetWindow.SetActive(true);
        }
        else //�Ȃ�->�x���E�B���h�E�\��
        {
            checkWindow.SetActive(false);
            warningWindow.SetActive(true);
        }
    }

    //�x�����o���čŏI�m�F������
    private void warningWindowChack()
    {
        warningWindow.SetActive(false);
        decisionWindow.SetActive(true);
    }

    private void OnNMButton()
    {
        nmWindow.SetActive(false);
    }
}
