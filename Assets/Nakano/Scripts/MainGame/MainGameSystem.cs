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
    [SerializeField] private StageManager stageManager;
    [SerializeField] private WindowController windowController;
    
    [SerializeField] private PlayerData player;
    [SerializeField] private Enemy[] enemies;

    private Enemy target = new();
    public Enemy Target { get => target; private set => target = value; }

    [SerializeField] private Character[] characters;
    private List<Character> charactersList = new();

    private int actionNum = 0;

    private delegate void Action();
    private Action action;

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
        SkillRelease();
    }

    void Update()
    {
        if (!isInitialized && stageManager.isSetCompleted)
        {
            Initialize();
            isInitialized = true;
        }
    }

    public void Initialize()
    {
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

        for (int i = 0; i < enemies.Length; i++)
        {
            if (enemies[i].currentHp > 0)
            {
                target = enemies[i];
                break;
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
        Judge();

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

            // �^�[���o��
            elapsedTurn++;
            elapsedTurn_Text.text = elapsedTurn.ToString();

            OrderAction();
        }
    }

    /// <summary>
    /// ���s����
    /// </summary>
    void Judge()
    {
        if (player.currentHp <= 0)
        {
            isLose = true;
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
        StartCoroutine(GameEnd());
    }

    IEnumerator GameEnd()
    {
        yield return new WaitForSeconds(1.0f);

        if (isWin) windowController.Open();
        else if (isLose) SceneManager.LoadScene("HomeScene");
    }

    /// <summary>
    /// �g�p�\�ȃX�L���{�^����\������
    /// </summary>
    void SkillRelease()
    {
        for (int j = 0; j < skillButtons.Length; j++)
        {
            skillButtons[j].gameObject.SetActive(false);
        }

        for (int i = 0; i < stm.specialTecniques.Length; i++)
        {
            for (int j = 0; j < skillButtons.Length; j++)
            {
                if (stm.specialTecniques[i].name == skillButtons[j].name && stm.specialTecniques[i].m_released)
                {
                    skillButtons[j].gameObject.SetActive(true);
                }
            }
        }
    }
}