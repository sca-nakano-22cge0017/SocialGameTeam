using System.Buffers;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class CharaStatus : MonoBehaviour
{
    [SerializeField,Header("育成終了ウィンドウ")] private GameObject resetWindow;
    [SerializeField,Header("チェックウィンドウ")] private GameObject checkWindow;
    //[SerializeField,Header("効果説明ウィンドウ")] private GameObject effectWindow;
    [SerializeField,Header("効果表示のテキスト")] private Text effectText;
    [SerializeField,Header("育成完了のfade")] private SpecialFade fade;
    [SerializeField,Header("育成完了の表示")] private GameObject resetFadeText;
    [SerializeField, Header("育成完了のテキスト")] private Text fadeText;

    string strEffect;
    string strReset;

    bool resetCheck = false;
    bool strplus = false;
    // Start is called before the first frame update
    void Start()
    {
        fade.EyeOpen();
        fade = FindObjectOfType<SpecialFade>();
        resetFadeText.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (resetCheck) //育成完了したら
        {
            strReset = "育成状況がリセットしました。\n";
            if (strplus)
            {
                strReset = strReset +
                            "プラス値が付与されました。\n" +
                            "対象ステータスの上限値が上昇します。";
            }
            fadeBool();
        }
        else //育成完了表示を非表示
        {
            resetFadeText.SetActive(false);
            strplus = false;
        }

        if (resetWindow.activeSelf) //効果が付与されたら
        {
            effectDescription();
        }

        //育成完了表示上でタップしたら
        if (Input.GetMouseButtonDown(0) && resetFadeText.activeSelf) 
        {
            resetCheck = false;
            ResetWindowScene("ResetHome");
            if (fade.IsFadeCompleted)
            {
                fade.IsFadeCompleted = false;
            }
        }
    }
    //リセットのボタン設定
    public void ResetWindowScene(string str)
    {
        switch (str)
        {
            case "Reset": //育成完了
                PlayerDataManager.TraningReset();
                Debug.Log("育成完了");
                resetCheck = true;
                checkWindow.SetActive(false);
                fade.EyeClose();
                break;
            case "ResetHome": //ホーム画面に戻る
                StartCoroutine(fadeClose());
                break;
            default:
                SceneLoader.LoadFade("TitleScene");
                break;
        }
    }
    //育成完了したら
    private void fadeBool()
    {
        if (fade.IsFadeCompleted)
        {
            fade.IsFadeCompleted = false;
            resetWindow.SetActive(false);
            resetFadeText.SetActive(true);
            fadeText.text = strReset.ToString();
        }
    }
    //効果のテキスト表示
    private void effectDescription()
    {
        strplus = true;
        strEffect = PlayerDataManager.GetResetEffects();
        effectText.text = strEffect.ToString();
    }
    private IEnumerator fadeClose()
    {
        fade.EyeOpen();
        yield return new WaitUntil(()=>fade.IsFadeCompleted);
        fade.IsFadeCompleted=false;
    }
}
