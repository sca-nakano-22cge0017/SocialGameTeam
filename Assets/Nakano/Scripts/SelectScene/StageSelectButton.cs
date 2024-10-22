using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StageSelectButton : MonoBehaviour
{
    [SerializeField] private StageSelect stageSelect;
    [SerializeField, Header("難易度")] private int difficulty;
    [SerializeField, Header("遷移エリアのID")] private int areaId;
    [SerializeField, Header("遷移ステージのID")] private int stageId;

    private int tapCount = 0;
    private bool isExit = false;

    // ロングタップ
    private float longTapTime_sec; // ターゲット変更に必要な長押し時間
    private float longTapElapsedTime_sec = 0;
    private bool isLongTapCount = false;
    private bool didTargetChange = false;

    private void Awake()
    {
        //longTapTime_sec = stageSelect.GetLongTapTime_sec;
    }

    private void Update()
    {
        if (isLongTapCount)
        {
            longTapElapsedTime_sec += Time.deltaTime;

            if (longTapElapsedTime_sec >= longTapTime_sec)
            {
                //stageSelect.TargetChange(this);

                isLongTapCount = false;
                longTapElapsedTime_sec = 0;
            }
        }
    }

    /// <summary>
    /// 初期選択
    /// </summary>
    public void FirstSelected()
    {
        tapCount = 1;
    }

    public void TapCountReset()
    {
        tapCount = 0;
    }

    public void TargetChange()
    {
        didTargetChange = true;
    }

    public void PointerDown()
    {
        stageSelect.Select(this);
        isExit = false;
    }

    public void PointerUp()
    {
        tapCount++;

        if (tapCount >= 2 && !isExit && !didTargetChange)
        {
            stageSelect.Transition(this, difficulty, areaId, stageId);
            tapCount = 0;
        }

        didTargetChange = false;
    }

    public void PointerExit()
    {
        isExit = true;
    }

    public void LongTapStart()
    {
        isLongTapCount = true;
        longTapElapsedTime_sec = 0;
    }

    public void LongTapEnd()
    {
        isLongTapCount = false;
        longTapElapsedTime_sec = 0;
    }
}
