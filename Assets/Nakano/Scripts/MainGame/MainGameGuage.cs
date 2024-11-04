using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainGameGuage : MonoBehaviour
{
    [SerializeField, Header("��u�Ō���Q�[�W")] private Image guage_first;
    [SerializeField, Header("���X�Ɍ���Q�[�W")] private Image guage_second;
    [SerializeField] private Text text;
    [SerializeField, Header("�������x")] private float decreaseSpeed;
    [SerializeField, Header("��������")] private float decreaseTime;
    private float t_speed;
    [SerializeField, Header("0:��葬�x�Ō��������� 1:��莞�ԓ��Ɍ����I��������")]
    private bool choice;

    private float diff = 100;
    private int current = 100; // ���݂̐��l
    private int max = 100;     // �ő�l

    private bool didChange = false; // ���l�ɕύX����������

    /// <summary>
    /// �Q�[�W������
    /// </summary>
    /// <param name="_maxAmount">�ő�l</param>
    public void Initialize(int _maxAmount)
    {
        max = _maxAmount;
        diff = max;
        current = max;

        text.text = _maxAmount.ToString() + "/" + _maxAmount.ToString();
    }

    /// <summary>
    /// ���ݒl����
    /// </summary>
    /// <param name="_amount">������</param>
    public void Add(int _amount)
    {
        current += _amount;
        if (current > max) current = max;

        text.text = current.ToString() + "/" + max.ToString();
        guage_first.fillAmount = (float)current / max;
        guage_second.fillAmount = (float)current / max;
    }

    /// <summary>
    /// ���ݒl����
    /// </summary>
    /// <param name="_amount">������</param>
    public void Sub(int _amount)
    {
        current -= _amount;
        if (current < 0) current = 0;

        ChangeFirstGuage();
    }

    /// <summary>
    /// ���ݒl�ύX
    /// </summary>
    /// <param name="_currentAmount">�w���</param>
    public void SetCurrent(int _currentAmount)
    {
        current = _currentAmount;

        ChangeFirstGuage();
    }

    /// <summary>
    /// ��ڂ̃Q�[�W����u�Ō���������
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
