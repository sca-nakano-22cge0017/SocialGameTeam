using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StageSelectButton : MonoBehaviour
{
    [SerializeField] private StageSelect stageSelect;
    [SerializeField, Header("難易度")] private int difficulty;
    public int Difficluty { get => difficulty; private set => difficulty = value;}

    [SerializeField, Header("遷移エリアのID")] private int areaId;
    public int AreaID { get => areaId; private set => areaId = value; }
    [SerializeField, Header("遷移ステージのID")] private int stageId;
    public int StageID { get => stageId; private set => stageId = value; }

    public void Select()
    {
        stageSelect.Select(this);
    }
}
