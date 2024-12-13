using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadManager : MonoBehaviour
{
    AsyncOperation async;

    [SerializeField] private GameObject loadingUI;

    private bool isLoading = false;
    public bool IsLoading { get { return isLoading; } private set { isLoading = value; } }

    [SerializeField, Header("�Œ���̃��[�h��ʕ\������")] private float lowestLoadTime;

    [SerializeField, Header("�t�F�[�h�����鎞��")] private float fadeTime;
    private CanvasGroup canvasGroup;

    private bool didFadeComplete = false;
    /// <summary>
    /// �t�F�[�h����������
    /// </summary>
    public bool DidFadeComplete { get { return didFadeComplete; } private set { didFadeComplete = value; } }
    private bool isFadeIn = false, isFadeOut = false;

    // �V���O���g��
    public static LoadManager Instance;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        DontDestroyOnLoad(this);

        canvasGroup = loadingUI.GetComponent<CanvasGroup>();
        canvasGroup.alpha = 0;
        loadingUI.SetActive(false);
    }

    void Update()
    {
        if(isFadeIn || isFadeOut) Fade();
    }

    /// <summary>
    /// �t�F�[�h
    /// </summary>
    void Fade()
    {
        if(isFadeIn)
        {
            if (canvasGroup.alpha >= 1)
            {
                canvasGroup.alpha = 1;
                isFadeIn = false;
            }
            else canvasGroup.alpha += Time.deltaTime / fadeTime;
        }

        else if (isFadeOut)
        {
            if (canvasGroup.alpha <= 0)
            {
                canvasGroup.alpha = 0;
                didFadeComplete = true;
                isFadeOut = false;
            }
            else canvasGroup.alpha -= Time.deltaTime / fadeTime;
        }

        // �����̂Ƃ��͑�UI������ł���悤��SetActive��false�ɕς���
        if(canvasGroup.alpha <= 0)
        {
            loadingUI.SetActive(false);
        }
        else loadingUI.SetActive(true);
    }

    public void LoadScene(string sceneName)
    {
        // ���Ƀ��[�h���Ȃ���s���Ȃ�
        if (isLoading) return;

        SoundController soundController = FindObjectOfType<SoundController>();
        if (soundController != null)
        {
            if (sceneName == "MainTest")
                soundController.MainToBattle();

            if (GameManager.lastScene == "MainTest")
                soundController.BattleToMain();
        }

        isLoading = true;

        // �t�F�[�h�C���J�n
        isFadeIn = true;

        // �ǂݍ��݊J�n
        StartCoroutine(LoadSceneCoroutine(sceneName));
    }

    /// <summary>
    /// ���[�f�B���O���Ƀf�[�^��ǂݍ���
    /// </summary>
    /// <param name="sceneName">�J�ڐ�V�[����</param>
    public void LoadScene_LoadData(string sceneName)
    {
        // ���Ƀ��[�h���Ȃ���s���Ȃ�
        if (isLoading) return;

        isLoading = true;

        // �t�F�[�h�C���J�n
        isFadeIn = true;

        // �ǂݍ��݊J�n
        StartCoroutine(LoadSceneCoroutine_LoadData(sceneName));
    }

    private IEnumerator LoadSceneCoroutine(string sceneName)
    {
        // �t�F�[�h�C���I����Ă���V�[�����[�h
        yield return new WaitUntil(() => !isFadeIn);

        // �V�[���ǂݍ��݊J�n
        async = SceneManager.LoadSceneAsync(sceneName);
        async.allowSceneActivation = false;

        while (async.progress < 0.9f)
        {
            yield return null;
        }
        //���[�h����

        // �J��
        async.allowSceneActivation = true;

        // �Œ���҂��Ă���t�F�[�h�A�E�g
        yield return new WaitForSeconds(lowestLoadTime);

        // �t�F�[�h�A�E�g
        isFadeOut = true;

        isLoading = false;
    }

    private IEnumerator LoadSceneCoroutine_LoadData(string sceneName)
    {
        yield return new WaitUntil(() => !isFadeIn);

        // �}�X�^�[�f�[�^�ǂݍ��݊���������
        yield return new WaitUntil(() => MasterDataLoader.MasterDataLoadComplete);

        // �Z�[�u�f�[�^�ǂݍ��݊���������
        yield return new WaitUntil(() => PlayerDataManager.PlayerDataLoadComplete);

        // �V�[���ǂݍ��݊J�n
        async = SceneManager.LoadSceneAsync(sceneName);
        async.allowSceneActivation = false;

        while (async.progress < 0.9f)
        {
            yield return null;
        }
        //���[�h����

        // �J��
        async.allowSceneActivation = true;

        // �Œ���҂��Ă���t�F�[�h�A�E�g
        yield return new WaitForSeconds(lowestLoadTime);

        // �t�F�[�h�A�E�g
        isFadeOut = true;

        isLoading = false;
    }
}
