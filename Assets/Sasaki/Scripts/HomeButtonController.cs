using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HomeButtonController : MonoBehaviour
{

    [SerializeField, Header("リセットwindow")] private GameObject resetWindow = null;
    [SerializeField, Header("確認window")] private GameObject checkWindow = null;
    [SerializeField, Header("警告window")] private GameObject warningWindow = null;
    [SerializeField,Header("最終確認window")] private GameObject decisionWindow = null;

    [SerializeField,Header("未実装window")] private GameObject nmWindow = null;

    private TutorialWindow tutorialWindow;

    bool effectCheck;

    private SoundController soundController;
    // Start is called before the first frame update
    void Start()
    {
        resetWindow.SetActive(false);
        checkWindow.SetActive(false);
        warningWindow.SetActive(false);
        decisionWindow.SetActive(false);
        nmWindow.SetActive(false); 

        tutorialWindow = FindObjectOfType<TutorialWindow>();
        soundController = FindObjectOfType<SoundController>();
    }

    // Update is called once per frame
    void Update()
    {
        //追加効果があるかを判定
        if (checkWindow.activeSelf)
        {
            effectCheck = PlayerDataManager.IsAddPlusStatus();
        }

        if (Input.GetMouseButtonDown(0))
        {
            if (warningWindow.activeSelf)
            {
                //soundController.PlayTap1SE();
                warningWindowChack();
            }
            if (nmWindow.activeSelf)
            {
                soundController.PlayTap1SE();
                OnNMButton();
            }
        }
    }

    //ホーム画面のボタン設定
    public void SelectOnClick(string str)
    {
        switch (str)
        {
            case "Traning": //育成
                SceneLoader.LoadFade("SelectScene_Traning");
                break;
            case "Boss": //ボス
                SceneLoader.LoadFade("SelectScene_Boss");
                break;
            case "Select": //キャラクター選択画面
                SceneLoader.LoadFade("CharaSelect");
                break;
            case "CheckInWindow": //確認Window
                checkWindow.SetActive(true);
                tutorialWindow.TraningReset();
                break;
            case "HomeWindow": //ホーム画面に戻る
                SceneLoader.Load("HomeScene");
                break;
            case "NextWindow": //追加効果があるか判定
                effectInCheck();
                break;
            case "NMWindow": //未実装window
                nmWindow.SetActive(true);
                break;
            default:
                SceneLoader.LoadFade("TitleScene");
                break;
        }
    }

    //追加効果があるか判定
    private void effectInCheck()
    {
        checkWindow.SetActive(false);
        //ある->リセットウィンドウ表示
        if (effectCheck)
        {
            resetWindow.SetActive(true);
        }
        else //ない->警告ウィンドウ表示
        {
            warningWindow.SetActive(true);
        }
    }

    //警告を出して最終確認をする
    private void warningWindowChack()
    {
        warningWindow.SetActive(false);
        decisionWindow.SetActive(true);
    }

    private void OnNMButton()
    {
        nmWindow.SetActive(false);
    }
}
