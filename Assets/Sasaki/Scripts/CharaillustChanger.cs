using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System;

public class CharaillustChanger : MonoBehaviour
{
    [SerializeField,Header("�L�����N�^�[")] private Sprite[] chara;
    [SerializeField,Header("�����̌��m")] private Sprite[] charaADifference;
    [SerializeField,Header("�����̃V�X�^�[")] private Sprite[] charaBDifference;
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
    //�I�΂ꂽ�L������\��
    void Update()
    {
        charaNumSelect = GameManager.SelectChara;

        type = PlayerDataManager.player.GetSelectEvolutionType(); 
        //������(��x�����Ă΂��)
        if (b)
        {
            StartImage();
            differenceNum = imageNum - 1;
        }

        if (charaNumSelect == 1)
        {
            switch (type)//���m
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
        if (charaNumSelect == 2)//�V�X�^�[
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

        //�����ꂽ��
        if (ispush)
        {
            charaLoss = false;
            
            //���m
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
            //�V�X�^�[
            if (charaNumSelect == 2)
            {
                tapChanger();
            }
        }
    }
    //�s�k��̉摜�ύX
    private void lossImage()
    {
        type = PlayerDataManager.player.GetSelectEvolutionType();
        if (charaLoss && SceneManager.GetActiveScene().name == "HomeScene")
        {
            //���m
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
            //�V�X�^�[
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

    //�^�b�v������摜�ύX
    private void tapChanger()
    {
        //���m
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
        //�V�X�^�[
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
    //�����l
    private void StartImage()
    {
        //���m
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
        //�V�X�^�[
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
    //�L�����^�b�v�{�^��
    public void DifferenceImage()
    {
        ispush = true;
        b = false;
        differenceNum++;
    }
}
