using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// �Q�[���S�̂Ŏg�p����ϐ������Ǘ�
/// </summary>
public class GameManager : MonoBehaviour
{
    // �V���O���g��
    private static GameManager instance;
    public static GameManager Instance
    {
        get
        {
            if (instance == null)
            {
                SetupInstance();
            }
            return instance;
        }
    }
    private static void SetupInstance()
    {
        instance = FindObjectOfType<GameManager>();

        if (instance == null)
        {
            GameObject prefab = Resources.Load<GameObject>("GameManager");
            var obj = Instantiate(prefab);
            instance = obj.GetComponent<GameManager>();
            DontDestroyOnLoad(obj);
        }
    }

    public static void GameManagerCreate()
    {
        SetupInstance();
    }

    // �G���[����p
    private const int errorNum = -1;

    // �I���L�����N�^�[
    private static int selectChara = -1;

    /// <summary>
    /// �I�𒆃L�����N�^�[ / 1:�V�X�^�[ , 2:���m , -1;�G���[
    /// </summary>
    public static int SelectChara
    {
        get
        {
            if (selectChara == errorNum)
            {
                Debug.Log("�I���X�e�[�W�F���͒l������܂���");
            }

            return selectChara;
        }
        set
        {
            if (value != 1 && value != 2)
            {
                Debug.Log("�I���L�����N�^�[�F���͒l���Ԉ���Ă��܂� / �͈͊O�̓��͂ł�");
                return;
            }

            selectChara = value;
            Debug.Log("�I���L�����N�^�[�F" + selectChara);
        }
    }

    // �I���X�e�[�W
    private static int selectStage = -1;
    private const int stageIdDigids = 5; // StageId�̌���

    /// <summary>
    /// �I�𒆃X�e�[�W�@�G���A�ԍ�(1��)+�X�e�[�W�ԍ�(2��)+��Փx(2��)��5��
    /// </summary>
    public static int SelectStage
    {
        get
        {
            if (selectStage == errorNum)
            {
                Debug.Log("�I���X�e�[�W�F���͒l������܂���");
            }
            return selectStage;
        }
        set
        {
            if ((int)Mathf.Log10(value) + 1 != stageIdDigids)
            {
                Debug.Log("�I���X�e�[�W�F���͒l���Ԉ���Ă��܂� / �������قȂ�܂�");
                return;
            }

            selectStage = value;
            Debug.Log("�I���X�e�[�W�F" + selectStage);
        }
    }

    [SerializeField] private StaminaManager staminaManager;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

        staminaManager.Initialize();
    }

    private void Update()
    {
        staminaManager.Recovery();
    }
}