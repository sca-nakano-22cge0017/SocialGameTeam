using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// メインゲーム制御　仮
/// </summary>
public class MainGameSystem : MonoBehaviour
{
    private LoadManager loadManager;
    [SerializeField] private StageManager stageManager;
    [SerializeField] private WindowController windowController;
    private SoundController soundController;
    [SerializeField] private MainDirection mainDirection;

    [SerializeField] private Button menuButton;
    [SerializeField] private PlayerData player;
    [SerializeField] private Enemy[] enemies;
    [SerializeField] private Canvas commands;

    // ターゲット
    private Enemy target = new();
    public Enemy Target { get => target; private set => target = value; }
    [SerializeField] private Image targetImage;

    [SerializeField] private Character[] characters;
    private List<Character> charactersList = new();

    private int actionNum = 0;

    // スキル/特殊技能
    [SerializeField] InitialSkill initialSkill;
    [SerializeField] HP_SpecialTecnique hp_st;
    [SerializeField] DEF_SpecialTecnique def_st;
    [SerializeField] ATK_SpecialTecnique atk_st;
    [SerializeField] MP_SpecialTecnique mp_st;
    [SerializeField] AGI_SpecialTecnique agi_st;
    [SerializeField] DEX_SpecialTecnique dex_st;

    SpecialTecniqueManager stm;
    [SerializeField] private Button[] skillButtons;

    // チュートリアル
    TutorialWindow tutorial;

    // 経過ターン
    private int elapsedTurn = 1;
    [SerializeField] private Text elapsedTurn_Text;

    private bool isInitialized = false;
    private bool isLose = false;
    private bool isWin = false;

    void Start()
    {
        stm = FindObjectOfType<SpecialTecniqueManager>();
        tutorial = FindObjectOfType<TutorialWindow>();
        loadManager = FindObjectOfType<LoadManager>();
        soundController = FindObjectOfType<SoundController>();

        SkillRelease();

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
    }

    void Update()
    {
        if (loadManager && !loadManager.DidFadeComplete) return;

        // 初期化完了済み　&　ステージのデータセット済み　&　チュートリアル完了済み
        if (!isInitialized && stageManager.isSetCompleted && tutorial.CompleteTutorial)
        {
            Initialize();
            isInitialized = true;
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

        StartCoroutine(GameStart());
    }

    IEnumerator GameStart()
    {
        yield return new WaitUntil(() => mainDirection.isCompleteStartDirection);

        yield return new WaitForSeconds(0.3f);

        OrderAction();
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
    }

    IEnumerator NextTurn()
    {
        yield return new WaitForSeconds(0.5f);

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
        }

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
        menuButton.interactable = false;
        StartCoroutine(GameEnd());
    }

    IEnumerator GameEnd()
    {
        targetImage.enabled = false;

        yield return new WaitForSeconds(1.0f);

        if (isWin)
        {
            player.WinMotion();
            mainDirection.Clear();

            yield return new WaitForSeconds(3.0f);

            windowController.Open();
        }
        else if (isLose)
        {
            mainDirection.GameOver();

            yield return new WaitForSeconds(3.0f);

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

        for (int i = 0; i < stm.specialTecniques.Length; i++)
        {
            for (int j = 0; j < skillButtons.Length; j++)
            {
                // ScriptableObjectとゲームオブジェクト(ボタン)の名前が同じなら
                // かつ解放済みなら
                if (stm.specialTecniques[i].name == skillButtons[j].name && 
                    stm.specialTecniques[i].m_released)
                {
                    skillButtons[j].gameObject.SetActive(true);
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
        target = _enemy;
        
        // ターゲットマークを移動
        targetImage.gameObject.transform.SetParent(target.gameObject.transform);
        targetImage.gameObject.transform.localPosition = new Vector3(0, 0, 0);
        targetImage.gameObject.transform.SetAsLastSibling();
    }
}
