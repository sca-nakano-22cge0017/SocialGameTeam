using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemiesIllust
{
    public string id;
    public Sprite sprite;
    public Animation anim;
}

/// <summary>
/// ステージ上のプレイヤーや敵の表示・初期設定制御
/// </summary>
public class StageManager : MonoBehaviour
{
    [SerializeField] private PlayerData player;
    [SerializeField] private Enemy[] enemies;
    [SerializeField, Header("敵のイラスト/アニメーション")] private EnemiesIllust[] enemiesIllust;

    StageDataManager stageDataManager;

    void Start()
    {
        stageDataManager = FindObjectOfType<StageDataManager>();
        if (stageDataManager)
        {
            if (GameManager.SelectArea == 1) TraningStageDataSet();
            if (GameManager.SelectArea == 2) BossStageDataSet();
        }
    }

    void Update()
    {
        
    }

    void TraningStageDataSet()
    {
        for (int e = 0; e < enemies.Length; e++)
        {
            enemies[e].gameObject.SetActive(false);
        }

        List<Master.EnemyData> data = StageDataManager.EnemiesData;
        for (int d = 0; d < data.Count; d++)
        {
            for (int e = 0; e < enemies.Length; e++)
            {
                if (data[d].placementId != enemies[e].POSITION) continue;

                for (int i = 0; i < enemiesIllust.Length; i++)
                {
                    if (data[d].enemyStatus.enemyId == enemiesIllust[i].id)
                    {
                        // イラスト変更
                        enemies[d].GetComponent<Image>().sprite = enemiesIllust[i].sprite;
                    }
                }

                enemies[d].gameObject.SetActive(true);
            }
        }
    }

    void BossStageDataSet()
    {
    }
}
