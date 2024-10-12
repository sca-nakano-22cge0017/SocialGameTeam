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

    private int totalPower = 0;          // �퓬��
    private int totalPower_Max = 999999; // �퓬�͍ő�l

    private StatusBase status = new(0, 0, 0, 0, 0, 0);         // �X�e�[�^�X
    private StatusBase status_Max = new(0, 0, 0, 0, 0, 0);     // �ő�l

    private StatusBase rankPoint = new(0, 0, 0, 0, 0, 0);      // �ݐσ����NPt
    private StatusBase rankPoint_Max = new(0, 0, 0, 0, 0, 0);  // �����NPt�ő�l

    private Dictionary<CombiRankType, Rank> combiRank = new(); // �����X�e�[�^�X�̃����N
    private int atkCombiRankPtMax = 0; // ���݃����N�ł̍ő�A�^�b�N�l
    private int defCombiRankPtMax = 0; // ���݃����N�ł̍ő�f�B�t�F���X�l
    private int tecCombiRankPtMax = 0; // ���݃����N�ł̍ő�e�N�j�J���l

    /// <summary>
    /// ID�ɉ����ď����X�e�[�^�X�ݒ�
    /// </summary>
    /// <param name="_id"></param>
    public PlayerStatus(int _id)
    {
        ID = _id;

        Rank initRank = Rank.C;

        // �����X�e�[�^�X�̃����N������
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
                    
                    // �X�e�[�^�X������
                    status = statusData.statusInit[initRank];
                    status_Max = statusData.statusMax[initRank];

                    // �����NPt������
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
    /// �퓬�͌��ݒl
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
    /// �퓬�͍ő�l
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
    /// �w�肵���X�e�[�^�X�̌��ݒl���擾
    /// </summary>
    /// <param name="_type">�X�e�[�^�X�̎��</param>
    /// <returns>�w�肵���X�e�[�^�X�̌��ݒl</returns>
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
    /// �w�肵���X�e�[�^�X�̒l��ύX
    /// </summary>
    /// <param name="_type">�X�e�[�^�X�̎��</param>
    /// <param name="_num">�ύX��̒l</param>
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
    /// �w��X�e�[�^�X�̍ő�l���擾
    /// </summary>
    /// <param name="_type">�X�e�[�^�X�̎��</param>
    /// <returns>�w��X�e�[�^�X�̍ő�l</returns>
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
    /// �w�肵���X�e�[�^�X�̍ő�l��ύX
    /// </summary>
    /// <param name="_type">�X�e�[�^�X�̎��</param>
    /// <param name="_num">�ύX��̍ő�l</param>
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
    /// �w�肵���X�e�[�^�X�̃����N�|�C���g���ݒl���擾
    /// </summary>
    /// <param name="_type">�X�e�[�^�X�̎��</param>
    /// <returns>�w�肵���X�e�[�^�X�̃����N�|�C���g���ݒl</returns>
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
    /// �w�肵���X�e�[�^�X�̃����N�|�C���g���ݒl��ύX
    /// </summary>
    /// <param name="_type">�X�e�[�^�X�̎��</param>
    /// <param name="_num">�ύX��̒l</param>
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
    /// �w�肵���X�e�[�^�X�̃����N�|�C���g�ő�l���擾
    /// </summary>
    /// <param name="_type">�X�e�[�^�X�̎��</param>
    /// <returns>�w�肵���X�e�[�^�X�̃����N�|�C���g�ő�l</returns>
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
    /// �w�肵���X�e�[�^�X�̃����N�|�C���g�ő�l��ύX
    /// </summary>
    /// <param name="_type">�X�e�[�^�X�̎��</param>
    /// <param name="_num">�ύX��̍ő�l</param>
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
    /// �w�肵�������X�e�[�^�X�̍��v�����N�|�C���g�ő�l���擾
    /// </summary>
    /// <param name="_type">�����X�e�[�^�X�̎��</param>
    /// <returns>�w�肵�������X�e�[�^�X�̍��v�����N�|�C���g�ő�l</returns>
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
    /// �w�肵�������X�e�[�^�X�̍��v�����N�|�C���g�ő�l��ύX
    /// </summary>
    /// <param name="_type">�����X�e�[�^�X�̎��</param>
    /// <param name="_num">�ύX��̍ő�l</param>
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
    /// �w�肵�������X�e�[�^�X�̃����N���擾
    /// </summary>
    /// <param name="_type">�����X�e�[�^�X�̎��</param>
    /// <returns>�w�肵�������X�e�[�^�X�̃����N</returns>
    public Rank GetCombiRank(CombiRankType _type)
    {
        Rank rank = combiRank[_type];
        return rank;
    }

    /// <summary>
    /// �w�肵�������X�e�[�^�X�̃����N��ύX
    /// </summary>
    /// <param name="_type">�����X�e�[�^�X�̎��</param>
    /// <param name="_num">�ύX��̃����N</param>
    public void SetCombiRank(CombiRankType _type, Rank _rank)
    {
        combiRank[_type] = _rank;
    }
}
