using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using Master;

public class MasterDataLoad : MonoBehaviour
{
    [HideInInspector] public bool masterDataLoadComlete = false;
    string fileName;

    private const string ignoreMark = "//"; // �s���΂��L��

    void GetData(string fileName)
    {
        AsyncOperationHandle<TextAsset> m_TextHandle;
        Addressables.LoadAssetAsync<TextAsset>(fileName).Completed += handle =>
        {
            m_TextHandle = handle;
            if (handle.Result == null)
            {
                Debug.Log("Load Error");
                return;
            }

            ConvertTextIntoList(handle.Result.text);
            masterDataLoadComlete = true;
        };
    }

    /// <summary>
    /// csv�t�@�C������ǂݍ��񂾕��͂����X�g�ɕϊ�
    /// </summary>
    /// <returns></returns>
    List<string[]> ConvertTextIntoList(string dataStrings)
    {
        string dataStr = dataStrings;
        List<string[]> datas = new();

        // �s����
        var line = dataStr.Split("\n");
        for (int l = 0; l < line.Length; l++)
        {
            if (line[l].Contains(ignoreMark)) continue;

            // �񕪊��EList�ɑ��
            datas.Add(line[l].Split(","));
        }

        return datas;
    }
}

public class MasterData
{
    public static List<StageData> stageDatas;
    public static List<EnemyStatus> enemyStutas;
    public static List<CharaInitialStutas> charaInitialStutas;
}

namespace Master
{
    /// <summary>
    /// �X�e�[�W�f�[�^
    /// �X�e�[�W�ԍ�(stageId), �G�z�u����List(enemyPlacement)
    /// </summary>
    public class StageData
    {
        /// <summary>
        /// �G���A�ԍ�(1��)/�X�e�[�W�ԍ�(2��)/��Փx(2��)�ɂ���ĐU�蕪������ID
        /// </summary>
        public int stageID;

        /// <summary>
        /// �G�z�u�f�[�^
        /// </summary>
        public List<EnemyPlacement> enemyPlacement;
    }

    /// <summary>
    /// �G�̔z�u�f�[�^
    /// �GID(enemyID), �z�u�ԍ�(placementID)
    /// </summary>
    public class EnemyPlacement
    {
        public string enemyID;

        /// <summary>
        /// �z�u�ʒu�ԍ� 1�`4�̐��l
        /// </summary>
        public int placementID;
    }

    /// <summary>
    /// �G�X�e�[�^�X
    /// �GID(enemyID), 
    /// �X�e�[�^�X(jp/mp/atk/def/spd/dex), 
    /// �A�^�b�N�p�^�[����List(attackPattern), 
    /// �h���b�v�A�C�e��(dropItem)
    /// </summary>
    public class EnemyStatus
    {
        /// <summary>
        /// ���(�p��)/Lv(2��)�ɂ���ĐU�蕪������ID
        /// </summary>
        public string enemyID;

        public int hp;
        public int def;
        public int mp;
        public int atk;
        public int spd;
        public int dex;

        public List<EnemyAttackPattern> attackPattern;
        
        public DropItem dropItem;
    }

    /// <summary>
    /// �A�^�b�N�p�^�[��
    /// �A�^�b�N�p�^�[��ID(attackID), 
    /// �����m��(probability)
    /// </summary>
    public class EnemyAttackPattern
    {
        public int attackID;

        /// <summary>
        /// �����m��
        /// </summary>
        public int probability;
    }

    /// <summary>
    /// �h���b�v�A�C�e��
    /// �A�C�e���^�C�v(DropItemType.type) �e�X�e�[�^�X6��
    /// �h���b�v��(dropAmount)
    /// </summary>
    public class DropItem
    {
        /// <summary>
        /// �h���b�v�A�C�e���̎��
        /// </summary>
        public enum DropItemType { HP, MP, ATK, DEF, SPD, DEX };
        public DropItemType type;

        /// <summary>
        /// �h���b�v��
        /// </summary>
        public int dropAmount;
    }

    /// <summary>
    /// �L�����N�^�[�����X�e�[�^�X
    /// �L����ID(charaId), 
    /// �����X�e�[�^�X(hp_init/mp_init/atk_init/def_init/spd_init/dex_init), 
    /// �X�e�[�^�X�ő�l(atk_max/def_amx/tec_max)
    /// </summary>
    public class CharaInitialStutas
    {
        public int charaID;

        // �����X�e�[�^�X
        public int hp_init;
        public int mp_init;
        public int atk_init;
        public int def_init;
        public int spd_init;
        public int dex_init;

        // �X�e�[�^�X�ő�l
        public int atk_max;
        public int def_max;
        public int tec_max;
    }
}