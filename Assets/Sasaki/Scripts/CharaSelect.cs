using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class CharaNum
{
    // キャラクターの変数
    //1=>シスター 2=>剣士
    public static int selectChara = 0;
}

public class CharaSelect : MonoBehaviour
{
    [SerializeField,Header("キャラオブジェクト")] private GameObject CharaObjects = null;
    private TutorialWindow tutorialWindow;

    private void Awake()
    {
        CharaObjects.SetActive(false);
        StartCoroutine(Loader());
    }
    private void Start()
    {
        tutorialWindow = FindObjectOfType<TutorialWindow>();
        tutorialWindow.CharaSelect();
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
        GameManager.isFirstStart = false;
        PlayerDataManager.Save();
        CharaObjects.SetActive(false);
        tutorialWindow.Home();
    }
    IEnumerator Loader()
    {
        yield return new WaitUntil(() => MasterDataLoader.MasterDataLoadComplete);

        yield return new WaitUntil(() => PlayerDataManager.PlayerDataLoadComplete);

        if (GameManager.isFirstStart)
        {
            CharaObjects.SetActive(true);
        }
        else
        {
            CharaObjects.SetActive(false);
        }
    }
}
