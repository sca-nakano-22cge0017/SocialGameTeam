using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharaillustChanger : MonoBehaviour
{
    [SerializeField,Header("キャラクター")] private Sprite[] chara;
    [SerializeField] private Image image = null;

    int charaNumSelect;
    // Start is called before the first frame update
    void Start()
    {
        charaNumSelect = GameManager.SelectChara;
    }

    //選ばれたキャラを表示
    void Update()
    {
        
        if (charaNumSelect == 1)
        {
            image.sprite = chara[0];//剣士
        }
        if (charaNumSelect == 2)
        {
            image.sprite = chara[1];//シスター
        }
    }
}
