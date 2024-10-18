using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class CharacterIllustration
{
    public int id;        // �L�����N�^�[ID
    public Sprite sprite; // �C���X�g
}

public class CharacterIllustController : MonoBehaviour
{
    [SerializeField] private Image charaImage;
    [SerializeField] private CharacterIllustration[] illustrations;

    private void Start()
    {
        ChangeIllust();
    }

    private void OnEnable()
    {
        ChangeIllust();
    }

    /// <summary>
    /// �\���C���X�g�ύX
    /// </summary>
    public void ChangeIllust()
    {
        int select = GameManager.SelectChara;

        for (int i = 0; i < illustrations.Length; i++)
        {
            if (select == illustrations[i].id)
            {
                charaImage.sprite = illustrations[i].sprite;
            }
        }
    }
}
