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
    [SerializeField,Header("スタミナの時間表示を何秒表示するか")] private float stopTime = 3.0f;
    private StaminaManager staminaManager;

    private int stamina;
    private int staminaMax;
    private string staminaTime; //i回復の時間

    // Start is called before the first frame update
    void Start()
    {
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
            staminaTimeText.text = "残り" + staminaTime.ToString();
        }
    }

    //スタミナの時間を表示するためのボタン
    public void OnStaminaButton()
    {
        staminaWindow.SetActive(true);
        StartCoroutine(StaminaTimeImage()); //時間表示
    }

    //時間表示のプログラム
    private IEnumerator StaminaTimeImage()
    {
        yield return new WaitForSeconds(stopTime);
        staminaWindow.SetActive(false);
    }
}
