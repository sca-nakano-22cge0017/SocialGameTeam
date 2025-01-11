using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class IconChanger : MonoBehaviour
{
    [SerializeField, Header("キャラクターアイコン")] private Sprite[] charaIcon;
    [SerializeField] private Image image = null;

    int charaNum;

    void Update()
    {
        charaNum = GameManager.SelectChara;

        if (charaNum == 1 )//剣士
        {
            image.sprite = charaIcon[0];
        }
        if (charaNum == 2)//シスター
        {
            image.sprite = charaIcon[1];
        }
    }
}
