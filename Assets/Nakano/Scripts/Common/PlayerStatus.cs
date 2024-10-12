using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Master;

public enum StatusType { HP, MP, ATK, DEF, SPD, DEX };
public enum CombiRankType { ATK, DEF, TEC, ALL};
public enum Rank { C, B, A, S, SS };

public class StatusBase
{
    public int hp;
    public int mp;
    public int atk;
    public int def;
    public int spd;
    public int dex;

    public StatusBase(int _hp, int _mp, int _atk, int _def, int _spd, int _dex)
    {
        hp = _hp;
        mp = _mp;
        atk = _atk;
        def = _def;
        spd = _spd;
        dex = _dex;
    }
}

public class PlayerStatus : MonoBehaviour
{
    public class Player
    {
        public int ID;

        public CharaInitialStutas statusData = new();

        public int totalPower;            // 戦闘力
        public const int totalPower_Max = 999999; // 戦闘力最大値
        public StatusBase status;         // ステータス
        public StatusBase status_Max;      // 最大値

        public StatusBase rankPoint;      // 累積ランクPt
        public StatusBase rankPoint_Max;  // ランクPt最大値

        public Dictionary<CombiRankType, Rank> combiRank; // 複合ステータスのランク
        public int atkCombiRankPtMax; // 現在ランクでの最大アタック値
        public int defCombiRankPtMax; // 現在ランクでの最大ディフェンス値
        public int tecCombiRankPtMax; // 現在ランクでの最大テクニカル値

        /// <summary>
        /// IDに応じて初期ステータス設定
        /// </summary>
        /// <param name="_id"></param>
        public Player(int _id)
        {
            ID = _id;

            if (MasterDataLoader.MasterDataLoadComplete)
            {
                for (int i = 0; i < MasterData.CharaInitialStatus.Count; i++)
                {
                    if (MasterData.CharaInitialStatus[i].charaId == _id)
                        statusData = MasterData.CharaInitialStatus[i];
                }

                CharacterRankPoint rankPtData = statusData.rankPoint;

                // ステータス初期化
                status = statusData.statusInit;
                status_Max = statusData.statusMax;

                // ランクPt初期化
                Rank initRank = Rank.C;
                rankPoint = new StatusBase(0, 0, 0, 0, 0, 0);
                rankPoint_Max = rankPtData.rankPtMax[initRank];

                atkCombiRankPtMax = rankPtData.atkRankPtMax[initRank];
                defCombiRankPtMax = rankPtData.defRankPtMax[initRank];
                tecCombiRankPtMax = rankPtData.tecRankPtMax[initRank];
            }
            else
            {
                status = new StatusBase(1000, 120, 400, 100, 50, 10);
                status_Max = new StatusBase(14000, 3000, 40000, 5000, 120, 100);

                rankPoint = new StatusBase(0, 0, 0, 0, 0, 0);
                rankPoint_Max = new StatusBase(12000, 6000, 12000, 6000, 12000, 6000);

                atkCombiRankPtMax = 1000;
                defCombiRankPtMax = 1000;
                tecCombiRankPtMax = 1000;
            }
        }
    }

    public static Player player = null;

    void Start()
    {
    }

    void Update()
    {
        
    }

    /// <summary>
    /// プレイヤー生成/初期化
    /// </summary>
    /// <param name="_id">キャラクターID</param>
    public static void PlayerCreate(int _id)
    {
        if (_id != 1 && _id != 2)
        {
            Debug.Log("キャラクターIDが誤っています");
            return;
        }

        Debug.Log("プレイヤー生成");
        player = null;
        player = new Player(_id);
    }

    /// <summary>
    /// ランクポイント追加
    /// </summary>
    /// <param name="_type">追加するステータスの種類</param>
    /// <param name="_amount">追加量</param>
    public static void RankPtUp(StatusType _type, int _amount)
    {
        switch (_type)
        {
            case StatusType.HP:
                if (player.rankPoint_Max.hp > player.rankPoint.hp)
                    player.rankPoint.hp += _amount;
                else player.rankPoint.hp = player.rankPoint_Max.hp;
                break;

            case StatusType.MP:
                if (player.rankPoint_Max.mp > player.rankPoint.mp)
                    player.rankPoint.mp += _amount;
                else player.rankPoint.mp = player.rankPoint_Max.mp;
                break;

            case StatusType.ATK:
                if (player.rankPoint_Max.atk > player.rankPoint.atk)
                    player.rankPoint.atk += _amount;
                else player.rankPoint.atk = player.rankPoint_Max.atk;
                break;

            case StatusType.DEF:
                if (player.rankPoint_Max.def > player.rankPoint.def)
                    player.rankPoint.def += _amount;
                else player.rankPoint.def = player.rankPoint_Max.def;
                break;

            case StatusType.SPD:
                if (player.rankPoint_Max.spd > player.rankPoint.spd)
                    player.rankPoint.spd += _amount;
                else player.rankPoint.spd = player.rankPoint_Max.spd;
                break;

            case StatusType.DEX:
                if (player.rankPoint_Max.dex > player.rankPoint.dex)
                    player.rankPoint.dex += _amount;
                else player.rankPoint.dex = player.rankPoint_Max.dex;
                break;
        }
    }

    static void CalcStatus()
    {

    }

    /// <summary>
    /// 複合ステータスのランク更新
    /// </summary>
    public static void CombiRankUp()
    {
        if (player.rankPoint.hp + player.rankPoint.def >= player.defCombiRankPtMax)
        {
            RankUp(CombiRankType.DEF);
        }

        if (player.rankPoint.mp + player.rankPoint.atk >= player.atkCombiRankPtMax)
        {
            RankUp(CombiRankType.ATK);
        }

        if (player.rankPoint.hp + player.rankPoint.def >= player.defCombiRankPtMax)
        {
            RankUp(CombiRankType.TEC);
        }
    }

    static void RankUp(CombiRankType _rankType)
    {
        // ランク上昇
        int rankNum = (int)player.combiRank[_rankType];
        rankNum++;
        player.combiRank[_rankType] = (Rank)System.Enum.ToObject(typeof(Rank), rankNum);

        // ランクに応じてランクポイント最大値更新
        CharacterRankPoint rankPtData = player.statusData.rankPoint;
        Rank rank = player.combiRank[_rankType];
        StatusBase maxPt = rankPtData.rankPtMax[rank];

        switch (_rankType)
        {
            case CombiRankType.ATK:
                player.atkCombiRankPtMax = rankPtData.atkRankPtMax[rank];
                player.rankPoint_Max.atk = maxPt.atk;
                player.rankPoint_Max.mp  = maxPt.mp;
                break;

            case CombiRankType.DEF:
                player.defCombiRankPtMax = rankPtData.defRankPtMax[rank];
                player.rankPoint_Max.hp = maxPt.hp;
                player.rankPoint_Max.def = maxPt.def;
                break;

            case CombiRankType.TEC:
                player.tecCombiRankPtMax = rankPtData.tecRankPtMax[rank];
                player.rankPoint_Max.spd = maxPt.spd;
                player.rankPoint_Max.dex = maxPt.dex;
                break;
        }
    }

    public static StatusBase GetStatus()
    {
        return player.status;
    }
}
