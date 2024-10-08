using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StaminaManager : MonoBehaviour
{
    private int stamina = 0;
    /// <summary>
    /// 現在のスタミナ量
    /// </summary>
    public int Stamina
    {
        get
        {
            return stamina;
        }
    }

    private int stamina_Max = 100;
    public int Stamina_Max
    {
        get
        {
            return stamina_Max;
        }
    }

    private string recoveryTimeText = "0:00";
    /// <summary>
    /// 次にスタミナが回復するまでの残り時間
    /// </summary>
    public string RecoveryTimeText
    {
        get
        {
            return recoveryTimeText;
        }
    }

    private string completeRecoveryTimeText = "0:00:00";
    /// <summary>
    /// 全快までの残り時間
    /// </summary>
    public string CompleteRecoveryTimeText
    {
        get
        {
            return completeRecoveryTimeText;
        }
    }

    private float recoveryTimeSec = 0;
    private string lastRecoveryTimeText = "0:00";
    private float completeRecoveryTimeSec = 0;
    private string lastCompleteRecoveryTimeText = "0:00:00";
    private float elapsedTimeSec = 0;

    private const int max_Initial = 100;  // スタミナ最大値の初期値

    private const int maxAdd_LevelUp = 3;      // レベルアップ時のスタミナ上限増加量
    private const int add_LevelUp = 3;         // レベルアップ時のスタミナ増加量
    private const int add_Recovery = 1;        // 一定時間毎のスタミナ回復量
    private const float recoveryIntervalMin = 5.0f; // スタミナ回復のインターバル(分)

    private const int cost_Traning = 5;   // 育成ステージでのスタミナ消費量
    private const int cost_Boss = 10;     // ボスステージでのスタミナ消費量

    private const int MINUTE_PER_HOUR = 60;
    private const int SECOND_PER_MINUTE = 60;

    public void Initialize()
    {
        stamina = max_Initial;
        stamina_Max = max_Initial;
    }

    /// <summary>
    /// スタミナ回復
    /// </summary>
    public void Recovery()
    {
        // スタミナ最大なら回復しない
        if (stamina >= stamina_Max)
        {
            recoveryTimeSec = 0;
            completeRecoveryTimeSec = 0;
            return;
        }

        if (elapsedTimeSec < recoveryIntervalMin * SECOND_PER_MINUTE)
        {
            elapsedTimeSec += Time.deltaTime;

            recoveryTimeSec = recoveryIntervalMin * (float)SECOND_PER_MINUTE - elapsedTimeSec;
            completeRecoveryTimeSec = (float)(stamina_Max - stamina) * recoveryIntervalMin * (float)SECOND_PER_MINUTE - elapsedTimeSec;

            var text1 = ConvertTimeToString((int)recoveryTimeSec, false);
            var text2 = ConvertTimeToString((int)completeRecoveryTimeSec, true);

            if (lastRecoveryTimeText != text1)
            {
                recoveryTimeText = text1;
                lastRecoveryTimeText = text1;
            }
            if (lastCompleteRecoveryTimeText != text2)
            {
                completeRecoveryTimeText = text2;
                lastCompleteRecoveryTimeText = text2;
            }
        }

        else
        {
            if (stamina < stamina_Max)
            {
                stamina += add_Recovery;
            }
            else stamina = stamina_Max;

            Debug.Log("現在のスタミナは" + stamina);

            elapsedTimeSec = 0;
        }
    }

    public void LevelUp()
    {
        stamina_Max += maxAdd_LevelUp;
        stamina += add_LevelUp;

        Debug.Log("現在のスタミナ最大値は" + stamina_Max + ", 現在のスタミナは" + stamina);
    }

    public void Traning()
    {
        Cost(cost_Traning);
    }

    public void Boss()
    {
        Cost(cost_Boss);
    }

    /// <summary>
    /// スタミナ消費
    /// </summary>
    /// <param name="_cost">消費量</param>
    private void Cost(int _cost)
    {
        if (stamina < _cost)
        {
            Debug.Log("スタミナが足りません");
            return;
        }

        stamina -= _cost;

        Debug.Log("現在のスタミナは" + stamina);
    }

    private string ConvertTimeToString(int _time, bool needHour)
    {
        // スタミナ最大なら回復しない
        if (stamina == stamina_Max)
        {
            if (needHour) return "0:00:00";
            else return "0:00";
        }

        string str = "";
        int time = _time;
        int hour = 0, min = 0, sec = 0;

        int sec_per_hour = MINUTE_PER_HOUR * SECOND_PER_MINUTE;

        // 残り時間が1時間以上なら
        if (time >= sec_per_hour)
        {
            hour = (int)(time / (sec_per_hour));
            time %= sec_per_hour;

            str += hour.ToString("d2") + ":";
        }
        else if (needHour)
        {
            str += "0:";
        }

        if (time >= SECOND_PER_MINUTE)
        {
            min = (int)(time / SECOND_PER_MINUTE);
            time %= SECOND_PER_MINUTE;

            str += min.ToString("d2") + ":";
        }
        else str += "00:";

        sec = time;
        str += sec.ToString("d2");

        return str;
    }
}
