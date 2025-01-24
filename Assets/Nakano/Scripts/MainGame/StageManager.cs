using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class EnemiesIllust
{
    public string enemyId;
    public int imageId;
    public GameObject prefab;
}

[System.Serializable]
public class PlayerIllust
{
    public int charaId;
    public CombiType evolutionType; // �`��
    public GameObject playerObj;
    public GameObject playerIllust;
    public Sprite specialAttackIcon;
    public SpineAnim spineAnim;
}

/// <summary>
/// �X�e�[�W��̃v���C���[��G�̕\���E�����ݒ萧��
/// </summary>
public class StageManager : MonoBehaviour
{
    [SerializeField] private GameObject bossHpGuage;

    // �v���C���[
    [SerializeField] private PlayerData player;
    [SerializeField, Header("�v���C���[�̃C���X�g/�A�j���[�V����")] private PlayerIllust[] playersIllust;

    // �G
    [SerializeField] private Enemy[] enemies;
    [SerializeField, Header("�G�̃C���X�g/�A�j���[�V����")] private EnemiesIllust[] enemiesIllust;
    [SerializeField] private GameObject[] hpGuage_Enemy;
    [SerializeField] private Image buffEffect;
    [SerializeField] private Image debuffEffect;

    // �w�i
    [SerializeField] private Image backGround;
    [SerializeField, Header("�w�i�摜")] private Sprite[] backGroundSprites;

    // �����F
    [SerializeField] private Color[] textColors;
    [SerializeField, Tooltip("�����F��w�i�ɉ����ĕύX�������e�L�X�g")] private Text[] colorChangeTexts;

    StageDataManager stageDataManager;

    bool isLoadStart = false;
    bool isPlayerDataLoaded = false;
    bool isEnemyDataLoaded = false;
    bool isBGDataLoaded = false;
    /// <summary>
    /// �Z�b�e�B���O����
    /// </summary>
    public bool isSetCompleted = false;

    [Header("�m�F�p")]
    [SerializeField, Range(1, 2), Header("1�F�琬�@2�F�{�X")] private int areaId = 1;
    [SerializeField, Range(1, 6), Header("�X�e�[�W�ԍ�")] private int stageId = 1;
    [SerializeField, Range(1, 5), Header("��Փx")] private int difficultyId = 1;

    [SerializeField] WindowController windowController;
    [SerializeField, Header("���A�G�o����(%)")] private float rareEnemyApp = 100;

    private bool hasRareEnemy = false;

    // ��S�ݒ�
    [SerializeField, Header("��S���@�����l(��)")] private float critRate_Init;
    [SerializeField, Header("��S���@�ő�l(��)")] private float critRate_Max;
    [SerializeField, Header("��S���{���@�����l(�{)")] private float critBuff_Init;
    [SerializeField, Header("��S���{���@�ő�l(�{)")] private float critBuff_Max;

    private void Awake()
    {
        isSetCompleted = false;
        Load();
    }

    private void Start()
    {
        Load();
    }

    private void Load()
    {
        if (isSetCompleted) return;
        StartCoroutine(LoadWait());

        stageDataManager = FindObjectOfType<StageDataManager>();

        if (!stageDataManager) return;

        // �X�e�[�W�f�[�^�����[�h�Ȃ�
        if (!StageDataManager.StageDataLoadComplete)
        {
            // ���[�h������̏���
            stageDataManager.LoadCompleteProcess = () =>
            {
                Setting();
                playersIllust[0].playerObj.SetActive(true);
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
        if (isLoadStart) return;

        isLoadStart = true;

        PlayerDataSet();
        EnemyDataSet();
        BGSet();
    }

    /// <summary>
    /// �v���C���[�ݒ�
    /// </summary>
    void PlayerDataSet()
    {
        // �X�e�[�^�X�擾
        player.ATK = PlayerDataManager.player.GetStatus(StatusType.ATK);
        player.MP  = PlayerDataManager.player.GetStatus(StatusType.MP);
        player.HP  = PlayerDataManager.player.GetStatus(StatusType.HP);
        player.DEF = PlayerDataManager.player.GetStatus(StatusType.DEF);
        player.AGI = PlayerDataManager.player.GetStatus(StatusType.AGI);
        player.DEX = PlayerDataManager.player.GetStatus(StatusType.DEX);
        
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
            player.power_SpecialMove = 10;
        }
        if (GameManager.SelectChara == 2)
        {
            player.power_NormalAttack = 0.9f;
            player.power_Skill = 1.3f;
            player.power_SpecialMove = 10;
        }

        // ��S
        player.power_CriticalInit = CalcCritBuff();
        player.criticalProbabilityInitial = CalcCritRate();

        // �\��
        for (int i = 0; i < playersIllust.Length; i++)
        {
            if (playersIllust[i].charaId == GameManager.SelectChara &&
                playersIllust[i].evolutionType == PlayerDataManager.player.GetSelectEvolutionType())
            {
                player.meshRenderer = playersIllust[i].playerIllust.gameObject.GetComponent<MeshRenderer>();
                player.motion = playersIllust[i].playerIllust.GetComponent<Animator>();
                player.spineAnim = playersIllust[i].spineAnim;

                playersIllust[i].playerObj.SetActive(true);
            }
        }

        player.Initialize();
        isPlayerDataLoaded = true;
    }

    /// <summary>
    /// �G�̐ݒ�
    /// </summary>
    void EnemyDataSet()
    {
        for (int e = 0; e < enemies.Length; e++)
        {
            enemies[e].gameObject.SetActive(false);
            hpGuage_Enemy[e].SetActive(false);
        }

        List<Master.EnemyData> data = StageDataManager.EnemiesData;
        List<Master.EnemyData> rareData = StageDataManager.RareEnemiesData;

        for (int d = 0; d < data.Count; d++)
        {
            for (int e = 0; e < enemies.Length; e++)
            {
                if (data[d].placementId != enemies[e].POSITION) continue;

                Master.EnemyData enemy = null;

                // ���A�G�o�����I
                int rnd = Random.Range(0, 100);
                if (rnd <= rareEnemyApp && rareData.Count > 0)
                {
                    enemy = rareData[0];
                    hasRareEnemy = true;
                }
                else
                {
                    enemy = data[d];
                }

                enemies[e].enemyId = enemy.enemyStatus.enemyId;

                // �X�e�[�^�X�擾
                enemies[e].HP = enemy.enemyStatus.hp;
                enemies[e].ATK = enemy.enemyStatus.atk;
                enemies[e].MP = enemy.enemyStatus.mp;
                enemies[e].DEF = enemy.enemyStatus.def;
                enemies[e].DEX = enemy.enemyStatus.dex;
                enemies[e].AGI = enemy.enemyStatus.spd;

                enemies[e].power_CriticalInit = 120.0f;

                // �A�^�b�N�p�^�[���擾
                for (int i = 0; i < enemy.enemyStatus.attackPattern.Count; i++)
                {
                    enemies[e].attackPattern.Add(enemy.enemyStatus.attackPattern[i]);
                }

                // �\��
                for (int i = 0; i < enemiesIllust.Length; i++)
                {
                    if (enemy.enemyStatus.enemyId.Substring(0, 1) == enemiesIllust[i].enemyId &&
                        enemy.enemyStatus.imageId == enemiesIllust[i].imageId)
                    {
                        // �C���X�g�ύX
                        var ene = Instantiate(enemiesIllust[i].prefab, enemies[e].gameObject.transform);
                        ene.transform.SetSiblingIndex(0);

                        switch (e)
                        {
                            case 0:
                                ene.transform.localPosition = new Vector3(602, -2000, 0);
                                break;
                            case 1:
                                ene.transform.localPosition = new Vector3(1245, -2121, 0);
                                break;
                            case 2:
                                ene.transform.localPosition = new Vector3(1886, -2224, 0);
                                break;
                            case 3:
                                ene.transform.localPosition = new Vector3(2545, -2326, 0);
                                break;
                        }

                        var child = ene.transform.GetChild(0).gameObject;

                        var mr = child.GetComponent<MeshRenderer>();
                        mr.sortingOrder = 50 - e - 1;

                        enemies[e].motion = ene.transform.GetChild(0).gameObject.GetComponent<Animator>();
                        enemies[e].spineAnim = ene.GetComponent<SpineAnim>();

                        if (GameManager.SelectArea == 2)
                        {
                            bossHpGuage.SetActive(true);
                            enemies[e].hpGuage = bossHpGuage.GetComponent<MainGameGuage>();
                            enemies[e].hpGuage_Obj.SetActive(false);
                            hpGuage_Enemy[e].SetActive(false);

                            var rect = enemies[e].image.GetComponent<RectTransform>();

                            Vector2 sd = rect.sizeDelta;
                            sd.x = 1500;
                            sd.y = 1500;
                            rect.sizeDelta = sd;

                            var rectBuff = buffEffect.GetComponent<RectTransform>();
                            Vector2 sdBuff = rectBuff.sizeDelta;
                            sdBuff.x = 500;
                            sdBuff.y = 500;
                            rectBuff.sizeDelta = sdBuff;

                            var rectDebuff = buffEffect.GetComponent<RectTransform>();
                            Vector2 sdDebuff = rectDebuff.sizeDelta;
                            sdDebuff.x = 500;
                            sdDebuff.y = 500;
                            rectDebuff.sizeDelta = sdDebuff;

                            if (enemy.enemyStatus.imageId == 1)
                            {
                                enemies[e].transform.localPosition = new Vector3(-602, -90, 0);

                                buffEffect.transform.localPosition = new Vector3(200, 100, 0);
                                debuffEffect.transform.localPosition = new Vector3(200, 100, 0);
                            }
                            else
                            {
                                enemies[e].transform.localPosition = new Vector3(-602, -300, 0);

                                buffEffect.transform.localPosition = new Vector3(100, 400, 0);
                                debuffEffect.transform.localPosition = new Vector3(100, 400, 0);
                            }
                        }
                        else
                        {
                            hpGuage_Enemy[e].SetActive(true);
                            bossHpGuage.SetActive(false);
                        }
                    }
                }

                enemies[e].gameObject.SetActive(true);
                enemies[e].Initialize();
            }
        }

        isEnemyDataLoaded = true;
    }

    /// <summary>
    /// �w�i�ݒ�
    /// </summary>
    void BGSet()
    {
        var sprite = backGroundSprites[0];
        Color c = Color.black;

        // ��Փx2�ȉ�
        if (GameManager.SelectArea == 1)
        {
            if (GameManager.SelectDifficulty <= 2)
            {
                sprite = backGroundSprites[0];
                c = textColors[0];
            }

            else
            {
                // DEF�X�e�[�W
                if (GameManager.SelectStage == 1 || GameManager.SelectStage == 2)
                {
                    sprite = backGroundSprites[5];
                    c = textColors[0];
                }
                // ATK�X�e�[�W
                if (GameManager.SelectStage == 3 || GameManager.SelectStage == 4)
                {
                    sprite = backGroundSprites[2];
                    c = textColors[0];
                }
                // TEC�X�e�[�W
                if (GameManager.SelectStage == 5 || GameManager.SelectStage == 6)
                {
                    sprite = backGroundSprites[6];
                    c = textColors[1];
                }
            }

            // �X���C����
            if (hasRareEnemy)
            {
                sprite = backGroundSprites[3];
                c = textColors[0];
            }
        }
        else
        {
            // �~��
            if (GameManager.SelectDifficulty % 2 == 1)
            {
                sprite = backGroundSprites[1];
                c = textColors[1];
            }
            // ����
            else
            {
                sprite = backGroundSprites[4];
                c = textColors[0];
            }
        }

        backGround.sprite = sprite;

        for (int i = 0; i < colorChangeTexts.Length; i++)
        {
            colorChangeTexts[i].color = c;
        }

        isBGDataLoaded = true;
    }

    /// <summary>
    /// ��S���v�Z
    /// </summary>
    /// <returns></returns>
    float CalcCritRate()
    {
        float r = 0;

        if (10 <= player.DEX && player.DEX < 20)
        {
            r = 10;
        }
        else if (20 <= player.DEX && player.DEX < 50)
        {
            r = 30;
        }
        else if (50 <= player.DEX && player.DEX < 70)
        {
            r = 50;
        }
        else if (70 <= player.DEX && player.DEX < 80)
        {
            r = 60;
        }
        else if (80 <= player.DEX && player.DEX < 100)
        {
            r = 70;
        }
        else if (100 <= player.DEX)
        {
            r = 80;
        }

        return r;
    }

    /// <summary>
    /// ��S���_���[�W�{��
    /// </summary>
    /// <returns></returns>
    float CalcCritBuff()
    {
        float r = 100;

        if (10 <= player.DEX && player.DEX < 35)
        {
            r += 50;
        }
        else if (35 <= player.DEX && player.DEX < 60)
        {
            r += 80;
        }
        else if (60 <= player.DEX && player.DEX < 70)
        {
            r += 100;
        }
        else if (70 <= player.DEX && player.DEX < 80)
        {
            r += 125;
        }
        else if (80 <= player.DEX && player.DEX < 100)
        {
            r += 150;
        }
        else if (100 <= player.DEX)
        {
            r += 200;
        }

        return r;
    }

    IEnumerator LoadWait()
    {
        yield return new WaitUntil(() => isPlayerDataLoaded);
        yield return new WaitUntil(() => isEnemyDataLoaded);
        yield return new WaitUntil(()=> isBGDataLoaded);

        isSetCompleted = true;
    }
}
