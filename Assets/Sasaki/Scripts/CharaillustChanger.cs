using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// ��ł��邱��
/// �����̃C���X�g��\������
/// </summary>
public class CharaillustChanger : MonoBehaviour
{
    [SerializeField,Header("�L�����N�^�[")] private Sprite[] chara;
    [SerializeField] private Image image = null;

    int charaNumSelect;
    // Start is called before the first frame update
    void Start()
    {
        charaNumSelect = GameManager.SelectChara;
    }

    //�I�΂ꂽ�L������\��
    void Update()
    {
        
        if (charaNumSelect == 1)
        {
            image.sprite = chara[0];//���m
        }
        if (charaNumSelect == 2)
        {
            image.sprite = chara[1];//�V�X�^�[
        }
    }
}
