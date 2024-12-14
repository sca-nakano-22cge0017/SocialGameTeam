using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Master;

public class PlayerDataManager : MonoBehaviour
{
    public static PlayerStatus player = new(1); // ���݂̎g�p�L�����N�^�[

    private static PlayerStatus chara1 = new(1);
    private static PlayerStatus chara2 = new(2);
    
    private static bool playerDataLoadComlete = false;
    public static bool PlayerDataLoadComplete
    {
        get { return playerDataLoadComlete; }
        private set { }
    }

    /// <summary>
    /// �v���C���[�̃f�[�^�ۑ�
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

        // �X�^�~�i�֘A
        saveData.staminaData.lastTime = StaminaManager.lastTimeStr;
        saveData.staminaData.lastStamina = StaminaManager.lastStamina;
        saveData.staminaData.lastRecoveryTime = StaminaManager.lastRecoveryTime;
        saveData.staminaData.lastCompleteRecoveryTime = StaminaManager.lastCompleteRecoveryTime;

        // �`���[�g���A���i��
        saveData.tutorialData = new TutorialProgress(GameManager.TutorialProgress);

        CharaSelectManager.savePlayerData(saveData);
        Debug.Log("�f�[�^�Z�[�u����");
    }
    
    /// <summary>
    /// �v���C���[�̃f�[�^�擾
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

        // �X�^�~�i�֘A
        StaminaManager.lastTimeStr = data.staminaData.lastTime;
        StaminaManager.lastStamina = data.staminaData.lastStamina;
        StaminaManager.lastRecoveryTime = data.staminaData.lastRecoveryTime;
        StaminaManager.lastCompleteRecoveryTime = data.staminaData.lastCompleteRecoveryTime;

        // �`���[�g���A���i��
        GameManager.TutorialProgress = new TutorialProgress(data.tutorialData);

        playerDataLoadComlete = true;
        Debug.Log("�Z�[�u�f�[�^���[�h����");
    }

    /// <summary>
    /// �L�����N�^�[�f�[�^������
    /// </summary>
    /// <param name="_id">�L�����N�^�[ID</param>
    public static void CharacterInitialize(int _id)
    {
        if (_id != 1 && _id != 2)
        {
            Debug.Log("�L�����N�^�[ID������Ă��܂�");
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
    /// �L�����N�^�[�f�[�^������
    /// </summary>
    /// <param name="_id">�L�����N�^�[ID</param>
    public static void CharacterInitialize()
    {
        player = new(1);
        chara1 = new(1);
        chara2 = new(2);

        Save();
    }

    /// <summary>
    /// �L�����N�^�[�琬���Z�b�g
    /// </summary>
    /// <param name="_id">�L�����N�^�[ID</param>
    public static void TraningReset(int _id)
    {
        if (_id != 1 && _id != 2)
        {
            Debug.Log("�L�����N�^�[ID������Ă��܂�");
            return;
        }

        // �v���X�l�͎����z��
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
    /// �g�p�L�����N�^�[�ύX
    /// </summary>
    /// <param name="_id">�L�����N�^�[ID</param>
    public static void CharacterChange(int _id)
    {
        if (_id != 1 && _id != 2)
        {
            Debug.Log("�L�����N�^�[ID������Ă��܂�");
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
    /// �����N�|�C���g�ǉ�
    /// </summary>
    /// <param name="_type">�ǉ�����X�e�[�^�X�̎��</param>
    /// <param name="_amount">�ǉ���</param>
    public static void RankPtUp(StatusType _type, int _amount)
    {
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
        CombiRankUpCheck();

        CalcStatus(_type);

        Save();
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
    }

    static void CombiRankUpCheck()
    {
        for (int j = 0; j < System.Enum.GetValues(typeof(CombiType)).Length - 1; j++)
        {
            CombiType c_type = (CombiType)System.Enum.ToObject(typeof(CombiType), j);
            int combiRankPt = player.GetCombiRankPt(c_type);               // ���݂̕����X�e�����NPt
            int combiRankPt_NextUp = player.GetCombiRankPtNextUp(c_type); // ���Ƀ����N�A�b�v����Ƃ��̗ݐ�Pt

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

        // �����N�㏸
        int rankNum = (int)player.GetRank(_type);
        rankNum++;
        player.SetRank(_type, (Rank)System.Enum.ToObject(typeof(Rank), rankNum));

        // �����N�ɉ����ă����N�|�C���g�ő�l�X�V
        Rank rank = player.GetRank(_type);

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

        // �i��
        if (player.GetCombiRank(_type) == Rank.SS)
        {
            player.SetEvolutionType(_type);
        }
    }

    /// <summary>
    /// �X�e�[�^�X�v�Z
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

        float a = (float)(statusMax - statusMin) / (float)(rankPtMax - rankPtMin); // �O���t�̌X��

        int currentStatus = (int)(((a * currentRankPt) + (statusMin - a * rankPtMin)));   // ���݂̃X�e�[�^�X
        player.SetStatus(_type, currentStatus);

        player.UpdateTotalPower();
    }

    /// <summary>
    /// �����N�A�b�v�{�[�i�X�ɂ��X�e�[�^�X�㏸
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
    /// �ǉ����ʁi�v���X�X�e�[�^�X�̏㏸�j�����邩�ǂ����𒲂ׂ�
    /// </summary>
    /// <returns>true�̂Ƃ��͒ǉ����ʃA��</returns>
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
    /// �琬���Z�b�g�̒ǉ����ʂ�string�ŕԂ�
    /// </summary>
    /// <returns>�琬���Z�b�g�̒ǉ�����</returns>
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
                t += StutasTypeToString(status) + "�X�e�[�^�X +" + st1 + "\n";
                t += StutasTypeToString(status) + "�X�e�[�^�X��� +" + st2 + "\n";
            }
        }

        return t;
    }

    /// <summary>
    /// �v���X�X�e�[�^�X�̌��ʂ�string�ŕԂ�
    /// </summary>
    /// <param name="_type">�X�e�[�^�X�̎��</param>
    /// <returns>����</returns>
    public static string GetResetCurrentEffects(StatusType _type)
    {
        string t = "";

        int st1 = player.GetAdditionalEffects(_type, false);
        int st2 = player.GetAdditionalEffects_Max(_type, false);

        if (st1 <= 0 || st2 <= 0) return t;

        t += StutasTypeToString(_type) + "�X�e�[�^�X +" + st1 + "\n";
        t += StutasTypeToString(_type) + "�X�e�[�^�X��� +" + st2 + "\n";

        return t;
    }

    /// <summary>
    /// �琬���Z�b�g
    /// </summary>
    public static void TraningReset()
    {
        AddPlusStatus();

        Save();

        TraningReset(GameManager.SelectChara);
    }

    /// <summary>
    /// �v���X�X�e�[�^�X����E�ǉ�
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
    /// �w�肵���X�e�[�^�X�𗘗p���镡���X�e�[�^�X���擾
    /// </summary>
    /// <param name="_type">���ׂ�X�e�[�^�X�̎��</param>
    /// <returns>�����̃X�e�[�^�X�𗘗p���镡���X�e�[�^�X�̎��</returns>
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
                return "�̗�";

            case StatusType.MP:
                return "����";

            case StatusType.ATK:
                return "�U��";

            case StatusType.DEF:
                return "���";

            case StatusType.AGI:
                return "���x";

            case StatusType.DEX:
                return "��p";

            default:
                return "�̗�";
        }
    }
}
