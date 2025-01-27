using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;

/// <summary>
/// メインゲーム制御　仮
/// </summary>
public class MainGameSystem : MonoBehaviour
{
    private LoadManager loadManager;
    [SerializeField] private StageManager stageManager;
    [SerializeField] private MainDirection mainDirection;
    [SerializeField] private DropController dropController;
    
    [SerializeField] private Button menuButton;
    [SerializeField] private PlayerData player;
    [SerializeField] private Enemy[] enemies;
    [SerializeField] private Canvas commands;

    // ターゲット
    private Enemy target = new();
    public Enemy Target { get => target; }
    [SerializeField] private Image targetImage;

    [SerializeField] private Character[] characters;
    private List<Character> charactersList = new();

    private int actionNum = 0;

    // 必殺技ボタン
    [SerializeField] Image bgSpecialCommand;
    [SerializeField] Sprite[] bgSpriteSpecialCommand;

    // スキル/特殊技能
    [SerializeField] InitialSkill initialSkill;
    [SerializeField] HP_SpecialTecnique hp_st;
    [SerializeField] DEF_SpecialTecnique def_st;
    [SerializeField] ATK_SpecialTecnique atk_st;
    [SerializeField] MP_SpecialTecnique mp_st;
    [SerializeField] AGI_SpecialTecnique agi_st;
    [SerializeField] DEX_SpecialTecnique dex_st;

    SpecialTecniqueManager spTecManager;
    [SerializeField] private Button[] skillButtons;

    // チュートリアル
    TutorialWindow tutorial;

    // 経過ターン
    private int elapsedTurn = 1;
    [SerializeField] private Text elapsedTurn_Text;

    // 倍速
    [SerializeField, Header("速度倍率")] private float[] spdMagnificationKind;
    [SerializeField] private Text spdMagnificationText;
    private int selectSpdMagNum = 0;
    private float currentSpdMagnification = 1.0f;
    public float CurrentSpeedRatio { get => currentSpdMagnification; }

    [SerializeField, Header("敵アニメーション速度倍率")] private float[] enmeyAnimSpdKind;

    // オート
    [SerializeField] private Image autoButton;
    [SerializeField] private Text autoButtonText;
    [SerializeField] private Sprite[] autoButtonSprites;
    public bool isAutoMode = false;
    public bool IsAutoMode { get => isAutoMode; }

    // バフデバフ
    [SerializeField] BuffDisplay buffDisplay;

    // リザルト
    [SerializeField] private ResultManager resultManager;

    public bool isGamePlaying = false;
    private bool isInitialized = false;
    private bool isLose = false;
    private bool isWin = false;

    [SerializeField] private GameObject[] enemyHp;

    bool isBattleInProgress = false;
    private static OngoingBattleInfomation ongoingBattleInfomation = new();
    public static OngoingBattleInfomation OngoingBattleInfomation { get => ongoingBattleInfomation; set => ongoingBattleInfomation = value;}

    void Start()
    {
        spTecManager = FindObjectOfType<SpecialTecniqueManager>();
        tutorial = FindObjectOfType<TutorialWindow>();
        loadManager = FindObjectOfType<LoadManager>();

        if (GameManager.SelectArea == 1)
        {
            // 初めての育成バトルは必殺技を使用不可にする
            if (!GameManager.TutorialProgress.checkedBossBattle)
            {
                player.canSpecialMove = false;
            }
            else player.canSpecialMove = true;

            tutorial.Battle();
        }
        if (GameManager.SelectArea == 2)
        {
            tutorial.BossBattle();
            player.canSpecialMove = true;

            targetImage.enabled = false;
        }

        bgSpecialCommand.sprite = GameManager.SelectChara == 1 ? bgSpriteSpecialCommand[0] : bgSpriteSpecialCommand[1];

        InitializeSetting();
        SkillRelease();
    }

    void Update()
    {
        // ステージのデータセット済み　&　チュートリアル完了済み
        if (stageManager.isSetCompleted && tutorial.CompleteTutorial)
        {
            if (GameManager.isBattleInProgress)
            {
                BattleInformationLoad();
            }
        }

        if (loadManager && !loadManager.DidFadeComplete) return;

        // 初期化完了済み　&　ステージのデータセット済み　&　チュートリアル完了済み
        if (!isInitialized && stageManager.isSetCompleted && tutorial.CompleteTutorial)
        {
            Initialize();
            isInitialized = true;
            isGamePlaying = true;
        }
    }

    public void Initialize()
    {
        menuButton.interactable = true;

        elapsedTurn = 1;
        elapsedTurn_Text.text = elapsedTurn.ToString();

        for (int i = 0; i < characters.Length; i++)
        {
            if (characters[i].gameObject.activeSelf)
            {
                charactersList.Add(characters[i]);
            }
        }

        if (GameManager.SelectArea == 2)
        {
            StartCoroutine(mainDirection.BossStart());
        }

        SkillCanActCheck();

        StartCoroutine(GameStart());
    }

    IEnumerator GameStart()
    {
        yield return new WaitUntil(() => mainDirection.isCompleteStartDirection);

        yield return new WaitForSecondsRealtime(0.5f);

        if (!isBattleInProgress)
            OrderAction();
        else
        {
            if (actionNum <= charactersList.Count - 1)
            {
                charactersList[actionNum].Move();
            }
            else
            {
                StartCoroutine(NextTurn());
            }
        }

        atk_st.GameStart();
        hp_st.GameStart();
        isBattleInProgress = false;
    }

    /// <summary>
    /// 行動順設定
    /// </summary>
    void OrderAction()
    {
        // 速度値で降順ソート
        charactersList.Sort((x, y) => y.AGI - x.AGI);

        // ターゲットが死亡していたらターゲットを変更する
        if (target.currentHp <= 0)
        {
            for (int i = 0; i < enemies.Length; i++)
            {
                if (enemies[i].currentHp > 0)
                {
                    TargetChange(enemies[i]);
                    break;
                }
            }
        }

        actionNum = 0;
        charactersList[actionNum].Move();
    }

    /// <summary>
    /// キャラクターの行動終了
    /// </summary>
    public void ActionEnd()
    {
        if (isWin || isLose) return;
        
        actionNum++;
        if (actionNum <= charactersList.Count - 1)
        {
            charactersList[actionNum].Move();
        }
        else
        {
            StartCoroutine(NextTurn());
        }

        buffDisplay.UpdateInformation();
    }

    IEnumerator NextTurn()
    {
        SkillCanActCheck();

        yield return new WaitForSecondsRealtime(0.5f);

        hp_st.TurnEnd();
        def_st.TurnEnd();
        atk_st.TurnEnd();
        mp_st.TurnEnd();
        agi_st.TurnEnd();
        dex_st.TurnEnd();

        for (int i = 0; i < charactersList.Count; i++)
        {
            if (charactersList[i].currentHp > 0)
                charactersList[i].TurnEnd();

            yield return null;
        }

        buffDisplay.UpdateInformation();

        // ターン経過
        elapsedTurn++;
        elapsedTurn_Text.text = elapsedTurn.ToString();

        OrderAction();
    }

    /// <summary>
    /// 勝敗判定
    /// </summary>
    public void Judge()
    {
        if (player.currentHp <= 0)
        {
            isLose = true;
            isGamePlaying = false;
            menuButton.interactable = false;
            StartCoroutine(GameEnd());
            return;
        }

        for (int i = 0; i < enemies.Length; i++)
        {
            if (enemies[i].gameObject.activeSelf && enemies[i].currentHp > 0)
            {
                return;
            }

            if (enemies[i].gameObject.activeSelf && enemies[i].currentHp <= 0)
            {
                continue;
            }
        }

        isWin = true;
        isGamePlaying = false;
        menuButton.interactable = false;
        StartCoroutine(GameEnd());
    }

    IEnumerator GameEnd()
    {
        buffDisplay.Close();
        targetImage.enabled = false;

        yield return new WaitForSecondsRealtime(1.0f);

        if (isWin)
        {
            player.WinMotion();
            mainDirection.Clear();

            yield return new WaitForSecondsRealtime(3.0f);

            Time.timeScale = 1.0f;

            resultManager.Initialize();
        }
        else if (isLose)
        {
            mainDirection.GameOver();

            yield return new WaitForSecondsRealtime(3.0f);

            Time.timeScale = 1.0f;
            GameManager.islastBattleLose = true;
            SceneLoader.LoadFade("HomeScene");
        }
    }

    /// <summary>
    /// 使用可能なスキルボタンを表示する
    /// </summary>
    void SkillRelease()
    {
        // 全て非表示にする
        for (int j = 0; j < skillButtons.Length; j++)
        {
            skillButtons[j].gameObject.SetActive(false);
        }

        for (int i = 0; i < spTecManager.specialTecniques.Length; i++)
        {
            for (int j = 0; j < skillButtons.Length; j++)
            {
                // ScriptableObjectとゲームオブジェクト(ボタン)の名前が同じなら
                // かつ解放済みなら
                if (spTecManager.specialTecniques[i].name == skillButtons[j].name && 
                    spTecManager.specialTecniques[i].m_released)
                {
                    skillButtons[j].gameObject.SetActive(true);
                }
            }
        }
    }

    /// <summary>
    /// スキルボタン押せるか確認
    /// </summary>
    void SkillCanActCheck()
    {
        for (int i = 0; i < spTecManager.specialTecniques.Length; i++)
        {
            for (int j = 0; j < skillButtons.Length; j++)
            {
                // ScriptableObjectとゲームオブジェクト(ボタン)の名前が同じなら
                if (spTecManager.specialTecniques[i].name == skillButtons[j].name)
                {
                    // MP不足
                    if (player.currentMp < spTecManager.specialTecniques[i].m_cost)
                    {
                        skillButtons[j].image.color = new Color(0.9f, 0.9f, 0.9f, 1.0f);
                    }
                    else
                    {
                        skillButtons[j].image.color = new Color(1.0f, 1.0f, 1.0f, 1.0f);
                    }
                }
            }
        }
    }

    /// <summary>
    /// ターゲット変更
    /// </summary>
    /// <param name="_enemy"></param>
    public void TargetChange(Enemy _enemy)
    {
        // ボス戦はターゲット変更不可
        if (GameManager.SelectArea == 2)
        {
            target = enemies[0];
            return;
        }
        
        target = _enemy;
        
        // ターゲットマークを移動
        targetImage.gameObject.transform.SetParent(target.gameObject.transform);
        targetImage.gameObject.transform.localPosition = new Vector3(0, 0, 0);
        targetImage.gameObject.transform.SetSiblingIndex(3);
    }

    void InitializeSetting()
    {
        if (GameManager.SelectArea == 1)
        {
            selectSpdMagNum = GameManager.Setting.speedForTraning;
            isAutoMode = GameManager.Setting.isAutoForTraning;
        }
        if (GameManager.SelectArea == 2)
        {
            selectSpdMagNum = GameManager.Setting.speedForBoss;
            isAutoMode = GameManager.Setting.isAutoForBoss;
        }

        currentSpdMagnification = spdMagnificationKind[selectSpdMagNum];
        spdMagnificationText.text = "x" + currentSpdMagnification.ToString("f1");
        Time.timeScale = currentSpdMagnification;
        AjustEnemyAnimSed();

        if (isAutoMode)
        {
            autoButton.sprite = autoButtonSprites[0];
            autoButtonText.color = new Color(1.0f, 1.0f, 1.0f, 1.0f);
        }
        else
        {
            autoButton.sprite = autoButtonSprites[1];
            autoButtonText.color = new Color(0.7f, 0.7f, 0.7f, 1.0f);
        }
    }

    /// <summary>
    /// 速度倍率変更
    /// </summary>
    public void SpeedChange()
    {
        if (selectSpdMagNum < spdMagnificationKind.Length - 1)
        {
            selectSpdMagNum++;
        }
        else selectSpdMagNum = 0;

        currentSpdMagnification = spdMagnificationKind[selectSpdMagNum];
        spdMagnificationText.text = "x" + currentSpdMagnification.ToString("f1");
        Time.timeScale = currentSpdMagnification;
        AjustEnemyAnimSed();

        if (GameManager.SelectArea == 1)
        {
            GameManager.Setting.speedForTraning = selectSpdMagNum;
        }
        if (GameManager.SelectArea == 2)
        {
            GameManager.Setting.speedForBoss = selectSpdMagNum;
        }
    }

    /// <summary>
    /// 敵アニメーション速度調整
    /// </summary>
    void AjustEnemyAnimSed()
    {
        for (int i = 0; i < enemies.Length; i++)
        {
            if (enemies[i].gameObject.activeSelf && enemies[i].motion != null)
            {
                var spd = enmeyAnimSpdKind[selectSpdMagNum];
                enemies[i].motion.SetFloat("taikiSpeed", enemies[i].defaultAnimSpd_Wait * spd);
                enemies[i].motion.SetFloat("attackSpeed", enemies[i].defaultAnimSpd_Attack * spd);
            }
        }
    }

    /// <summary>
    /// オート切替
    /// </summary>
    public void AutoModeChange()
    {
        isAutoMode = !isAutoMode;

        if (actionNum >= 0 && actionNum < charactersList.Count && 
            charactersList[actionNum].GetComponent<PlayerData>() != null && 
            player != null && isAutoMode)
        {
            if (player.isInputWaiting)
            {
                player.NormalAttack();
            }
        }

        if (GameManager.SelectArea == 1)
        {
            GameManager.Setting.isAutoForTraning = isAutoMode;
        }
        if (GameManager.SelectArea == 2)
        {
            GameManager.Setting.isAutoForBoss = isAutoMode;
        }

        if (isAutoMode)
        {
            autoButton.sprite = autoButtonSprites[0];
            autoButtonText.color = new Color(1.0f, 1.0f, 1.0f, 1.0f);
        }
        else
        {
            autoButton.sprite = autoButtonSprites[1];
            autoButtonText.color = new Color(0.7f, 0.7f, 0.7f, 1.0f);
        }
    }

    /// <summary>
    /// 進行中バトルのデータを保存
    /// </summary>
    public void BattleInformationSave()
    {
        GameManager.isBattleInProgress = true;

        // 経過ターン
        ongoingBattleInfomation.elapsedTurn = elapsedTurn;

        // 行動順
        ongoingBattleInfomation.actionNum = actionNum;
        ongoingBattleInfomation.actionOrder = new int[charactersList.Count];
        for (int i = 0; i < charactersList.Count; i++)
        {
            if (charactersList[i].gameObject.GetComponent<PlayerData>())
            {
                if (player.currentHp > 0) ongoingBattleInfomation.actionOrder[i] = 0;
            }

            else
            {
                var ene = charactersList[i].gameObject.GetComponent<Enemy>();
                if (ene.currentHp > 0) ongoingBattleInfomation.actionOrder[i] = ene.POSITION;
            }
        }

        // ターゲット
        ongoingBattleInfomation.targetNum = target.POSITION;

        // プレイヤーのHP/MP/必殺ゲージ現在値を保存
        ongoingBattleInfomation.player.currentHp = player.currentHp;
        ongoingBattleInfomation.player.currentMp = player.currentMp;
        ongoingBattleInfomation.player.currentGuageAmount = player.specialMoveGuageAmount;

        // 掛かっているバフデバフを保存
        ongoingBattleInfomation.player.stateAmount = player.state.Count;
        ongoingBattleInfomation.player.state = new OngoingBattleInfomation.StateData[player.state.Count];
        for (int i = 0; i < player.state.Count; i++)
        {
            OngoingBattleInfomation.StateData s = new();
            s.id = player.state[i].stateId;
            s.value = player.state[i].value;
            s.elapsedTurn = player.state[i].elapsedTurn;
            s.continuationTurn = player.state[i].continuationTurn;
            ongoingBattleInfomation.player.state[i] = s;
        }

        ongoingBattleInfomation.enemies = new OngoingBattleInfomation.EnemyData[enemies.Length];

        // 各敵の情報を保存
        for (int i = 0; i < ongoingBattleInfomation.enemies.Length; i++)
        {
            if (enemies[i] != null)
            {
                ongoingBattleInfomation.enemies[i] = new();
                ongoingBattleInfomation.enemies[i].currentHp = enemies[i].currentHp;

                ongoingBattleInfomation.enemies[i].stateAmount = enemies[i].state.Count;
                ongoingBattleInfomation.enemies[i].state = new OngoingBattleInfomation.StateData[enemies[i].state.Count];
                for (int j = 0; j < enemies[i].state.Count; j++)
                {
                    OngoingBattleInfomation.StateData s = new();
                    s.id = enemies[i].state[j].stateId;
                    s.value = enemies[i].state[j].value;
                    s.elapsedTurn = enemies[i].state[j].elapsedTurn;
                    s.continuationTurn = enemies[i].state[j].continuationTurn;
                    ongoingBattleInfomation.enemies[i].state[j] = s;
                }
            }
        }

        // ドロップ情報を保存
        ongoingBattleInfomation.drops = new OngoingBattleInfomation.DropData[dropController.DropedItems.Count];
        for (int i = 0; i < dropController.DropedItems.Count; i++)
        {
            ongoingBattleInfomation.drops[i] = new();
            ongoingBattleInfomation.drops[i].type = dropController.DropedItems[i].itemType.ToString();
            ongoingBattleInfomation.drops[i].amount = dropController.DropedItems[i].dropAmount;
        }
    }

    /// <summary>
    /// 進行中バトルのデータ取得
    /// </summary>
    public void BattleInformationLoad()
    {
        GameManager.isBattleInProgress = false;

        // 経過ターン
        elapsedTurn = ongoingBattleInfomation.elapsedTurn;
        elapsedTurn_Text.text = elapsedTurn.ToString();

        // 行動順
        actionNum = ongoingBattleInfomation.actionNum;
        for (int i = 0; i < charactersList.Count; i++)
        {
            if (ongoingBattleInfomation.actionOrder[i] == 0)
            {
                charactersList[i] = player;
            }
            else
            {
                for (int e = 0; e < enemies.Length; e++)
                {
                    if (ongoingBattleInfomation.actionOrder[i] == enemies[e].POSITION)
                    {
                        charactersList[i] = enemies[e];
                    }
                }
            }
        }

        // ターゲット
        for (int i = 0; i < enemies.Length; i++)
        {
            if (ongoingBattleInfomation.targetNum == enemies[i].POSITION)
            {
                TargetChange(enemies[i]);
            }
        }

        player.currentHp = ongoingBattleInfomation.player.currentHp;
        player.currentMp = ongoingBattleInfomation.player.currentMp;
        player.specialMoveGuageAmount = ongoingBattleInfomation.player.currentGuageAmount;

        for (int i = 0; i < ongoingBattleInfomation.player.stateAmount; i++)
        {
            var s = ongoingBattleInfomation.player.state[i];
            player.SetState(s.id, s.value, s.elapsedTurn, s.continuationTurn);
        }

        for (int i = 0; i < ongoingBattleInfomation.enemies.Length; i++)
        {
            if (enemies[i].gameObject.activeSelf)
            {
                enemies[i].currentHp = ongoingBattleInfomation.enemies[i].currentHp;

                for (int j = 0; j < ongoingBattleInfomation.enemies[i].stateAmount; j++)
                {
                    var s = ongoingBattleInfomation.enemies[i].state[j];
                    enemies[i].SetState(s.id, s.value, s.elapsedTurn, s.continuationTurn);
                }

                if (enemies[i].currentHp <= 0)
                {
                    enemies[i].gameObject.SetActive(false);
                    enemyHp[i].SetActive(false);
                }
            }
        }

        player.RestartInitialize();

        for (int i = 0; i < enemies.Length; i++)
        {
            if (enemies[i].gameObject.activeSelf)
            {
                enemies[i].RestartInitialize();
            }
        }

        // ドロップ
        for (int i = 0; i < ongoingBattleInfomation.drops.Length; i++)
        {
            var type = PlayerDataManager.StringToStutasType(ongoingBattleInfomation.drops[i].type);
            dropController.AddDropAmount(type, ongoingBattleInfomation.drops[i].amount);
        }

        buffDisplay.UpdateInformation();
        isBattleInProgress = true;
    }
}
