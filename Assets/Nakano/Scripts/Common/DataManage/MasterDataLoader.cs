using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using System;
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

    [SerializeField] private string enemyStatusKey;
    [SerializeField] private string enemyAttackPatternKey;
    [SerializeField] private string dropDataKey;
    [SerializeField] private string playerStatusKey;
    [SerializeField] private string playerRankKey;
    [SerializeField] private string playerUltKey;
    [SerializeField] private string specialTecniqueKey;

    private const string ignoreMark = "//"; // 行を飛ばす記号

    private Dictionary<string, List<string[]>> textDatas = new();
    private Dictionary<string, bool> dataLoadComplete = new();

    private bool stageDataLoaded = false;
    private bool enemyDataLoaded = false;
    private bool charaDataLoaded = false;
    private bool specialTecniqueDataLoaded = false;

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
        GetSpecialTecnique();

        yield return new WaitUntil(() => stageDataLoaded);
        yield return new WaitUntil(() => enemyDataLoaded);
        yield return new WaitUntil(() => charaDataLoaded);
        yield return new WaitUntil(() => specialTecniqueDataLoaded);

        masterDataLoadComlete = true;
        Debug.Log("マスターデータ読み込み完了");
    }

    /// <summary>
    /// 敵ステータス取得
    /// </summary>
    void GetEnemyStatus()
    {
        // 各カラムが何列目にあるか　一番左を0列目とする
        const int placeIdColumn = 4;
        const int idColumn = 5;
        const int imageIdColumn = 6;
        const int atkColumn = 7;
        const int mpColumn = 8;
        const int hpColumn = 9;
        const int defColumn = 10;
        const int spdColumn = 11;
        const int dexColumn = 12;

        List<string[]> datas = textDatas[enemyStatusKey];

        for (int l = 0; l < datas.Count - 1; l++)
        {
            EnemyStatus enemyStatus = new();

            enemyStatus.enemyId = datas[l][idColumn];
            enemyStatus.imageId = int.Parse(datas[l][imageIdColumn]);
            enemyStatus.atk = int.Parse(datas[l][atkColumn]);
            enemyStatus.mp = int.Parse(datas[l][mpColumn]);
            enemyStatus.hp = int.Parse(datas[l][hpColumn]);
            enemyStatus.def = int.Parse(datas[l][defColumn]);
            enemyStatus.spd = int.Parse(datas[l][spdColumn]);
            enemyStatus.dex = int.Parse(datas[l][dexColumn]);

            enemyStatus.attackPattern = GetEnemyAttackPattern(enemyStatus.enemyId, int.Parse(datas[l][imageIdColumn]), int.Parse(datas[l][placeIdColumn]));

            MasterData.EnemyStatus.Add(enemyStatus);
        }

        enemyDataLoaded = true;
    }

    /// <summary>
    /// 敵アタックパターン取得
    /// </summary>
    /// <param name="_enemyId"></param>
    /// <returns></returns>
    List<EnemyAttackPattern> GetEnemyAttackPattern(string _enemyId, int _imageId, int _positionId)
    {
        const int idColumn = 1;
        const int imageIdColumn = 2;
        const int positionIdColumn = 3;
        const int attackIdColumn = 4;
        const int turnColumn = 6;
        const int valueColumn = 7;
        const int probabilityColumn = 10;
        const int criticalColumn = 11;

        List<EnemyAttackPattern> attackPattern = new();
        List<string[]> datas = textDatas[enemyAttackPatternKey];

        for (int l = 0; l < datas.Count - 1; l++)
        {
            if (_enemyId == datas[l][idColumn] && _imageId == int.Parse(datas[l][imageIdColumn]) && _positionId == int.Parse(datas[l][positionIdColumn]))
            {
                string[] data = datas[l];
                EnemyAttackPattern ap = new();

                ap.attackId = int.Parse(data[attackIdColumn]);
                ap.turn = int.Parse(data[turnColumn]);
                ap.value = float.Parse(data[valueColumn]);
                ap.probability = GetProbability(data[probabilityColumn]);
                ap.criticalProbability = GetProbability(data[criticalColumn]);

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
                    sd.dropItem = GetDropItem(sd.difficulty, sd.areaId, sd.stageId);

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
        const int placeIdColumn = 4;
        const int idColumn = 5;

        List<EnemyPlacement> placement = new();
        List<string[]> datas = textDatas[enemyStatusKey];

        for (int l = 0; l < datas.Count - 1; l++)
        {
            if (_difficulty == int.Parse(datas[l][difficultyColumn]) && 
                _areaId == int.Parse(datas[l][areaIdColumn]) && 
                _stageId == int.Parse(datas[l][stageIdColumn]))
            {
                EnemyPlacement pos = new();
                pos.enemyId = datas[l][idColumn];
                pos.placementId = int.Parse(datas[l][placeIdColumn]);

                placement.Add(pos);
            }
        }

        return placement;
    }

    List<DropItem> GetDropItem(int _difficulty,int _areaId, int _stageId)
    {
        const int difficultyColumn = 1;
        const int areaIdColumn = 2;
        const int stageIdColumn = 3;
        const int typeIdColumn = 4;
        const int amountColumn = 5;
        const int probabilityColmun = 8;

        List<DropItem> items = new();
        List<string[]> datas = textDatas[dropDataKey];

        for (int l = 0; l < datas.Count - 1; l++)
        {
            if (_difficulty == int.Parse(datas[l][difficultyColumn]) && _areaId == int.Parse(datas[l][areaIdColumn]) && _stageId == int.Parse(datas[l][stageIdColumn]))
            {
                string[] data = datas[l];
                DropItem item = new();

                item.dropAmount = int.Parse(data[amountColumn]);
                item.dropProbability = GetProbability(data[probabilityColmun]);
                switch (int.Parse(data[typeIdColumn]))
                {
                    case 1:
                        item.itemType = StatusType.HP;
                        break;
                    case 2:
                        item.itemType = StatusType.MP;
                        break;
                    case 3:
                        item.itemType = StatusType.ATK;
                        break;
                    case 4:
                        item.itemType = StatusType.DEF;
                        break;
                    case 5:
                        item.itemType = StatusType.AGI;
                        break;
                    case 6:
                        item.itemType = StatusType.DEX;
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
        const int rankIdColumn = 2;

        const int atk_InitColumn = 3;
        const int atk_MaxColumn = 4;
        const int atk_BonusColumn = 5;

        const int mp_InitColumn = 6;
        const int mp_MaxColumn = 7;
        const int mp_BonusColumn = 5;

        const int hp_InitColumn = 10;
        const int hp_MaxColumn = 11;
        const int hp_BonusColumn = 12;

        const int def_InitColumn = 13;
        const int def_MaxColumn = 14;
        const int def_BonusColumn = 15;

        const int spd_InitColumn = 17;
        const int spd_MaxColumn = 18;
        const int spd_BonusColumn = 19;

        const int dex_InitColumn = 20;
        const int dex_MaxColumn = 21;
        const int dex_BonusColumn = 22;

        List<string[]> datas = textDatas[playerStatusKey];

        int rankAmount = Enum.GetValues(typeof(Rank)).Length;

        for (int c = 0; c < charaAmount; c++)
        {
            CharaInitialStutas status = new();

            for (int r = 0; r < rankAmount; r++)
            {
                int cell = c * rankAmount + r;
                status.charaId = int.Parse(datas[cell][charaIdColumn]);

                Rank rank = (Rank)Enum.Parse(typeof(Rank), datas[cell][rankIdColumn]);

                if (c + 1 == int.Parse(datas[cell][charaIdColumn]))
                {
                    // ステ初期値
                    int hp_Init = int.Parse(datas[cell][hp_InitColumn]);
                    int mp_Init = int.Parse(datas[cell][mp_InitColumn]);
                    int atk_Init = int.Parse(datas[cell][atk_InitColumn]);
                    int def_Init = int.Parse(datas[cell][def_InitColumn]);
                    int spd_Init = int.Parse(datas[cell][spd_InitColumn]);
                    int dex_Init = int.Parse(datas[cell][dex_InitColumn]);

                    status.statusInit[rank] = new Status(hp_Init, mp_Init, atk_Init, def_Init, spd_Init, dex_Init);

                    // ステ最大値
                    int hp_Max = int.Parse(datas[cell][hp_MaxColumn]);
                    int mp_Max = int.Parse(datas[cell][mp_MaxColumn]);
                    int atk_Max = int.Parse(datas[cell][atk_MaxColumn]);
                    int def_Max = int.Parse(datas[cell][def_MaxColumn]);
                    int spd_Max = int.Parse(datas[cell][spd_MaxColumn]);
                    int dex_Max = int.Parse(datas[cell][dex_MaxColumn]);

                    status.statusMax[rank] = new Status(hp_Max, mp_Max, atk_Max, def_Max, spd_Max, dex_Max);

                    // ランクアップ時ボーナス
                    int hp_Bonus = int.Parse(datas[cell][hp_BonusColumn]);
                    int mp_Bonus = int.Parse(datas[cell][mp_BonusColumn]);
                    int atk_Bonus = int.Parse(datas[cell][atk_BonusColumn]);
                    int def_Bonus = int.Parse(datas[cell][def_BonusColumn]);
                    int spd_Bonus = int.Parse(datas[cell][spd_BonusColumn]);
                    int dex_Bonus = int.Parse(datas[cell][dex_BonusColumn]);

                    status.rankUpBonus[rank] = new Status(hp_Bonus, mp_Bonus, atk_Bonus, def_Bonus, spd_Bonus, dex_Bonus);
                }

                status.rankPoint = GetRankPointSetting(status.charaId);
                status.specialMoveGuagesSetting = GetSpecialMoveGuageSetting(status.charaId);
            }

            MasterData.CharaInitialStatus.Add(status);
        }

        charaDataLoaded = true;
    }

    CharacterRankPoint GetRankPointSetting(int _charaId)
    {
        const int charaAmount = 2;

        const int charaIdColumn = 1;
        const int rankIdColumn = 2;

        const int rank_Atk_MaxColumn = 3;
        const int rank_Atk_STColumn = 4;
        const int rank_Mp_MaxColumn = 5;
        const int rank_Mp_STColumn = 6;
        const int rank_TotalAtk_MaxColumn = 7;

        const int rank_Hp_MaxColumn   = 8;
        const int rank_Hp_STColumn = 9;
        const int rank_Def_MaxColumn  = 10;
        const int rank_Def_STColumn = 11;
        const int rank_TotalDef_MaxColumn = 12;

        const int rank_Spd_MaxColumn  = 13;
        const int rank_Spd_STColumn = 14;
        const int rank_Dex_MaxColumn  = 15;
        const int rank_Dex_STColumn = 16;
        const int rank_TotalTec_MaxColumn = 17;

        CharacterRankPoint rankPoint = new();
        List<string[]> datas = textDatas[playerRankKey];
        int rankAmount = Enum.GetValues(typeof(Rank)).Length;

        for (int l = 0; l < charaAmount * rankAmount; l++)
        {
            if (_charaId == int.Parse(datas[l][charaIdColumn]))
            {
                Rank rank = (Rank)Enum.Parse(typeof(Rank), datas[l][rankIdColumn]);
                
                int hp_Max  = int.Parse(datas[l][rank_Hp_MaxColumn]);
                int mp_Max  = int.Parse(datas[l][rank_Mp_MaxColumn]);
                int atk_Max = int.Parse(datas[l][rank_Atk_MaxColumn]);
                int def_Max = int.Parse(datas[l][rank_Def_MaxColumn]);
                int spd_Max = int.Parse(datas[l][rank_Spd_MaxColumn]);
                int dex_Max = int.Parse(datas[l][rank_Dex_MaxColumn]);

                rankPoint.rankPt_NextUp[rank] = new Status(hp_Max, mp_Max, atk_Max, def_Max, spd_Max, dex_Max);

                int hp_ST = int.Parse(datas[l][rank_Hp_STColumn]);
                int mp_ST = int.Parse(datas[l][rank_Mp_STColumn]);
                int atk_ST = int.Parse(datas[l][rank_Atk_STColumn]);
                int def_ST = int.Parse(datas[l][rank_Def_STColumn]);
                int spd_ST = int.Parse(datas[l][rank_Spd_STColumn]);
                int dex_ST = int.Parse(datas[l][rank_Dex_STColumn]);

                rankPoint.releaseSTId[rank] = new Status(hp_ST, mp_ST, atk_ST, def_ST, spd_ST, dex_ST);

                rankPoint.atkRankPt_NextUp[rank] = int.Parse(datas[l][rank_TotalAtk_MaxColumn]);
                rankPoint.defRankPt_NextUp[rank] = int.Parse(datas[l][rank_TotalDef_MaxColumn]);
                rankPoint.tecRankPt_NextUp[rank] = int.Parse(datas[l][rank_TotalTec_MaxColumn]);
            }
        }

        return rankPoint;
    }

    List<SpecialMoveGuageSetting> GetSpecialMoveGuageSetting(int _charaId)
    {
        const int charaAmount          = 2;
        const int actionTypeAmount     = 5;

        const int charaIdColumn        = 1;
        const int actionTypeColumn     = 2;
        const int activateCountColumn  = 3;
        const int guageUpAmountColumn  = 4;
        const int guageMaxAmountColumn = 5;

        List<SpecialMoveGuageSetting> guagesSetting = new();
        List<string[]> datas = textDatas[playerUltKey];

        for (int l = 0; l < charaAmount * actionTypeAmount; l++)
        {
            if (_charaId == int.Parse(datas[l][charaIdColumn]))
            {
                SpecialMoveGuageSetting s = new();

                s.actionType = int.Parse(datas[l][actionTypeColumn]);
                s.activateCount = int.Parse(datas[l][activateCountColumn]);
                s.guageUpAmount = int.Parse(datas[l][guageUpAmountColumn]);
                s.guageMaxAmount = int.Parse(datas[l][guageMaxAmountColumn]);

                guagesSetting.Add(s);
            }
        }

        return guagesSetting;
    }

    void GetSpecialTecnique()
    {
        const int idColumn = 1;
        const int nameColumn = 2;
        const int isSkillColumn = 3;
        const int typeColumn = 4;
        const int continuationTurnColumn = 5;
        const int value1Column = 6;
        const int value2Column = 7;
        const int effectsColumn = 8;
        const int costColumn = 1;

        List<string[]> datas = textDatas[specialTecniqueKey];

        for (int l = 0; l < datas.Count - 1; l++)
        {
            SpecialTecniqueData d = new();

            d.id = int.Parse(datas[l][idColumn]);
            d.name = datas[l][nameColumn];
            d.isSkill = int.Parse(datas[l][isSkillColumn]) == 0 ? false : true;
            d.type = int.Parse(datas[l][typeColumn]);
            d.continuationTurn = int.Parse(datas[l][continuationTurnColumn]);
            d.value1 = int.Parse(datas[l][value1Column]);
            d.value2 = int.Parse(datas[l][value2Column]);
            d.effects = datas[l][effectsColumn];
            d.cost = int.Parse(datas[l][costColumn]);

            MasterData.SpecialTecniques.Add(d);
        }

        specialTecniqueDataLoaded = true;
    }

    private int GetProbability(string _text)
    {
        string str = _text.Replace("%", "");
        int probability = int.Parse(str);
        return probability;
    }
}

namespace Master
{
    public class MasterData
    {
        public static List<StageData> StageDatas = new();
        public static List<EnemyStatus> EnemyStatus = new();
        public static List<CharaInitialStutas> CharaInitialStatus = new();
        public static List<SpecialTecniqueData> SpecialTecniques = new();
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
    /// アタックパターンのList(attackPattern)
    /// </summary>
    public class EnemyStatus
    {
        /// <summary>
        /// 種類(英字)/Lv(2桁)によって振り分けられるId
        /// </summary>
        public string enemyId;

        public int imageId;

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

        /// <summary>
        /// 効果ターン数
        /// </summary>
        public int turn;

        /// <summary>
        /// 効果量
        /// </summary>
        public float value;

        /// <summary>
        /// 発動確率
        /// </summary>
        public int probability;

        /// <summary>
        /// 会心率
        /// </summary>
        public float criticalProbability;
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
        public StatusType itemType;

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
    /// 初期ステータス, 
    /// ステータス最大値
    /// ランクアップボーナス
    /// </summary>
    public class CharaInitialStutas
    {
        public int charaId;

        public Dictionary<Rank, Status> statusInit = new();
        public Dictionary<Rank, Status> statusMax = new();
        public Dictionary<Rank, Status> rankUpBonus = new();

        public CharacterRankPoint rankPoint = new();

        public List<SpecialMoveGuageSetting> specialMoveGuagesSetting = new();

        public CharaInitialStutas()
        {
            for (int r = 0; r < Enum.GetValues(typeof(Rank)).Length; r++)
            {
                Rank rank = (Rank)System.Enum.ToObject(typeof(Rank), r);

                Status s = new(0,0,0,0,0,0);

                statusInit.Add(rank, s);
                statusMax.Add(rank, s);
                rankUpBonus.Add(rank, s);
            }
        }
    }

    public class CharacterRankPoint
    {
        public Dictionary<Rank, Status> rankPt_NextUp = new();
        public Dictionary<Rank, Status> releaseSTId = new();   // 解放する特殊技能ID

        public Dictionary<Rank, int> atkRankPt_NextUp = new();
        public Dictionary<Rank, int> defRankPt_NextUp = new();
        public Dictionary<Rank, int> tecRankPt_NextUp = new();

        public CharacterRankPoint()
        {
            for (int r = 0; r < Enum.GetValues(typeof(Rank)).Length; r++)
            {
                Rank rank = (Rank)System.Enum.ToObject(typeof(Rank), r);

                rankPt_NextUp.Add(rank, null);
                releaseSTId.Add(rank, null);

                atkRankPt_NextUp.Add(rank, 0);
                defRankPt_NextUp.Add(rank, 0);
                tecRankPt_NextUp.Add(rank, 0);
            }
        }

        public CharacterRankPoint(CharacterRankPoint _init)
        {
            for (int r = 0; r < Enum.GetValues(typeof(Rank)).Length; r++)
            {
                Rank rank = (Rank)System.Enum.ToObject(typeof(Rank), r);

                rankPt_NextUp[rank] = _init.rankPt_NextUp[rank];
                releaseSTId[rank] = _init.releaseSTId[rank];

                atkRankPt_NextUp[rank] = _init.atkRankPt_NextUp[rank];
                defRankPt_NextUp[rank] = _init.defRankPt_NextUp[rank];
                tecRankPt_NextUp[rank] = _init.tecRankPt_NextUp[rank];
            }
        }

        public int GetCombiRankNextPt(CombiType _type, Rank _rank)
        {
            switch (_type)
            {
                case CombiType.ATK:
                    if (_rank >= Rank.SS) return atkRankPt_NextUp[Rank.SS];
                    return atkRankPt_NextUp[_rank];

                case CombiType.DEF:
                    if (_rank >= Rank.SS) return defRankPt_NextUp[Rank.SS];
                    return defRankPt_NextUp[_rank];

                case CombiType.TEC:
                    if (_rank >= Rank.SS) return tecRankPt_NextUp[Rank.SS];
                    return tecRankPt_NextUp[_rank];

                default:
                    return -1;
            }
        }
    }

    /// <summary>
    /// 必殺技ゲージ設定
    /// </summary>
    public class SpecialMoveGuageSetting
    {
        /// <summary>
        /// 挙動タイプ
        /// 1:通常攻撃 2:防御状態で被ダメ 3:非防御状態で被ダメ 4:ターン経過 5:上昇用スキル使用
        /// </summary>
        public int actionType;

        /// <summary>
        /// 発動回数
        /// </summary>
        public int activateCount;

        /// <summary>
        /// 上昇量
        /// </summary>
        public int guageUpAmount;

        /// <summary>
        /// ゲージ最大値
        /// </summary>
        public int guageMaxAmount;
    }

    /// <summary>
    /// 特殊技能
    /// </summary>
    public class SpecialTecniqueData
    {
        // 特殊技能ID
        public int id;

        // 名前
        public string name;

        // スキルかどうか
        public bool isSkill = false;

        // 技能タイプ
        public int type;

        // 継続ターン数
        public int continuationTurn;

        // 効果量
        public int value1;
        public int value2;

        // 効果内容（プレイヤー向け）
        public string effects;

        // 消費MP
        public int cost;
    }
}