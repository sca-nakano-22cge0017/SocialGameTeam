using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class CharaillustChanger : MonoBehaviour
{
    [SerializeField,Header("�L�����N�^�[")] private Sprite[] chara;
    [SerializeField,Header("�����̌��m(�m�[�}���̂�)")] private Sprite[] charaADifference;
    [SerializeField,Header("�����̃V�X�^�[(�m�[�}���̂�)")] private Sprite[] charaBDifference;
    [SerializeField] private Image image = null;

    int charaNumSelect;
    int differenceNum = 0;
    bool charaLoss;
    bool ispush = false;

    CombiType type;

    private void Start()
    {
        charaLoss = GameManager.islastBattleLose;
    }
    //�I�΂ꂽ�L������\��
    void Update()
    {
        charaNumSelect = GameManager.SelectChara;

        type = PlayerDataManager.player.GetSelectEvolutionType();
        if (charaNumSelect == 1)
        {
            switch (type)//���m
            {
                case CombiType.ATK:
                    image.sprite = chara[1];
                    break;
                case CombiType.DEF:
                    image.sprite = chara[2];
                    break;
                case CombiType.TEC:
                    image.sprite = chara[3];
                    break;
                case CombiType.NORMAL:
                    image.sprite = chara[0];
                    if (charaLoss && SceneManager.GetActiveScene().name == "HomeScene")
                    {
                        image.sprite =charaADifference[0];
                        differenceNum = 4;
                    }
                    break;
                default: 
                    image.sprite = chara[0]; 
                    break;
            }
        }
        if (charaNumSelect == 2)//�V�X�^�[
        {
            switch (type)
            {
                case CombiType.ATK:
                    image.sprite = chara[5];
                    break;
                case CombiType.DEF:
                    image.sprite = chara[6];
                    break;
                case CombiType.TEC:
                    image.sprite = chara[7];
                    break;
                case CombiType.NORMAL:
                        image.sprite = chara[4];
                    if (charaLoss && SceneManager.GetActiveScene().name == "HomeScene")
                    {
                        image.sprite = charaBDifference[0];
                        differenceNum = 4;
                    }
                    break;
                default:
                    image.sprite = chara[4];
                    break;
            }
        }
        if (ispush)
        {
            charaLoss = false;
            //���m���m�[�}���̎�
            if (charaNumSelect == 1 && type == CombiType.NORMAL)
            {
                if (differenceNum == 1)
                {
                    image.sprite = charaADifference[differenceNum];
                }
                if (differenceNum == 2)
                {
                    image.sprite = charaADifference[differenceNum];
                }
                if (differenceNum == 3)
                {
                    image.sprite = charaADifference[differenceNum];
                }
                if (differenceNum > 3)
                {
                    differenceNum = 0;
                }
            }

            //�V�X�^�[���m�[�}���̎�
            if (charaNumSelect == 2 && type == CombiType.NORMAL)
            {
                if (differenceNum == 1)
                {
                    image.sprite = charaBDifference[differenceNum];
                }
                if (differenceNum == 2)
                {
                    image.sprite = charaBDifference[differenceNum];
                }
                if (differenceNum > 2)
                {
                    differenceNum = 0;
                }
            }
        }
    }
    public void differenceImage()
    {
        ispush = true;
        differenceNum++;
    }
}
