using Spine;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public static class CharaNum
{
    // キャラクターの変数
    //1=>シスター 2=>剣士
    public static int selectChara = 0;
}

public class CharaSelect : MonoBehaviour
{
    [SerializeField,Header("キャラオブジェクト")] private GameObject CharaObjects = null;
    [SerializeField] private GameObject[] Window = null;
    [SerializeField] private GameObject[] chara = null;
    [SerializeField] private Text[] statusText = null;
    [SerializeField] private GameObject status;
    private TutorialWindow tutorialWindow;

    int count = 0;
    [SerializeField] private Animator[] animator = null;

    private void Awake()
    {
        CharaObjects.SetActive(false);
        status.gameObject.SetActive(false);
        Window[0].gameObject.SetActive(false);
        Window[1].gameObject.SetActive(false);
        StartCoroutine(Loader());
    }
    private void Start()
    {
        tutorialWindow = FindObjectOfType<TutorialWindow>();
        tutorialWindow.CharaSelect(); 

    }
    //どのキャラクターを選んだか
    //選択されたキャラとその育成状況によって変わるスプライト流用できるプログラムをchar作る
    //window非表示
    public void CharaOnClick(string name)
    {
        count++;
        Debug.Log(count);
        int c;
        switch (name)
        {
            case "SwordsWoman"://剣士
                c = 1;

                if (count == 1)
                {
                    Window[0].gameObject.SetActive(true);
                    chara[1].gameObject.SetActive(false);
                    status.transform.localPosition = new Vector2(1000, -90);
                    Debug.Log(status.transform.position.x);
                    status.gameObject.SetActive(true);

                    animator[0].SetBool("bool", true);
                }
                if (count >= 2)
                {
                    CharaObjects.SetActive (false);

                }
                break;
            case "Sister"://シスター
                c = 2;

                if (count == 1)
                {
                    Window[1].gameObject.SetActive(true);
                    chara[0].gameObject.SetActive(false);
                    status.transform.localPosition = new Vector2(200, -90);
                    status.gameObject.SetActive(true);

                    animator[1].SetBool("bool", true);
                }
                if (count >= 2)
                {
                    CharaObjects.SetActive(false);
                }
                break;
            default:
                c = -1;
                break;
        }
        GameManager.SelectChara = c;

        //初期ステータス表示
        statusText[0].text = PlayerDataManager.player.GetStatus(StatusType.HP).ToString();
        statusText[1].text = PlayerDataManager.player.GetStatus(StatusType.DEF).ToString();
        statusText[2].text = PlayerDataManager.player.GetStatus(StatusType.ATK).ToString();
        statusText[3].text = PlayerDataManager.player.GetStatus(StatusType.MP).ToString();
        statusText[4].text = PlayerDataManager.player.GetStatus(StatusType.AGI).ToString();
        statusText[5].text = PlayerDataManager.player.GetStatus(StatusType.DEX).ToString();
        if (count >= 2)
        {
            GameManager.isFirstStart = false;
            PlayerDataManager.Save();
        }

        
        if (!CharaObjects.activeSelf)
        {
            tutorialWindow.Home();
        }
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

    public void BG()
    {
        count = 0;
        chara[0].gameObject.SetActive(true);
        chara[1].gameObject.SetActive(true);
        Window[0].gameObject.SetActive(false);
        Window[1].gameObject.SetActive(false);

        status.gameObject.SetActive(false);

        animator[0].SetBool("bool", false);
        animator[1].SetBool("bool", false);
    }
}
