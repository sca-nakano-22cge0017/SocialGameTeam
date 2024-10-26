using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class CharaNum
{
    // �L�����N�^�[�̕ϐ�
    //1=>�V�X�^�[ 2=>���m
    public static int selectChara = 0;
}

public class CharaSelect : MonoBehaviour
{
    [SerializeField] private GameObject CharaObjects = null;
    [SerializeField,Header("���Z�b�gwindow")] private GameObject resetWindow = null;
    [SerializeField,Header("�m�Fwindow")] private GameObject checkWindow = null;

    private void Start()
    {
        resetWindow.SetActive(false);
        checkWindow.SetActive(false);
    }

    //�ǂ̃L�����N�^�[��I�񂾂�
    //�I�����ꂽ�L�����Ƃ��̈琬�󋵂ɂ���ĕς��X�v���C�g���p�ł���v���O���������
    //window��\��
    public void CharaOnClick(string name)
    {
        int c;
        switch (name)
        {
            case "SwordsWoman":
                c = 1;
                break;
            case "Sister":
                c = 2;
                break;
            default:
                c = -1;
                break;
        }
        GameManager.SelectChara = c;
        CharaObjects.SetActive(false);
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
                checkWindow.SetActive(false);
                break;
            case "ResetMoveOn": //�琬������ʂɐi��
                resetWindow.SetActive(true);
                break;
            default:
                SceneManager.LoadScene("TitleScene");
                break;
        }
    }
}
