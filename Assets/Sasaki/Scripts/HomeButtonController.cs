using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class HomeButtonController : MonoBehaviour
{

    [SerializeField, Header("���Z�b�gwindow")] private GameObject resetWindow = null;
    [SerializeField, Header("�m�Fwindow")] private GameObject checkWindow = null;

    // Start is called before the first frame update
    void Start()
    {
        resetWindow.SetActive(false);
        checkWindow.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
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
                checkWindow.SetActive(false);
                break;
            default:
                SceneManager.LoadScene("TitleScene");
                break;
        }
    }
}
