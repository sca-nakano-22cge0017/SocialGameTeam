using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class CharaStatus : MonoBehaviour
{
    [SerializeField,Header("育成終了ウィンドウ")] private GameObject resetWindow;
    //[SerializeField,Header("効果説明ウィンドウ")] private GameObject effectWindow;
    [SerializeField,Header("効果表示のテキスト")] private Text effectText;

    string strEffect;
    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        if (resetWindow.activeSelf)
        {
            effectDescription();
        }
    }
    //リセットのボタン設定
    public void ResetWindowScene(string str)
    {
        switch (str)
        {
            case "Reset": //育成完了(ホームに戻る)
                PlayerDataManager.TraningReset();
                Debug.Log("育成完了");
                SceneManager.LoadScene("HomeScene");
                break;
            default:
                SceneManager.LoadScene("TitleScene");
                break;
        }
    }

    private void effectDescription()
    {
        strEffect = PlayerDataManager.GetResetEffects();
        effectText.text = strEffect.ToString();
    }
}
