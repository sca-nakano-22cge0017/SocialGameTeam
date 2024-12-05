using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Master;

public class StageDataManager : MonoBehaviour
{
    [SerializeField] private MasterDataLoader masterDataLoader;

    private StageData stageData = null;
    private List<EnemyStatus> enemyStatus = null;

    public static List<EnemyData> EnemiesData = null;
    public static List<DropItem> DropData = null;

    private bool enemiesDataLoaded = false;
    private bool dropDataLoaded = false;

    private static bool isLoadComplete = false;
    public static bool StageDataLoadComplete
    {
        get
        {
            return isLoadComplete;
        }
        private set { }
    }

    public delegate void OnCompleteDelegate();
    /// <summary>
    /// ステージデータ読み込み完了時の処理
    /// </summary>
    public OnCompleteDelegate LoadCompleteProcess;

    /// <summary>
    /// ステージのデータを読み込み
    /// </summary>
    /// <param name="_difficulty">難易度　1～5</param>
    /// <param name="_areaId">エリア　1:育成　2:ボス</param>
    /// <param name="_stageId">ステージ　1～6　1:体力　2:魔力　3:攻撃　4:守備　5:速度　6:器用</param>
    public void LoadData(int _difficulty, int _areaId, int _stageId)
    {
        DataReset();

        StartCoroutine(MasterDataLoadedCheck(_difficulty, _areaId, _stageId));
    }

    IEnumerator MasterDataLoadedCheck(int _difficulty, int _areaId, int _stageId)
    {
        if (!MasterDataLoader.MasterDataLoadComplete) masterDataLoader.DataLoad();

        yield return new WaitUntil(() => MasterDataLoader.MasterDataLoadComplete);

        enemyStatus = MasterData.EnemyStatus;

        if (_areaId == 2) _stageId = 1;
        for (int d = 0; d < MasterData.StageDatas.Count; d++)
        {
            StageData data = MasterData.StageDatas[d];
            if (data.difficulty == _difficulty && data.areaId == _areaId && data.stageId == _stageId)
            {
                stageData = data;
                break;
            }
        }

        GetEnemiesData();
        GetDropData();

        yield return new WaitUntil(() => enemiesDataLoaded && dropDataLoaded);

        isLoadComplete = true;
        Debug.Log("ステージデータ読み込み完了");

        LoadCompleteProcess();
    }

    /// <summary>
    /// 敵データを取得
    /// </summary>
    void GetEnemiesData()
    {
        List<EnemyData> _enemiesData = new();

        for (int sd = 0; sd < stageData.enemyPlacement.Count; sd++)
        {
            EnemyData enemyData = new();
            enemyData.placementId = stageData.enemyPlacement[sd].placementId;

            for (int ed = 0; ed < enemyStatus.Count; ed++)
            {
                if (enemyStatus[ed].enemyId == stageData.enemyPlacement[sd].enemyId)
                {
                    enemyData.enemyStatus = enemyStatus[ed];
                }
            }

            _enemiesData.Add(enemyData);
        }

        EnemiesData = _enemiesData;
        enemiesDataLoaded = true;
    }

    /// <summary>
    /// ドロップ情報を取得
    /// </summary>
    void GetDropData()
    {
        DropData = stageData.dropItem;
        dropDataLoaded = true;
    }

    public void DataReset()
    {
        stageData = null;
        enemyStatus = null;

        EnemiesData = null;
        DropData = null;

        isLoadComplete = false;
    }
}
