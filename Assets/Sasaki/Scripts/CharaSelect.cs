using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CharaSelect : MonoBehaviour
{
    [SerializeField] private GameObject CharaObjects = null;
    public int charaNum;

    //�ǂ̃L�����N�^�[��I�񂾂�
    //window��\��
    public void CharaOnClick(string name)
    {
        int c;
        switch (name)
        {
            case "Sister":
                c = 1;
                break;
            case "SwordsWoman":
                c = 2;
                break;
            default:
                c = -1;
                break;
        }
        charaNum = c;
        if (charaNum == 1)
        {
            Debug.Log("�V�X�^�[");
        }
        if (charaNum == 2)
        {
            Debug.Log("���m");
        }
        else
        {
            Debug.Log("error");
        }
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
            case 3: //�L�^
                SceneManager.LoadScene("TitleScene");
                break;
            case 4: //���\
                SceneManager.LoadScene("TitleScene");
                break;
            case 5: //�琬����
                SceneManager.LoadScene("TrainingCompletedScene");
                break;
            default:
                SceneManager.LoadScene("HomeScene");
                break;
        }
    }
}
