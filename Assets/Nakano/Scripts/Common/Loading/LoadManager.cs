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

    private TutorialWindow tutorialWindow = null;

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

        if (tutorialWindow == null && FindObjectOfType<TutorialWindow>())
        {
            tutorialWindow = FindObjectOfType<TutorialWindow>();
        }
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
            else canvasGroup.alpha += Time.unscaledDeltaTime / fadeTime;
        }

        else if (isFadeOut)
        {
            if (canvasGroup.alpha <= 0)
            {
                canvasGroup.alpha = 0;
                didFadeComplete = true;
                isFadeOut = false;
            }
            else canvasGroup.alpha -= Time.unscaledDeltaTime / fadeTime;
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
            if (sceneName != "MainTest" || GameManager.lastScene != "MainTest")
            {
                if (sceneName == "MainTest")
                    soundController.MainToBattle();

                if (GameManager.lastScene == "MainTest")
                {
                    soundController.BattleToMain();
                }
            }
        }

        isLoading = true;
        didFadeComplete = false;

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

        SoundController soundController = FindObjectOfType<SoundController>();
        if (soundController != null)
        {
            if (sceneName != "MainTest" || GameManager.lastScene != "MainTest")
            {
                if (sceneName == "MainTest")
                    soundController.MainToBattle();

                if (GameManager.lastScene == "MainTest")
                {
                    soundController.BattleToMain();
                }
            }
        }

        isLoading = true;
        didFadeComplete = false;

        // �t�F�[�h�C���J�n
        isFadeIn = true;

        // �ǂݍ��݊J�n
        StartCoroutine(LoadSceneCoroutine_LoadData(sceneName));
    }

    private IEnumerator LoadSceneCoroutine(string sceneName)
    {
        // �t�F�[�h�C���I����Ă���V�[�����[�h
        yield return new WaitUntil(() => !isFadeIn);

        if (Time.timeScale != 1) Time.timeScale = 1;

        yield return new WaitForSecondsRealtime(lowestLoadTime);

        // �V�[���ǂݍ���
        AsyncOperation _async = SceneManager.LoadSceneAsync(sceneName);
        yield return _async;

        if (tutorialWindow) tutorialWindow.CameraChange();
        CameraChange();

        // �Œ���҂��Ă���t�F�[�h�A�E�g
        yield return new WaitForSecondsRealtime(lowestLoadTime);

        // �t�F�[�h�A�E�g
        isFadeOut = true;

        isLoading = false;
    }

    private IEnumerator LoadSceneCoroutine_LoadData(string sceneName)
    {
        yield return new WaitUntil(() => !isFadeIn);

        if (Time.timeScale != 1) Time.timeScale = 1;

        // �}�X�^�[�f�[�^�ǂݍ��݊���������
        yield return new WaitUntil(() => MasterDataLoader.MasterDataLoadComplete);

        // �Z�[�u�f�[�^�ǂݍ��݊���������
        yield return new WaitUntil(() => PlayerDataManager.PlayerDataLoadComplete);

        yield return new WaitForSecondsRealtime(lowestLoadTime);

        // �V�[���ǂݍ���
        AsyncOperation _async = SceneManager.LoadSceneAsync(sceneName);
        yield return _async;

        if (tutorialWindow) tutorialWindow.CameraChange();
        CameraChange();

        // �Œ���҂��Ă���t�F�[�h�A�E�g
        yield return new WaitForSecondsRealtime(lowestLoadTime);

        // �t�F�[�h�A�E�g
        isFadeOut = true;

        isLoading = false;
    }

    public void CameraChange()
    {
        // ���C���J�����ݒ�
        var canvas = loadingUI.GetComponent<Canvas>();
        canvas.worldCamera = Camera.main;
    }
}
