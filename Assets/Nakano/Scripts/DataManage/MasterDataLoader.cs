using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using Master;

[System.Serializable]
public class FileName
{
    public string key;
    public string fileName;
}

public class MasterDataLoader : MonoBehaviour
{
    [SerializeField] private FileName[] fileNames;

    private const string ignoreMark = "//"; // 行を飛ばす記号

    private Dictionary<string, List<string[]>> textDatas = new();
    private Dictionary<string, bool> dataLoadComplete = new();

    private bool stageDataLoaded = false;
    private bool enemyDataLoaded = false;
    private bool charaDataLoaded = false;

    private static bool masterDataLoadComlete = false;
    public static bool MasterDataLoadComplete
    {
        get { return masterDataLoadComlete; }
        private set { }
    }

    void Awake()
    {
        DataReset();
    }
    
    public void DataReset()
    {
        masterDataLoadComlete = false;
        stageDataLoaded = false;
        enemyDataLoaded = false;
        charaDataLoaded = false;

        dataLoadComplete.Clear();
        textDatas.Clear();

        for (int i = 0; i < fileNames.Length; i++)
        {
            dataLoadComplete.Add(fileNames[i].key, false);
            textDatas.Add(fileNames[i].key, null);
        }
    }

    /// <summary>
    /// データ取得
    /// </summary>
    public void DataLoad()
    {
        DataReset();

        for (int i = 0; i < fileNames.Length; i++)
        {
            GetTextData(fileNames[i].fileName, fileNames[i].key);
        }

        StartCoroutine(ConvertStringListIntoDataList());
    }

    /// <summary>
    /// 指定ファイルからデータ読み込み
    /// </summary>
    /// <param name="fileName"></param>
    /// <param name="key"></param>
    void GetTextData(string fileName, string key)
    {
        AsyncOperationHandle<TextAsset> m_TextHandle;
        Addressables.LoadAssetAsync<TextAsset>(fileName).Completed += handle =>
        {
            m_TextHandle = handle;
            if (handle.Result == null)
            {
                Debug.Log("マスターデータ読み込み失敗");
                return;
            }

            textDatas[key] = ConvertTextIntoList(handle.Result.text);
            dataLoadComplete[key] = true;
        };
    }

    private const int ignoreLines = 9;
    /// <summary>
    /// csvファイルから読み込んだ文章をリストに変換
    /// </summary>
    /// <returns></returns>
    List<string[]> ConvertTextIntoList(string dataStrings)
    {
        string dataStr = dataStrings;
        List<string[]> datas = new();

        // 行分割
        var line = dataStr.Split("\n");
        for (int l = 0; l < line.Length; l++)
        {
            if (l < ignoreLines) continue;
            if (line[l].Contains(ignoreMark)) continue;

            // 列分割
            datas.Add(line[l].Split(","));
        }

        return datas;
    }

    /// <summary>
    /// List<string[]>からMasterData用のListに変換する
    /// </summary>
    /// <returns></returns>
    IEnumerator ConvertStringListIntoDataList()
    {
        for (int i = 0; i < fileNames.Length; i++)
        {
            yield return new WaitUntil(() => dataLoadComplete[fileNames[i].key]);
        }

        GetStageData();
        GetEnemyStatus();
        GetCharaInitialStatus();

        yield return new WaitUntil(() => stageDataLoaded);
        yield return new WaitUntil(() => enemyDataLoaded);
        yield return new WaitUntil(() => charaDataLoaded);

        masterDataLoadComlete = true;
        Debug.Log("マスターデータ読み込み完了");
    }

    /// <summary>
    /// 敵ステータス取得
    /// </summary>
    void GetEnemyStatus()
    {
        // 各カラムが何列目にあるか　一番左を0列目とする
        const int idColumn = 6;
        const int atkColumn = 7;
        const int mpColumn = 8;
        const int hpColumn = 9;
        const int defColumn = 10;
        const int spdColumn = 11;
        const int dexColumn = 12;

        List<string[]> datas = textDatas["e_status"];

        for (int l = 0; l < datas.Count - 1; l++)
        {
            EnemyStatus enemyStatus = new();

            enemyStatus.enemyId = datas[l][idColumn];
            enemyStatus.atk = int.Parse(datas[l][atkColumn]);
            enemyStatus.mp = int.Parse(datas[l][mpColumn]);
            enemyStatus.hp = int.Parse(datas[l][hpColumn]);
            enemyStatus.def = int.Parse(datas[l][defColumn]);
            enemyStatus.spd = int.Parse(datas[l][spdColumn]);
            enemyStatus.dex = int.Parse(datas[l][dexColumn]);

            enemyStatus.attackPattern = GetEnemyAttackPattern(enemyStatus.enemyId);

            MasterData.EnemyStatus.Add(enemyStatus);
        }

        enemyDataLoaded = true;
    }

    /// <summary>
    /// 敵アタックパターン取得
    /// </summary>
    /// <param name="_enemyId"></param>
    /// <returns></returns>
    List<EnemyAttackPattern> GetEnemyAttackPattern(string _enemyId)
    {
        const int idColumn = 1;
        const int attackIdColumn = 2;
        const int typeAttackColumn = 3;
        const int typeBuffColumn = 4;
        const int probabilityColumn = 7;

        List<EnemyAttackPattern> attackPattern = new();
        List<string[]> datas = textDatas["e_attackpattern"];

        for (int l = 0; l < datas.Count - 1; l++)
        {
            if (_enemyId == datas[l][idColumn])
            {
                string[] data = datas[l];
                EnemyAttackPattern ap = new();

                ap.attackId = int.Parse(data[attackIdColumn]);
                if (int.Parse(data[typeAttackColumn]) == 1) ap.type = EnemyAttackPattern.AttackType.ATTACK;
                if (int.Parse(data[typeBuffColumn]) == 1) ap.type = EnemyAttackPattern.AttackType.BUFF;
                ap.probability = GetProbability(data[probabilityColumn]);

                attackPattern.Add(ap);
            }
        }
        return attackPattern;
    }

    void GetStageData()
    {
        const int difficultyAmount = 5;
        const int areaAmount = 2;
        const int stageAmount_Traning = 6;
        const int stageAmount_Boss = 1;

        List<StageData> stageDatas = new();

        for (int d = 1; d <= difficultyAmount; d++)
        {
            for (int a = 1; a <= areaAmount; a++)
            {
                int stageAmount = (a == 1) ? stageAmount_Traning : stageAmount_Boss;

                for (int s = 1; s <= stageAmount; s++)
                {
                    StageData sd = new();

                    sd.difficulty = d;
                    sd.areaId = a;
                    sd.stageId = s;
                    sd.enemyPlacement = GetEnemyPlacement(d, a, s);
                    if (a == 1) sd.dropItem = GetDropItem(sd.difficulty, sd.stageId);
                    else sd.dropItem = null;

                    stageDatas.Add(sd);
                }
            }
        }

        MasterData.StageDatas = stageDatas;

        stageDataLoaded = true;
    }

    List<EnemyPlacement> GetEnemyPlacement(int _difficulty, int _areaId, int _stageId)
    {
        const int difficultyColumn = 1;
        const int areaIdColumn = 2;
        const int stageIdColumn = 3;
        const int placeIdColumn = 5;
        const int idColumn = 6;

        List<EnemyPlacement> placement = new();
        List<string[]> datas = textDatas["e_status"];

        for (int l = 0; l < datas.Count - 1; l++)
        {
            if (_difficulty == int.Parse(datas[l][difficultyColumn]) && _areaId == int.Parse(datas[l][areaIdColumn]) && _stageId == int.Parse(datas[l][stageIdColumn]))
            {
                EnemyPlacement pos = new();
                pos.enemyId = datas[l][idColumn];
                pos.placementId = int.Parse(datas[l][placeIdColumn]);

                placement.Add(pos);
            }
        }

        return placement;
    }

    List<DropItem> GetDropItem(int _difficulty, int _stageId)
    {
        const int difficultyColumn = 1;
        const int stageIdColumn = 2;
        const int typeIdColumn = 3;
        const int amountColumn = 4;
        const int probabilityColmun = 7;

        List<DropItem> items = new();
        List<string[]> datas = textDatas["item"];

        for (int l = 0; l < datas.Count - 1; l++)
        {
            if (_difficulty == int.Parse(datas[l][difficultyColumn]) && _stageId == int.Parse(datas[l][stageIdColumn]))
            {
                string[] data = datas[l];
                DropItem item = new();

                item.dropAmount = int.Parse(data[amountColumn]);
                item.dropProbability = GetProbability(data[probabilityColmun]);
                switch (int.Parse(data[typeIdColumn]))
                {
                    case 1:
                        item.itemType = StatusRankType.StatusType.HP;
                        break;
                    case 2:
                        item.itemType = StatusRankType.StatusType.MP;
                        break;
                    case 3:
                        item.itemType = StatusRankType.StatusType.ATK;
                        break;
                    case 4:
                        item.itemType = StatusRankType.StatusType.DEF;
                        break;
                    case 5:
                        item.itemType = StatusRankType.StatusType.SPD;
                        break;
                    case 6:
                        item.itemType = StatusRankType.StatusType.DEX;
                        break;
                }

                items.Add(item);
            }
        }

        return items;
    }

    void GetCharaInitialStatus()
    {
        const int charaAmount = 2;
        const int charaIdColumn = 1;
        const int atk_InitColumn = 2;
        const int atk_MaxColumn  = 3;
        const int  mp_InitColumn  = 4;
        const int  mp_MaxColumn   = 5;
        const int  hp_InitColumn  = 6;
        const int  hp_MaxColumn   = 7;
        const int def_InitColumn = 8;
        const int def_MaxColumn  = 9;
        const int spd_InitColumn = 10;
        const int spd_MaxColumn  = 11;
        const int dex_InitColumn = 12;
        const int dex_MaxColumn  = 13;

        List<CharaInitialStutas> charaInitialStatus = new();
        List<string[]> datas = textDatas["p_status"];

        for (int l = 0; l < charaAmount; l++)
        {
            CharaInitialStutas status = new();

            status.charaId = int.Parse(datas[l][charaIdColumn]);

            status.hp_Init = int.Parse(datas[l][hp_InitColumn]);
            status.mp_Init = int.Parse(datas[l][mp_InitColumn]);
            status.atk_Init = int.Parse(datas[l][atk_InitColumn]);
            status.def_Init = int.Parse(datas[l][def_InitColumn]);
            status.spd_Init = int.Parse(datas[l][spd_InitColumn]);
            status.dex_Init = int.Parse(datas[l][dex_InitColumn]);

            status.hp_Max = int.Parse(datas[l][hp_MaxColumn]);
            status.mp_Max = int.Parse(datas[l][mp_MaxColumn]);
            status.atk_Max = int.Parse(datas[l][atk_MaxColumn]);
            status.def_Max = int.Parse(datas[l][def_MaxColumn]);
            status.spd_Max = int.Parse(datas[l][spd_MaxColumn]);
            status.dex_Max = int.Parse(datas[l][dex_MaxColumn]);

            status.rankPoint = GetRankPointSetting(status.charaId);

            charaInitialStatus.Add(status);
        }

        MasterData.CharaInitialStatus = charaInitialStatus;

        charaDataLoaded = true;
    }

    CharacterRankPoint GetRankPointSetting(int _charaId)
    {
        const int charaAmount = 2;
        const int charaIdColumn = 1;
        const int rank_Hp_InitColumn  = 2;
        const int rank_Hp_MaxColumn   = 3;
        const int rank_Mp_InitColumn  = 4;
        const int rank_Mp_MaxColumn   = 5;
        const int rank_Atk_InitColumn = 6;
        const int rank_Atk_MaxColumn  = 7;
        const int rank_Def_InitColumn = 8;
        const int rank_Def_MaxColumn  = 9;
        const int rank_Spd_InitColumn = 10;
        const int rank_Spd_MaxColumn  = 11;
        const int rank_Dex_InitColumn = 12;
        const int rank_Dex_MaxColumn  = 13;

        CharacterRankPoint rankPoint = new();
        List<string[]> datas = textDatas["p_lankPt"];

        for (int l = 0; l < charaAmount; l++)
        {
            if (_charaId == int.Parse(datas[l][charaIdColumn]))
            {
                rankPoint.rank_Hp_Init = int.Parse(datas[l][rank_Hp_InitColumn]);
                rankPoint.rank_Mp_Init = int.Parse(datas[l][rank_Mp_InitColumn]);
                rankPoint.rank_Atk_Init = int.Parse(datas[l][rank_Atk_InitColumn]);
                rankPoint.rank_Def_Init = int.Parse(datas[l][rank_Def_InitColumn]);
                rankPoint.rank_Spd_Init = int.Parse(datas[l][rank_Spd_InitColumn]);
                rankPoint.rank_Dex_Init = int.Parse(datas[l][rank_Dex_InitColumn]);

                rankPoint.rank_Hp_Max = int.Parse(datas[l][rank_Hp_MaxColumn]);
                rankPoint.rank_Mp_Max = int.Parse(datas[l][rank_Mp_MaxColumn]);
                rankPoint.rank_Atk_Max = int.Parse(datas[l][rank_Atk_MaxColumn]);
                rankPoint.rank_Def_Max = int.Parse(datas[l][rank_Def_MaxColumn]);
                rankPoint.rank_Spd_Max = int.Parse(datas[l][rank_Spd_MaxColumn]);
                rankPoint.rank_Dex_Max = int.Parse(datas[l][rank_Dex_MaxColumn]);
            }
        }

        return rankPoint;
    }

    private int GetProbability(string _text)
    {
        string str = _text.Replace("%", "");
        int probability = int.Parse(str);
        return probability;
    }
}

public class SelectedCharacterData
{
    public static CharaInitialStutas charaInitialStutas = new();
}

namespace Master
{
    public class MasterData
    {
        public static List<StageData> StageDatas = new();
        public static List<EnemyStatus> EnemyStatus = new();
        public static List<CharaInitialStutas> CharaInitialStatus = new();
    }

    /// <summary>
    /// ステージデータ
    /// ステージ番号(areaId/difficulty/stageId), 敵配置情報のList(enemyPlacement)
    /// </summary>
    public class StageData
    {
        public int difficulty;

        /// <summary>
        /// エリア番号 1：育成ステージ, 2：ボスステージ
        /// </summary>
        public int areaId;
        public int stageId;

        /// <summary>
        /// 敵配置データ
        /// </summary>
        public List<EnemyPlacement> enemyPlacement;

        public List<DropItem> dropItem;
    }

    /// <summary>
    /// 敵の配置データ
    /// 敵Id(enemyId), 配置番号(placementId)
    /// </summary>
    public class EnemyPlacement
    {
        public string enemyId;

        /// <summary>
        /// 配置位置番号 1～4の数値
        /// </summary>
        public int placementId;
    }

    /// <summary>
    /// 敵のデータ 敵の配置番号(placementId), 敵ステータス(enemyStatus)
    /// </summary>
    public class EnemyData
    {
        /// <summary>
        /// 配置位置
        /// </summary>
        public int placementId;

        /// <summary>
        /// エネミーのステータス・アタックパターン
        /// </summary>
        public EnemyStatus enemyStatus;
    }

    /// <summary>
    /// 敵ステータス
    /// 敵Id(enemyId), 
    /// ステータス(jp/mp/atk/def/spd/dex), 
    /// アタックパターンのList(attackPattern), 
    /// ドロップアイテム(dropItem)
    /// </summary>
    public class EnemyStatus
    {
        /// <summary>
        /// 種類(英字)/Lv(2桁)によって振り分けられるId
        /// </summary>
        public string enemyId;

        public int hp;
        public int def;
        public int mp;
        public int atk;
        public int spd;
        public int dex;

        public List<EnemyAttackPattern> attackPattern = new();
    }

    /// <summary>
    /// アタックパターン
    /// アタックパターンId(attackId), 
    /// 発動確率(probability)
    /// </summary>
    public class EnemyAttackPattern
    {
        public int attackId;

        public enum AttackType { ATTACK, BUFF };
        public AttackType type;

        /// <summary>
        /// 発動確率
        /// </summary>
        public int probability;
    }

    /// <summary>
    /// ドロップアイテム
    /// アイテムタイプ(DropItemType.type) 各ステータス6種
    /// ドロップ数(dropAmount)
    /// </summary>
    public class DropItem
    {
        /// <summary>
        /// ドロップアイテムの種類
        /// </summary>
        public StatusRankType.StatusType itemType;

        /// <summary>
        /// ドロップ量
        /// </summary>
        public int dropAmount;

        /// <summary>
        /// ドロップ確率
        /// </summary>
        public float dropProbability;
    }

    /// <summary>
    /// キャラクター初期ステータス
    /// キャラId(charaId), 
    /// 初期ステータス(hp_init/mp_init/atk_init/def_init/spd_init/dex_init), 
    /// ステータス最大値(atk_max/def_amx/tec_max)
    /// </summary>
    public class CharaInitialStutas
    {
        public int charaId;

        // 初期ステータス
        public int  hp_Init;
        public int  mp_Init;
        public int atk_Init;
        public int def_Init;
        public int spd_Init;
        public int dex_Init;

        // ステータス最大値
        public int hp_Max;
        public int mp_Max;
        public int atk_Max;
        public int def_Max;
        public int spd_Max;
        public int dex_Max;

        public CharacterRankPoint rankPoint;
    }

    public class CharacterRankPoint
    {
        // ランクポイント初期値
        public int rank_Hp_Init;
        public int rank_Mp_Init;
        public int rank_Atk_Init;
        public int rank_Def_Init;
        public int rank_Spd_Init;
        public int rank_Dex_Init;

        // ランクポイント最大値
        public int rank_Hp_Max;
        public int rank_Mp_Max;
        public int rank_Atk_Max;
        public int rank_Def_Max;
        public int rank_Spd_Max;
        public int rank_Dex_Max;
    }
}