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

    //�ǂ̃L�����N�^�[��I�񂾂�
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

    public void SelectOnClick(int num)
    {
        switch (num)
        {
            case 1: //�琬
                SceneManager.LoadScene("SelectScene_Traning");
                break;
            case 2: //�{�X
                SceneManager.LoadScene("SelectScene_Boss");
                break;
            case 3: //�L�����N�^�[�I�����
                SceneManager.LoadScene("TitleScene");
                break;
            case 4: //�琬����
                SceneManager.LoadScene("TrainingCompletedScene");
                break;
            default:
                SceneManager.LoadScene("HomeScene");
                break;
        }
    }
}
