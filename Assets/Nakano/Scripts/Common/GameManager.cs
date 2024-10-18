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
    /// 選択中キャラクター / 1:剣士, 2:シスター, -1:エラー
    /// </summary>
    public static int SelectChara
    {
        get
        {
            if (selectChara == errorNum)
            {
                Debug.Log("選択キャラクター：入力値がありません。1を入力します。");
                selectChara = 1;
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
        }
    }

    /// <summary>
    /// 初回のキャラ選択
    /// </summary>
    /// <param name="_id">キャラクターID / 1:剣士, 2:シスター</param>
    public static void FirstCharaSelect(int _id)
    {
        if (_id != 1 && _id != 2)
        {
            Debug.Log("初回キャラクター選択：入力値が間違っています。");
            return;
        }

        SelectChara = _id;

        PlayerDataManager.PlayerCreate(SelectChara);

        Status status = PlayerDataManager.player.AllStatus;
        Debug.Log(string.Format("キャラクターID:{6}, HP:{0}, MP:{1}, ATK:{2}, DEF:{3}, SPD:{4}, DEX:{5}",
            status.hp, status.mp, status.atk, status.def, status.spd, status.dex, SelectChara));
    }

    // 選択難易度
    private static int selectDifficulty = -1;
    /// <summary>
    /// 選択中難易度 1～5 , -1:エラー
    /// </summary>
    public static int SelectDifficulty
    {
        get
        {
            if (selectDifficulty == errorNum)
            {
                Debug.Log("選択難易度：入力値がありません");
            }
            return selectDifficulty;
        }
        set
        {
            if (value < 0)
            {
                Debug.Log("選択難易度：入力値が間違っています");
                return;
            }

            selectDifficulty = value;
        }
    }

    // 選択エリア
    private static int selectArea = -1;
    /// <summary>
    /// 選択中エリア / 1:育成 , 2:ボス , -1;エラー
    /// </summary>
    public static int SelectArea
    {
        get
        {
            if (selectArea == errorNum)
            {
                Debug.Log("選択エリア：入力値がありません");
            }
            return selectArea;
        }
        set
        {
            if (value < 0)
            {
                Debug.Log("選択エリア：入力値が間違っています");
                return;
            }

            selectArea = value;
        }
    }

    // 選択ステージ
    private static int selectStage = -1;
    /// <summary>
    /// 選択中ステージ 1～6 , -1:エラー
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
            if (value < 0)
            {
                Debug.Log("選択ステージ：入力値が間違っています");
                return;
            }

            selectStage = value;
        }
    }

    [SerializeField] private StaminaManager staminaManager;
    [SerializeField] private MasterDataLoader masterDataLoader;

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

        ScreenOperationSetting();
        staminaManager.Initialize();
        masterDataLoader.DataLoad();

        StartCoroutine(LoadComplete());
    }

    private void Update()
    {
        staminaManager.Recovery();
    }

    IEnumerator LoadComplete()
    {
        yield return new WaitUntil(() => MasterDataLoader.MasterDataLoadComplete);

        FirstCharaSelect(SelectChara);
    }

    /// <summary>
    /// 画面回転設定
    /// </summary>
    private void ScreenOperationSetting()
    {
        // 左向きを有効にする
        Screen.autorotateToLandscapeLeft = true;
        // 右向きを有効にする
        Screen.autorotateToLandscapeRight = true;

        // 画面の向きを自動回転に設定する
        Screen.orientation = ScreenOrientation.AutoRotation;
    }
}