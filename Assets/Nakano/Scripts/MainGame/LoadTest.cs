using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Master;

public class LoadTest : MonoBehaviour
{
    private StageDataManager stageDataManager;
    [SerializeField, Header("難易度"), Range(1, 5)] int difficulty;
    [SerializeField, Header("エリア　1:育成 2:ボス"), Range(1, 2)] int area;
    [SerializeField, Header("ステージ 1:体力 2;魔力 3:攻撃 4:守備 5:速度 6:器用"), Range(1, 6)] int stage;

    List<EnemyData> enemiesData = new();
    List<DropItem> dropData = new();

    bool loadCompleted = false;

    private void Awake()
    {
        enemiesData = null;
        dropData = null;

        stageDataManager = FindObjectOfType<StageDataManager>();

        if (stageDataManager != null)
        {
            // ステージデータが読み込まれていない場合
            if (!StageDataManager.StageDataLoadComplete)
            {
                stageDataManager.LoadCompleteProcess += LoadComplete;
                stageDataManager.LoadData(difficulty, area, stage);
            }

            else LoadComplete();
        }
    }

    void Start()
    {
        
    }

    void Update()
    {
        if (!loadCompleted) return;
    }

    // 読み込み完了時の処理
    void LoadComplete()
    {
        enemiesData = StageDataManager.EnemiesData;
        dropData = StageDataManager.DropData;
        loadCompleted = true;
    }
}
