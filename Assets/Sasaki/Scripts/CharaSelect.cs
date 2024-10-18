using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CharaSelect : MonoBehaviour
{
    [SerializeField] private GameObject CharaObjects = null;
    public int charaNum;

    //どのキャラクターを選んだか
    //window非表示
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
            Debug.Log("シスター");
        }
        if (charaNum == 2)
        {
            Debug.Log("剣士");
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
            case 1: //育成
                SceneManager.LoadScene("SelectScene_Traning");
                break;
            case 2: //ボス
                SceneManager.LoadScene("SelectScene_Boss");
                break;
            case 3: //記録
                SceneManager.LoadScene("TitleScene");
                break;
            case 4: //性能
                SceneManager.LoadScene("TitleScene");
                break;
            case 5: //育成完了
                SceneManager.LoadScene("TrainingCompletedScene");
                break;
            default:
                SceneManager.LoadScene("HomeScene");
                break;
        }
    }
}
