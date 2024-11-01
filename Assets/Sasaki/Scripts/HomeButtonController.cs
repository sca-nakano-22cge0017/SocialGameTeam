using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class HomeButtonController : MonoBehaviour
{

    [SerializeField, Header("リセットwindow")] private GameObject resetWindow = null;
    [SerializeField, Header("確認window")] private GameObject checkWindow = null;

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
                checkWindow.SetActive(false);
                break;
            default:
                SceneManager.LoadScene("TitleScene");
                break;
        }
    }
}
