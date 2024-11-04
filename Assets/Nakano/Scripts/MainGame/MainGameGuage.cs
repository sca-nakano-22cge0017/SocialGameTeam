using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainGameGuage : MonoBehaviour
{
    [SerializeField, Header("一瞬で減るゲージ")] private Image guage_first;
    [SerializeField, Header("徐々に減るゲージ")] private Image guage_second;
    [SerializeField] private Text text;
    [SerializeField, Header("減少速度")] private float decreaseSpeed;
    [SerializeField, Header("減少時間")] private float decreaseTime;
    private float t_speed;
    [SerializeField, Header("0:一定速度で減少させる 1:一定時間内に減少終了させる")]
    private bool choice;

    private float diff = 100;
    private int current = 100; // 現在の数値
    private int max = 100;     // 最大値

    private bool didChange = false; // 数値に変更があったか

    /// <summary>
    /// ゲージ初期化
    /// </summary>
    /// <param name="_maxAmount">最大値</param>
    public void Initialize(int _maxAmount)
    {
        max = _maxAmount;
        diff = max;
        current = max;

        text.text = _maxAmount.ToString() + "/" + _maxAmount.ToString();
    }

    /// <summary>
    /// 現在値増加
    /// </summary>
    /// <param name="_amount">増加量</param>
    public void Add(int _amount)
    {
        current += _amount;
        if (current > max) current = max;

        text.text = current.ToString() + "/" + max.ToString();
        guage_first.fillAmount = (float)current / max;
        guage_second.fillAmount = (float)current / max;
    }

    /// <summary>
    /// 現在値減少
    /// </summary>
    /// <param name="_amount">減少量</param>
    public void Sub(int _amount)
    {
        current -= _amount;
        if (current < 0) current = 0;

        ChangeFirstGuage();
    }

    /// <summary>
    /// 現在値変更
    /// </summary>
    /// <param name="_currentAmount">指定量</param>
    public void SetCurrent(int _currentAmount)
    {
        current = _currentAmount;

        ChangeFirstGuage();
    }

    /// <summary>
    /// 一つ目のゲージを一瞬で減少させる
    /// </summary>
    void ChangeFirstGuage()
    {
        text.text = current.ToString() + "/" + max.ToString();
        guage_first.fillAmount = (float)current / max;

        didChange = true;
        t_speed = (float)(diff - current) / decreaseTime;
    }

    void Update()
    {
        if (didChange)
        {
            if (diff >= current)
            {
                if (!choice)
                {
                    diff -= decreaseSpeed * Time.deltaTime;
                }
                else
                {
                    diff -= t_speed * Time.deltaTime;
                }
            }
            else diff = current;
            guage_second.fillAmount = diff / max;
        }
    }
}
