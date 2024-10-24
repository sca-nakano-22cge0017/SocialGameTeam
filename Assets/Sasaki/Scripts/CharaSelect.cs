using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class CharaNum
{
    // キャラクターの変数
    //1=>シスター 2=>剣士
    public static int selectChara = 0;
}

public class CharaSelect : MonoBehaviour
{
    [SerializeField] private GameObject CharaObjects = null;
    [SerializeField,Header("リセットwindow")] private GameObject resetWindow = null;

    private void Start()
    {
        resetWindow.SetActive(false);
    }

    //どのキャラクターを選んだか
    //window非表示
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
            case 1: //育成
                SceneManager.LoadScene("SelectScene_Traning");
                break;
            case 2: //ボス
                SceneManager.LoadScene("SelectScene_Boss");
                break;
            case 3: //キャラクター選択画面
                SceneManager.LoadScene("TitleScene");
                break;
            case 4: //育成完了ウィンドウ表示
                resetWindow.SetActive(true);
                break;
            case 5: //ホーム画面に戻る
                resetWindow.SetActive(false);
                break;
            case 6: //育成完了
                SceneManager.LoadScene("TitleScene");
                break;
            default:
                SceneManager.LoadScene("HomeScene");
                break;
        }
    }
}
