using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharaillustChanger : MonoBehaviour
{
    [SerializeField,Header("�L�����N�^�[")] private Sprite[] chara;
    [SerializeField] private Image image = null;

    int charaNumSelect;

    //�I�΂ꂽ�L������\��
    void Update()
    {
        charaNumSelect = GameManager.SelectChara;

        CombiType type = PlayerDataManager.player.GetSelectEvolutionType();
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
                    break;
                default:
                    image.sprite = chara[4];
                    break;
            }
        }
    }
}
