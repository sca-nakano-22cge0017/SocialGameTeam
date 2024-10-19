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
    /// �v���C���[����/������
    /// </summary>
    /// <param name="_id">�L�����N�^�[ID</param>
    public static void PlayerCreate(int _id)
    {
        if (_id != 1 && _id != 2)
        {
            Debug.Log("�L�����N�^�[ID������Ă��܂�");
            return;
        }

        Debug.Log("�v���C���[����");
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
    /// �����N�|�C���g�ǉ�
    /// </summary>
    /// <param name="_type">�ǉ�����X�e�[�^�X�̎��</param>
    /// <param name="_amount">�ǉ���</param>
    public static void RankPtUp(StatusType _type, int _amount)
    {
        PlayerNullCheck();

        int rankPt = player.GetRankPt(_type);               // ���݂̃����NPt
        int rankPt_Max = player.GetRankPtMax(_type);       // �ő僉���NPt
        int result = rankPt + _amount;                      // ���Z��̐��l

        CombiType c_type = NormalStatusToCombiStatus(_type);
        int combiRankPt = player.GetCombiRankPt(c_type);         // ���݂̕����X�e�����NPt
        int combiRankPt_Max = player.GetCombiRankPtMax(c_type); // �����X�e�ő僉���NPt
        int combiResult = combiRankPt + _amount;                 // ���Z��̐��l

        // �X�ePt�ő�l�����@���@�����X�ePt�ő�l����
        if (rankPt_Max > result && combiRankPt_Max > combiResult)
        {
            // �X�ePt�E�����X�ePt���Z
            player.SetRankPt(_type, rankPt + _amount);
            player.SetCombiRankPt(c_type, combiRankPt + _amount);
        }
        // �X�ePt�ő�l�ȏ�@���@�����X�ePt�ő�l����
        else if (rankPt_Max <= result && combiRankPt_Max > combiResult)
        {
            int diff = rankPt_Max - rankPt;

            // �X�ePt������܂ŉ��Z�A�����X�ePt��(�X�ePt�ő�l-�X�ePt���ݒl)���Z
            player.SetRankPt(_type, rankPt_Max);
            player.SetCombiRankPt(c_type, combiRankPt + diff);
        }
        // �X�ePt�ő�l�����@���@�����X�ePt�ő�l�ȏ�
        else if (rankPt_Max > result && combiRankPt_Max <= combiResult)
        {
            int diff = combiRankPt_Max - combiRankPt;

            // �X�ePt��(�����X�ePt�ő�l-�����X�ePt���ݒl)���Z�A�����X�ePt������܂ŉ��Z
            player.SetRankPt(_type, rankPt + diff);
            player.SetCombiRankPt(c_type, combiRankPt_Max);
        }
        // �X�ePt�ő�l�ȏ�@���@�����X�ePt�ő�l�ȏ�
        else if (rankPt_Max <= result && combiRankPt_Max <= combiResult)
        {
            int diff_normal = rankPt_Max - rankPt;
            int diff_combi = combiRankPt_Max - combiRankPt;

            int diff = diff_normal < diff_combi ? diff_normal : diff_combi;

            // ���ő�l�ƌ��ݒl�̍������������̍��𑫂�
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
            int rankPt = player.GetRankPt(_type);               // ���݂̃����NPt
            int rankPt_NextUp = player.GetRankPtNextUp(_type); // ���Ƀ����N�A�b�v����Ƃ��̗ݐ�Pt
            
            if (rankPt >= rankPt_NextUp)
            {
                RankUp(_type);
            }
        }

        for (int i = 0; i < System.Enum.GetValues(typeof(CombiType)).Length; i++)
        {
            CombiType c_type = (CombiType)System.Enum.ToObject(typeof(CombiType), i);
            int combiRankPt = player.GetCombiRankPt(c_type);               // ���݂̕����X�e�����NPt
            int combiRankPt_NextUp = player.GetCombiRankPtNextUp(c_type); // ���Ƀ����N�A�b�v����Ƃ��̗ݐ�Pt

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

        // �����N�㏸
        int rankNum = (int)player.GetRank(_type);
        rankNum++;
        player.SetRank(_type, (Rank)System.Enum.ToObject(typeof(Rank), rankNum));

        // �����N�ɉ����ă����N�|�C���g�ő�l�X�V
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

        // �����N�㏸
        int rankNum = (int)player.GetCombiRank(_type);
        rankNum++;
        player.SetCombiRank(_type, (Rank)System.Enum.ToObject(typeof(Rank), rankNum));

        // �����N�ɉ����ă����N�|�C���g�ő�l�X�V
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
    /// �X�e�[�^�X�v�Z
    /// </summary>
    static void CalcStatus(StatusType _type)
    {
        Rank rank = player.GetRank(_type);
        int currentStatus = player.GetStatus(_type);
    }

    /// <summary>
    /// �����N�A�b�v�{�[�i�X�ɂ��X�e�[�^�X�㏸
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
    /// �琬���Z�b�g
    /// </summary>
    public static void TraningReset()
    {
        
    }

    /// <summary>
    /// �w�肵���X�e�[�^�X�𗘗p���镡���X�e�[�^�X���擾
    /// </summary>
    /// <param name="_type">���ׂ�X�e�[�^�X�̎��</param>
    /// <returns>�����̃X�e�[�^�X�𗘗p���镡���X�e�[�^�X�̎��</returns>
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
