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
            drop.dropAmount = 300; // Debug
            dropedItems.Add(drop);
        }
    }

    /// <summary>
    /// ドロップ抽選
    /// </summary>
    public void DropLottery()
    {
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
                AddDropAmount(data[i].itemType, data[i].dropAmount);

                break;
            }
        }

        range.Clear();
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
