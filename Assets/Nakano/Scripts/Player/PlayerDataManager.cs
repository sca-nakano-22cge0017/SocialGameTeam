using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Master;

public class PlayerDataManager : MonoBehaviour
{
    public static PlayerStatus player = new(1); // ���݂̎g�p�L�����N�^�[

    private static PlayerSaveData chara1 = new();
    private static PlayerSaveData chara2 = new();

    void Start()
    {
    }

    void Update()
    {
        
    }

    public static void Save()
    {
        if (GameManager.SelectChara == 1)
        {
            chara1.id = 1;
            chara1.status = player.AllStatus;
            chara1.rankPoint = player.GetRankPt();
            chara1.plusStatus = player.GetPlusStatus();

            // chara1��JSON�����ĕۑ�
        }

        if (GameManager.SelectChara == 2)
        {
            chara2.id = 2;
            chara2.status = player.AllStatus;
            chara2.rankPoint = player.GetRankPt();
            chara2.plusStatus = player.GetPlusStatus();

            // chara2��JSON�����ĕۑ�
        }
    }

    public static void Load()
    {
        chara1 = new PlayerSaveData(); //JSON����擾
        chara2 = new PlayerSaveData(); //JSON����擾

        PlayerSaveData chara = GameManager.SelectChara == 1 ? chara1 : chara2;

        // �m�F�p
        chara.id = 1;
        chara.status = new(1000, 1000, 1000, 1000, 1000, 1000);
        chara.rankPoint = new(1000, 1000, 1000, 1000, 1000, 1000);
        chara.plusStatus = new(1, 1, 1, 1, 1, 1);

        player.Initialize(chara);

        Status status = PlayerDataManager.player.AllStatus;
        Debug.Log(string.Format("HP:{0}, MP:{1}, ATK:{2}, DEF:{3}, SPD:{4}, DEX:{5}",
            status.hp, status.mp, status.atk, status.def, status.spd, status.dex));
    }

    /// <summary>
    /// �v���C���[������
    /// </summary>
    /// <param name="_id">�L�����N�^�[ID</param>
    public static void PlayerInitialize(int _id)
    {
        if (_id != 1 && _id != 2)
        {
            Debug.Log("�L�����N�^�[ID������Ă��܂�");
            return;
        }
        
        // Todo �Z�[�u�f�[�^������Γǂݍ���
        // Todo �v���X�X�e�[�^�X���̃X�e�[�^�X���Z
        if (_id == 1)
        {
            player = new PlayerStatus(1);
        }
        if (_id == 2)
        {
            player = new PlayerStatus(2);
        }
    }

    static void PlayerNullCheck()
    {
        if (player == null)
        {
            int chara = GameManager.SelectChara != -1 ? GameManager.SelectChara : 1;

            PlayerInitialize(chara);
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
            player.SetRankPt(_type, result);
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
            StatusType type = (StatusType)System.Enum.ToObject(typeof(StatusType), i);
            int rankPt = player.GetRankPt(type);               // ���݂̃����NPt
            int rankPt_NextUp = player.GetRankPtNextUp(type); // ���Ƀ����N�A�b�v����Ƃ��̗ݐ�Pt
            
            if (rankPt >= rankPt_NextUp && player.GetRank(type) != Rank.SS)
            {
                RankUp(type);
                RankUpCheck();
            }
        }

        for (int j = 0; j < System.Enum.GetValues(typeof(CombiType)).Length; j++)
        {
            CombiType c_type = (CombiType)System.Enum.ToObject(typeof(CombiType), j);
            int combiRankPt = player.GetCombiRankPt(c_type);               // ���݂̕����X�e�����NPt
            int combiRankPt_NextUp = player.GetCombiRankPtNextUp(c_type); // ���Ƀ����N�A�b�v����Ƃ��̗ݐ�Pt

            if (combiRankPt >= combiRankPt_NextUp && player.GetCombiRank(c_type) != Rank.SS)
            {
                CombiRankUp(c_type);
                RankUpCheck();
            }
        }
    }

    static void RankUp(StatusType _type)
    {
        if (player.GetRank(_type) == Rank.SS) return;

        CharacterRankPoint rankPtData = player.StatusData.rankPoint;

        AddStatus_Bonus(_type);
        Rank lastRank = player.GetRank(_type);
        Status lastPt = rankPtData.rankPt_NextUp[lastRank];

        // �����N�㏸
        int rankNum = (int)player.GetRank(_type);
        rankNum++;
        player.SetRank(_type, (Rank)System.Enum.ToObject(typeof(Rank), rankNum));

        // �����N�ɉ����ă����N�|�C���g�ő�l�X�V
        Rank rank = player.GetRank(_type);
        Status nextPt = rankPtData.rankPt_NextUp[rank];

        player.SetRankPtLastUp(_type, lastPt.GetStatus(_type));
        player.SetRankPtNextUp(_type, nextPt.GetStatus(_type));

        // �X�e�[�^�X�ŏ�/�ő�l�X�V
        int statusMin = player.StatusData.statusInit[rank].GetStatus(_type);
        int statusMax = player.StatusData.statusMax[rank].GetStatus(_type);
        player.SetStatusMin(_type, statusMin);
        player.SetStatusMax(_type, statusMax);
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

        player.SetCombiRankPtNextUp(_type, rankPtData.GetCombiRankNextPt(_type, rank));
    }

    /// <summary>
    /// �X�e�[�^�X�v�Z
    /// </summary>
    static void CalcStatus(StatusType _type)
    {
        Rank rank = player.GetRank(_type);
        int currentRankPt = player.GetRankPt(_type);

        int statusMin = player.GetStatusMin(_type);
        int statusMax = player.GetStatusMax(_type);

        int rankPtMin = rank == Rank.C ? 0 : player.GetRankPtLastUp(_type);
        int rankPtMax = player.GetRankPtNextUp(_type);

        float a = (float)(statusMax - statusMin) / (float)(rankPtMax - rankPtMin); // �O���t�̌X��

        int currentStatus = (int)((a * (currentRankPt - rankPtMin)) + (statusMin - a * rankPtMin));   // ���݂̃X�e�[�^�X
        player.SetStatus(_type, currentStatus);
    }

    /// <summary>
    /// �����N�A�b�v�{�[�i�X�ɂ��X�e�[�^�X�㏸
    /// </summary>
    /// <param name="_type"></param>
    static void AddStatus_Bonus(StatusType _type)
    {
        Rank rank = player.GetRank(_type);

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
    /// �ǉ����ʁi�v���X�X�e�[�^�X�̏㏸�j�����邩�ǂ����𒲂ׂ�
    /// </summary>
    /// <returns>true�̂Ƃ��͒ǉ����ʃA��</returns>
    public static bool IsAddPlusStatus()
    {
        bool a = false;

        for (int s = 0; s < System.Enum.GetValues(typeof(StatusType)).Length; s++)
        {
            StatusType status = (StatusType)System.Enum.ToObject(typeof(StatusType), s);

            if (player.GetRank(status) == Rank.SS)
            {
                a = true;
                break;
            }
        }

        return a;
    }

    /// <summary>
    /// �琬���Z�b�g�̒ǉ����ʂ�string�ŕԂ�
    /// </summary>
    /// <returns>�琬���Z�b�g�̒ǉ�����</returns>
    public static string GetResetEffects()
    {
        string t = "";

        for (int s = 0; s < System.Enum.GetValues(typeof(StatusType)).Length; s++)
        {
            StatusType status = (StatusType)System.Enum.ToObject(typeof(StatusType), s);

            if (player.GetRank(status) == Rank.SS)
            {
                int st = player.GetAdditionalEffects(status, true);
                t += StutasTypeToString(status) + "�X�e�[�^�X +" + st + "\n";
            }
        }

        return t;
    }

    /// <summary>
    /// �琬���Z�b�g
    /// </summary>
    public static void TraningReset()
    {
        PlayerNullCheck();

        AddPlusStatus();
        // Todo �Z�[�u�f�[�^�X�V

        PlayerInitialize(GameManager.SelectChara);
    }

    /// <summary>
    /// �v���X�X�e�[�^�X����E�ǉ�
    /// </summary>
    static void AddPlusStatus()
    {
        int statusAmount = System.Enum.GetValues(typeof(StatusType)).Length;

        for (int s = 0; s < statusAmount; s++)
        {
            StatusType status = (StatusType)System.Enum.ToObject(typeof(StatusType), s);

            if (player.GetRank(status) == Rank.SS)
            {
                int current = player.GetPlusStatus(status);
                player.SetPlusStatus(status, current + 1);
            }
        }
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

    static string StutasTypeToString(StatusType _type)
    {
        switch (_type)
        {
            case StatusType.HP:
                return "�̗�";

            case StatusType.MP:
                return "����";

            case StatusType.ATK:
                return "�U��";

            case StatusType.DEF:
                return "���";

            case StatusType.SPD:
                return "���x";

            case StatusType.DEX:
                return "��p";

            default:
                return "�̗�";
        }
    }
}
