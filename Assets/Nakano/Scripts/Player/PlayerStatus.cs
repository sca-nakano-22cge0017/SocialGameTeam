using Master;
using System.Collections.Generic;
using UnityEngine;

public enum StatusType { HP, MP, ATK, DEF, AGI, DEX };
public enum CombiType { ATK, DEF, TEC, NORMAL };
public enum Rank { D = 0, C = 1, B = 2, A = 3, S = 4, SS = 5 };

public class Status
{
    public int hp;
    public int mp;
    public int atk;
    public int def;
    public int agi;
    public int dex;

    public Status(int _hp, int _mp, int _atk, int _def, int _agi, int _dex)
    {
        hp = _hp;
        mp = _mp;
        atk = _atk;
        def = _def;
        agi = _agi;
        dex = _dex;
    }

    public Status(Status _status)
    {
        hp = _status.hp;
        mp = _status.mp;
        atk = _status.atk;
        def = _status.def;
        agi = _status.agi;
        dex = _status.dex;
    }

    public int GetStatus(StatusType _type)
    {
        switch (_type)
        {
            case StatusType.HP:
                return hp;

            case StatusType.MP:
                return mp;

            case StatusType.ATK:
                return atk;

            case StatusType.DEF:
                return def;

            case StatusType.AGI:
                return agi;

            case StatusType.DEX:
                return dex;

            default:
                return 0;
        }
    }

    public void SetStatus(StatusType _type, int _num)
    {
        switch (_type)
        {
            case StatusType.HP:
                hp = _num;
                break;

            case StatusType.MP:
                mp = _num;
                break;

            case StatusType.ATK:
                atk = _num;
                break;

            case StatusType.DEF:
                def = _num;
                break;

            case StatusType.AGI:
                agi = _num;
                break;

            case StatusType.DEX:
                dex = _num;
                break;
        }
    }
}

public class PlayerStatus
{
    private int id = -1;

    private CharaInitialStutas statusData = new();

    private int totalPower = 0;          // 戦闘力
    private int totalPower_Max = 999999; // 戦闘力最大値
       
    private Status status = new(0, 0, 0, 0, 0, 0);      // 追加効果無しのステータス
    private Status status_Min = new(0, 0, 0, 0, 0, 0);       // 現ランクでの最小値
    private Status status_Max = new(0, 0, 0, 0, 0, 0);       // 最大値

    private Status rankPoint = new(0, 0, 0, 0, 0, 0);        // 累積ランクPt
    private Status rankPoint_Max = new(0, 0, 0, 0, 0, 0);    // ステータスのランクPt最大値 プラスステータスを除き、上昇しない
    private Dictionary<StatusType, Rank> statusRank = new(); // 各ステータスのランク

    private Dictionary<CombiType, Rank> combiRank = new();         // 複合ステータスのランク
    private Dictionary<CombiType, int> combiRankPt = new();        // 複合ステータスのランクPt現在値
    private Dictionary<CombiType, int> combiRankPt_NextUp = new(); // 次にランクアップするときの累積ランクPt
    private Dictionary<CombiType, int> combiRankPtMax = new();     // 複合ステータスのランクPt最大値 プラスステータスを除き、上昇しない

    private Status resetBonusCoefficient = new(100,100,100,100,100,100);                // リセット時のステータス上昇量の係数　上昇量 = resetBonusCoefficient * plusStatus
    private Status resetBonusCoefficient_Max = new(1000, 1000, 1000, 1000, 1000, 1000); // リセット時のステータス最大値上昇量の係数　上昇量 = resetBonusCoefficient_Max * plusStatus
    private Status plusStatus = new(0, 0, 0, 0, 0, 0);    // 周回によるプラスステータス 1～99

    public CombiType evolutionType = CombiType.NORMAL; // 現在の進化形態
    public CombiType selectEvolutionType = CombiType.NORMAL; // 現在選択中の進化形態差分

    // 各進化形態解放済みかどうか
    public bool atkTypeReleased = false;
    public bool defTypeReleased = false;
    public bool tecTypeReleased = false;

    /// <summary>
    /// IDに応じて初期ステータス設定
    /// </summary>
    /// <param name="_id"></param>
    public PlayerStatus(int _id)
    {
        id = _id;

        Initialize(_id);
    }

    public CharaInitialStutas StatusData
    {
        get => statusData;
    }

    /// <summary>
    /// 戦闘力現在値
    /// </summary>
    public int TotalPower
    {
        get => totalPower;
    }

    /// <summary>
    /// 戦闘力最大値
    /// </summary>
    public int TotalPower_Max
    {
        get => totalPower_Max;
        set => totalPower_Max = value;
    }

    /// <summary>
    /// 初期化
    /// </summary>
    /// <param name="_id"></param>
    void Initialize(int _id)
    {
        Rank initRank = (Rank)System.Enum.ToObject(typeof(Rank), 0);     // 初期ランク
        Rank highestRank = (Rank)(System.Enum.GetValues(typeof(Rank)).Length - 1); // 最高ランク

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
        combiRankPt_NextUp.Clear();
        combiRankPtMax.Clear();
        for (int c = 0; c < System.Enum.GetValues(typeof(CombiType)).Length - 1; c++)
        {
            CombiType combi = (CombiType)System.Enum.ToObject(typeof(CombiType), c);
            combiRank.Add(combi, initRank);
            combiRankPt.Add(combi, 0);
            combiRankPt_NextUp.Add(combi, 0);
            combiRankPtMax.Add(combi, 0);
        }

        // 進化形態初期化
        evolutionType = CombiType.NORMAL;
        selectEvolutionType = CombiType.NORMAL;

        if (MasterDataLoader.MasterDataLoadComplete)
        {
            for (int i = 0; i < MasterData.CharaInitialStatus.Count; i++)
            {
                if (MasterData.CharaInitialStatus[i].charaId == _id)
                {
                    statusData = MasterData.CharaInitialStatus[i];

                    // ステータス初期化
                    status = new(statusData.statusInit[initRank]);
                    status_Min = new(statusData.statusInit[initRank]);
                    status_Max = new(statusData.statusMax[initRank]);

                    CalcStatusMin();

                    // ランクPt初期化
                    CharacterRankPoint rankPtData = new(statusData.rankPoint);

                    rankPoint = new Status(0, 0, 0, 0, 0, 0);

                    rankPoint_Max = new(rankPtData.rankPt_NextUp[highestRank]);

                    combiRankPt_NextUp[CombiType.ATK] = rankPtData.atkRankPt_NextUp[initRank];
                    combiRankPt_NextUp[CombiType.DEF] = rankPtData.defRankPt_NextUp[initRank];
                    combiRankPt_NextUp[CombiType.TEC] = rankPtData.tecRankPt_NextUp[initRank];

                    combiRankPtMax[CombiType.ATK] = rankPtData.atkRankPt_NextUp[highestRank];
                    combiRankPtMax[CombiType.DEF] = rankPtData.defRankPt_NextUp[highestRank];
                    combiRankPtMax[CombiType.TEC] = rankPtData.tecRankPt_NextUp[highestRank];
                }
            }
        }

        UpdateTotalPower();
    }

    /// <summary>
    /// セーブデータを元に初期化
    /// </summary>
    /// <param name="_data"></param>
    public void Initialize(PlayerSaveData _data)
    {
        id = _data.id;
        Initialize(_data.id);

        status = new Status(_data.hp, _data.mp, _data.atk, _data.def, _data.agi, _data.dex);
        rankPoint = new Status(_data.hp_rankPt, _data.mp_rankPt, _data.atk_rankPt, _data.def_rankPt, _data.agi_rankPt, _data.dex_rankPt);
        plusStatus = new Status(_data.hp_plusStatus, _data.mp_plusStatus, _data.atk_plusStatus, _data.def_plusStatus, _data.agi_plusStatus, _data.dex_plusStatus);

        evolutionType = (CombiType)System.Enum.Parse(typeof(CombiType), _data.evolutionType);
        selectEvolutionType = (CombiType)System.Enum.Parse(typeof(CombiType), _data.selectEvolutionType);

        atkTypeReleased = _data.atkTypeReleased;
        defTypeReleased = _data.defTypeReleased;
        tecTypeReleased = _data.tecTypeReleased;

        SetData();

        // クリア状況
        if (id == 1)
        {
            // クリア状況
            for (int i = 0; i < GameManager.IsBossClear1.Length; i++)
            {
                GameManager.IsBossClear1[i] = _data.isBossClear[i];
            }
            DifficultyManager.IsClearBossDifficulty1 = _data.isCrearBossDifficulty;
            GameManager.lastSelectDifficulty1 = _data.selectDifficulty;
        }
        else if (id == 2)
        {
            // クリア状況
            for (int i = 0; i < GameManager.IsBossClear2.Length; i++)
            {
                GameManager.IsBossClear2[i] = _data.isBossClear[i];
            }
            DifficultyManager.IsClearBossDifficulty2 = _data.isCrearBossDifficulty;
            GameManager.lastSelectDifficulty2 = _data.selectDifficulty;
        }
        else
        {
            // クリア状況
            for (int i = 0; i < GameManager.IsBossClear1.Length; i++)
            {
                GameManager.IsBossClear1[i] = false;
                GameManager.IsBossClear2[i] = false;
            }
            DifficultyManager.IsClearBossDifficulty1 = 0;
            DifficultyManager.IsClearBossDifficulty2 = 0;
        }
    }

    /// <summary>
    /// セーブデータから必要なデータや数値を算出
    /// </summary>
    void SetData()
    {
        UpdateTotalPower();

        for (int st = 0; st < System.Enum.GetValues(typeof(StatusType)).Length; st++)
        {
            StatusType type = (StatusType)System.Enum.ToObject(typeof(StatusType), st);

            Rank rank = (Rank)System.Enum.ToObject(typeof(Rank), 0);
            int rankNum = 0;
            Status rankPtNextUp = new(0, 0, 0, 0, 0, 0);

            for (int r = 0; r < System.Enum.GetValues(typeof(Rank)).Length; r++)
            {
                rank = (Rank)System.Enum.ToObject(typeof(Rank), r);

                rankPtNextUp = StatusData.rankPoint.rankPt_NextUp[rank];
                if (rankPoint.GetStatus(type) >= rankPtNextUp.GetStatus(type) && rank < (Rank)(System.Enum.GetValues(typeof(Rank)).Length - 1))
                {
                    rankNum++;
                    continue;
                }
                else break;
            }

            // ランク変更
            statusRank[type] = (Rank)System.Enum.ToObject(typeof(Rank), rankNum);

            // ステータス最小/最大値更新
            int statusMin = StatusData.statusInit[rank].GetStatus(type);
            SetStatusMin(type, statusMin);
            int statusMax = StatusData.statusMax[rank].GetStatus(type);
            SetStatusMax(type, statusMax);
        }

        // 複合ランク系
        for (int ct = 0; ct < System.Enum.GetValues(typeof(CombiType)).Length - 1; ct++)
        {
            CombiType type = (CombiType)System.Enum.ToObject(typeof(CombiType), ct);

            // 累積Pt計算
            switch (type)
            {
                case CombiType.ATK:
                    combiRankPt[type] = rankPoint.atk + rankPoint.mp;
                    break;

                case CombiType.DEF:
                    combiRankPt[type] = rankPoint.hp + rankPoint.def;
                    break;

                case CombiType.TEC:
                    combiRankPt[type] = rankPoint.agi + rankPoint.dex;
                    break;
            }

            Rank rank = (Rank)System.Enum.ToObject(typeof(Rank), 0);
            int rankNum = 0;

            for (int r = 0; r < System.Enum.GetValues(typeof(Rank)).Length; r++)
            {
                rank = (Rank)System.Enum.ToObject(typeof(Rank), r);

                if (combiRankPt[type] >= StatusData.rankPoint.GetCombiRankNextPt(type, rank))
                {
                    rankNum++;
                }
            }

            // ランク変更
            combiRank[type] = (Rank)System.Enum.ToObject(typeof(Rank), rankNum);

            // ランクに応じてランクポイント最大値更新
            SetCombiRankPtNextUp(type, StatusData.rankPoint.GetCombiRankNextPt(type, rank));
        }
    }

    // 以下変数取得・変更系
    // 総合戦闘力
    /// <summary>
    /// 総合戦闘力の計算
    /// </summary>
    public void UpdateTotalPower()
    {
        int total = 0;

        for (int st = 0; st < System.Enum.GetValues(typeof(StatusType)).Length; st++)
        {
            StatusType type = (StatusType)System.Enum.ToObject(typeof(StatusType), st);

            if (type == StatusType.MP || type == StatusType.DEF || type == StatusType.DEX)
            {
                total += GetStatus(type) * 2;
            }
            else total += GetStatus(type);

            // ランクボーナス
            var rank = GetRank(type);
            switch (rank)
            {
                case Rank.C:
                    total += 1000;
                    break;
                case Rank.B:
                    total += 2000;
                    break;
                case Rank.A:
                    total += 4000;
                    break;
                case Rank.S:
                    total += 7000;
                    break;
                case Rank.SS:
                    total += 12000;
                    break;
            }
        }
        
        // 複合ランクボーナス
        for (int cr = 0; cr < System.Enum.GetValues(typeof(CombiType)).Length; cr++)
        {
            CombiType type = (CombiType)System.Enum.ToObject(typeof(CombiType), cr);

            if (type == CombiType.NORMAL) continue;

            var rank = GetCombiRank(type);

            switch (rank)
            {
                case Rank.C:
                    total += 500;
                    break;
                case Rank.B:
                    total += 1000;
                    break;
                case Rank.A:
                    total += 2000;
                    break;
                case Rank.S:
                    total += 3000;
                    break;
                case Rank.SS:
                    total += 5000;
                    break;
            }
        }

        if (total <= totalPower_Max)
        {
            totalPower = total;
        }
        else totalPower = totalPower_Max;
    }

    // ステータス
    /// <summary>
    /// 指定したステータスの現在値を取得
    /// </summary>
    /// <param name="_type">ステータスの種類</param>
    /// <returns>指定したステータスの現在値</returns>
    public int GetStatus(StatusType _type)
    {
        return status.GetStatus(_type);
    }

    /// <summary>
    /// 指定したステータスの値を変更
    /// </summary>
    /// <param name="_type">ステータスの種類</param>
    /// <param name="_num">変更後の値</param>
    public void SetStatus(StatusType _type, int _num)
    {
        status.SetStatus(_type, _num);
    }

    /// <summary>
    /// 指定ステータスの最小値を取得
    /// </summary>
    /// <param name="_type">ステータスの種類</param>
    /// <returns>指定ステータスの最小値</returns>
    public int GetStatusMin(StatusType _type)
    {
        return status_Min.GetStatus(_type);
    }

    /// <summary>
    /// 指定したステータスの最小値を変更
    /// </summary>
    /// <param name="_type">ステータスの種類</param>
    /// <param name="_num">変更後の最小値</param>
    public void SetStatusMin(StatusType _type, int _num)
    {
        int plusBonus = GetAdditionalEffects(_type, false);
        status_Min.SetStatus(_type, _num + plusBonus);
    }

    /// <summary>
    /// 指定ステータスの最大値を取得
    /// </summary>
    /// <param name="_type">ステータスの種類</param>
    /// <returns>指定ステータスの最大値</returns>
    public int GetStatusMax(StatusType _type)
    {
        return status_Max.GetStatus(_type);
    }

    /// <summary>
    /// 指定したステータスの最大値を変更
    /// </summary>
    /// <param name="_type">ステータスの種類</param>
    /// <param name="_num">変更後の最大値</param>
    public void SetStatusMax(StatusType _type, int _num)
    {
        int plusBonus = GetAdditionalEffects(_type, false);
        if (GetRank(_type) == Rank.SS) plusBonus = GetAdditionalEffects_Max(_type, false);

        status_Max.SetStatus(_type, _num + plusBonus);
    }

    /// <summary>
    /// 現在の進化形態を取得
    /// </summary>
    public CombiType GetEvolutionType()
    {
        return evolutionType;
    }

    public void SetEvolutionType(CombiType _type)
    {
        if (evolutionType == CombiType.NORMAL)
        {
            var sound = GameObject.FindObjectOfType<SoundController>();
            if (sound != null) sound.PlayCharaReleaseJingle();

            evolutionType = _type;
            SetSelectEvolutionType(_type);

            switch (_type)
            {
                case CombiType.ATK:
                    PlayerDataManager.player.atkTypeReleased = true;
                    break;
                case CombiType.DEF:
                    PlayerDataManager.player.defTypeReleased = true;
                    break;
                case CombiType.TEC:
                    PlayerDataManager.player.tecTypeReleased = true;
                    break;
            }
        }
    }

    public CombiType GetSelectEvolutionType()
    {
        return selectEvolutionType;
    }

    public void SetSelectEvolutionType(CombiType _type)
    {
        selectEvolutionType = _type;
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

    public Status GetRank()
    {
        Status rank = new((int)statusRank[StatusType.HP], (int)statusRank[StatusType.MP], (int)statusRank[StatusType.ATK], 
            (int)statusRank[StatusType.DEF], (int)statusRank[StatusType.AGI], (int)statusRank[StatusType.DEX]);
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
        int n = rankPoint.GetStatus(_type);
        return n;
    }

    /// <summary>
    /// 指定したステータスのランクポイント現在値を変更
    /// </summary>
    /// <param name="_type">ステータスの種類</param>
    /// <param name="_num">変更後の値</param>
    public void SetRankPt(StatusType _type, int _num)
    {
        rankPoint.SetStatus(_type, _num);
    }

    /// <summary>
    /// 指定したステータスの、前回ランクアップしたときの累積Ptを取得
    /// </summary>
    /// <param name="_type">ステータスの種類</param>
    public int GetRankPtLastUp(StatusType _type)
    {
        var rank = GetRank(_type);
        int a = 0;
        if ((int)rank > 0)
        {
            a = StatusData.rankPoint.rankPt_NextUp[(Rank)(rank - 1)].GetStatus(_type);
        }

        return a;
    }

    public int GetRankPtUp(StatusType _type, Rank _rank)
    {
        int n = StatusData.rankPoint.rankPt_NextUp[_rank].GetStatus(_type);
        return n;
    }

    /// <summary>
    /// 指定したステータスの、次にランクアップするときの累積Ptを取得
    /// </summary>
    /// <param name="_type">ステータスの種類</param>
    public int GetRankPtNextUp(StatusType _type)
    {
        var rank = GetRank(_type);
        int a = StatusData.rankPoint.rankPt_NextUp[rank].GetStatus(_type);
        return a;
    }

    /// <summary>
    /// 指定したステータスのランクポイントの最大値を取得
    /// </summary>
    /// <param name="_type">ステータスの種類</param>
    /// <returns>指定したステータスのランクポイント最大値</returns>
    public int GetRankPtMax(StatusType _type)
    {
        int n = rankPoint_Max.GetStatus(_type);
        return n;
    }

    // 複合ステータスのランク
    /// <summary>
    /// 指定した複合ステータスのランクを取得
    /// </summary>
    /// <param name="_type">複合ステータスの種類</param>
    /// <returns>指定した複合ステータスのランク</returns>
    public Rank GetCombiRank(CombiType _type)
    {
        if (_type == CombiType.NORMAL) return 0;
        
        Rank rank = combiRank[_type];
        if (rank > Rank.SS) rank = Rank.SS;

        return rank;
    }

    /// <summary>
    /// 指定した複合ステータスのランクを変更
    /// </summary>
    /// <param name="_type">複合ステータスの種類</param>
    /// <param name="_num">変更後のランク</param>
    public void SetCombiRank(CombiType _type, Rank _rank)
    {
        var r = _rank;
        if (_rank > Rank.SS) r = Rank.SS;
        combiRank[_type] = r;
    }

    // 複合ステータスのランクPt
    /// <summary>
    /// 指定した複合ステータスのランクポイント現在値を取得
    /// </summary>
    /// <param name="_type">複合ステータスの種類</param>
    /// <returns>指定した複合ステータスのランクポイント現在値</returns>
    public int GetCombiRankPt(CombiType _type)
    {
        int n = combiRankPt[_type];
        return n;
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
        int n = combiRankPtMax[_type];
        return n;
    }

    /// <summary>
    /// 指定した複合ステータスの、次にランクアップするときの累積Ptを取得
    /// </summary>
    /// <param name="_type">ステータスの種類</param>
    public int GetCombiRankPtNextUp(CombiType _type)
    {
        int n = combiRankPt_NextUp[_type];
        return n;
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
        int n = plusStatus.GetStatus(_type);
        return n;
    }

    /// <summary>
    /// プラスステータスを変更
    /// </summary>
    public void SetPlusStatus(Status _status)
    {
        plusStatus = new (_status);
    }

    /// <summary>
    /// 指定したステータスのプラスステータスを変更
    /// </summary>
    /// <param name="_type">ステータスの種類</param>
    /// <param name="_num">変更後のプラスステータス</param>
    public void SetPlusStatus(StatusType _type, int _num)
    {
        plusStatus.SetStatus(_type, _num);
    }

    /// <summary>
    /// 育成リセットによる追加効果量（ステータス上昇量）を取得
    /// </summary>
    /// <param name="_type">ステータスの種類</param>
    /// <param name="isNextEffects">falseのとき上昇量の現在値を返す　trueのとき次育成リセットした場合の上昇量を返す</param>
    /// <returns>育成リセットによる追加効果量（ステータス上昇量）</returns>
    public int GetAdditionalEffects(StatusType _type, bool isNextEffects)
    {
        int a = 0;

        int plus = plusStatus.GetStatus(_type);

        if (isNextEffects)
        {
            a = (plus + 1) * resetBonusCoefficient.GetStatus(_type);
        }
        else a = plus * resetBonusCoefficient.GetStatus(_type);

        return a;
    }

    public int GetAdditionalEffects_Max(StatusType _type, bool isNextEffects)
    {
        int a = 0;

        int plus = plusStatus.GetStatus(_type);

        if (isNextEffects)
        {
            a = (plus + 1) * resetBonusCoefficient_Max.GetStatus(_type);
        }
        else a = plus * resetBonusCoefficient_Max.GetStatus(_type);

        return a;
    }

    public void CalcStatusMin()
    {
        status.hp += GetAdditionalEffects(StatusType.HP, false);
        status.mp += GetAdditionalEffects(StatusType.MP, false);
        status.atk += GetAdditionalEffects(StatusType.ATK, false);
        status.def += GetAdditionalEffects(StatusType.DEF, false);
        status.agi += GetAdditionalEffects(StatusType.AGI, false);
        status.dex += GetAdditionalEffects(StatusType.DEX, false);

        status_Min.hp += GetAdditionalEffects(StatusType.HP, false);
        status_Min.mp += GetAdditionalEffects(StatusType.MP, false);
        status_Min.atk += GetAdditionalEffects(StatusType.ATK, false);
        status_Min.def += GetAdditionalEffects(StatusType.DEF, false);
        status_Min.agi += GetAdditionalEffects(StatusType.AGI, false);
        status_Min.dex += GetAdditionalEffects(StatusType.DEX, false);
    }
}
