using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// シーン遷移演出再生
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
    /// 前のシーンに戻る
    /// </summary>
    public static void Back()
    {
        SceneManager.LoadScene(GameManager.lastScene);
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
