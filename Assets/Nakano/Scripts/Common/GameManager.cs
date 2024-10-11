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

            PlayerStatus.PlayerCreate(selectChara);
            
            StatusBase status = PlayerStatus.player.status;
            Debug.Log(string.Format("HP:{0}, MP:{1}, ATK:{2}, DEF:{3}, SPD:{4}, DEX:{5}", status.hp, status.mp, status.atk, status.def, status.spd, status.dex));
        }
    }

    // �I���Փx
    private static int selectDifficulty = -1;
    /// <summary>
    /// �I�𒆓�Փx 1�`5 , -1:�G���[
    /// </summary>
    public static int SelectDifficulty
    {
        get
        {
            if (selectDifficulty == errorNum)
            {
                Debug.Log("�I���Փx�F���͒l������܂���");
            }
            return selectDifficulty;
        }
        set
        {
            if (value < 0)
            {
                Debug.Log("�I���Փx�F���͒l���Ԉ���Ă��܂�");
                return;
            }

            selectDifficulty = value;
            Debug.Log("�I���Փx�F" + selectDifficulty);
        }
    }

    // �I���G���A
    private static int selectArea = -1;
    /// <summary>
    /// �I�𒆃G���A / 1:�琬 , 2:�{�X , -1;�G���[
    /// </summary>
    public static int SelectArea
    {
        get
        {
            if (selectArea == errorNum)
            {
                Debug.Log("�I���G���A�F���͒l������܂���");
            }
            return selectArea;
        }
        set
        {
            if (value < 0)
            {
                Debug.Log("�I���G���A�F���͒l���Ԉ���Ă��܂�");
                return;
            }

            selectArea = value;
            Debug.Log("�I���G���A�F" + selectArea);
        }
    }

    // �I���X�e�[�W
    private static int selectStage = -1;
    /// <summary>
    /// �I�𒆃X�e�[�W 1�`6 , -1:�G���[
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
            if (value < 0)
            {
                Debug.Log("�I���X�e�[�W�F���͒l���Ԉ���Ă��܂�");
                return;
            }

            selectStage = value;
            Debug.Log("�I���X�e�[�W�F" + selectStage);
        }
    }

    [SerializeField] private StaminaManager staminaManager;
    [SerializeField] private MasterDataLoader masterDataLoader;

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
        masterDataLoader.DataLoad();
    }

    private void Update()
    {
        staminaManager.Recovery();
    }
}