using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StageSelectButton : MonoBehaviour
{
    [SerializeField] private StageSelect stageSelect;
    [SerializeField, Header("��Փx")] private int difficulty;
    public int Difficluty { get => difficulty; private set => difficulty = value;}

    [SerializeField, Header("�J�ڃG���A��ID")] private int areaId;
    public int AreaID { get => areaId; private set => areaId = value; }
    [SerializeField, Header("�J�ڃX�e�[�W��ID")] private int stageId;
    public int StageID { get => stageId; private set => stageId = value; }

    public void Select()
    {
        stageSelect.Select(this);
    }
}
