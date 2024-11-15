using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class HomeButtonController : MonoBehaviour
{

    [SerializeField, Header("リセットwindow")] private GameObject resetWindow = null;
    [SerializeField, Header("確認window")] private GameObject checkWindow = null;
    [SerializeField, Header("警告window")] private GameObject warningWindow = null;
    [SerializeField,Header("最終確認window")] private GameObject decisionWindow = null;

    bool effectCheck;
    // Start is called before the first frame update
    void Start()
    {
        resetWindow.SetActive(false);
        checkWindow.SetActive(false);
        warningWindow.SetActive(false);
        decisionWindow.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        //追加効果があるかを判定
        if (checkWindow.activeSelf)
        {
            effectCheck = PlayerDataManager.IsAddPlusStatus();
        }

        if (Input.GetMouseButtonDown(0) && warningWindow.activeSelf)
        {
            warningWindowChack();
        }
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
                SceneManager.LoadScene("CharaSelect");
                break;
            case "CheckInWindow": //確認Window
                checkWindow.SetActive(true);
                break;
            case "BackWindow": //ホーム画面に戻る
                Debug.Log("ホームに戻る");
                SceneManager.LoadScene("HomeScene");
                break;
            case "NextWindow": //追加効果があるか判定
                effectInCheck();
                break;
            default:
                SceneManager.LoadScene("TitleScene");
                break;
        }
    }

    //追加効果があるか判定
    private void effectInCheck()
    {
        //ある->リセットウィンドウ表示
        if (effectCheck)
        {
            resetWindow.SetActive(true);
        }
        else //ない->警告ウィンドウ表示
        {
            checkWindow.SetActive(false);
            warningWindow.SetActive(true);
        }
    }

    //警告を出して最終確認をする
    private void warningWindowChack()
    {
        warningWindow.SetActive(false);
        decisionWindow.SetActive(true);
    }
}
