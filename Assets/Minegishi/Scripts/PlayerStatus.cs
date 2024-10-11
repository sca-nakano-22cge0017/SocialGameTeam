using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Master;

public class StatusRankType
{
    public enum StatusType { HP, MP, ATK, DEF, SPD, DEX };
    public enum RankType { ATK, DEF, TEC, ALL };
    public enum Rank { D, C, B, A, S, SS };
}

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
        public int hitpoint; //体力
        public int defense; //守備
        public int attack; //攻撃
        public int magicPawer; //魔力
        public int speed; //速度
        public int dexterity; //器用

        public int totalPower;       // 戦闘力
        public const int totalPower_Max = 999999; // 戦闘力最大値
        public StatusBase status;    // ステータス
        public StatusBase maxStatus; // 最大値
        public StatusBase rankPoint; // 累積ランクPt
        public StatusBase rankPoint_Max;  // ランクPt最大値

        public Player(int ID, int hp, int def, int atk, int mgk, int spd, int dex)
        {
            hitpoint = hp;
            defense = def;
            attack = atk;
            magicPawer = mgk;
            speed = spd;
            dexterity = dex;
        }

        /// <summary>
        /// IDに応じて初期ステータス設定
        /// </summary>
        /// <param name="_id"></param>
        public Player(int _id)
        {
            ID = _id;

            if (MasterDataLoader.MasterDataLoadComplete)
            {
                CharaInitialStutas statusData = new();
                for (int i = 0; i < MasterData.CharaInitialStatus.Count; i++)
                {
                    if (MasterData.CharaInitialStatus[i].charaId == _id)
                        statusData = MasterData.CharaInitialStatus[i];
                }

                CharacterRankPoint rankPtData = statusData.rankPoint;

                // ステータス初期化
                status = new StatusBase(statusData.hp_Init, statusData.mp_Init, statusData.atk_Init, statusData.def_Init, statusData.spd_Init, statusData.dex_Init);
                maxStatus = new StatusBase(statusData.hp_Max, statusData.mp_Max, statusData.atk_Max, statusData.def_Max, statusData.spd_Max, statusData.dex_Max);

                // ランクPt初期化
                rankPoint = new StatusBase(rankPtData.rank_Hp_Init, rankPtData.rank_Mp_Init, rankPtData.rank_Atk_Init, rankPtData.rank_Def_Init, rankPtData.rank_Spd_Init, rankPtData.rank_Def_Init);
                rankPoint_Max = new StatusBase(rankPtData.rank_Hp_Max, rankPtData.rank_Mp_Max, rankPtData.rank_Atk_Max, rankPtData.rank_Def_Max, rankPtData.rank_Spd_Max, rankPtData.rank_Def_Max);
            }
            else
            {
                status = new StatusBase(1000, 120, 400, 100, 50, 10);
                maxStatus = new StatusBase(14000, 3000, 40000, 5000, 120, 100);

                rankPoint = new StatusBase(0, 0, 0, 0, 0, 0);
                rankPoint_Max = new StatusBase(12000, 6000, 12000, 6000, 12000, 6000);
            }
        }
    }

    //Player playerStatus = new Player(1, 100, 15, 15, 10, 30, 50);
    public static Player player = null;

    void Start()
    {
        //Debug.Log(playerStatus.hitpoint);
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
    /// ステータス上昇
    /// </summary>
    /// <param name="_type">上昇させるステータスの種類</param>
    /// <param name="_amount">上昇量</param>
    public static void StatusUp(StatusRankType.StatusType _type, int _amount)
    {
        switch(_type)
        {
            case StatusRankType.StatusType.HP:
                if (player.maxStatus.hp > player.status.hp)
                    player.status.hp += _amount;
                else player.status.hp = player.maxStatus.hp;
                break;

            case StatusRankType.StatusType.MP:
                if (player.maxStatus.mp > player.status.mp)
                    player.status.mp += _amount;
                else player.status.mp = player.maxStatus.mp;
                break;

            case StatusRankType.StatusType.ATK:
                if (player.maxStatus.atk > player.status.atk)
                    player.status.atk += _amount;
                else player.status.atk = player.maxStatus.atk;
                break;

            case StatusRankType.StatusType.DEF:
                if (player.maxStatus.def > player.status.def)
                    player.status.def += _amount;
                else player.status.def = player.maxStatus.def;
                break;

            case StatusRankType.StatusType.SPD:
                if (player.maxStatus.spd > player.status.spd)
                    player.status.spd += _amount;
                else player.status.spd = player.maxStatus.spd;
                break;

            case StatusRankType.StatusType.DEX:
                if (player.maxStatus.dex > player.status.dex)
                    player.status.dex += _amount;
                else player.status.dex = player.maxStatus.dex;
                break;
        }

        // 総合戦闘力更新
        int total = player.status.hp + player.status.mp + player.status.atk + player.status.def + player.status.spd + player.status.dex;
        if (total > Player.totalPower_Max) player.totalPower = Player.totalPower_Max;
        else player.totalPower = total;
    }

    /// <summary>
    /// ランクポイント追加
    /// </summary>
    /// <param name="_type">追加するステータスの種類</param>
    /// <param name="_amount">追加量</param>
    public static void RankPtUp(StatusRankType.StatusType _type, int _amount)
    {
        switch (_type)
        {
            case StatusRankType.StatusType.HP:
                if (player.rankPoint_Max.hp > player.rankPoint.hp)
                    player.rankPoint.hp += _amount;
                else player.rankPoint.hp = player.rankPoint_Max.hp;
                break;

            case StatusRankType.StatusType.MP:
                if (player.rankPoint_Max.mp > player.rankPoint.mp)
                    player.rankPoint.mp += _amount;
                else player.rankPoint.mp = player.rankPoint_Max.mp;
                break;

            case StatusRankType.StatusType.ATK:
                if (player.rankPoint_Max.atk > player.rankPoint.atk)
                    player.rankPoint.atk += _amount;
                else player.rankPoint.atk = player.rankPoint_Max.atk;
                break;

            case StatusRankType.StatusType.DEF:
                if (player.rankPoint_Max.def > player.rankPoint.def)
                    player.rankPoint.def += _amount;
                else player.rankPoint.def = player.rankPoint_Max.def;
                break;

            case StatusRankType.StatusType.SPD:
                if (player.rankPoint_Max.spd > player.rankPoint.spd)
                    player.rankPoint.spd += _amount;
                else player.rankPoint.spd = player.rankPoint_Max.spd;
                break;

            case StatusRankType.StatusType.DEX:
                if (player.rankPoint_Max.dex > player.rankPoint.dex)
                    player.rankPoint.dex += _amount;
                else player.rankPoint.dex = player.rankPoint_Max.dex;
                break;
        }
    }

    public static StatusBase GetStatus()
    {
        return player.status;
    }
}
