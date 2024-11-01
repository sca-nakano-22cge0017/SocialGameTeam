using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// PlayerDataManager.TraningReset()でデータ初期化
/// </summary>
public class CharaStatus : MonoBehaviour
{
    [SerializeField,Header("育成終了ウィンドウ")] private GameObject ResetWindow;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    //リセットのボタン設定
    public void ResetWindowScene(string str)
    {
        switch (str)
        {
            case "Reset": //育成完了(タイトルに戻る)
                //PlayerDataManager.TraningReset();
                SceneManager.LoadScene("TitleScene");
                break;
            case "HomeWindow": //ホームウィンドウに戻る
                ResetWindow.SetActive(false);
                break;
            default:
                SceneManager.LoadScene("TitleScene");
                break;
        }
    }
}
