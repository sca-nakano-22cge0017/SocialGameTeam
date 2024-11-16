using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Master;
using UnityEngine.UI;

public class DropController : MonoBehaviour
{
    private List<DropItem> data = new();

    private List<DropItem> dropedItems = new();
    public List<DropItem> DropedItems
    {
        get { return dropedItems; }
        private set { }
    }

    [SerializeField] DEX_SpecialTecnique dex_st;
    private float power = 1; // ドロップ量上昇倍率

    void Start()
    {
        Initialize();
    }

    public void Initialize()
    {
        dropedItems.Clear();

        for (int i = 0; i < System.Enum.GetValues(typeof(StatusType)).Length; i++)
        {
            DropItem drop = new();
            drop.itemType = (StatusType)System.Enum.ToObject(typeof(StatusType), i);
            drop.dropAmount = 0;
            dropedItems.Add(drop);
        }

        if (dex_st == null) return;

        power = 1 + dex_st.RankB(); // 成長の道
    }

    /// <summary>
    /// ドロップ抽選
    /// </summary>
    /// <returns>ドロップ量</returns>
    public int DropLottery()
    {
        int drop = 0;

        if (StageDataManager.DropData != null)
        {
            data = StageDataManager.DropData;
        }

        List<float> range = new();
        float t = 0;
        range.Add(0);
        for (int i = 0; i < data.Count; i++)
        {
            t += data[i].dropProbability;
            range.Add(t);
        }

        int rnd = Random.Range(0, 100);
        for (int i = 0; i < range.Count - 1; i++)
        {
            if (range[i] <= rnd && rnd < range[i + 1])
            {
                float d = data[i].dropAmount * power;

                AddDropAmount(data[i].itemType, (int)d);
                drop = (int)d;

                break;
            }
        }

        range.Clear();
        return drop;
    }

    /// <summary>
    /// ドロップ結果の加算
    /// </summary>
    /// <param name="_type"></param>
    /// <param name="_amount"></param>
    void AddDropAmount(StatusType _type, int _amount)
    {
        for (int i = 0; i < dropedItems.Count; i++)
        {
            if (_type == dropedItems[i].itemType)
            {
                dropedItems[i].dropAmount += _amount;
            }
        }
    }
}
