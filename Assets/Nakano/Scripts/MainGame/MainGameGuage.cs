using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainGameGuage : MonoBehaviour
{
    [SerializeField, Header("��u�Ō���Q�[�W")] private Image guage_first;
    [SerializeField, Header("���X�Ɍ���Q�[�W")] private Image guage_second;
    [SerializeField] private Text text;
    [SerializeField, Header("����/�������x")] private float in_decreaseSpeed;
    [SerializeField, Header("����/��������")] private float in_decreaseTime;
    private float t_speed;
    [SerializeField, Header("0:��葬�x�Ō��������� 1:��莞�ԓ��Ɍ����I��������")]
    private bool choice;

    [SerializeField] private GameObject headImage;
    [SerializeField] private Vector2 specialIconCenter;

    private float diff = 100;
    private int current = 100; // ���݂̐��l
    private int max = 100;     // �ő�l

    private bool didDecrease = false; // ���l������������
    private bool didIncrease = false; // ���l������������

    public bool isDirectionCompleted = true; // ���o�I��

    /// <summary>
    /// �Q�[�W������
    /// </summary>
    /// <param name="_maxAmount">�ő�l</param>
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
    /// �Q�[�W������
    /// </summary>
    /// <param name="_maxAmount">�ő�l</param>
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
    /// ���ݒl����
    /// </summary>
    /// <param name="_amount">������</param>
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
    /// ���ݒl����
    /// </summary>
    /// <param name="_amount">������</param>
    public void Sub(int _amount)
    {
        isDirectionCompleted = false;

        current -= _amount;
        if (current < 0) current = 0;

        DecreaseFirstGuage();
    }

    /// <summary>
    /// ���ݒl�ύX
    /// </summary>
    /// <param name="_currentAmount">�w���</param>
    public void SetCurrent(int _currentAmount)
    {
        isDirectionCompleted = false;
        current = _currentAmount;

        DecreaseFirstGuage();
    }

    /// <summary>
    /// ��ڂ̃Q�[�W����u�Ō���������
    /// </summary>
    void DecreaseFirstGuage()
    {
        if (text != null) text.text = current.ToString() + "/" + max.ToString();
        if (guage_first != null) guage_first.fillAmount = (float)current / max;

        didDecrease = true;
        t_speed = (float)(diff - current) / in_decreaseTime;
    }

    /// <summary>
    /// ��ڂ̃Q�[�W����u�ő���������
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
