using System.Collections;
using System.Collections.Generic;
using UnityEngine;
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
        player = null;
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

        int rankPt = player.GetRankPt(_type);        // 現在のランクポイント
        int rankPt_Max = player.GetRankPtMax(_type); // 現在のランクでの最大ランクポイント

        // ランクポイント最大値まで加算
        if (rankPt_Max > rankPt)
            player.SetRankPt(_type, rankPt + _amount);
        else player.SetRankPt(_type, rankPt_Max);

        CalcStatus(_type);
    }

    /// <summary>
    /// ステータス計算
    /// </summary>
    static void CalcStatus(StatusType _type)
    {

    }

    /// <summary>
    /// 複合ステータスのランク更新
    /// </summary>
    public static void CombiRankUp()
    {
        PlayerNullCheck();

        if (player.GetRankPt(StatusType.HP) + player.GetRankPt(StatusType.DEF) >= player.GetCombiRankPtMax(CombiRankType.DEF))
        {
            RankUp(CombiRankType.DEF);
        }

        if (player.GetRankPt(StatusType.ATK) + player.GetRankPt(StatusType.MP) >= player.GetCombiRankPtMax(CombiRankType.ATK))
        {
            RankUp(CombiRankType.ATK);
        }

        if (player.GetRankPt(StatusType.SPD) + player.GetRankPt(StatusType.DEX) >= player.GetCombiRankPtMax(CombiRankType.TEC))
        {
            RankUp(CombiRankType.TEC);
        }
    }

    static void RankUp(CombiRankType _rankType)
    {
        // ランク上昇
        int rankNum = (int)player.GetCombiRank(_rankType);
        rankNum++;
        player.SetCombiRank(_rankType, (Rank)System.Enum.ToObject(typeof(Rank), rankNum));

        // ランクに応じてランクポイント最大値更新
        CharacterRankPoint rankPtData = player.StatusData.rankPoint;
        Rank rank = player.GetCombiRank(_rankType);
        StatusBase maxPt = rankPtData.rankPtMax[rank];

        switch (_rankType)
        {
            case CombiRankType.ATK:
                player.SetCombiRankPtMax(_rankType, rankPtData.atkRankPtMax[rank]);
                player.SetRankPtMax(StatusType.ATK, maxPt.atk);
                player.SetRankPtMax(StatusType.MP,  maxPt.mp);
                break;

            case CombiRankType.DEF:
                player.SetCombiRankPtMax(_rankType, rankPtData.defRankPtMax[rank]);
                player.SetRankPtMax(StatusType.HP,  maxPt.hp);
                player.SetRankPtMax(StatusType.DEX, maxPt.dex);
                break;

            case CombiRankType.TEC:
                player.SetCombiRankPtMax(_rankType, rankPtData.tecRankPtMax[rank]);
                player.SetRankPtMax(StatusType.SPD, maxPt.spd);
                player.SetRankPtMax(StatusType.DEX, maxPt.dex);
                break;
        }
    }

    public static StatusBase GetAllStatus()
    {
        PlayerNullCheck();

        return player.AllStatus;
    }
}
