using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class IconChanger : MonoBehaviour
{
    [SerializeField, Header("�L�����N�^�[�A�C�R��")] private Sprite[] charaIcon;
    [SerializeField] private Image image = null;

    int charaNum;

    void Update()
    {
        charaNum = GameManager.SelectChara;

        if (charaNum == 1 )//���m
        {
            image.sprite = charaIcon[0];
        }
        if (charaNum == 2)//�V�X�^�[
        {
            image.sprite = charaIcon[1];
        }
    }
}
