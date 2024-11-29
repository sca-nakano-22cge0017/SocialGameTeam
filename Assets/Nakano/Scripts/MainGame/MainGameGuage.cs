using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainGameGuage : MonoBehaviour
{
    [SerializeField, Header("一瞬で減るゲージ")] private Image guage_first;
    [SerializeField, Header("徐々に減るゲージ")] private Image guage_second;
    [SerializeField] private Text text;
    [SerializeField, Header("減少/増加速度")] private float in_decreaseSpeed;
    [SerializeField, Header("減少/増加時間")] private float in_decreaseTime;
    private float t_speed;
    [SerializeField, Header("0:一定速度で減少させる 1:一定時間内に減少終了させる")]
    private bool choice;

    [SerializeField] private GameObject headImage;
    [SerializeField] private Vector2 specialIconCenter;

    private float diff = 100;
    private int current = 100; // 現在の数値
    private int max = 100;     // 最大値

    private bool didDecrease = false; // 数値が減少したか
    private bool didIncrease = false; // 数値が増加したか

    public bool isDirectionCompleted = true; // 演出終了

    /// <summary>
    /// ゲージ初期化
    /// </summary>
    /// <param name="_maxAmount">最大値</param>
    public void Initialize(int _maxAmount)
    {
        max = _maxAmount;
        diff = max;
        current = max;

        didDecrease = false;
        didIncrease = false;

        if (text != null) text.text = _maxAmount.ToString() + "/" + _maxAmount.ToString();
    }

    /// <summary>
    /// ゲージ初期化
    /// </summary>
    /// <param name="_maxAmount">最大値</param>
    public void Initialize(int _maxAmount, int _current)
    {
        max = _maxAmount;
        diff = _current;
        current = _current;

        didDecrease = false;
        didIncrease = false;

        if (text != null) text.text = _maxAmount.ToString() + "/" + _maxAmount.ToString();
    }

    /// <summary>
    /// 現在値増加
    /// </summary>
    /// <param name="_amount">増加量</param>
    public void Add(int _amount)
    {
        isDirectionCompleted = false;

        current += _amount;
        if (current > max) current = max;

        if (text != null) text.text = current.ToString() + "/" + max.ToString();
        if (guage_first != null) guage_first.fillAmount = (float)current / max;
        if (guage_second != null) guage_second.fillAmount = (float)current / max;

        IncreaseFirstGuage();
    }

    /// <summary>
    /// 現在値減少
    /// </summary>
    /// <param name="_amount">減少量</param>
    public void Sub(int _amount)
    {
        isDirectionCompleted = false;

        current -= _amount;
        if (current < 0) current = 0;

        DecreaseFirstGuage();
    }

    /// <summary>
    /// 現在値変更
    /// </summary>
    /// <param name="_currentAmount">指定量</param>
    public void SetCurrent(int _currentAmount)
    {
        isDirectionCompleted = false;
        current = _currentAmount;

        DecreaseFirstGuage();
    }

    /// <summary>
    /// 一つ目のゲージを一瞬で減少させる
    /// </summary>
    void DecreaseFirstGuage()
    {
        if (text != null) text.text = current.ToString() + "/" + max.ToString();
        if (guage_first != null) guage_first.fillAmount = (float)current / max;

        didDecrease = true;
        t_speed = (float)(diff - current) / in_decreaseTime;
    }

    /// <summary>
    /// 一つ目のゲージを一瞬で増加させる
    /// </summary>
    void IncreaseFirstGuage()
    {
        if (text != null) text.text = current.ToString() + "/" + max.ToString();
        if (guage_first != null) guage_first.fillAmount = (float)current / max;

        didIncrease = true;
        t_speed = (float)(current - diff) / in_decreaseTime;
    }

    void Update()
    {
        if (didDecrease)
        {
            if (diff >= current)
            {
                if (!choice)
                {
                    diff -= in_decreaseSpeed * Time.deltaTime;
                }
                else
                {
                    diff -= t_speed * Time.deltaTime;
                }
            }
            else
            {
                diff = current;
                isDirectionCompleted = true;
                didDecrease = false;
            }

            if (guage_second != null) guage_second.fillAmount = diff / max;
            HeadImageMove();
        }

        if (didIncrease)
        {
            if (diff <= current)
            {
                if (!choice)
                {
                    diff += in_decreaseSpeed * Time.deltaTime;
                }
                else
                {
                    diff += t_speed * Time.deltaTime;
                }
            }
            else
            {
                diff = current;
                isDirectionCompleted = true;

                didIncrease = false;
            }

            if (guage_second != null) guage_second.fillAmount = diff / max;
            HeadImageMove();
        }
    }

    void HeadImageMove()
    {
        if (headImage == null) return;

        float angle = 360.0f * guage_second.fillAmount - 180.0f;
        
        var pos = headImage.transform.localPosition;

        pos.x = -Mathf.Cos(-angle * Mathf.Deg2Rad) * 90;
        pos.y = -Mathf.Sin(-angle * Mathf.Deg2Rad) * 90;

        headImage.transform.localPosition = pos;
    }
}
