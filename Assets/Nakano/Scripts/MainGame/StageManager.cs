using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class EnemiesIllust
{
    public string enemyId;
    public Sprite sprite;
    public Animation anim;
}

[System.Serializable]
public class PlayerIllust
{
    public int charaId;
    public CombiType evolutionType; // 形態
    public Sprite sprite;
    public Animation anim;
}

/// <summary>
/// ステージ上のプレイヤーや敵の表示・初期設定制御
/// </summary>
public class StageManager : MonoBehaviour
{
    [SerializeField] private PlayerData player;
    [SerializeField, Header("プレイヤーのイラスト/アニメーション")] private PlayerIllust[] playersIllust;
    [SerializeField] private Enemy[] enemies;
    [SerializeField, Header("敵のイラスト/アニメーション")] private EnemiesIllust[] enemiesIllust;

    StageDataManager stageDataManager;

    /// <summary>
    /// セッティング完了
    /// </summary>
    public bool isSetCompleted = false;

    [Header("確認用")]
    [SerializeField, Range(1, 2), Header("1：育成　2：ボス")] private int areaId = 1;
    [SerializeField, Range(1, 6), Header("ステージ番号")] private int stageId = 1;
    [SerializeField, Range(1, 5), Header("難易度")] private int difficultyId = 1;

    [SerializeField] WindowController windowController;

    private void Awake()
    {
        isSetCompleted = false;
        Load();
    }

    private void Start()
    {
        Load();
    }

    void Update()
    {
        
    }

    private void Load()
    {
        if (isSetCompleted) return;

        stageDataManager = FindObjectOfType<StageDataManager>();

        if (!stageDataManager) return;

        // ステージデータ未ロードなら
        if (!StageDataManager.StageDataLoadComplete)
        {
            // ロード完了後の処理
            stageDataManager.LoadCompleteProcess += () =>
            {
                Setting();
            };

            // 指定したステージのデータをロード
            GameManager.SelectDifficulty = difficultyId;
            GameManager.SelectArea = areaId;
            GameManager.SelectStage = stageId;

            stageDataManager.LoadData(difficultyId, areaId, stageId);
        }
        
        else
        {
            Setting();
        }
    }

    void Setting()
    {
        PlayerDataSet();
        EnemyDataSet();
        isSetCompleted = true;
    }

    /// <summary>
    /// プレイヤー設定
    /// </summary>
    void PlayerDataSet()
    {
        // ステータス取得
        Status status = new(PlayerDataManager.player.AllStatus);
        player.ATK = status.atk;
        player.MP = status.mp;
        player.HP = status.hp;
        player.DEF = status.def;
        player.AGI = status.agi;
        player.DEX = status.dex;
        
        // 必殺ゲージ
        Master.CharaInitialStutas statusData = PlayerDataManager.player.StatusData;
        player.specialMoveGuageMax = statusData.specialMoveGuagesSetting[0].guageMaxAmount;
        player.sm_NormalAttack = statusData.specialMoveGuagesSetting[0];
        player.sm_Guard = statusData.specialMoveGuagesSetting[1];
        player.sm_Damage = statusData.specialMoveGuagesSetting[2];
        player.sm_Turn = statusData.specialMoveGuagesSetting[3];
        player.sm_Skill = statusData.specialMoveGuagesSetting[4];

        // 攻撃倍率
        if (GameManager.SelectChara == 1)
        {
            player.power_NormalAttack = 1.2f;
            player.power_Skill = 1.0f;
            player.power_Critical = 1.5f;
            player.criticalProbability = 10;
            player.power_SpecialMove = 10;
        }
        if (GameManager.SelectChara == 2)
        {
            player.power_NormalAttack = 0.9f;
            player.power_Skill = 1.3f;
            player.power_Critical = 1.5f;
            player.criticalProbability = 10;
            player.power_SpecialMove = 10;
        }

        // 表示
        for (int i = 0; i < playersIllust.Length; i++)
        {
            if (playersIllust[i].charaId == GameManager.SelectChara &&
                playersIllust[i].evolutionType == PlayerDataManager.player.GetEvolutionType())
            {
                player.image.sprite = playersIllust[i].sprite;
            }
        }

        player.Initialize();
    }

    /// <summary>
    /// 敵の設定
    /// </summary>
    void EnemyDataSet()
    {
        for (int e = 0; e < enemies.Length; e++)
        {
            enemies[e].gameObject.SetActive(false);
        }

        List<Master.EnemyData> data = StageDataManager.EnemiesData;
        for (int d = 0; d < data.Count; d++)
        {
            for (int e = 0; e < enemies.Length; e++)
            {
                if (data[d].placementId != enemies[e].POSITION) continue;

                // ステータス取得
                enemies[e].HP = data[d].enemyStatus.hp;
                enemies[e].ATK = data[d].enemyStatus.atk;
                enemies[e].MP = data[d].enemyStatus.mp;
                enemies[e].DEF = data[d].enemyStatus.def;
                enemies[e].DEX = data[d].enemyStatus.dex;
                enemies[e].AGI = data[d].enemyStatus.spd;

                // アタックパターン取得
                for (int i = 0; i < data[d].enemyStatus.attackPattern.Count; i++)
                {
                    enemies[e].attackPattern.Add(data[d].enemyStatus.attackPattern[i]);
                }

                // 表示
                for (int i = 0; i < enemiesIllust.Length; i++)
                {
                    if (data[d].enemyStatus.enemyId.Substring(0, 1) == enemiesIllust[i].enemyId)
                    {
                        // イラスト変更
                        enemies[e].image.sprite = enemiesIllust[i].sprite;

                        // Todo アニメーション/Spineの変更
                    }
                }

                enemies[e].gameObject.SetActive(true);
                enemies[e].Initialize();
            }
        }
    }
}
