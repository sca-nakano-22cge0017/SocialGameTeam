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

    private int totalPower = 0;          // �퓬��
    private int totalPower_Max = 999999; // �퓬�͍ő�l
       
    private Status status = new(0, 0, 0, 0, 0, 0);      // �ǉ����ʖ����̃X�e�[�^�X
    private Status status_Min = new(0, 0, 0, 0, 0, 0);       // �������N�ł̍ŏ��l
    private Status status_Max = new(0, 0, 0, 0, 0, 0);       // �ő�l

    private Status rankPoint = new(0, 0, 0, 0, 0, 0);        // �ݐσ����NPt
    private Status rankPoint_Max = new(0, 0, 0, 0, 0, 0);    // �X�e�[�^�X�̃����NPt�ő�l �v���X�X�e�[�^�X�������A�㏸���Ȃ�
    private Dictionary<StatusType, Rank> statusRank = new(); // �e�X�e�[�^�X�̃����N

    private Dictionary<CombiType, Rank> combiRank = new();         // �����X�e�[�^�X�̃����N
    private Dictionary<CombiType, int> combiRankPt = new();        // �����X�e�[�^�X�̃����NPt���ݒl
    private Dictionary<CombiType, int> combiRankPt_NextUp = new(); // ���Ƀ����N�A�b�v����Ƃ��̗ݐσ����NPt
    private Dictionary<CombiType, int> combiRankPtMax = new();     // �����X�e�[�^�X�̃����NPt�ő�l �v���X�X�e�[�^�X�������A�㏸���Ȃ�

    private Status resetBonusCoefficient = new(100,100,100,100,100,100);                // ���Z�b�g���̃X�e�[�^�X�㏸�ʂ̌W���@�㏸�� = resetBonusCoefficient * plusStatus
    private Status resetBonusCoefficient_Max = new(1000, 1000, 1000, 1000, 1000, 1000); // ���Z�b�g���̃X�e�[�^�X�ő�l�㏸�ʂ̌W���@�㏸�� = resetBonusCoefficient_Max * plusStatus
    private Status plusStatus = new(0, 0, 0, 0, 0, 0);    // ����ɂ��v���X�X�e�[�^�X 1�`99

    public CombiType evolutionType = CombiType.NORMAL; // ���݂̐i���`��
    public CombiType selectEvolutionType = CombiType.NORMAL; // ���ݑI�𒆂̐i���`�ԍ���

    // �e�i���`�ԉ���ς݂��ǂ���
    public bool atkTypeReleased = false;
    public bool defTypeReleased = false;
    public bool tecTypeReleased = false;

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
        Rank initRank = (Rank)System.Enum.ToObject(typeof(Rank), 0);     // ���������N
        Rank highestRank = (Rank)(System.Enum.GetValues(typeof(Rank)).Length - 1); // �ō������N

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
        for (int c = 0; c < System.Enum.GetValues(typeof(CombiType)).Length - 1; c++)
        {
            CombiType combi = (CombiType)System.Enum.ToObject(typeof(CombiType), c);
            combiRank.Add(combi, initRank);
            combiRankPt.Add(combi, 0);
            combiRankPt_NextUp.Add(combi, 0);
            combiRankPtMax.Add(combi, 0);
        }

        // �i���`�ԏ�����
        evolutionType = CombiType.NORMAL;
        selectEvolutionType = CombiType.NORMAL;

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

                    CalcStatusMin();

                    // �����NPt������
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
    /// �Z�[�u�f�[�^�����ɏ�����
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

        // �N���A��
        if (id == 1)
        {
            // �N���A��
            for (int i = 0; i < GameManager.IsBossClear1.Length; i++)
            {
                GameManager.IsBossClear1[i] = _data.isBossClear[i];
            }
            DifficultyManager.IsClearBossDifficulty1 = _data.isCrearBossDifficulty;
            GameManager.lastSelectDifficulty1 = _data.selectDifficulty;
        }
        else if (id == 2)
        {
            // �N���A��
            for (int i = 0; i < GameManager.IsBossClear2.Length; i++)
            {
                GameManager.IsBossClear2[i] = _data.isBossClear[i];
            }
            DifficultyManager.IsClearBossDifficulty2 = _data.isCrearBossDifficulty;
            GameManager.lastSelectDifficulty2 = _data.selectDifficulty;
        }
        else
        {
            // �N���A��
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
    /// �Z�[�u�f�[�^����K�v�ȃf�[�^�␔�l���Z�o
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

            // �����N�ύX
            statusRank[type] = (Rank)System.Enum.ToObject(typeof(Rank), rankNum);

            // �X�e�[�^�X�ŏ�/�ő�l�X�V
            int statusMin = StatusData.statusInit[rank].GetStatus(type);
            SetStatusMin(type, statusMin);
            int statusMax = StatusData.statusMax[rank].GetStatus(type);
            SetStatusMax(type, statusMax);
        }

        // ���������N�n
        for (int ct = 0; ct < System.Enum.GetValues(typeof(CombiType)).Length - 1; ct++)
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

            if (type == StatusType.MP || type == StatusType.DEF || type == StatusType.DEX)
            {
                total += GetStatus(type) * 2;
            }
            else total += GetStatus(type);

            // �����N�{�[�i�X
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
        
        // ���������N�{�[�i�X
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
        int plusBonus = GetAdditionalEffects(_type, false);
        status_Min.SetStatus(_type, _num + plusBonus);
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
        int plusBonus = GetAdditionalEffects(_type, false);
        if (GetRank(_type) == Rank.SS) plusBonus = GetAdditionalEffects_Max(_type, false);

        status_Max.SetStatus(_type, _num + plusBonus);
    }

    /// <summary>
    /// ���݂̐i���`�Ԃ��擾
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

    public Status GetRank()
    {
        Status rank = new((int)statusRank[StatusType.HP], (int)statusRank[StatusType.MP], (int)statusRank[StatusType.ATK], 
            (int)statusRank[StatusType.DEF], (int)statusRank[StatusType.AGI], (int)statusRank[StatusType.DEX]);
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
    /// �w�肵���X�e�[�^�X�́A���Ƀ����N�A�b�v����Ƃ��̗ݐ�Pt���擾
    /// </summary>
    /// <param name="_type">�X�e�[�^�X�̎��</param>
    public int GetRankPtNextUp(StatusType _type)
    {
        var rank = GetRank(_type);
        int a = StatusData.rankPoint.rankPt_NextUp[rank].GetStatus(_type);
        return a;
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
        if (_type == CombiType.NORMAL) return 0;
        
        Rank rank = combiRank[_type];
        if (rank > Rank.SS) rank = Rank.SS;

        return rank;
    }

    /// <summary>
    /// �w�肵�������X�e�[�^�X�̃����N��ύX
    /// </summary>
    /// <param name="_type">�����X�e�[�^�X�̎��</param>
    /// <param name="_num">�ύX��̃����N</param>
    public void SetCombiRank(CombiType _type, Rank _rank)
    {
        var r = _rank;
        if (_rank > Rank.SS) r = Rank.SS;
        combiRank[_type] = r;
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
    /// �v���X�X�e�[�^�X��ύX
    /// </summary>
    public void SetPlusStatus(Status _status)
    {
        plusStatus = new (_status);
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
    /// �琬���Z�b�g�ɂ��ǉ����ʗʁi�X�e�[�^�X�㏸�ʁj���擾
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
