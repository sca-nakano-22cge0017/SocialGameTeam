using System.Buffers;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class CharaStatus : MonoBehaviour
{
    [SerializeField,Header("�琬�I���E�B���h�E")] private GameObject resetWindow;
    [SerializeField,Header("�`�F�b�N�E�B���h�E")] private GameObject checkWindow;
    //[SerializeField,Header("���ʐ����E�B���h�E")] private GameObject effectWindow;
    [SerializeField,Header("���ʕ\���̃e�L�X�g")] private Text effectText;
    [SerializeField,Header("�琬������fade")] private SpecialFade fade;
    [SerializeField,Header("�琬�����̕\��")] private GameObject resetFadeText;
    [SerializeField, Header("�琬�����̃e�L�X�g")] private Text fadeText;

    string strEffect;
    string strReset;

    bool resetCheck = false;
    bool strplus = false;
    // Start is called before the first frame update
    void Start()
    {
        fade.EyeOpen();
        fade = FindObjectOfType<SpecialFade>();
        resetFadeText.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (resetCheck) //�琬����������
        {
            strReset = "�琬�󋵂����Z�b�g���܂����B\n";
            if (strplus)
            {
                strReset = strReset +
                            "�v���X�l���t�^����܂����B\n" +
                            "�ΏۃX�e�[�^�X�̏���l���㏸���܂��B";
            }
            fadeBool();
        }
        else //�琬�����\�����\��
        {
            resetFadeText.SetActive(false);
            strplus = false;
        }

        if (resetWindow.activeSelf) //���ʂ��t�^���ꂽ��
        {
            effectDescription();
        }

        //�琬�����\����Ń^�b�v������
        if (Input.GetMouseButtonDown(0) && resetFadeText.activeSelf) 
        {
            resetCheck = false;
            ResetWindowScene("ResetHome");
            if (fade.IsFadeCompleted)
            {
                fade.IsFadeCompleted = false;
            }
        }
    }
    //���Z�b�g�̃{�^���ݒ�
    public void ResetWindowScene(string str)
    {
        switch (str)
        {
            case "Reset": //�琬����
                PlayerDataManager.TraningReset();
                Debug.Log("�琬����");
                resetCheck = true;
                checkWindow.SetActive(false);
                fade.EyeClose();
                break;
            case "ResetHome": //�z�[����ʂɖ߂�
                StartCoroutine(fadeClose());
                break;
            default:
                SceneLoader.LoadFade("TitleScene");
                break;
        }
    }
    //�琬����������
    private void fadeBool()
    {
        if (fade.IsFadeCompleted)
        {
            fade.IsFadeCompleted = false;
            resetWindow.SetActive(false);
            resetFadeText.SetActive(true);
            fadeText.text = strReset.ToString();
        }
    }
    //���ʂ̃e�L�X�g�\��
    private void effectDescription()
    {
        strplus = true;
        strEffect = PlayerDataManager.GetResetEffects();
        effectText.text = strEffect.ToString();
    }
    private IEnumerator fadeClose()
    {
        fade.EyeOpen();
        yield return new WaitUntil(()=>fade.IsFadeCompleted);
        fade.IsFadeCompleted=false;
    }
}
