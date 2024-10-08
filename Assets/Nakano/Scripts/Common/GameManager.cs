using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ゲーム全体で使用する変数等を管理
/// </summary>
public class GameManager : MonoBehaviour
{
    // シングルトン
    private static GameManager instance;
    public static GameManager Instance
    {
        get
        {
            if (instance == null)
            {
                SetupInstance();
            }
            return instance;
        }
    }
    private static void SetupInstance()
    {
        instance = FindObjectOfType<GameManager>();

        if (instance == null)
        {
            GameObject prefab = Resources.Load<GameObject>("GameManager");
            var obj = Instantiate(prefab);
            instance = obj.GetComponent<GameManager>();
            DontDestroyOnLoad(obj);
        }
    }

    public static void GameManagerCreate()
    {
        SetupInstance();
    }

    // エラー判定用
    private const int errorNum = -1;

    // 選択キャラクター
    private static int selectChara = -1;

    /// <summary>
    /// 選択中キャラクター / 1:シスター , 2:剣士 , -1;エラー
    /// </summary>
    public static int SelectChara
    {
        get
        {
            if (selectChara == errorNum)
            {
                Debug.Log("選択ステージ：入力値がありません");
            }

            return selectChara;
        }
        set
        {
            if (value != 1 && value != 2)
            {
                Debug.Log("選択キャラクター：入力値が間違っています / 範囲外の入力です");
                return;
            }

            selectChara = value;
            Debug.Log("選択キャラクター：" + selectChara);
        }
    }

    // 選択ステージ
    private static int selectStage = -1;
    private const int stageIdDigids = 5; // StageIdの桁数

    /// <summary>
    /// 選択中ステージ　エリア番号(1桁)+ステージ番号(2桁)+難易度(2桁)の5桁
    /// </summary>
    public static int SelectStage
    {
        get
        {
            if (selectStage == errorNum)
            {
                Debug.Log("選択ステージ：入力値がありません");
            }
            return selectStage;
        }
        set
        {
            if ((int)Mathf.Log10(value) + 1 != stageIdDigids)
            {
                Debug.Log("選択ステージ：入力値が間違っています / 桁数が異なります");
                return;
            }

            selectStage = value;
            Debug.Log("選択ステージ：" + selectStage);
        }
    }

    [SerializeField] private StaminaManager staminaManager;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

        staminaManager.Initialize();
    }

    private void Update()
    {
        staminaManager.Recovery();
    }
}