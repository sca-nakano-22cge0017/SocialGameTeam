using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.UI;

public class StaminaController : MonoBehaviour
{
    [SerializeField,Header("スライダーバー")] private Slider slider;
    [SerializeField,Header("スタミナ表示のテキスト")] private Text staminaText;
    private StaminaManager staminaManager;

    private int stamina;
    private int staminaMax;

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
    }
}
