using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// シーン遷移演出再生
/// </summary>
public class SceneLoader : MonoBehaviour
{
    public static void LoadScene(string scene)
    {
        LoadManager loadManager;

        if (FindObjectOfType<LoadManager>() == null)
        {
            // UIが無ければプレハブ生成
            GameObject loadingUIPrefab = Resources.Load<GameObject>("LoadingUI");
            loadManager = Instantiate(loadingUIPrefab).GetComponent<LoadManager>();
        }
        else
        {
            loadManager = FindObjectOfType<LoadManager>();
        }

        // ロード中でないなら
        if (!loadManager.IsLoading)
        {
            loadManager.LoadScene(scene);
        }
    }

    /// <summary>
    /// シーン読み込み
    /// </summary>
    /// <param name="scene">遷移先シーン名</param>
    /// <param name="didLoadSaveData">シーン読み込み時にセーブデータを読み込むかどうか</param>
    public static void LoadScene(string scene, bool didLoadSaveData)
    {
        LoadManager loadManager;

        if (FindObjectOfType<LoadManager>() == null)
        {
            // UIが無ければプレハブ生成
            GameObject loadingUIPrefab = Resources.Load<GameObject>("LoadingUI");
            loadManager = Instantiate(loadingUIPrefab).GetComponent<LoadManager>();
        }
        else
        {
            loadManager = FindObjectOfType<LoadManager>();
        }

        // ロード中でないなら
        if (!loadManager.IsLoading)
        {
            loadManager.LoadScene_LoadData(scene);
        }
    }
}
