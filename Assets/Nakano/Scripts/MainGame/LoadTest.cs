using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Master;

public class LoadTest : MonoBehaviour
{
    private StageDataManager stageDataManager;
    [SerializeField, Header("��Փx"), Range(1, 5)] int difficulty;
    [SerializeField, Header("�G���A�@1:�琬 2:�{�X"), Range(1, 2)] int area;
    [SerializeField, Header("�X�e�[�W 1:�̗� 2;���� 3:�U�� 4:��� 5:���x 6:��p"), Range(1, 6)] int stage;

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
            // �X�e�[�W�f�[�^���ǂݍ��܂�Ă��Ȃ��ꍇ
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

    // �ǂݍ��݊������̏���
    void LoadComplete()
    {
        enemiesData = StageDataManager.EnemiesData;
        dropData = StageDataManager.DropData;
        loadCompleted = true;
    }
}
