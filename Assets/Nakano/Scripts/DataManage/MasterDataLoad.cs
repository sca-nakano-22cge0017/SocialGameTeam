using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using Master;

public class MasterDataLoad : MonoBehaviour
{
    [HideInInspector] public bool masterDataLoadComlete = false;
    string fileName;

    private const string ignoreMark = "//"; // 行を飛ばす記号

    void GetData(string fileName)
    {
        AsyncOperationHandle<TextAsset> m_TextHandle;
        Addressables.LoadAssetAsync<TextAsset>(fileName).Completed += handle =>
        {
            m_TextHandle = handle;
            if (handle.Result == null)
            {
                Debug.Log("Load Error");
                return;
            }

            ConvertTextIntoList(handle.Result.text);
            masterDataLoadComlete = true;
        };
    }

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
            if (line[l].Contains(ignoreMark)) continue;

            // 列分割・Listに代入
            datas.Add(line[l].Split(","));
        }

        return datas;
    }
}

public class MasterData
{
    public static List<StageData> stageDatas;
    public static List<EnemyStatus> enemyStutas;
    public static List<CharaInitialStutas> charaInitialStutas;
}

namespace Master
{
    /// <summary>
    /// ステージデータ
    /// ステージ番号(stageId), 敵配置情報のList(enemyPlacement)
    /// </summary>
    public class StageData
    {
        /// <summary>
        /// エリア番号(1桁)/ステージ番号(2桁)/難易度(2桁)によって振り分けられるID
        /// </summary>
        public int stageID;

        /// <summary>
        /// 敵配置データ
        /// </summary>
        public List<EnemyPlacement> enemyPlacement;
    }

    /// <summary>
    /// 敵の配置データ
    /// 敵ID(enemyID), 配置番号(placementID)
    /// </summary>
    public class EnemyPlacement
    {
        public string enemyID;

        /// <summary>
        /// 配置位置番号 1～4の数値
        /// </summary>
        public int placementID;
    }

    /// <summary>
    /// 敵ステータス
    /// 敵ID(enemyID), 
    /// ステータス(jp/mp/atk/def/spd/dex), 
    /// アタックパターンのList(attackPattern), 
    /// ドロップアイテム(dropItem)
    /// </summary>
    public class EnemyStatus
    {
        /// <summary>
        /// 種類(英字)/Lv(2桁)によって振り分けられるID
        /// </summary>
        public string enemyID;

        public int hp;
        public int def;
        public int mp;
        public int atk;
        public int spd;
        public int dex;

        public List<EnemyAttackPattern> attackPattern;
        
        public DropItem dropItem;
    }

    /// <summary>
    /// アタックパターン
    /// アタックパターンID(attackID), 
    /// 発動確率(probability)
    /// </summary>
    public class EnemyAttackPattern
    {
        public int attackID;

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
        public enum DropItemType { HP, MP, ATK, DEF, SPD, DEX };
        public DropItemType type;

        /// <summary>
        /// ドロップ量
        /// </summary>
        public int dropAmount;
    }

    /// <summary>
    /// キャラクター初期ステータス
    /// キャラID(charaId), 
    /// 初期ステータス(hp_init/mp_init/atk_init/def_init/spd_init/dex_init), 
    /// ステータス最大値(atk_max/def_amx/tec_max)
    /// </summary>
    public class CharaInitialStutas
    {
        public int charaID;

        // 初期ステータス
        public int hp_init;
        public int mp_init;
        public int atk_init;
        public int def_init;
        public int spd_init;
        public int dex_init;

        // ステータス最大値
        public int atk_max;
        public int def_max;
        public int tec_max;
    }
}