using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class EnemiesIllust
{
    public string enemyId;
    public Sprite sprite;
    public Animation anim;
}

[System.Serializable]
public class PlayerIllust
{
    public int charaId;
    public CombiType evolutionType; // �`��
    public Sprite sprite;
    public Animation anim;
}

/// <summary>
/// �X�e�[�W��̃v���C���[��G�̕\���E�����ݒ萧��
/// </summary>
public class StageManager : MonoBehaviour
{
    [SerializeField] private PlayerData player;
    [SerializeField, Header("�v���C���[�̃C���X�g/�A�j���[�V����")] private PlayerIllust[] playersIllust;
    [SerializeField] private Enemy[] enemies;
    [SerializeField, Header("�G�̃C���X�g/�A�j���[�V����")] private EnemiesIllust[] enemiesIllust;

    StageDataManager stageDataManager;

    /// <summary>
    /// �Z�b�e�B���O����
    /// </summary>
    public bool isSetCompleted = false;

    [Header("�m�F�p")]
    [SerializeField, Range(1, 2), Header("1�F�琬�@2�F�{�X")] private int areaId = 1;
    [SerializeField, Range(1, 6), Header("�X�e�[�W�ԍ�")] private int stageId = 1;
    [SerializeField, Range(1, 5), Header("��Փx")] private int difficultyId = 1;

    [SerializeField] WindowController windowController;

    private void Awake()
    {
        isSetCompleted = false;
        Load();
    }

    private void Start()
    {
        Load();
    }

    void Update()
    {
        
    }

    private void Load()
    {
        if (isSetCompleted) return;

        stageDataManager = FindObjectOfType<StageDataManager>();

        if (!stageDataManager) return;

        // �X�e�[�W�f�[�^�����[�h�Ȃ�
        if (!StageDataManager.StageDataLoadComplete)
        {
            // ���[�h������̏���
            stageDataManager.LoadCompleteProcess += () =>
            {
                Setting();
            };

            // �w�肵���X�e�[�W�̃f�[�^�����[�h
            GameManager.SelectDifficulty = difficultyId;
            GameManager.SelectArea = areaId;
            GameManager.SelectStage = stageId;

            stageDataManager.LoadData(difficultyId, areaId, stageId);
        }
        
        else
        {
            Setting();
        }
    }

    void Setting()
    {
        PlayerDataSet();
        EnemyDataSet();
        isSetCompleted = true;
    }

    /// <summary>
    /// �v���C���[�ݒ�
    /// </summary>
    void PlayerDataSet()
    {
        // �X�e�[�^�X�擾
        Status status = new(PlayerDataManager.player.AllStatus);
        player.ATK = status.atk;
        player.MP = status.mp;
        player.HP = status.hp;
        player.DEF = status.def;
        player.AGI = status.agi;
        player.DEX = status.dex;
        
        // �K�E�Q�[�W
        Master.CharaInitialStutas statusData = PlayerDataManager.player.StatusData;
        player.specialMoveGuageMax = statusData.specialMoveGuagesSetting[0].guageMaxAmount;
        player.sm_NormalAttack = statusData.specialMoveGuagesSetting[0];
        player.sm_Guard = statusData.specialMoveGuagesSetting[1];
        player.sm_Damage = statusData.specialMoveGuagesSetting[2];
        player.sm_Turn = statusData.specialMoveGuagesSetting[3];
        player.sm_Skill = statusData.specialMoveGuagesSetting[4];

        // �U���{��
        if (GameManager.SelectChara == 1)
        {
            player.power_NormalAttack = 1.2f;
            player.power_Skill = 1.0f;
            player.power_Critical = 1.5f;
            player.criticalProbability = 10;
            player.power_SpecialMove = 10;
        }
        if (GameManager.SelectChara == 2)
        {
            player.power_NormalAttack = 0.9f;
            player.power_Skill = 1.3f;
            player.power_Critical = 1.5f;
            player.criticalProbability = 10;
            player.power_SpecialMove = 10;
        }

        // �\��
        for (int i = 0; i < playersIllust.Length; i++)
        {
            if (playersIllust[i].charaId == GameManager.SelectChara &&
                playersIllust[i].evolutionType == PlayerDataManager.player.GetEvolutionType())
            {
                player.image.sprite = playersIllust[i].sprite;
            }
        }

        player.Initialize();
    }

    /// <summary>
    /// �G�̐ݒ�
    /// </summary>
    void EnemyDataSet()
    {
        for (int e = 0; e < enemies.Length; e++)
        {
            enemies[e].gameObject.SetActive(false);
        }

        List<Master.EnemyData> data = StageDataManager.EnemiesData;
        for (int d = 0; d < data.Count; d++)
        {
            for (int e = 0; e < enemies.Length; e++)
            {
                if (data[d].placementId != enemies[e].POSITION) continue;

                // �X�e�[�^�X�擾
                enemies[e].HP = data[d].enemyStatus.hp;
                enemies[e].ATK = data[d].enemyStatus.atk;
                enemies[e].MP = data[d].enemyStatus.mp;
                enemies[e].DEF = data[d].enemyStatus.def;
                enemies[e].DEX = data[d].enemyStatus.dex;
                enemies[e].AGI = data[d].enemyStatus.spd;

                // �A�^�b�N�p�^�[���擾
                for (int i = 0; i < data[d].enemyStatus.attackPattern.Count; i++)
                {
                    enemies[e].attackPattern.Add(data[d].enemyStatus.attackPattern[i]);
                }

                // �\��
                for (int i = 0; i < enemiesIllust.Length; i++)
                {
                    if (data[d].enemyStatus.enemyId.Substring(0, 1) == enemiesIllust[i].enemyId)
                    {
                        // �C���X�g�ύX
                        enemies[e].image.sprite = enemiesIllust[i].sprite;

                        // Todo �A�j���[�V����/Spine�̕ύX
                    }
                }

                enemies[e].gameObject.SetActive(true);
                enemies[e].Initialize();
            }
        }
    }
}
