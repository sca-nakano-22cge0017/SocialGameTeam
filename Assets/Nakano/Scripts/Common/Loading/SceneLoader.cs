using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// �V�[���J�ډ��o�Đ�
/// </summary>
public class SceneLoader : MonoBehaviour
{
    public static void LoadFade(string scene)
    {
        LoadManager loadManager;
        GameManager.lastScene = SceneManager.GetActiveScene().name;

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
        if (GameManager.lastScene != "MainTest")
        {
            SceneManager.LoadScene(GameManager.lastScene);
        }
        else GameManager.lastScene = SceneManager.GetActiveScene().name;
    }

    /// <summary>
    /// �V�[���ǂݍ���
    /// </summary>
    /// <param name="scene">�J�ڐ�V�[����</param>
    /// <param name="didLoadSaveData">�V�[���ǂݍ��ݎ��ɃZ�[�u�f�[�^��ǂݍ��ނ��ǂ���</param>
    public static void LoadScene(string scene, bool didLoadSaveData)
    {
        if (!didLoadSaveData)
        {
            LoadFade(scene);
            return;
        }

        LoadManager loadManager;
        GameManager.lastScene = SceneManager.GetActiveScene().name;

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
