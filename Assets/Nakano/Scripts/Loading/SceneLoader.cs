using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// �V�[���J�ډ��o�Đ�
/// </summary>
public class SceneLoader : MonoBehaviour
{
    public static void Load(string scene)
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
}
