using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

/// <summary>
/// ���C���Q�[������@��
/// </summary>
public class MainGameSystem : MonoBehaviour
{
    private LoadManager loadManager;
    [SerializeField] private StageManager stageManager;
    [SerializeField] private WindowController windowController;
    
    [SerializeField] private Button menuButton;
    [SerializeField] private PlayerData player;
    [SerializeField] private Enemy[] enemies;

    private Enemy target = new();
    public Enemy Target { get => target; private set => target = value; }
    [SerializeField] private Image targetImage;

    [SerializeField] private Character[] characters;
    private List<Character> charactersList = new();

    private int actionNum = 0;

    [SerializeField] InitialSkill initialSkill;
    [SerializeField] HP_SpecialTecnique hp_st;
    [SerializeField] DEF_SpecialTecnique def_st;
    [SerializeField] ATK_SpecialTecnique atk_st;
    [SerializeField] MP_SpecialTecnique mp_st;
    [SerializeField] AGI_SpecialTecnique agi_st;
    [SerializeField] DEX_SpecialTecnique dex_st;

    SpecialTecniqueManager stm;
    [SerializeField] private Button[] skillButtons;

    private int elapsedTurn = 1;
    [SerializeField] private Text elapsedTurn_Text;

    private bool isStart = false;

    private bool isInitialized = false;
    private bool isLose = false;
    private bool isWin = false;

    void Start()
    {
        stm = FindObjectOfType<SpecialTecniqueManager>();
        loadManager = FindObjectOfType<LoadManager>();
        SkillRelease();
    }

    void Update()
    {
        if (loadManager && !loadManager.DidFadeComplete) return;

        if (!isInitialized && stageManager.isSetCompleted)
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

        Invoke("GameStart", 1.0f);
    }

    void GameStart()
    {
        isStart = true;
        OrderAction();
    }

    /// <summary>
    /// �s�����ݒ�
    /// </summary>
    void OrderAction()
    {
        // ���x�l�ō~���\�[�g
        charactersList.Sort((x, y) => y.AGI - x.AGI);

        // �^�[�Q�b�g�����S���Ă�����^�[�Q�b�g��ύX����
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
    /// �L�����N�^�[�̍s���I��
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

            StartCoroutine(NextTurn());
        }
    }

    IEnumerator NextTurn()
    {
        yield return new WaitForSeconds(1.0f);

        // �^�[���o��
        elapsedTurn++;
        elapsedTurn_Text.text = elapsedTurn.ToString();

        OrderAction();
    }

    /// <summary>
    /// ���s����
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
        yield return new WaitForSeconds(1.0f);

        if (isWin)
        {
            player.WinMotion();

            yield return new WaitForSeconds(3.0f);

            windowController.Open();
        }
        else if (isLose)
        {
            SceneLoader.LoadScene("HomeScene");
        }
    }

    /// <summary>
    /// �g�p�\�ȃX�L���{�^����\������
    /// </summary>
    void SkillRelease()
    {
        //�@SpecialTecniqueManager stm;�@Start��FindObjectObType�g���Ď擾
        //�@[SerializeField] private Button[] skillButtons;

        // �S�Ĕ�\���ɂ���
        for (int j = 0; j < skillButtons.Length; j++)
        {
            skillButtons[j].gameObject.SetActive(false);
        }

        for (int i = 0; i < stm.specialTecniques.Length; i++)
        {
            for (int j = 0; j < skillButtons.Length; j++)
            {
                // ScriptableObject�ƃQ�[���I�u�W�F�N�g(�{�^��)�̖��O�������Ȃ�
                // ������ς݂Ȃ�
                if (stm.specialTecniques[i].name == skillButtons[j].name && 
                    stm.specialTecniques[i].m_released)
                {
                    skillButtons[j].gameObject.SetActive(true);
                }
            }
        }
    }

    /// <summary>
    /// �^�[�Q�b�g�ύX
    /// </summary>
    /// <param name="_enemy"></param>
    public void TargetChange(Enemy _enemy)
    {
        target = _enemy;
        
        // �^�[�Q�b�g�}�[�N���ړ�
        targetImage.gameObject.transform.SetParent(target.gameObject.transform);
        targetImage.gameObject.transform.localPosition = new Vector3(-173, 245, 0);
        targetImage.gameObject.transform.SetAsLastSibling();
    }
}
