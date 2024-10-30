using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Master;

public enum StatusType { HP, MP, ATK, DEF, SPD, DEX };
public enum CombiType { ATK, DEF, TEC };
public enum Rank { C = 0, B = 1, A = 2, S = 3, SS = 4 };

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

    public Status(Status _status)
    {
        hp = _status.hp;
        mp = _status.mp;
        atk = _status.atk;
        def = _status.def;
        spd = _status.spd;
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

            case StatusType.SPD:
                return spd;

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

            case StatusType.SPD:
                spd = _num;
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

    private int totalPower = 0;          // �퓬��
    private int totalPower_Max = 999999; // �퓬�͍ő�l

    private Status status = new(0, 0, 0, 0, 0, 0);           // ���݂̃X�e�[�^�X
    private Status status_Min = new(0, 0, 0, 0, 0, 0);       // �������N�ł̍ŏ��l
    private Status status_Max = new(0, 0, 0, 0, 0, 0);       // �ő�l

    private Status rankPoint = new(0, 0, 0, 0, 0, 0);        // �ݐσ����NPt
    private Status rankPoint_LastUp = new(0, 0, 0, 0, 0, 0); // �O�񃉃��N�A�b�v�����Ƃ��̗ݐσ����NPt
    private Status rankPoint_NextUp = new(0, 0, 0, 0, 0, 0); // ���Ƀ����N�A�b�v����Ƃ��̗ݐσ����NPt
    private Status rankPoint_Max = new(0, 0, 0, 0, 0, 0);    // �X�e�[�^�X�̃����NPt�ő�l �v���X�X�e�[�^�X�������A�㏸���Ȃ�
    private Dictionary<StatusType, Rank> statusRank = new(); // �e�X�e�[�^�X�̃����N

    private Dictionary<CombiType, Rank> combiRank = new();         // �����X�e�[�^�X�̃����N
    private Dictionary<CombiType, int> combiRankPt = new();        // �����X�e�[�^�X�̃����NPt���ݒl
    private Dictionary<CombiType, int> combiRankPt_NextUp = new(); // ���Ƀ����N�A�b�v����Ƃ��̗ݐσ����NPt
    private Dictionary<CombiType, int> combiRankPtMax = new();     // �����X�e�[�^�X�̃����NPt�ő�l �v���X�X�e�[�^�X�������A�㏸���Ȃ�

    private const int resetBonusCoefficient = 1000;       // ���Z�b�g���̃X�e�[�^�X�㏸�ʂ̌W���@�㏸�� = resetBonusCoefficient * plusStatus
    private Status plusStatus = new(0, 0, 0, 0, 0, 0);     // ����ɂ��v���X�X�e�[�^�X 1�`99

    /// <summary>
    /// ID�ɉ����ď����X�e�[�^�X�ݒ�
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

    public Status AllStatus
    {
        get => status;
    }

    /// <summary>
    /// �퓬�͌��ݒl
    /// </summary>
    public int TotalPower
    {
        get => totalPower;
    }

    /// <summary>
    /// �퓬�͍ő�l
    /// </summary>
    public int TotalPower_Max
    {
        get => totalPower_Max;
        set => totalPower_Max = value;
    }

    /// <summary>
    /// ������
    /// </summary>
    /// <param name="_id"></param>
    void Initialize(int _id)
    {
        Rank initRank = Rank.C;     // ���������N
        Rank highestRank = Rank.SS; // �ō������N

        // �X�e�[�^�X�̃����N������
        statusRank.Clear();
        for (int s = 0; s < System.Enum.GetValues(typeof(StatusType)).Length; s++)
        {
            StatusType status = (StatusType)System.Enum.ToObject(typeof(StatusType), s);
            statusRank.Add(status, initRank);
        }

        // �����X�e�[�^�X�̃����N/�����NPt�ő�l������
        combiRank.Clear();
        combiRankPt.Clear();
        combiRankPt_NextUp.Clear();
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

                    // �X�e�[�^�X������
                    status = new(statusData.statusInit[initRank]);
                    status_Min = new(statusData.statusInit[initRank]);
                    status_Max = new(statusData.statusMax[initRank]);

                    // �����NPt������
                    CharacterRankPoint rankPtData = statusData.rankPoint;

                    rankPoint = new Status(0, 0, 0, 0, 0, 0);
                    rankPoint_LastUp = new Status(0, 0, 0, 0, 0, 0);
                    rankPoint_NextUp = new(rankPtData.rankPt_NextUp[initRank]);

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

    /// <summary>
    /// �Z�[�u�f�[�^�����ɏ�����
    /// </summary>
    /// <param name="_data"></param>
    public void Initialize(PlayerSaveData _data)
    {
        Initialize(_data.id);

        id = _data.id;
        status = _data.status;
        rankPoint = _data.rankPoint;
        plusStatus = _data.plusStatus;

        SetData();
    }

    void SetData()
    {
        UpdateTotalPower();

        for (int st = 0; st < System.Enum.GetValues(typeof(StatusType)).Length; st++)
        {
            StatusType type = (StatusType)System.Enum.ToObject(typeof(StatusType), st);

            Rank rank = Rank.C;
            int rankNum = (int)Rank.C;
            Status rankPtNextUp = new(0, 0, 0, 0, 0, 0);

            for (int r = 0; r < System.Enum.GetValues(typeof(Rank)).Length; r++)
            {
                rank = (Rank)System.Enum.ToObject(typeof(Rank), r);

                rankPtNextUp = StatusData.rankPoint.rankPt_NextUp[rank];

                if (rankPoint.GetStatus(type) >= rankPtNextUp.GetStatus(type))
                {
                    rankNum++;
                }
            }

            // �����N�ύX
            statusRank[type] = (Rank)System.Enum.ToObject(typeof(Rank), rankNum);

            // �����N�ɉ����ă����N�|�C���g�ŏ��l/�ő�l�X�V
            int lastRankNum = rankNum - 1 > 0 ? rankNum - 1 : 0;
            Rank lastRank = (Rank)System.Enum.ToObject(typeof(Rank), lastRankNum);
            rankPoint_LastUp = StatusData.rankPoint.rankPt_NextUp[lastRank];
            rankPoint_NextUp = rankPtNextUp;

            // �X�e�[�^�X�ŏ�/�ő�l�X�V
            status_Min.SetStatus(type, StatusData.statusInit[rank].GetStatus(type));
            status_Max.SetStatus(type, StatusData.statusMax[rank].GetStatus(type));
        }

        // ���������N�n
        for (int ct = 0; ct < System.Enum.GetValues(typeof(CombiType)).Length; ct++)
        {
            CombiType type = (CombiType)System.Enum.ToObject(typeof(CombiType), ct);

            // �ݐ�Pt�v�Z
            switch (type)
            {
                case CombiType.ATK:
                    combiRankPt[type] = rankPoint.atk + rankPoint.mp;
                    break;

                case CombiType.DEF:
                    combiRankPt[type] = rankPoint.hp + rankPoint.def;
                    break;

                case CombiType.TEC:
                    combiRankPt[type] = rankPoint.spd + rankPoint.dex;
                    break;
            }

            Rank rank = Rank.C;
            int rankNum = (int)Rank.C;

            for (int r = 0; r < System.Enum.GetValues(typeof(Rank)).Length; r++)
            {
                rank = (Rank)System.Enum.ToObject(typeof(Rank), r);

                if (combiRankPt[type] >= StatusData.rankPoint.GetCombiRankNextPt(type, rank))
                {
                    rankNum++;
                }
            }

            // �����N�ύX
            combiRank[type] = (Rank)System.Enum.ToObject(typeof(Rank), rankNum);

            // �����N�ɉ����ă����N�|�C���g�ő�l�X�V
            SetCombiRankPtNextUp(type, StatusData.rankPoint.GetCombiRankNextPt(type, rank));
        }
    }

    // �ȉ��ϐ��擾�E�ύX�n
    // �����퓬��
    /// <summary>
    /// �����퓬�͂̌v�Z
    /// </summary>
    public void UpdateTotalPower()
    {
        int total = 0;

        for (int st = 0; st < System.Enum.GetValues(typeof(StatusType)).Length; st++)
        {
            StatusType type = (StatusType)System.Enum.ToObject(typeof(StatusType), st);
            total += status.GetStatus(type);
        }

        if (total <= totalPower_Max)
        {
            totalPower = total;
        }
        else totalPower = totalPower_Max;
    }

    // �X�e�[�^�X
    /// <summary>
    /// �w�肵���X�e�[�^�X�̌��ݒl���擾
    /// </summary>
    /// <param name="_type">�X�e�[�^�X�̎��</param>
    /// <returns>�w�肵���X�e�[�^�X�̌��ݒl</returns>
    public int GetStatus(StatusType _type)
    {
        return status.GetStatus(_type);
    }

    /// <summary>
    /// �w�肵���X�e�[�^�X�̒l��ύX
    /// </summary>
    /// <param name="_type">�X�e�[�^�X�̎��</param>
    /// <param name="_num">�ύX��̒l</param>
    public void SetStatus(StatusType _type, int _num)
    {
        status.SetStatus(_type, _num);
    }

    /// <summary>
    /// �w��X�e�[�^�X�̍ŏ��l���擾
    /// </summary>
    /// <param name="_type">�X�e�[�^�X�̎��</param>
    /// <returns>�w��X�e�[�^�X�̍ŏ��l</returns>
    public int GetStatusMin(StatusType _type)
    {
        return status_Min.GetStatus(_type);
    }

    /// <summary>
    /// �w�肵���X�e�[�^�X�̍ŏ��l��ύX
    /// </summary>
    /// <param name="_type">�X�e�[�^�X�̎��</param>
    /// <param name="_num">�ύX��̍ŏ��l</param>
    public void SetStatusMin(StatusType _type, int _num)
    {
        status_Min.SetStatus(_type, _num);
    }

    /// <summary>
    /// �w��X�e�[�^�X�̍ő�l���擾
    /// </summary>
    /// <param name="_type">�X�e�[�^�X�̎��</param>
    /// <returns>�w��X�e�[�^�X�̍ő�l</returns>
    public int GetStatusMax(StatusType _type)
    {
        return status_Max.GetStatus(_type);
    }

    /// <summary>
    /// �w�肵���X�e�[�^�X�̍ő�l��ύX
    /// </summary>
    /// <param name="_type">�X�e�[�^�X�̎��</param>
    /// <param name="_num">�ύX��̍ő�l</param>
    public void SetStatusMax(StatusType _type, int _num)
    {
        status_Max.SetStatus(_type, _num);
    }

    // �����N
    /// <summary>
    /// �w�肵���X�e�[�^�X�̃����N���擾
    /// </summary>
    /// <param name="_type">�X�e�[�^�X�̎��</param>
    /// <returns>�w�肵���X�e�[�^�X�̃����N</returns>
    public Rank GetRank(StatusType _type)
    {
        Rank rank = statusRank[_type];
        return rank;
    }

    /// <summary>
    /// �w�肵���X�e�[�^�X�̃����N��ύX
    /// </summary>
    /// <param name="_type">�X�e�[�^�X�̎��</param>
    /// <param name="_num">�ύX��̃����N</param>
    public void SetRank(StatusType _type, Rank _rank)
    {
        statusRank[_type] = _rank;
    }

    // �����NPt
    public Status GetRankPt()
    {
        return rankPoint;
    }

    /// <summary>
    /// �w�肵���X�e�[�^�X�̃����N�|�C���g���ݒl���擾
    /// </summary>
    /// <param name="_type">�X�e�[�^�X�̎��</param>
    /// <returns>�w�肵���X�e�[�^�X�̃����N�|�C���g���ݒl</returns>
    public int GetRankPt(StatusType _type)
    {
        int n = rankPoint.GetStatus(_type);
        return n;
    }

    /// <summary>
    /// �w�肵���X�e�[�^�X�̃����N�|�C���g���ݒl��ύX
    /// </summary>
    /// <param name="_type">�X�e�[�^�X�̎��</param>
    /// <param name="_num">�ύX��̒l</param>
    public void SetRankPt(StatusType _type, int _num)
    {
        rankPoint.SetStatus(_type, _num);
    }

    /// <summary>
    /// �w�肵���X�e�[�^�X�́A�O�񃉃��N�A�b�v�����Ƃ��̗ݐ�Pt���擾
    /// </summary>
    /// <param name="_type">�X�e�[�^�X�̎��</param>
    public int GetRankPtLastUp(StatusType _type)
    {
        int n = rankPoint_LastUp.GetStatus(_type);
        return n;
    }

    /// <summary>
    /// �w�肵���X�e�[�^�X�́A�O�񃉃��N�A�b�v�����Ƃ��̗ݐ�Pt��ύX
    /// </summary>
    /// <param name="_type">�X�e�[�^�X�̎��</param>
    /// <param name="_num">�ύX��̒l</param>
    public void SetRankPtLastUp(StatusType _type, int _num)
    {
        rankPoint_LastUp.SetStatus(_type, _num);
    }

    public int GetRankPtUp(StatusType _type, Rank _rank)
    {
        return StatusData.rankPoint.rankPt_NextUp[_rank].GetStatus(_type);
    }

    /// <summary>
    /// �w�肵���X�e�[�^�X�́A���Ƀ����N�A�b�v����Ƃ��̗ݐ�Pt���擾
    /// </summary>
    /// <param name="_type">�X�e�[�^�X�̎��</param>
    public int GetRankPtNextUp(StatusType _type)
    {
        return rankPoint_NextUp.GetStatus(_type);
    }

    /// <summary>
    /// �w�肵���X�e�[�^�X�́A���Ƀ����N�A�b�v����Ƃ��̗ݐ�Pt��ύX
    /// </summary>
    /// <param name="_type">�X�e�[�^�X�̎��</param>
    /// <param name="_num">�ύX��̒l</param>
    public void SetRankPtNextUp(StatusType _type, int _num)
    {
        rankPoint_NextUp.SetStatus(_type, _num);
    }

    /// <summary>
    /// �w�肵���X�e�[�^�X�̃����N�|�C���g�̍ő�l���擾
    /// </summary>
    /// <param name="_type">�X�e�[�^�X�̎��</param>
    /// <returns>�w�肵���X�e�[�^�X�̃����N�|�C���g�ő�l</returns>
    public int GetRankPtMax(StatusType _type)
    {
        int n = rankPoint_Max.GetStatus(_type);
        return n;
    }

    // �����X�e�[�^�X�̃����N
    /// <summary>
    /// �w�肵�������X�e�[�^�X�̃����N���擾
    /// </summary>
    /// <param name="_type">�����X�e�[�^�X�̎��</param>
    /// <returns>�w�肵�������X�e�[�^�X�̃����N</returns>
    public Rank GetCombiRank(CombiType _type)
    {
        Rank rank = combiRank[_type];
        return rank;
    }

    /// <summary>
    /// �w�肵�������X�e�[�^�X�̃����N��ύX
    /// </summary>
    /// <param name="_type">�����X�e�[�^�X�̎��</param>
    /// <param name="_num">�ύX��̃����N</param>
    public void SetCombiRank(CombiType _type, Rank _rank)
    {
        combiRank[_type] = _rank;
    }

    // �����X�e�[�^�X�̃����NPt
    /// <summary>
    /// �w�肵�������X�e�[�^�X�̃����N�|�C���g���ݒl���擾
    /// </summary>
    /// <param name="_type">�����X�e�[�^�X�̎��</param>
    /// <returns>�w�肵�������X�e�[�^�X�̃����N�|�C���g���ݒl</returns>
    public int GetCombiRankPt(CombiType _type)
    {
        int n = combiRankPt[_type];
        return n;
    }

    /// <summary>
    /// �w�肵�������X�e�[�^�X�̃����N�|�C���g���ݒl��ύX
    /// </summary>
    /// <param name="_type">�X�e�[�^�X�̎��</param>
    /// <param name="_num">�ύX��̌��ݒl</param>
    public void SetCombiRankPt(CombiType _type, int _num)
    {
        combiRankPt[_type] = _num;
    }

    /// <summary>
    /// �w�肵�������X�e�[�^�X�̍��v�����N�|�C���g�ő�l���擾
    /// </summary>
    /// <param name="_type">�����X�e�[�^�X�̎��</param>
    /// <returns>�w�肵�������X�e�[�^�X�̍��v�����N�|�C���g�ő�l</returns>
    public int GetCombiRankPtMax(CombiType _type)
    {
        int n = combiRankPtMax[_type];
        return n;
    }

    /// <summary>
    /// �w�肵�������X�e�[�^�X�́A���Ƀ����N�A�b�v����Ƃ��̗ݐ�Pt���擾
    /// </summary>
    /// <param name="_type">�X�e�[�^�X�̎��</param>
    public int GetCombiRankPtNextUp(CombiType _type)
    {
        int n = combiRankPt_NextUp[_type];
        return n;
    }

    /// <summary>
    /// �w�肵�������X�e�[�^�X�́A���Ƀ����N�A�b�v����Ƃ��̗ݐ�Pt��ύX
    /// </summary>
    /// <param name="_type">�X�e�[�^�X�̎��</param>
    /// <param name="_num">�ύX��̒l</param>
    public void SetCombiRankPtNextUp(CombiType _type, int _num)
    {
        combiRankPt_NextUp[_type] = _num;
    }

    // �v���X�X�e�[�^�X
    /// <summary>
    /// �S�Ẵv���X�X�e�[�^�X���擾
    /// </summary>
    /// <returns>�S�Ẵv���X�X�e�[�^�X</returns>
    public Status GetPlusStatus()
    {
        return plusStatus;
    }

    /// <summary>
    /// �w�肵���X�e�[�^�X�̃v���X�X�e�[�^�X���擾
    /// </summary>
    /// <param name="_type">�X�e�[�^�X�̎��</param>
    /// <returns>�X�e�[�^�X�̃v���X�X�e�[�^�X</returns>
    public int GetPlusStatus(StatusType _type)
    {
        int n = plusStatus.GetStatus(_type);
        return n;
    }

    /// <summary>
    /// �w�肵���X�e�[�^�X�̃v���X�X�e�[�^�X��ύX
    /// </summary>
    /// <param name="_type">�X�e�[�^�X�̎��</param>
    /// <param name="_num">�ύX��̃v���X�X�e�[�^�X</param>
    public void SetPlusStatus(StatusType _type, int _num)
    {
        plusStatus.SetStatus(_type, _num);
    }

    /// <summary>
    /// �琬���Z�b�g�ɂ��ǉ����ʗʁi�X�e�[�^�X�㏸�ʁj���擾�@
    /// �X�e�[�^�X�����l + �琬�ɂ��㏸�� + �琬���Z�b�g�ɂ��㏸��
    /// </summary>
    /// <param name="_type">�X�e�[�^�X�̎��</param>
    /// <param name="isNextEffects">false�̂Ƃ��㏸�ʂ̌��ݒl��Ԃ��@true�̂Ƃ����琬���Z�b�g�����ꍇ�̏㏸�ʂ�Ԃ�</param>
    /// <returns>�琬���Z�b�g�ɂ��ǉ����ʗʁi�X�e�[�^�X�㏸�ʁj</returns>
    public int GetAdditionalEffects(StatusType _type, bool isNextEffects)
    {
        int a = 0;

        int plus = plusStatus.GetStatus(_type);

        if (isNextEffects)
        {
            a = (plus + 1) * resetBonusCoefficient;
        }
        else a = plus * resetBonusCoefficient;

        return a;
    }
}
