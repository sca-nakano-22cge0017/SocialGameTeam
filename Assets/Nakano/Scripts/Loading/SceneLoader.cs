using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// �V�[���J�ډ��o�Đ�
/// </summary>
public class SceneLoader : MonoBehaviour
{
    public static void LoadScene(string scene)
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
            loadManager.LoadScene(scene);
        }
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
