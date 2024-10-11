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
    /// �X�e�[�W�f�[�^�ǂݍ��݊������̏���
    /// </summary>
    public OnCompleteDelegate LoadCompleteProcess;

    /// <summary>
    /// �X�e�[�W�̃f�[�^��ǂݍ���
    /// </summary>
    /// <param name="_difficulty">��Փx�@1�`5</param>
    /// <param name="_areaId">�G���A�@1:�琬�@2:�{�X</param>
    /// <param name="_stageId">�X�e�[�W�@1�`6�@1:�̗́@2:���́@3:�U���@4:����@5:���x�@6:��p</param>
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
        Debug.Log("�X�e�[�W�f�[�^�ǂݍ��݊���");

        LoadCompleteProcess();
    }

    /// <summary>
    /// �G�f�[�^���擾
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
    /// �h���b�v�����擾
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
