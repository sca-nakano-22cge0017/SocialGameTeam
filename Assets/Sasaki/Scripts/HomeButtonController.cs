using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class HomeButtonController : MonoBehaviour
{

    [SerializeField, Header("���Z�b�gwindow")] private GameObject resetWindow = null;
    [SerializeField, Header("�m�Fwindow")] private GameObject checkWindow = null;
    [SerializeField, Header("�x��window")] private GameObject warningWindow = null;

    bool effectCheck;
    // Start is called before the first frame update
    void Start()
    {
        resetWindow.SetActive(false);
        checkWindow.SetActive(false);
        warningWindow.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        //�ǉ����ʂ����邩�𔻒�
        if (checkWindow.activeSelf)
        {
            effectCheck = PlayerDataManager.IsAddPlusStatus();
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
                SceneManager.LoadScene("TitleScene");
                break;
            case "CheckInWindow": //�m�FWindow
                checkWindow.SetActive(true);
                break;
            case "BackWindow": //�z�[����ʂɖ߂�
                SceneManager.LoadScene("HomeScene");
                break;
            case "NextWindow": //�ǉ����ʂ����邩����
                effectInCheck();
                break;
            case "ResetWindow": //�琬������ʂɐi��
                warningWindow.SetActive(false);
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
}