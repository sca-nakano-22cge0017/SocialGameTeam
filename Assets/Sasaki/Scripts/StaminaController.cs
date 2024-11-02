using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.UI;

public class StaminaController : MonoBehaviour
{
    [SerializeField,Header("スライダーバー")] private Slider slider;
    [SerializeField,Header("スタミナ表示のテキスト")] private Text staminaText;
    [SerializeField,Header("スタミナの時間表示ウィンドウ")] private GameObject staminaWindow;
    [SerializeField,Header("スタミナの時間テキスト")] private Text staminaTimeText;
    private StaminaManager staminaManager;

    private int stamina;
    private int staminaMax;
    private string staminaTime; //1回復の時間

    int tapNum = 0; //押した回数

    // Start is called before the first frame update
    void Start()
    {
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

        slider.value = 1; //スライダー満タン
    }

    // Update is called once per frame
    void Update()
    {
        stamina = staminaManager.Stamina;
        slider.value = (float)stamina/(float)staminaMax;//バー表示反映
        //テキスト表示
        staminaText.text = stamina.ToString() + "/" + staminaMax.ToString();

        //タイム表示
        if (staminaWindow.activeSelf)
        {
            staminaTime = staminaManager.RecoveryTimeText;
            staminaTimeText.text = "次の回復まで残り" + staminaTime.ToString();
        }
    }

    //スタミナの時間を表示するためのボタン
    public void OnStaminaButton()
    {
        tapNum += 1;
        //スタミナを押しても時間表示ウィンドウを非表示にする
        if (tapNum <= 1)
        {
            staminaWindow.SetActive(true);
        }
        else
        {
            staminaWindow.SetActive(false);
            tapNum = 0;
        }
    }

    //時間表示のプログラム
    public void StaminaHidden()
    {
        staminaWindow.SetActive(false);
        tapNum = 0;
    }
}
