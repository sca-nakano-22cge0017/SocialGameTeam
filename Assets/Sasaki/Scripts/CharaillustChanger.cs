using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System;

public class CharaillustChanger : MonoBehaviour
{
    [SerializeField,Header("キャラクター")] private Sprite[] chara;
    [SerializeField,Header("差分の剣士")] private Sprite[] charaADifference;
    [SerializeField,Header("差分のシスター")] private Sprite[] charaBDifference;
    [SerializeField] private Image image = null;

    int charaNumSelect;
    int differenceNum = 0;
    int imageNum = 0;
    bool charaLoss;
    bool ispush = false;

    CombiType type;
    bool b;
    private void Start()
    {
        charaLoss = GameManager.islastBattleLose;
        b = true;
    }
    //選ばれたキャラを表示
    void Update()
    {
        charaNumSelect = GameManager.SelectChara;

        type = PlayerDataManager.player.GetSelectEvolutionType(); 
        //初期化(一度だけ呼ばれる)
        if (b)
        {
            StartImage();
            differenceNum = imageNum - 1;
        }

        if (charaNumSelect == 1)
        {
            switch (type)//剣士
            {                
                case CombiType.NORMAL:
                    image.sprite = chara[0];
                    lossImage();
                    break;
                case CombiType.ATK:
                    image.sprite = chara[1];
                    lossImage();
                    break;
                case CombiType.DEF:
                    image.sprite = chara[2];
                    lossImage();
                    break;
                case CombiType.TEC:
                    image.sprite = chara[3];
                    lossImage();
                    break;
                default:
                    image.sprite = null;
                    break;
            }
        }
        if (charaNumSelect == 2)//シスター
        {
            switch (type)
            {               
                case CombiType.NORMAL:
                    image.sprite = chara[4];
                    lossImage();
                    break;
                case CombiType.ATK:
                    image.sprite = chara[5];
                    lossImage();
                    break;
                case CombiType.DEF:
                    image.sprite = chara[6];
                    lossImage();
                    break;
                case CombiType.TEC:
                    image.sprite = chara[7];
                    lossImage();
                    break;
                default:
                    image.sprite = null;
                    break;
            }
        }

        //押されたら
        if (ispush)
        {
            charaLoss = false;
            
            //剣士
            if (charaNumSelect == 1)            
            {
                if (type == CombiType.NORMAL)
                {
                    if (differenceNum == imageNum)
                    {
                        image.sprite = charaADifference[differenceNum];
                    }
                    if (differenceNum == imageNum + 1)
                    {
                        image.sprite = charaADifference[differenceNum];
                    }
                    if (differenceNum == imageNum + 2)
                    {
                        image.sprite = charaADifference[differenceNum];
                    }
                    if (differenceNum > imageNum + 2)
                    {
                        differenceNum = imageNum- 1;
                    }
                }
                else
                {
                    tapChanger();
                }
            }
            //シスター
            if (charaNumSelect == 2)
            {
                tapChanger();
            }
        }
    }
    //敗北後の画像変更
    private void lossImage()
    {
        type = PlayerDataManager.player.GetSelectEvolutionType();
        if (charaLoss && SceneManager.GetActiveScene().name == "HomeScene")
        {
            //剣士
            if (charaNumSelect == 1)
            {
                switch (type)
                {
                    case CombiType.NORMAL:
                        image.sprite = charaADifference[0];
                        
                        break;
                    case CombiType.ATK:
                        image.sprite = charaADifference[4];
                        break;
                    case CombiType.DEF:
                        image.sprite = charaADifference[7];
                        break;
                    case CombiType.TEC:
                        image.sprite = charaADifference[10];
                        break;
                    default:
                        image.sprite = null;
                        break;
                }
            }
            //シスター
            if (charaNumSelect == 2)
            {
                switch (type)
                {
                    case CombiType.NORMAL:
                        image.sprite = charaBDifference[0];
                        break;
                    case CombiType.ATK:
                        image.sprite = charaBDifference[3];
                        break;
                    case CombiType.DEF:
                        image.sprite = charaBDifference[6];
                        break;
                    case CombiType.TEC:
                        image.sprite = charaBDifference[9];
                        break;
                    default:
                        image.sprite = null;
                        break;
                }
            }
            differenceNum = imageNum - 1;
        }
    }

    //タップしたら画像変更
    private void tapChanger()
    {
        //剣士
        if (charaNumSelect == 1)
        {
            if (differenceNum == imageNum)
            {
                image.sprite = charaADifference[differenceNum];
            }
            if (differenceNum == imageNum + 1)
            {
                image.sprite = charaADifference[differenceNum];
            }
            if (differenceNum > imageNum + 1)
            {
                differenceNum = imageNum -1;
            }
        }
        //シスター
        if (charaNumSelect == 2)
        {
            if (differenceNum == imageNum)
            {
                image.sprite = charaBDifference[differenceNum];
            }
            if (differenceNum == imageNum + 1)
            {
                image.sprite = charaBDifference[differenceNum];
            }
            if (differenceNum > imageNum + 1)
            {
                differenceNum = imageNum -1;
            }
        }
    }
    //初期値
    private void StartImage()
    {
        //剣士
        if (charaNumSelect == 1)
        {
            switch (type)
            {
                case CombiType.NORMAL:
                    imageNum = 1;
                    break;
                case CombiType.ATK:
                    imageNum = 5;
                    break;
                case CombiType.DEF:
                    imageNum = 8;
                    break;
                case CombiType.TEC:
                    imageNum = 11;
                    break;
                default:
                    image.sprite = null;
                    break;
            }
        }
        //シスター
        if (charaNumSelect == 2)
        {
            switch (type)
            {
                case CombiType.NORMAL:
                    imageNum = 1;
                    break;
                case CombiType.ATK:
                    imageNum = 4;
                    break;
                case CombiType.DEF:
                    imageNum = 7;
                    break;
                case CombiType.TEC:
                    imageNum = 10;
                    break;
                default:
                    image.sprite = null;
                    break;
            }
        }

    }
    //キャラタップボタン
    public void DifferenceImage()
    {
        ispush = true;
        b = false;
        differenceNum++;
    }
}
