using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StaminaManager : MonoBehaviour
{
    private int stamina = 0;
    /// <summary>
    /// ���݂̃X�^�~�i��
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
    /// ���ɃX�^�~�i���񕜂���܂ł̎c�莞��
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
    /// �S���܂ł̎c�莞��
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

    private const int max_Initial = 100;  // �X�^�~�i�ő�l�̏����l

    private const int maxAdd_LevelUp = 30;      // ���x���A�b�v���̃X�^�~�i���������
    private const int add_LevelUp = 30;         // ���x���A�b�v���̃X�^�~�i������
    private const int add_Recovery = 1;        // ��莞�Ԗ��̃X�^�~�i�񕜗�
    private const float recoveryIntervalMin = 5.0f; // �X�^�~�i�񕜂̃C���^�[�o��(��)
    float recoveryIntervalSec = 0;                   // �X�^�~�i�񕜂̃C���^�[�o��(�b)

    private const int cost_Traning = 5;   // �琬�X�e�[�W�ł̃X�^�~�i�����
    public int GetCost_Traning { get => cost_Traning; }
    private const int cost_Boss = 10;     // �{�X�X�e�[�W�ł̃X�^�~�i�����
    public int GetCost_Boss { get => cost_Boss; }

    private const int MINUTE_PER_HOUR = 60;
    private const int SECOND_PER_MINUTE = 60;

    [SerializeField] private GameObject staminaWindow;
    [SerializeField] private WindowController staminaWindow_wc;

    public static int lastStamina = 0;
    public static string lastTimeStr = "";
    public System.DateTime lastTime;
    public static float lastRecoveryTime = 0;
    public static float lastCompleteRecoveryTime = 0;

    public void Initialize()
    {
        recoveryIntervalSec = (float)recoveryIntervalMin * (float)SECOND_PER_MINUTE;

        if (GameManager.isFirstStart)
        {
            stamina = max_Initial;
            stamina_Max = max_Initial;
        }
        else
        {
            System.TimeSpan span;
            double spantime = 0;
            if (System.DateTime.TryParse(lastTimeStr, out System.DateTime t))
            {
                lastTime = t;
            }
            else
            {
                lastTime = System.DateTime.Now;
            }
            span = System.DateTime.Now - lastTime;
            spantime = span.TotalSeconds;

            int clearBossTopDifficulty = 0;
            if (DifficultyManager.IsClearBossDifficulty1 >= DifficultyManager.IsClearBossDifficulty2) 
                clearBossTopDifficulty = DifficultyManager.IsClearBossDifficulty1;
            else clearBossTopDifficulty = DifficultyManager.IsClearBossDifficulty2;

            stamina_Max = clearBossTopDifficulty * add_LevelUp + max_Initial;

            float rCompTime = lastCompleteRecoveryTime - (float)spantime;
            if (rCompTime < 0) stamina = stamina_Max;
            else
            {
                float value = ((float)spantime - lastRecoveryTime) / recoveryIntervalSec;
                value = value < 0 ? 0 : value;

                int currentAmount = lastStamina + (int)value;
                stamina = currentAmount > stamina_Max ? stamina_Max : currentAmount;

                elapsedTimeSec = ((lastStamina + 1) * recoveryIntervalSec - lastRecoveryTime + (float)spantime) % recoveryIntervalSec;
            }
        }
    }

    /// <summary>
    /// �X�^�~�i��
    /// </summary>
    public void Recovery()
    {
        // �X�^�~�i�ő�Ȃ�񕜂��Ȃ�
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

            Debug.Log("���݂̃X�^�~�i��" + stamina);

            elapsedTimeSec = 0;
        }
    }

    /// <summary>
    /// �X�^�~�i�S�� �f�o�b�O�p
    /// </summary>
    public void DebugRecovery()
    {
        stamina = stamina_Max;
    }

    public void LevelUp()
    {
        stamina_Max += maxAdd_LevelUp;
        stamina += add_LevelUp;

        Debug.Log("���݂̃X�^�~�i�ő�l��" + stamina_Max + ", ���݂̃X�^�~�i��" + stamina);
    }

    public bool Traning()
    {
        return Cost(cost_Traning);
    }

    public bool Boss()
    {
        return Cost(cost_Boss);
    }

    /// <summary>
    /// �r���ŏI�������Ƃ��ɃX�^�~�i�Ԋ�
    /// </summary>
    public void TraningExit()
    {
        var st = stamina;
        st += cost_Traning;

        if (st >= stamina_Max) stamina = stamina_Max;
        else stamina = st;
    }

    /// <summary>
    /// �r���ŏI�������Ƃ��ɃX�^�~�i�Ԋ�
    /// </summary>
    public void BossExit()
    {
        var st = stamina;
        st += cost_Boss;

        if (st >= stamina_Max) stamina = stamina_Max;
        else stamina = st;
    }

    /// <summary>
    /// �X�^�~�i����
    /// </summary>
    /// <param name="_cost">�����</param>
    public bool Cost(int _cost)
    {
        if (stamina < _cost)
        {
            StartCoroutine(DisplayWindow());
            return false;
        }

        stamina -= _cost;

        Debug.Log("���݂̃X�^�~�i��" + stamina);
        return true;
    }

    private string ConvertTimeToString(int _time, bool needHour)
    {
        // �X�^�~�i�ő�Ȃ�񕜂��Ȃ�
        if (stamina == stamina_Max)
        {
            if (needHour) return "0:00:00";
            else return "0:00";
        }

        string str = "";
        int time = _time;
        int hour = 0, min = 0, sec = 0;

        int sec_per_hour = MINUTE_PER_HOUR * SECOND_PER_MINUTE;

        // �c�莞�Ԃ�1���Ԉȏ�Ȃ�
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

    IEnumerator DisplayWindow()
    {
        if (staminaWindow.activeSelf) yield break;

        staminaWindow_wc.Open();

        yield return new WaitForSeconds(1.0f);

        staminaWindow_wc.Close();

        yield break;
    }

    void OnApplicationQuit()
    {
        Save();
    }

    /// <summary>
    /// �X�^�~�i���ۑ�
    /// </summary>
    public void Save()
    {
        lastTimeStr = System.DateTime.Now.ToString();
        lastStamina = stamina;
        lastRecoveryTime = recoveryTimeSec;
        lastCompleteRecoveryTime = completeRecoveryTimeSec;
    }
}
