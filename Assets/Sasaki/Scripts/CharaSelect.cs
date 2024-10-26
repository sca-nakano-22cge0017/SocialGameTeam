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
    [SerializeField,Header("確認window")] private GameObject checkWindow = null;

    private void Start()
    {
        resetWindow.SetActive(false);
        checkWindow.SetActive(false);
    }

    //どのキャラクターを選んだか
    //選択されたキャラとその育成状況によって変わるスプライト流用できるプログラムを作る
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
    //ホーム画面のボタン設定
    public void SelectOnClick(string str)
    {
        switch (str)
        {
            case "Traning": //育成
                SceneManager.LoadScene("SelectScene_Traning");
                break;
            case "Boss": //ボス
                SceneManager.LoadScene("SelectScene_Boss");
                break;
            case "Select": //キャラクター選択画面
                SceneManager.LoadScene("TitleScene");
                break;
            case "CheckInWindow": //確認Window
                checkWindow.SetActive(true);
                break;
            case "BackWindow": //ホーム画面に戻る
                checkWindow.SetActive(false);
                break;
            case "ResetMoveOn": //育成完了画面に進む
                resetWindow.SetActive(true);
                break;
            default:
                SceneManager.LoadScene("TitleScene");
                break;
        }
    }
}
