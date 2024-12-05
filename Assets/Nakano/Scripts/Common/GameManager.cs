using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

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

    [RuntimeInitializeOnLoadMethod]
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

    /// <summary>
    /// 初回起動かどうか
    /// </summary>
    public static bool isFirstStart = true;

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
                Debug.Log("選択キャラクター：入力値がありません。キャラクター1を選択します");
                selectChara = 1;
                PlayerDataManager.CharacterChange(selectChara);
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

            PlayerDataManager.CharacterChange(selectChara);
        }
    }

    // 選択難易度
    private static int selectDifficulty = 1;
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

    /// <summary>
    /// ボスクリア状況
    /// </summary>
    private static bool[] isBossClear = { false, false, false, false, false };
    public static bool[] IsBossClear { get => isBossClear; set => isBossClear = value; }

    [SerializeField] private StaminaManager staminaManager;
    [SerializeField] private MasterDataLoader masterDataLoader;
    [SerializeField] private SpecialTecniqueManager specialTecniqueManager;
    [SerializeField] private int fps;

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

        masterDataLoader.DataLoad();

        StartCoroutine(LoadComplete());
    }

    private void Update()
    {
        Application.targetFrameRate = fps;
        staminaManager.Recovery();

        // Test用
        if (Input.GetKeyDown(KeyCode.C))
        {
            PlayerDataManager.TraningReset();
        }
        if (Input.GetKeyDown(KeyCode.Delete))
        {
            CharaSelectManager.DeleteSaveData();
        }
    }

    IEnumerator LoadComplete()
    {
        yield return new WaitUntil(() => MasterDataLoader.MasterDataLoadComplete);
        PlayerDataManager.Load();

        yield return new WaitUntil(() => PlayerDataManager.PlayerDataLoadComplete);
        specialTecniqueManager.Load();

        staminaManager.Initialize();
    }
}