using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.UI;

public class StaminaController : MonoBehaviour
{
    [SerializeField,Header("スライダーバー")] private Image slider;
    [SerializeField,Header("スタミナ表示のテキスト")] private Text staminaText;
    [SerializeField,Header("スタミナの時間表示ウィンドウ")] private GameObject staminaWindow;
    [SerializeField,Header("スタミナの時間テキスト")] private Text staminaTimeText;
    private StaminaManager staminaManager;

    private int stamina;
    private int staminaMax;
    private string staminaTime; //1回復の時間

    [SerializeField, Header("アイコンを押した時window")] private GameObject iconWindow = null;

    private SoundController soundController;

    // Start is called before the first frame update
    void Start()
    {
        soundController = FindObjectOfType<SoundController>();

        iconWindow.SetActive(false);

        staminaWindow.SetActive(false);
        staminaManager = FindObjectOfType<StaminaManager>();
        //staminaManagerがnullだったら作る
        if (staminaManager == null)
        {
            GameManager.GameManagerCreate();
            GameManager gameManager;
            gameManager = GameManager.Instance;
            staminaManager = gameManager.gameObject.GetComponent<StaminaManager>();
        }

        staminaMax = staminaManager.Stamina_Max;

        slider.fillAmount = 1; //スライダー満タン
    }

    // Update is called once per frame
    void Update()
    {
        stamina = staminaManager.Stamina;
        slider.fillAmount = (float)stamina/(float)staminaMax;//バー表示反映
        //テキスト表示
        staminaText.text = stamina.ToString() + "/" + staminaMax.ToString();

        //タイム表示
        if (staminaWindow.activeSelf)
        {
            staminaTime = staminaManager.RecoveryTimeText;
            staminaTimeText.text = "次の回復まで残り" + staminaTime.ToString();
        }

        if (Input.GetMouseButtonDown(0) && staminaWindow.activeSelf) 
        {
            StaminaHidden();
        }
    }

    //スタミナの時間を表示するためのボタン
    public void OnStaminaButton()
    {
        staminaWindow.SetActive(true);
    }

    //時間表示のプログラム
    void StaminaHidden()
    {
        soundController.PlayTap1SE();
        staminaWindow.SetActive(false);
    }

    public void OnIconWindow(string str)
    {
        switch (str)
        {
            case "IconWindow": //アイコンボタン
                iconWindow.SetActive(true);
                break;
            case "DetachedButton": //アイコンボタンを離した時
                iconWindow.SetActive(false);
                break;
            case "HomeWindow": //ホーム画面に戻る
                soundController.PlayTap2SE();
                SceneLoader.Load("HomeScene");
                break;
            case "BackButton": //1つ前のシーンに戻る
                soundController.PlayTap2SE();
                SceneLoader.Back();
                break;
            default:
                SceneLoader.LoadFade("TitleScene");
                break;
        }
    }
}
