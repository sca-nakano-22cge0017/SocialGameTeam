using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Master;

public enum StatusType { HP, MP, ATK, DEF, SPD, DEX };
public enum CombiRankType { ATK, DEF, TEC, ALL };
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

public class PlayerStatus
{
    private int ID = -1;

    private CharaInitialStutas statusData = new();

    private int totalPower = 0;          // 戦闘力
    private int totalPower_Max = 999999; // 戦闘力最大値

    private StatusBase status = new(0, 0, 0, 0, 0, 0);         // ステータス
    private StatusBase status_Max = new(0, 0, 0, 0, 0, 0);     // 最大値

    private StatusBase rankPoint = new(0, 0, 0, 0, 0, 0);      // 累積ランクPt
    private StatusBase rankPoint_Max = new(0, 0, 0, 0, 0, 0);  // ランクPt最大値

    private Dictionary<CombiRankType, Rank> combiRank = new(); // 複合ステータスのランク
    private int atkCombiRankPtMax = 0; // 現在ランクでの最大アタック値
    private int defCombiRankPtMax = 0; // 現在ランクでの最大ディフェンス値
    private int tecCombiRankPtMax = 0; // 現在ランクでの最大テクニカル値

    /// <summary>
    /// IDに応じて初期ステータス設定
    /// </summary>
    /// <param name="_id"></param>
    public PlayerStatus(int _id)
    {
        ID = _id;

        Rank initRank = Rank.C;

        // 複合ステータスのランク初期化
        combiRank.Add(CombiRankType.ATK, initRank);
        combiRank.Add(CombiRankType.DEF, initRank);
        combiRank.Add(CombiRankType.TEC, initRank);

        if (MasterDataLoader.MasterDataLoadComplete)
        {
            for (int i = 0; i < MasterData.CharaInitialStatus.Count; i++)
            {
                if (MasterData.CharaInitialStatus[i].charaId == _id)
                {
                    statusData = MasterData.CharaInitialStatus[i];
                    
                    // ステータス初期化
                    status = statusData.statusInit[initRank];
                    status_Max = statusData.statusMax[initRank];

                    // ランクPt初期化
                    CharacterRankPoint rankPtData = statusData.rankPoint;

                    rankPoint = new StatusBase(0, 0, 0, 0, 0, 0);
                    rankPoint_Max = rankPtData.rankPtMax[initRank];

                    atkCombiRankPtMax = rankPtData.atkRankPtMax[initRank];
                    defCombiRankPtMax = rankPtData.defRankPtMax[initRank];
                    tecCombiRankPtMax = rankPtData.tecRankPtMax[initRank];
                }
            }
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

    public CharaInitialStutas StatusData
    {
        get
        {
            return statusData;
        }
    }

    public StatusBase AllStatus
    {
        get
        {
            return status;
        }
    }

    /// <summary>
    /// 戦闘力現在値
    /// </summary>
    public int TotalPower
    {
        get
        {
            return totalPower;
        }
        set
        {
            if (totalPower > value) totalPower = value;
            else totalPower = totalPower_Max;
        }
    }

    /// <summary>
    /// 戦闘力最大値
    /// </summary>
    public int TotalPower_Max
    {
        get
        {
            return totalPower_Max;
        }
        set
        {
            totalPower_Max = value;
        }
    }

    /// <summary>
    /// 指定したステータスの現在値を取得
    /// </summary>
    /// <param name="_type">ステータスの種類</param>
    /// <returns>指定したステータスの現在値</returns>
    public int GetStatus(StatusType _type)
    {
        int s = 0;

        switch (_type)
        {
            case StatusType.HP:
                s = status.hp;
                break;

            case StatusType.MP:
                s = status.mp;
                break;

            case StatusType.ATK:
                s = status.atk;
                break;

            case StatusType.DEF:
                s = status.def;
                break;

            case StatusType.SPD:
                s = status.spd;
                break;

            case StatusType.DEX:
                s = status.dex;
                break;
        }

        return s;
    }

    /// <summary>
    /// 指定したステータスの値を変更
    /// </summary>
    /// <param name="_type">ステータスの種類</param>
    /// <param name="_num">変更後の値</param>
    public void SetState(StatusType _type, int _num)
    {
        switch (_type)
        {
            case StatusType.HP:
                status.hp = _num;
                break;

            case StatusType.MP:
                status.mp = _num;
                break;

            case StatusType.ATK:
                status.atk = _num;
                break;

            case StatusType.DEF:
                status.def = _num;
                break;

            case StatusType.SPD:
                status.spd = _num;
                break;

            case StatusType.DEX:
                status.dex = _num;
                break;
        }
    }

    /// <summary>
    /// 指定ステータスの最大値を取得
    /// </summary>
    /// <param name="_type">ステータスの種類</param>
    /// <returns>指定ステータスの最大値</returns>
    public int GetStatusMax(StatusType _type)
    {
        int s = 0;

        switch (_type)
        {
            case StatusType.HP:
                s = status_Max.hp;
                break;

            case StatusType.MP:
                s = status_Max.mp;
                break;

            case StatusType.ATK:
                s = status_Max.atk;
                break;

            case StatusType.DEF:
                s = status_Max.def;
                break;

            case StatusType.SPD:
                s = status_Max.spd;
                break;

            case StatusType.DEX:
                s = status_Max.dex;
                break;
        }

        return s;
    }

    /// <summary>
    /// 指定したステータスの最大値を変更
    /// </summary>
    /// <param name="_type">ステータスの種類</param>
    /// <param name="_num">変更後の最大値</param>
    public void SetStateMax(StatusType _type, int _num)
    {
        switch (_type)
        {
            case StatusType.HP:
                status_Max.hp = _num;
                break;

            case StatusType.MP:
                status_Max.mp = _num;
                break;

            case StatusType.ATK:
                status_Max.atk = _num;
                break;

            case StatusType.DEF:
                status_Max.def = _num;
                break;

            case StatusType.SPD:
                status_Max.spd = _num;
                break;

            case StatusType.DEX:
                status_Max.dex = _num;
                break;
        }
    }

    /// <summary>
    /// 指定したステータスのランクポイント現在値を取得
    /// </summary>
    /// <param name="_type">ステータスの種類</param>
    /// <returns>指定したステータスのランクポイント現在値</returns>
    public int GetRankPt(StatusType _type)
    {
        int s = 0;

        switch (_type)
        {
            case StatusType.HP:
                s = rankPoint.hp;
                break;

            case StatusType.MP:
                s = rankPoint.mp;
                break;

            case StatusType.ATK:
                s = rankPoint.atk;
                break;

            case StatusType.DEF:
                s = rankPoint.def;
                break;

            case StatusType.SPD:
                s = rankPoint.spd;
                break;

            case StatusType.DEX:
                s = rankPoint.dex;
                break;
        }

        return s;
    }

    /// <summary>
    /// 指定したステータスのランクポイント現在値を変更
    /// </summary>
    /// <param name="_type">ステータスの種類</param>
    /// <param name="_num">変更後の値</param>
    public void SetRankPt(StatusType _type, int _num)
    {
        switch (_type)
        {
            case StatusType.HP:
                rankPoint.hp = _num;
                break;

            case StatusType.MP:
                rankPoint.mp = _num;
                break;

            case StatusType.ATK:
                rankPoint.atk = _num;
                break;

            case StatusType.DEF:
                rankPoint.def = _num;
                break;

            case StatusType.SPD:
                rankPoint.spd = _num;
                break;

            case StatusType.DEX:
                rankPoint.dex = _num;
                break;
        }
    }

    /// <summary>
    /// 指定したステータスのランクポイント最大値を取得
    /// </summary>
    /// <param name="_type">ステータスの種類</param>
    /// <returns>指定したステータスのランクポイント最大値</returns>
    public int GetRankPtMax(StatusType _type)
    {
        int s = 0;

        switch (_type)
        {
            case StatusType.HP:
                s = rankPoint_Max.hp;
                break;

            case StatusType.MP:
                s = rankPoint_Max.mp;
                break;

            case StatusType.ATK:
                s = rankPoint_Max.atk;
                break;

            case StatusType.DEF:
                s = rankPoint_Max.def;
                break;

            case StatusType.SPD:
                s = rankPoint_Max.spd;
                break;

            case StatusType.DEX:
                s = rankPoint_Max.dex;
                break;
        }

        return s;
    }

    /// <summary>
    /// 指定したステータスのランクポイント最大値を変更
    /// </summary>
    /// <param name="_type">ステータスの種類</param>
    /// <param name="_num">変更後の最大値</param>
    public void SetRankPtMax(StatusType _type, int _num)
    {
        switch (_type)
        {
            case StatusType.HP:
                rankPoint_Max.hp = _num;
                break;

            case StatusType.MP:
                rankPoint_Max.mp = _num;
                break;

            case StatusType.ATK:
                rankPoint_Max.atk = _num;
                break;

            case StatusType.DEF:
                rankPoint_Max.def = _num;
                break;

            case StatusType.SPD:
                rankPoint_Max.spd = _num;
                break;

            case StatusType.DEX:
                rankPoint_Max.dex = _num;
                break;
        }
    }

    /// <summary>
    /// 指定した複合ステータスの合計ランクポイント最大値を取得
    /// </summary>
    /// <param name="_type">複合ステータスの種類</param>
    /// <returns>指定した複合ステータスの合計ランクポイント最大値</returns>
    public int GetCombiRankPtMax(CombiRankType _type)
    {
        int max = 0;

        switch (_type)
        {
            case CombiRankType.ATK:
                max = atkCombiRankPtMax;
                break;
            case CombiRankType.DEF:
                max = defCombiRankPtMax;
                break;
            case CombiRankType.TEC:
                max = tecCombiRankPtMax;
                break;
            case CombiRankType.ALL:
                break;
        }

        return max;
    }

    /// <summary>
    /// 指定した複合ステータスの合計ランクポイント最大値を変更
    /// </summary>
    /// <param name="_type">複合ステータスの種類</param>
    /// <param name="_num">変更後の最大値</param>
    public void SetCombiRankPtMax(CombiRankType _type, int _amount)
    {
        switch (_type)
        {
            case CombiRankType.ATK:
                atkCombiRankPtMax = _amount;
                break;
            case CombiRankType.DEF:
                defCombiRankPtMax = _amount;
                break;
            case CombiRankType.TEC:
                tecCombiRankPtMax = _amount;
                break;
            case CombiRankType.ALL:
                break;
        }
    }

    /// <summary>
    /// 指定した複合ステータスのランクを取得
    /// </summary>
    /// <param name="_type">複合ステータスの種類</param>
    /// <returns>指定した複合ステータスのランク</returns>
    public Rank GetCombiRank(CombiRankType _type)
    {
        Rank rank = combiRank[_type];
        return rank;
    }

    /// <summary>
    /// 指定した複合ステータスのランクを変更
    /// </summary>
    /// <param name="_type">複合ステータスの種類</param>
    /// <param name="_num">変更後のランク</param>
    public void SetCombiRank(CombiRankType _type, Rank _rank)
    {
        combiRank[_type] = _rank;
    }
}
