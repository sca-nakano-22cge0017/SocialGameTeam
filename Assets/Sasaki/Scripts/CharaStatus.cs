using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class CharaStatus : MonoBehaviour
{
    [SerializeField,Header("�琬�I���E�B���h�E")] private GameObject resetWindow;
    //[SerializeField,Header("���ʐ����E�B���h�E")] private GameObject effectWindow;
    [SerializeField,Header("���ʕ\���̃e�L�X�g")] private Text effectText;

    string strEffect;
    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        if (resetWindow.activeSelf)
        {
            effectDescription();
        }
    }
    //���Z�b�g�̃{�^���ݒ�
    public void ResetWindowScene(string str)
    {
        switch (str)
        {
            case "Reset": //�琬����(�z�[���ɖ߂�)
                PlayerDataManager.TraningReset();
                Debug.Log("�琬����");
                SceneManager.LoadScene("HomeScene");
                break;
            default:
                SceneManager.LoadScene("TitleScene");
                break;
        }
    }

    private void effectDescription()
    {
        strEffect = PlayerDataManager.GetResetEffects();
        effectText.text = strEffect.ToString();
    }
}
