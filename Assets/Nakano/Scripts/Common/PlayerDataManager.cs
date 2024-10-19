using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Master;

public class PlayerDataManager : MonoBehaviour
{
    public static PlayerStatus player = null;

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
        player = new PlayerStatus(_id);
    }

    static void PlayerNullCheck()
    {
        if (player == null)
        {
            int chara = GameManager.SelectChara != -1 ? GameManager.SelectChara : 1;

            PlayerCreate(chara);
        }
    }

    /// <summary>
    /// ランクポイント追加
    /// </summary>
    /// <param name="_type">追加するステータスの種類</param>
    /// <param name="_amount">追加量</param>
    public static void RankPtUp(StatusType _type, int _amount)
    {
        PlayerNullCheck();

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
            player.SetRankPt(_type, rankPt + _amount);
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

        CalcStatus(_type);
    }

    static void RankUpCheck()
    {
        for (int i = 0; i < System.Enum.GetValues(typeof(StatusType)).Length; i++)
        {
            StatusType _type = (StatusType)System.Enum.ToObject(typeof(StatusType), i);
            int rankPt = player.GetRankPt(_type);               // 現在のランクPt
            int rankPt_NextUp = player.GetRankPtNextUp(_type); // 次にランクアップするときの累積Pt
            
            if (rankPt >= rankPt_NextUp)
            {
                RankUp(_type);
            }
        }

        for (int i = 0; i < System.Enum.GetValues(typeof(CombiType)).Length; i++)
        {
            CombiType c_type = (CombiType)System.Enum.ToObject(typeof(CombiType), i);
            int combiRankPt = player.GetCombiRankPt(c_type);               // 現在の複合ステランクPt
            int combiRankPt_NextUp = player.GetCombiRankPtNextUp(c_type); // 次にランクアップするときの累積Pt

            if (combiRankPt >= combiRankPt_NextUp)
            {
                CombiRankUp(c_type);
            }
        }
    }

    static void RankUp(StatusType _type)
    {
        if (player.GetRank(_type) == Rank.SS) return;

        AddStatus_Bonus(_type);

        // ランク上昇
        int rankNum = (int)player.GetRank(_type);
        rankNum++;
        player.SetRank(_type, (Rank)System.Enum.ToObject(typeof(Rank), rankNum));

        // ランクに応じてランクポイント最大値更新
        CharacterRankPoint rankPtData = player.StatusData.rankPoint;
        Rank rank = player.GetRank(_type);
        Status nextPt = rankPtData.rankPt_NextUp[rank];

        switch (_type)
        {
            case StatusType.ATK:
                player.SetRankPtNextUp(_type, nextPt.atk);
                break;

            case StatusType.MP:
                player.SetRankPtNextUp(_type, nextPt.mp);
                break;

            case StatusType.HP:
                player.SetRankPtNextUp(_type, nextPt.hp);
                break;

            case StatusType.DEF:
                player.SetRankPtNextUp(_type, nextPt.def);
                break;

            case StatusType.SPD:
                player.SetRankPtNextUp(_type, nextPt.spd);
                break;

            case StatusType.DEX:
                player.SetRankPtNextUp(_type, nextPt.dex);
                break;
        }
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

        switch (_type)
        {
            case CombiType.ATK:
                player.SetCombiRankPtNextUp(_type, rankPtData.atkRankPt_NextUp[rank]);
                break;

            case CombiType.DEF:
                player.SetCombiRankPtNextUp(_type, rankPtData.defRankPt_NextUp[rank]);
                break;

            case CombiType.TEC:
                player.SetCombiRankPtNextUp(_type, rankPtData.tecRankPt_NextUp[rank]);
                break;
        }
    }

    /// <summary>
    /// ステータス計算
    /// </summary>
    static void CalcStatus(StatusType _type)
    {
        Rank rank = player.GetRank(_type);
        int currentStatus = player.GetStatus(_type);
    }

    /// <summary>
    /// ランクアップボーナスによるステータス上昇
    /// </summary>
    /// <param name="_type"></param>
    static void AddStatus_Bonus(StatusType _type)
    {
        Rank rank = player.GetRank(_type);
        Status bonus = player.StatusData.rankUpBonus[rank];
        int amount = 0;

        switch (_type)
        {
            case StatusType.HP:
                amount = bonus.hp;
                break;

            case StatusType.MP:
                amount = bonus.mp;
                break;

            case StatusType.ATK:
                amount = bonus.atk;
                break;

            case StatusType.DEF:
                amount = bonus.def;
                break;

            case StatusType.SPD:
                amount = bonus.spd;
                break;

            case StatusType.DEX:
                amount = bonus.dex;
                break;
        }

        int currentStatus = player.GetStatus(_type);
        player.SetStatus(_type, currentStatus + amount);
    }

    /// <summary>
    /// 育成リセット
    /// </summary>
    public static void TraningReset()
    {
        
    }

    /// <summary>
    /// 指定したステータスを利用する複合ステータスを取得
    /// </summary>
    /// <param name="_type">調べるステータスの種類</param>
    /// <returns>引数のステータスを利用する複合ステータスの種類</returns>
    static CombiType NormalStatusToCombiStatus(StatusType _type)
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

            case StatusType.SPD:
                return CombiType.TEC;

            case StatusType.DEX:
                return CombiType.TEC;

            default:
                return CombiType.ATK;
        }
    }
}
