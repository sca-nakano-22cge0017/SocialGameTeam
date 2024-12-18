using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class EnemiesIllust
{
    public string enemyId;
    public int imageId;
    public GameObject prefab;
}

[System.Serializable]
public class PlayerIllust
{
    public int charaId;
    public CombiType evolutionType; // 形態
    public GameObject playerObj;
    public GameObject playerIllust;
    public Sprite specialAttackIcon;
    public SpineAnim spineAnim;
}

/// <summary>
/// ステージ上のプレイヤーや敵の表示・初期設定制御
/// </summary>
public class StageManager : MonoBehaviour
{
    [SerializeField] private Image specialAttackIcon;
    [SerializeField] private GameObject bossHpGuage;

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
    [SerializeField, Header("レア敵出現率")] private float rareEnemyApp = 100;

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
            stageDataManager.LoadCompleteProcess = () =>
            {
                Setting();
                playersIllust[0].playerObj.SetActive(true);
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
            player.power_CriticalInit = 1.5f;
            player.criticalProbabilityInitial = 10;
            player.power_SpecialMove = 10;
        }
        if (GameManager.SelectChara == 2)
        {
            player.power_NormalAttack = 0.9f;
            player.power_Skill = 1.3f;
            player.power_CriticalInit = 1.5f;
            player.criticalProbabilityInitial = 10;
            player.power_SpecialMove = 10;
        }

        // 表示
        for (int i = 0; i < playersIllust.Length; i++)
        {
            if (playersIllust[i].charaId == GameManager.SelectChara &&
                playersIllust[i].evolutionType == PlayerDataManager.player.GetEvolutionType())
            {
                player.meshRenderer = playersIllust[i].playerIllust.gameObject.GetComponent<MeshRenderer>();
                player.motion = playersIllust[i].playerIllust.GetComponent<Animator>();
                player.spineAnim = playersIllust[i].spineAnim;

                specialAttackIcon.sprite = playersIllust[i].specialAttackIcon;

                playersIllust[i].playerObj.SetActive(true);
            }
        }

        player.Initialize();
    }

    /// <summary>
    /// 敵の設定
    /// </summary>
    void EnemyDataSet()
    {
        Debug.Log("敵情報ロード");
        for (int e = 0; e < enemies.Length; e++)
        {
            enemies[e].gameObject.SetActive(false);
        }

        List<Master.EnemyData> data = StageDataManager.EnemiesData;
        List<Master.EnemyData> rareData = StageDataManager.RareEnemiesData;

        for (int d = 0; d < data.Count; d++)
        {
            for (int e = 0; e < enemies.Length; e++)
            {
                if (data[d].placementId != enemies[e].POSITION) continue;

                Master.EnemyData enemy = null;

                // レア敵出現抽選
                int rnd = Random.Range(0, 100);
                if (rnd <= rareEnemyApp)
                {
                    enemy = rareData[0];
                }
                else
                {
                    enemy = data[d];
                }

                enemies[e].enemyId = enemy.enemyStatus.enemyId;

                // ステータス取得
                enemies[e].HP = enemy.enemyStatus.hp;
                enemies[e].ATK = enemy.enemyStatus.atk;
                enemies[e].MP = enemy.enemyStatus.mp;
                enemies[e].DEF = enemy.enemyStatus.def;
                enemies[e].DEX = enemy.enemyStatus.dex;
                enemies[e].AGI = enemy.enemyStatus.spd;

                enemies[e].power_CriticalInit = 1.2f;

                // アタックパターン取得
                for (int i = 0; i < enemy.enemyStatus.attackPattern.Count; i++)
                {
                    enemies[e].attackPattern.Add(enemy.enemyStatus.attackPattern[i]);
                }

                // 表示
                for (int i = 0; i < enemiesIllust.Length; i++)
                {
                    if (enemy.enemyStatus.enemyId.Substring(0, 1) == enemiesIllust[i].enemyId &&
                        enemy.enemyStatus.imageId == enemiesIllust[i].imageId)
                    {
                        // イラスト変更
                        var ene = Instantiate(enemiesIllust[i].prefab, enemies[e].gameObject.transform);
                        ene.transform.SetSiblingIndex(0);

                        switch (e)
                        {
                            case 0:
                                ene.transform.localPosition = new Vector3(602, -2000, 0);
                                break;
                            case 1:
                                ene.transform.localPosition = new Vector3(1245, -2121, 0);
                                break;
                            case 2:
                                ene.transform.localPosition = new Vector3(1886, -2224, 0);
                                break;
                            case 3:
                                ene.transform.localPosition = new Vector3(2545, -2326, 0);
                                break;
                        }

                        var child = ene.transform.GetChild(0).gameObject;

                        var mr = child.GetComponent<MeshRenderer>();
                        mr.sortingOrder = 50 - e - 1;

                        enemies[e].spineAnim = ene.GetComponent<SpineAnim>();

                        if (GameManager.SelectArea == 2)
                        {
                            bossHpGuage.SetActive(true);
                            enemies[e].hpGuage = bossHpGuage.GetComponent<MainGameGuage>();
                            enemies[e].hpGuage_Obj.SetActive(false);

                            var rect = enemies[e].image.GetComponent<RectTransform>();

                            Vector2 sd = rect.sizeDelta;
                            sd.x = 1500;
                            sd.y = 1500;
                            rect.sizeDelta = sd;

                            enemies[e].transform.localPosition = new Vector3(-602, -90, 0);
                        }
                        else bossHpGuage.SetActive(false);
                    }
                }

                enemies[e].gameObject.SetActive(true);
                enemies[e].Initialize();
            }
        }
    }
}
