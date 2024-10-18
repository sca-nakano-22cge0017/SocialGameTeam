using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Master;

public enum StatusType { HP, MP, ATK, DEF, SPD, DEX };
public enum CombiType { ATK, DEF, TEC };
public enum Rank { C, B, A, S, SS };

public class Status
{
    public int hp;
    public int mp;
    public int atk;
    public int def;
    public int spd;
    public int dex;

    public Status(int _hp, int _mp, int _atk, int _def, int _spd, int _dex)
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

    private Status status = new(0, 0, 0, 0, 0, 0);           // ステータス
    private Status status_Max = new(0, 0, 0, 0, 0, 0);       // 最大値

    private Status rankPoint = new(0, 0, 0, 0, 0, 0);        // 累積ランクPt
    private Status rankPoint_LastUp = new(0, 0, 0, 0, 0, 0); // 前回ランクアップしたときの累積ランクPt
    private Status rankPoint_NextUp = new(0, 0, 0, 0, 0, 0); // 次にランクアップするときの累積ランクPt
    private Status rankPoint_Max = new(0, 0, 0, 0, 0, 0);    // ステータスのランクPt最大値 プラスステータスを除き、上昇しない
    private Dictionary<StatusType, Rank> statusRank = new(); // 各ステータスのランク

    private Dictionary<CombiType, Rank> combiRank = new();         // 複合ステータスのランク
    private Dictionary<CombiType, int> combiRankPt = new();        // 複合ステータスのランクPt現在値
    private Dictionary<CombiType, int> combiRankPt_NextUp = new(); // 次にランクアップするときの累積ランクPt
    private Dictionary<CombiType, int> combiRankPtMax = new();     // 複合ステータスのランクPt最大値 プラスステータスを除き、上昇しない

    private Status plusStatus = new(0, 0, 0, 0, 0, 0);     // 周回によるプラスステータス 1～99

    /// <summary>
    /// IDに応じて初期ステータス設定
    /// </summary>
    /// <param name="_id"></param>
    public PlayerStatus(int _id)
    {
        ID = _id;

        Initialize(_id);
    }

    public CharaInitialStutas StatusData
    {
        get
        {
            return statusData;
        }
    }

    public Status AllStatus
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
    /// 初期化
    /// </summary>
    /// <param name="_id"></param>
    void Initialize(int _id)
    {
        Rank initRank = Rank.C;     // 初期ランク
        Rank highestRank = Rank.SS; // 最高ランク

        // ステータスのランク初期化
        statusRank.Clear();
        for (int s = 0; s < System.Enum.GetValues(typeof(StatusType)).Length; s++)
        {
            StatusType status = (StatusType)System.Enum.ToObject(typeof(StatusType), s);
            statusRank.Add(status, initRank);
        }

        // 複合ステータスのランク/ランクPt最大値初期化
        combiRank.Clear();
        combiRankPt.Clear();
        combiRankPtMax.Clear();
        for (int c = 0; c < System.Enum.GetValues(typeof(CombiType)).Length; c++)
        {
            CombiType combi = (CombiType)System.Enum.ToObject(typeof(CombiType), c);
            combiRank.Add(combi, initRank);
            combiRankPt.Add(combi, 0);
            combiRankPt_NextUp.Add(combi, 0);
            combiRankPtMax.Add(combi, 0);
        }

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

                    rankPoint = new Status(0, 0, 0, 0, 0, 0);
                    rankPoint_LastUp = new Status(0, 0, 0, 0, 0, 0);
                    rankPoint_NextUp = rankPtData.rankPt_NextUp[initRank];
                    rankPoint_Max = rankPtData.rankPt_NextUp[highestRank];

                    combiRankPt_NextUp[CombiType.ATK] = rankPtData.atkRankPt_NextUp[initRank];
                    combiRankPt_NextUp[CombiType.DEF] = rankPtData.defRankPt_NextUp[initRank];
                    combiRankPt_NextUp[CombiType.TEC] = rankPtData.tecRankPt_NextUp[initRank];

                    combiRankPtMax[CombiType.ATK] = rankPtData.atkRankPt_NextUp[highestRank];
                    combiRankPtMax[CombiType.DEF] = rankPtData.defRankPt_NextUp[highestRank];
                    combiRankPtMax[CombiType.TEC] = rankPtData.tecRankPt_NextUp[highestRank];
                }
            }
        }
        else
        {
            status = new Status(1000, 120, 400, 100, 50, 10);
            status_Max = new Status(14000, 3000, 40000, 5000, 120, 100);

            rankPoint = new Status(0, 0, 0, 0, 0, 0);
            rankPoint_LastUp = new Status(0, 0, 0, 0, 0, 0);
            rankPoint_NextUp = new Status(12000, 6000, 12000, 6000, 12000, 6000);
            rankPoint_Max = new Status(12000, 6000, 12000, 6000, 12000, 6000);

            combiRankPt_NextUp[CombiType.ATK] = 1000;
            combiRankPt_NextUp[CombiType.DEF] = 1000;
            combiRankPt_NextUp[CombiType.TEC] = 1000;

            combiRankPtMax[CombiType.ATK] = 14000;
            combiRankPtMax[CombiType.DEF] = 14000;
            combiRankPtMax[CombiType.TEC] = 14000;
        }
    }

    // 以下変数取得・変更系
    // ステータス
    /// <summary>
    /// 指定したステータスの現在値を取得
    /// </summary>
    /// <param name="_type">ステータスの種類</param>
    /// <returns>指定したステータスの現在値</returns>
    public int GetStatus(StatusType _type)
    {
        switch (_type)
        {
            case StatusType.HP:
                return status.hp;

            case StatusType.MP:
                return status.mp;
                
            case StatusType.ATK:
                return status.atk;
               
            case StatusType.DEF:
                return status.def;
                
            case StatusType.SPD:
                return status.spd;
                
            case StatusType.DEX:
                return status.dex;
                
            default:
                return 0;
        }
    }

    /// <summary>
    /// 指定したステータスの値を変更
    /// </summary>
    /// <param name="_type">ステータスの種類</param>
    /// <param name="_num">変更後の値</param>
    public void SetStatus(StatusType _type, int _num)
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
        switch (_type)
        {
            case StatusType.HP:
                return status_Max.hp;
                
            case StatusType.MP:
                return status_Max.mp;
                
            case StatusType.ATK:
                return status_Max.atk;
                
            case StatusType.DEF:
                return status_Max.def;
                
            case StatusType.SPD:
                return status_Max.spd;
                
            case StatusType.DEX:
                return status_Max.dex;
                
            default:
                return 0;
        }
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

    // ランク
    /// <summary>
    /// 指定したステータスのランクを取得
    /// </summary>
    /// <param name="_type">ステータスの種類</param>
    /// <returns>指定したステータスのランク</returns>
    public Rank GetRank(StatusType _type)
    {
        Rank rank = statusRank[_type];
        return rank;
    }

    /// <summary>
    /// 指定したステータスのランクを変更
    /// </summary>
    /// <param name="_type">ステータスの種類</param>
    /// <param name="_num">変更後のランク</param>
    public void SetRank(StatusType _type, Rank _rank)
    {
        statusRank[_type] = _rank;
    }

    // ランクPt
    /// <summary>
    /// 指定したステータスのランクポイント現在値を取得
    /// </summary>
    /// <param name="_type">ステータスの種類</param>
    /// <returns>指定したステータスのランクポイント現在値</returns>
    public int GetRankPt(StatusType _type)
    {
        switch (_type)
        {
            case StatusType.HP:
                return rankPoint.hp;

            case StatusType.MP:
                return rankPoint.mp;

            case StatusType.ATK:
                return rankPoint.atk;

            case StatusType.DEF:
                return rankPoint.def;

            case StatusType.SPD:
                return rankPoint.spd;

            case StatusType.DEX:
                return rankPoint.dex;

            default:
                return 0;
        }
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
    /// 指定したステータスの、前回ランクアップしたときの累積Ptを取得
    /// </summary>
    /// <param name="_type">ステータスの種類</param>
    public int GetRankPtLastUp(StatusType _type)
    {
        switch (_type)
        {
            case StatusType.HP:
                return rankPoint_LastUp.hp;

            case StatusType.MP:
                return rankPoint_LastUp.mp;

            case StatusType.ATK:
                return rankPoint_LastUp.atk;

            case StatusType.DEF:
                return rankPoint_LastUp.def;

            case StatusType.SPD:
                return rankPoint_LastUp.spd; 

            case StatusType.DEX:
                return rankPoint_LastUp.dex;

            default:
                return 0;
        }
    }

    /// <summary>
    /// 指定したステータスの、次にランクアップするときの累積Ptを取得
    /// </summary>
    /// <param name="_type">ステータスの種類</param>
    public int GetRankPtNextUp(StatusType _type)
    {
        switch (_type)
        {
            case StatusType.HP:
                return rankPoint_NextUp.hp;

            case StatusType.MP:
                return rankPoint_NextUp.mp;

            case StatusType.ATK:
                return rankPoint_NextUp.atk;

            case StatusType.DEF:
                return rankPoint_NextUp.def;

            case StatusType.SPD:
                return rankPoint_NextUp.spd;

            case StatusType.DEX:
                return rankPoint_NextUp.dex;

            default:
                return 0;
        }
    }

    /// <summary>
    /// 指定したステータスの、次にランクアップするときの累積Ptを変更
    /// </summary>
    /// <param name="_type">ステータスの種類</param>
    /// <param name="_num">変更後の値</param>
    public void SetRankPtNextUp(StatusType _type, int _num)
    {
        switch (_type)
        {
            case StatusType.HP:
                rankPoint_LastUp.hp = rankPoint.hp;
                rankPoint_NextUp.hp = _num;
                break;

            case StatusType.MP:
                rankPoint_LastUp.mp = rankPoint.mp;
                rankPoint_NextUp.mp = _num;
                break;

            case StatusType.ATK:
                rankPoint_LastUp.atk = rankPoint.atk;
                rankPoint_NextUp.atk = _num;
                break;

            case StatusType.DEF:
                rankPoint_LastUp.def = rankPoint.def;
                rankPoint_NextUp.def = _num;
                break;

            case StatusType.SPD:
                rankPoint_LastUp.spd = rankPoint.spd;
                rankPoint_NextUp.spd = _num;
                break;

            case StatusType.DEX:
                rankPoint_LastUp.dex = rankPoint.dex;
                rankPoint_NextUp.dex = _num;
                break;
        }
    }

    /// <summary>
    /// 指定したステータスのランクポイントの最大値を取得
    /// </summary>
    /// <param name="_type">ステータスの種類</param>
    /// <returns>指定したステータスのランクポイント最大値</returns>
    public int GetRankPtMax(StatusType _type)
    {
        switch (_type)
        {
            case StatusType.HP:
                return rankPoint_Max.hp;

            case StatusType.MP:
                return rankPoint_Max.mp;

            case StatusType.ATK:
                return rankPoint_Max.atk;

            case StatusType.DEF:
                return rankPoint_Max.def;

            case StatusType.SPD:
                return rankPoint_Max.spd;

            case StatusType.DEX:
                return rankPoint_Max.dex;

            default:
                return 0;
        }
    }

    // 複合ステータスのランク
    /// <summary>
    /// 指定した複合ステータスのランクを取得
    /// </summary>
    /// <param name="_type">複合ステータスの種類</param>
    /// <returns>指定した複合ステータスのランク</returns>
    public Rank GetCombiRank(CombiType _type)
    {
        Rank rank = combiRank[_type];
        return rank;
    }

    /// <summary>
    /// 指定した複合ステータスのランクを変更
    /// </summary>
    /// <param name="_type">複合ステータスの種類</param>
    /// <param name="_num">変更後のランク</param>
    public void SetCombiRank(CombiType _type, Rank _rank)
    {
        combiRank[_type] = _rank;
    }

    // 複合ステータスのランクPt
    /// <summary>
    /// 指定した複合ステータスのランクポイント現在値を取得
    /// </summary>
    /// <param name="_type">複合ステータスの種類</param>
    /// <returns>指定した複合ステータスのランクポイント現在値</returns>
    public int GetCombiRankPt(CombiType _type)
    {
        return combiRankPt[_type];
    }

    /// <summary>
    /// 指定した複合ステータスのランクポイント現在値を変更
    /// </summary>
    /// <param name="_type">ステータスの種類</param>
    /// <param name="_num">変更後の現在値</param>
    public void SetCombiRankPt(CombiType _type, int _num)
    {
        combiRankPt[_type] = _num;
    }

    /// <summary>
    /// 指定した複合ステータスの合計ランクポイント最大値を取得
    /// </summary>
    /// <param name="_type">複合ステータスの種類</param>
    /// <returns>指定した複合ステータスの合計ランクポイント最大値</returns>
    public int GetCombiRankPtMax(CombiType _type)
    {
        return combiRankPtMax[_type];
    }

    /// <summary>
    /// 指定した複合ステータスの、次にランクアップするときの累積Ptを取得
    /// </summary>
    /// <param name="_type">ステータスの種類</param>
    public int GetCombiRankPtNextUp(CombiType _type)
    {
        return combiRankPt_NextUp[_type];
    }

    /// <summary>
    /// 指定した複合ステータスの、次にランクアップするときの累積Ptを変更
    /// </summary>
    /// <param name="_type">ステータスの種類</param>
    /// <param name="_num">変更後の値</param>
    public void SetCombiRankPtNextUp(CombiType _type, int _num)
    {
        combiRankPt_NextUp[_type] = _num;
    }

    // プラスステータス
    /// <summary>
    /// 全てのプラスステータスを取得
    /// </summary>
    /// <returns>全てのプラスステータス</returns>
    public Status GetPlusStatus()
    {
        return plusStatus;
    }

    /// <summary>
    /// 指定したステータスのプラスステータスを取得
    /// </summary>
    /// <param name="_type">ステータスの種類</param>
    /// <returns>ステータスのプラスステータス</returns>
    public int GetPlusStatus(StatusType _type)
    {
        switch (_type)
        {
            case StatusType.HP:
                return plusStatus.hp;

            case StatusType.MP:
                return plusStatus.mp;

            case StatusType.ATK:
                return plusStatus.atk;

            case StatusType.DEF:
                return plusStatus.def;

            case StatusType.SPD:
                return plusStatus.spd;

            case StatusType.DEX:
                return plusStatus.dex;

            default:
                return 0;
        }
    }

    /// <summary>
    /// 指定したステータスのプラスステータスを変更
    /// </summary>
    /// <param name="_type">ステータスの種類</param>
    /// <param name="_num">変更後のプラスステータス</param>
    public void SetPlusStatus(StatusType _type, int _num)
    {
        switch (_type)
        {
            case StatusType.HP:
                plusStatus.hp = _num;
                break;

            case StatusType.MP:
                plusStatus.mp = _num;
                break;

            case StatusType.ATK:
                plusStatus.atk = _num;
                break;

            case StatusType.DEF:
                plusStatus.def = _num;
                break;

            case StatusType.SPD:
                plusStatus.spd = _num;
                break;

            case StatusType.DEX:
                plusStatus.dex = _num;
                break;
        }
    }
}
