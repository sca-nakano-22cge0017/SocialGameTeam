using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Master;

public class PlayerDataManager : MonoBehaviour
{
    public static PlayerStatus player = new(1); // 現在の使用キャラクター

    private static PlayerStatus chara1 = new(1);
    private static PlayerStatus chara2 = new(2);
    
    private static bool playerDataLoadComlete = false;
    public static bool PlayerDataLoadComplete
    {
        get { return playerDataLoadComlete; }
        private set { }
    }

    /// <summary>
    /// プレイヤーのデータ保存
    /// </summary>
    public static void Save()
    {
        if (GameManager.SelectChara == 1)
            chara1 = player;
        if (GameManager.SelectChara == 2)
            chara2 = player;

        SaveData saveData = new();

        for (int i = 1; i <= 2; i++)
        {
            PlayerStatus p = i == 1 ? chara1 : chara2;

            PlayerSaveData c = new();
            c.id = i;

            c.evolutionType = p.evolutionType.ToString();
            c.selectEvolutionType = p.selectEvolutionType.ToString();

            c.atkTypeReleased = p.atkTypeReleased;
            c.defTypeReleased = p.defTypeReleased;
            c.tecTypeReleased = p.tecTypeReleased;

            c.hp = p.GetStatus(StatusType.HP);
            c.mp = p.GetStatus(StatusType.MP);
            c.atk = p.GetStatus(StatusType.ATK);
            c.def = p.GetStatus(StatusType.DEF);
            c.agi = p.GetStatus(StatusType.AGI);
            c.dex = p.GetStatus(StatusType.DEX);

            c.hp_rankPt = p.GetRankPt(StatusType.HP);
            c.mp_rankPt = p.GetRankPt(StatusType.MP);
            c.atk_rankPt = p.GetRankPt(StatusType.ATK);
            c.def_rankPt = p.GetRankPt(StatusType.DEF);
            c.agi_rankPt = p.GetRankPt(StatusType.AGI);
            c.dex_rankPt = p.GetRankPt(StatusType.DEX);

            c.hp_plusStatus = p.GetPlusStatus(StatusType.HP);
            c.mp_plusStatus = p.GetPlusStatus(StatusType.MP);
            c.atk_plusStatus = p.GetPlusStatus(StatusType.ATK);
            c.def_plusStatus = p.GetPlusStatus(StatusType.DEF);
            c.agi_plusStatus = p.GetPlusStatus(StatusType.AGI);
            c.dex_plusStatus = p.GetPlusStatus(StatusType.DEX);

            if (i == 1) saveData.chara1 = c;
            if (i == 2) saveData.chara2 = c;
        }

        saveData.selectChara = GameManager.SelectChara == -1 ? 1 : GameManager.SelectChara;
        saveData.isFirstStart = GameManager.isFirstStart;
        saveData.isCrearBossDifficulty = DifficultyManager.IsClearBossDifficulty;

        for (int i = 0; i < GameManager.IsBossClear.Length; i++)
        {
            saveData.isBossClear[i] = GameManager.IsBossClear[i];
        }

        // スタミナ関連
        saveData.staminaData.lastTime = StaminaManager.lastTimeStr;
        saveData.staminaData.lastStamina = StaminaManager.lastStamina;
        saveData.staminaData.lastRecoveryTime = StaminaManager.lastRecoveryTime;
        saveData.staminaData.lastCompleteRecoveryTime = StaminaManager.lastCompleteRecoveryTime;

        // チュートリアル進捗
        saveData.tutorialData = new TutorialProgress(GameManager.TutorialProgress);

        CharaSelectManager.savePlayerData(saveData);
        Debug.Log("データセーブ完了");
    }
    
    /// <summary>
    /// プレイヤーのデータ取得
    /// </summary>
    public static void Load()
    {
        SaveData data = CharaSelectManager.loadPlayerData();

        player.Initialize(data.chara1);
        chara1.Initialize(data.chara1);
        chara2.Initialize(data.chara2);

        player = chara1;

        for (int i = 0; i < GameManager.IsBossClear.Length; i++)
        {
            GameManager.IsBossClear[i] = data.isBossClear[i];
        }

        GameManager.isFirstStart = data.isFirstStart;
        DifficultyManager.IsClearBossDifficulty = data.isCrearBossDifficulty;
        GameManager.SelectChara = data.selectChara;

        // スタミナ関連
        StaminaManager.lastTimeStr = data.staminaData.lastTime;
        StaminaManager.lastStamina = data.staminaData.lastStamina;
        StaminaManager.lastRecoveryTime = data.staminaData.lastRecoveryTime;
        StaminaManager.lastCompleteRecoveryTime = data.staminaData.lastCompleteRecoveryTime;

        // チュートリアル進捗
        GameManager.TutorialProgress = new TutorialProgress(data.tutorialData);

        playerDataLoadComlete = true;
        Debug.Log("セーブデータロード完了");
    }

    /// <summary>
    /// キャラクターデータ初期化
    /// </summary>
    /// <param name="_id">キャラクターID</param>
    public static void CharacterInitialize(int _id)
    {
        if (_id != 1 && _id != 2)
        {
            Debug.Log("キャラクターIDが誤っています");
            return;
        }

        if (_id == 1)
        {
            player = new(1);
        }
        if (_id == 2)
        {
            player = new(2);
        }
    }

    /// <summary>
    /// キャラクターデータ初期化
    /// </summary>
    /// <param name="_id">キャラクターID</param>
    public static void CharacterInitialize()
    {
        player = new(1);
        chara1 = new(1);
        chara2 = new(2);

        Save();
    }

    /// <summary>
    /// キャラクター育成リセット
    /// </summary>
    /// <param name="_id">キャラクターID</param>
    public static void TraningReset(int _id)
    {
        if (_id != 1 && _id != 2)
        {
            Debug.Log("キャラクターIDが誤っています");
            return;
        }

        // プラス値は持ち越す
        Status plus = new(player.GetPlusStatus());

        bool atk = player.atkTypeReleased;
        bool def = player.defTypeReleased;
        bool tec = player.tecTypeReleased;

        if (_id == 1)
        {
            player = new(1);
            chara1 = new(1);
        }
        if (_id == 2)
        {
            player = new(2);
            chara2 = new(2);
        }

        player.SetPlusStatus(plus);
        player.atkTypeReleased = atk;
        player.defTypeReleased = def;
        player.tecTypeReleased = tec;

        Save();

        SpecialTecniqueManager stm = FindObjectOfType<SpecialTecniqueManager>();
        if (stm) stm.ReleaseInitialize();
    }

    /// <summary>
    /// 使用キャラクター変更
    /// </summary>
    /// <param name="_id">キャラクターID</param>
    public static void CharacterChange(int _id)
    {
        if (_id != 1 && _id != 2)
        {
            Debug.Log("キャラクターIDが誤っています");
            return;
        }

        if (_id == 1)
        {
            player = chara1;
        }
        if (_id == 2)
        {
            player = chara2;
        }

        Save();

        SpecialTecniqueManager stm = FindObjectOfType<SpecialTecniqueManager>();
        if (stm) stm.ReleaseInitialize();
    }

    /// <summary>
    /// ランクポイント追加
    /// </summary>
    /// <param name="_type">追加するステータスの種類</param>
    /// <param name="_amount">追加量</param>
    public static void RankPtUp(StatusType _type, int _amount)
    {
        int rankPt = player.GetRankPt(_type);               // 現在のランクPt
        int rankPt_Max = player.GetRankPtMax(_type);       // 最大ランクPt
        int result = rankPt + _amount;                      // 加算後の数値

        CombiType c_type = NormalStatusToCombiStatus(_type);
        int combiRankPt = player.GetCombiRankPt(c_type);         // 現在の複合ステランクPt
        int combiRankPt_Max = player.GetCombiRankPtMax(c_type); // 複合ステ最大ランクPt
        int combiResult = combiRankPt + _amount;                 // 加算後の数値

        // ステPt最大値未満　かつ　複合ステPt最大値未満
        if (rankPt_Max > result && combiRankPt_Max > combiResult)
        {
            // ステPt・複合ステPt加算
            player.SetRankPt(_type, result);
            player.SetCombiRankPt(c_type, combiRankPt + _amount);
        }
        // ステPt最大値以上　かつ　複合ステPt最大値未満
        else if (rankPt_Max <= result && combiRankPt_Max > combiResult)
        {
            int diff = rankPt_Max - rankPt;

            // ステPtを上限まで加算、複合ステPtに(ステPt最大値-ステPt現在値)加算
            player.SetRankPt(_type, rankPt_Max);
            player.SetCombiRankPt(c_type, combiRankPt + diff);
        }
        // ステPt最大値未満　かつ　複合ステPt最大値以上
        else if (rankPt_Max > result && combiRankPt_Max <= combiResult)
        {
            int diff = combiRankPt_Max - combiRankPt;

            // ステPtに(複合ステPt最大値-複合ステPt現在値)加算、複合ステPtを上限まで加算
            player.SetRankPt(_type, rankPt + diff);
            player.SetCombiRankPt(c_type, combiRankPt_Max);
        }
        // ステPt最大値以上　かつ　複合ステPt最大値以上
        else if (rankPt_Max <= result && combiRankPt_Max <= combiResult)
        {
            int diff_normal = rankPt_Max - rankPt;
            int diff_combi = combiRankPt_Max - combiRankPt;

            int diff = diff_normal < diff_combi ? diff_normal : diff_combi;

            // より最大値と現在値の差が小さい方の差を足す
            player.SetRankPt(_type, rankPt + diff);
            player.SetCombiRankPt(c_type, combiRankPt + diff);
        }

        RankUpCheck();
        CombiRankUpCheck();

        CalcStatus(_type);

        Save();
    }

    static void RankUpCheck()
    {
        for (int i = 0; i < System.Enum.GetValues(typeof(StatusType)).Length; i++)
        {
            StatusType type = (StatusType)System.Enum.ToObject(typeof(StatusType), i);
            int rankPt = player.GetRankPt(type);               // 現在のランクPt
            int rankPt_NextUp = player.GetRankPtNextUp(type); // 次にランクアップするときの累積Pt

            if (rankPt >= rankPt_NextUp && player.GetRank(type) != Rank.SS)
            {
                RankUp(type);
                RankUpCheck();
            }
        }
    }

    static void CombiRankUpCheck()
    {
        for (int j = 0; j < System.Enum.GetValues(typeof(CombiType)).Length - 1; j++)
        {
            CombiType c_type = (CombiType)System.Enum.ToObject(typeof(CombiType), j);
            int combiRankPt = player.GetCombiRankPt(c_type);               // 現在の複合ステランクPt
            int combiRankPt_NextUp = player.GetCombiRankPtNextUp(c_type); // 次にランクアップするときの累積Pt

            if (combiRankPt >= combiRankPt_NextUp && player.GetCombiRank(c_type) != Rank.SS)
            {
                CombiRankUp(c_type);
                CombiRankUpCheck();
            }
        }
    }

    static void RankUp(StatusType _type)
    {
        if (player.GetRank(_type) >= (Rank)(System.Enum.GetValues(typeof(Rank)).Length - 1)) return;

        CharacterRankPoint rankPtData = player.StatusData.rankPoint;

        AddStatus_Bonus(_type);
        Rank lastRank = player.GetRank(_type);
        Status lastPt = (int)lastRank == 0 ? new Status(0,0,0,0,0,0) : rankPtData.rankPt_NextUp[lastRank];

        // ランク上昇
        int rankNum = (int)player.GetRank(_type);
        rankNum++;
        player.SetRank(_type, (Rank)System.Enum.ToObject(typeof(Rank), rankNum));

        // ランクに応じてランクポイント最大値更新
        Rank rank = player.GetRank(_type);

        // ステータス最小/最大値更新
        int statusMin = player.StatusData.statusInit[rank].GetStatus(_type);
        int statusMax = player.StatusData.statusMax[rank].GetStatus(_type);

        player.SetStatusMin(_type, statusMin);
        player.SetStatusMax(_type, statusMax);
    }

    static void CombiRankUp(CombiType _type)
    {
        if (player.GetCombiRank(_type) == Rank.SS) return;

        // ランク上昇
        int rankNum = (int)player.GetCombiRank(_type);
        rankNum++;
        player.SetCombiRank(_type, (Rank)System.Enum.ToObject(typeof(Rank), rankNum));

        // ランクに応じてランクポイント最大値更新
        CharacterRankPoint rankPtData = player.StatusData.rankPoint;
        Rank rank = player.GetCombiRank(_type);

        player.SetCombiRankPtNextUp(_type, rankPtData.GetCombiRankNextPt(_type, rank));

        // 進化
        if (player.GetCombiRank(_type) == Rank.SS)
        {
            player.SetEvolutionType(_type);
        }
    }

    /// <summary>
    /// ステータス計算
    /// </summary>
    static void CalcStatus(StatusType _type)
    {
        Rank rank = player.GetRank(_type);

        float currentRankPt = player.GetRankPt(_type);

        if (currentRankPt >= player.GetRankPtMax(_type))
        {
            int s = player.StatusData.statusMax[(Rank)(System.Enum.GetValues(typeof(Rank)).Length - 1)].GetStatus(_type) + player.GetAdditionalEffects_Max(_type, false);
            player.SetStatus(_type, s);
            return;
        }

        float statusMin = player.GetStatusMin(_type);
        float statusMax = player.GetStatusMax(_type);

        float rankPtMin = (int)rank == 0 ? 0 : player.GetRankPtLastUp(_type);
        float rankPtMax = player.GetRankPtNextUp(_type);

        float a = (float)(statusMax - statusMin) / (float)(rankPtMax - rankPtMin); // グラフの傾き

        int currentStatus = (int)(((a * currentRankPt) + (statusMin - a * rankPtMin)));   // 現在のステータス
        player.SetStatus(_type, currentStatus);

        player.UpdateTotalPower();
    }

    /// <summary>
    /// ランクアップボーナスによるステータス上昇
    /// </summary>
    /// <param name="_type"></param>
    static void AddStatus_Bonus(StatusType _type)
    {
        Rank rank = player.GetRank(_type);

        if (rank >= (Rank)(System.Enum.GetValues(typeof(Rank)).Length - 1)) return;

        Status bonus = player.StatusData.rankUpBonus[rank];
        int amount = bonus.GetStatus(_type);

        Status statusMax = player.StatusData.statusMax[rank];
        int statusMaxAmount = statusMax.GetStatus(_type);

        player.SetStatus(_type, statusMaxAmount);

        int currentStatus = player.GetStatus(_type);
        player.SetStatus(_type, currentStatus + amount);

        player.UpdateTotalPower();
    }

    /// <summary>
    /// 追加効果（プラスステータスの上昇）があるかどうかを調べる
    /// </summary>
    /// <returns>trueのときは追加効果アリ</returns>
    public static bool IsAddPlusStatus()
    {
        bool a = false;

        for (int s = 0; s < System.Enum.GetValues(typeof(StatusType)).Length; s++)
        {
            StatusType status = (StatusType)System.Enum.ToObject(typeof(StatusType), s);

            if (player.GetRank(status) == (Rank)(System.Enum.GetValues(typeof(Rank)).Length - 1))
            {
                a = true;
                break;
            }
        }

        return a;
    }

    /// <summary>
    /// 育成リセットの追加効果をstringで返す
    /// </summary>
    /// <returns>育成リセットの追加効果</returns>
    public static string GetResetEffects()
    {
        string t = "";

        for (int s = 0; s < System.Enum.GetValues(typeof(StatusType)).Length; s++)
        {
            StatusType status = (StatusType)System.Enum.ToObject(typeof(StatusType), s);

            if (player.GetRank(status) == (Rank)(System.Enum.GetValues(typeof(Rank)).Length - 1))
            {
                int st1 = player.GetAdditionalEffects(status, true);
                int st2 = player.GetAdditionalEffects_Max(status, true);
                t += StutasTypeToString(status) + "ステータス +" + st1 + "\n";
                t += StutasTypeToString(status) + "ステータス上限 +" + st2 + "\n";
            }
        }

        return t;
    }

    /// <summary>
    /// プラスステータスの効果をstringで返す
    /// </summary>
    /// <param name="_type">ステータスの種類</param>
    /// <returns>効果</returns>
    public static string GetResetCurrentEffects(StatusType _type)
    {
        string t = "";

        int st1 = player.GetAdditionalEffects(_type, false);
        int st2 = player.GetAdditionalEffects_Max(_type, false);

        if (st1 <= 0 || st2 <= 0) return t;

        t += StutasTypeToString(_type) + "ステータス +" + st1 + "\n";
        t += StutasTypeToString(_type) + "ステータス上限 +" + st2 + "\n";

        return t;
    }

    /// <summary>
    /// 育成リセット
    /// </summary>
    public static void TraningReset()
    {
        AddPlusStatus();

        Save();

        TraningReset(GameManager.SelectChara);
    }

    /// <summary>
    /// プラスステータス判定・追加
    /// </summary>
    static void AddPlusStatus()
    {
        int statusAmount = System.Enum.GetValues(typeof(StatusType)).Length;

        for (int s = 0; s < statusAmount; s++)
        {
            StatusType type = (StatusType)System.Enum.ToObject(typeof(StatusType), s);

            if (player.GetRank(type) == (Rank)(System.Enum.GetValues(typeof(Rank)).Length - 1))
            {
                int current = player.GetPlusStatus(type);
                player.SetPlusStatus(type, current + 1);

                player.SetRankPt(type, 0);
            }
        }
    }

    /// <summary>
    /// 指定したステータスを利用する複合ステータスを取得
    /// </summary>
    /// <param name="_type">調べるステータスの種類</param>
    /// <returns>引数のステータスを利用する複合ステータスの種類</returns>
    public static CombiType NormalStatusToCombiStatus(StatusType _type)
    {
        switch (_type)
        {
            case StatusType.HP:
                return CombiType.DEF;

            case StatusType.MP:
                return CombiType.ATK;

            case StatusType.ATK:
                return CombiType.ATK;

            case StatusType.DEF:
                return CombiType.DEF;

            case StatusType.AGI:
                return CombiType.TEC;

            case StatusType.DEX:
                return CombiType.TEC;

            default:
                return CombiType.ATK;
        }
    }

    public static string StutasTypeToString(StatusType _type)
    {
        switch (_type)
        {
            case StatusType.HP:
                return "体力";

            case StatusType.MP:
                return "魔力";

            case StatusType.ATK:
                return "攻撃";

            case StatusType.DEF:
                return "守備";

            case StatusType.AGI:
                return "速度";

            case StatusType.DEX:
                return "器用";

            default:
                return "体力";
        }
    }
}
