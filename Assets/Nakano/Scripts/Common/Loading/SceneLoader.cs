using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// �V�[���J�ډ��o�Đ�
/// </summary>
public class SceneLoader : MonoBehaviour
{
    private static SoundController soundController;

    private void Start()
    {
        soundController = FindObjectOfType<SoundController>();
    }

    public static void LoadFade(string scene)
    {
        LoadManager loadManager;

        if (soundController != null && scene == "MainTest")
        {
            soundController.StopMainTheme();

            if (GameManager.SelectArea == 1) soundController.PlayBattleTheme();
            if (GameManager.SelectArea == 2) soundController.PlayBossTheme();
        }

        if (GameManager.lastScene == "MainTest")
        {
            soundController.StopBattleTheme();
            soundController.StopBossTheme();

            soundController.PlayMainTheme();
        }

        if (FindObjectOfType<LoadManager>() == null)
        {
            // UI��������΃v���n�u����
            GameObject loadingUIPrefab = Resources.Load<GameObject>("LoadingUI");
            loadManager = Instantiate(loadingUIPrefab).GetComponent<LoadManager>();
        }
        else
        {
            loadManager = FindObjectOfType<LoadManager>();
        }

        // ���[�h���łȂ��Ȃ�
        if (!loadManager.IsLoading)
        {
            GameManager.lastScene = SceneManager.GetActiveScene().name;
            loadManager.LoadScene(scene);
        }
    }

    public static void Load(string scene)
    {
        GameManager.lastScene = SceneManager.GetActiveScene().name;

        SceneManager.LoadScene(scene);
    }

    /// <summary>
    /// �O�̃V�[���ɖ߂�
    /// </summary>
    public static void Back()
    {
        SceneManager.LoadScene(GameManager.lastScene);
    }

    /// <summary>
    /// �V�[���ǂݍ���
    /// </summary>
    /// <param name="scene">�J�ڐ�V�[����</param>
    /// <param name="didLoadSaveData">�V�[���ǂݍ��ݎ��ɃZ�[�u�f�[�^��ǂݍ��ނ��ǂ���</param>
    public static void LoadScene(string scene, bool didLoadSaveData)
    {
        LoadManager loadManager;

        if (FindObjectOfType<LoadManager>() == null)
        {
            // UI��������΃v���n�u����
            GameObject loadingUIPrefab = Resources.Load<GameObject>("LoadingUI");
            loadManager = Instantiate(loadingUIPrefab).GetComponent<LoadManager>();
        }
        else
        {
            loadManager = FindObjectOfType<LoadManager>();
        }

        // ���[�h���łȂ��Ȃ�
        if (!loadManager.IsLoading)
        {
            loadManager.LoadScene_LoadData(scene);
        }
    }
}
