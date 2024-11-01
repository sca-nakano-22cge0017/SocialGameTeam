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
            if (!MasterDataLoader.MasterDataLoadComplete)
            {
                //Debug.Log("マスターデータの読み込みが完了していません");
            }

            if (selectChara == errorNum)
            {
                //Debug.Log("選択キャラクター：入力値がありません。");
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

            //Status status = PlayerDataManager.player.AllStatus;
            //Debug.Log(string.Format("キャラクターID:{6}, HP:{0}, MP:{1}, ATK:{2}, DEF:{3}, SPD:{4}, DEX:{5}",
            //    status.hp, status.mp, status.atk, status.def, status.spd, status.dex, selectChara));
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

        staminaManager.Initialize();
        masterDataLoader.DataLoad();

        StartCoroutine(LoadComplete());
    }

    private void Update()
    {
        staminaManager.Recovery();

        // Test用
        if (Input.GetKeyDown(KeyCode.C))
        {
            PlayerDataManager.TraningReset();
        }
        if (Input.GetKeyDown(KeyCode.A))
        {
            DropController d = FindObjectOfType<DropController>();
            d.DropLottery();
        }
        if (Input.GetKeyDown(KeyCode.D))
        {
            DropController d = FindObjectOfType<DropController>();
            d.Initialize();
        }
    }

    IEnumerator LoadComplete()
    {
        yield return new WaitUntil(() => MasterDataLoader.MasterDataLoadComplete);
        PlayerDataManager.Load();

        yield return new WaitUntil(() => PlayerDataManager.PlayerDataLoadComplete);
        SelectChara = 1;
    }
}