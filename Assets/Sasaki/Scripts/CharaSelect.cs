using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class CharaNum
{
    // �L�����N�^�[�̕ϐ�
    //1=>�V�X�^�[ 2=>���m
    public static int selectChara = 0;
}

public class CharaSelect : MonoBehaviour
{
    [SerializeField,Header("�L�����I�u�W�F�N�g")] private GameObject CharaObjects = null;
    private TutorialWindow tutorialWindow;

    private void Awake()
    {
        CharaObjects.SetActive(false);
        StartCoroutine(Loader());
    }
    private void Start()
    {
        tutorialWindow = FindObjectOfType<TutorialWindow>();
        tutorialWindow.CharaSelect();
    }
    //�ǂ̃L�����N�^�[��I�񂾂�
    //�I�����ꂽ�L�����Ƃ��̈琬�󋵂ɂ���ĕς��X�v���C�g���p�ł���v���O���������
    //window��\��
    public void CharaOnClick(string name)
    {
        int c;
        switch (name)
        {
            case "SwordsWoman":
                c = 1;
                break;
            case "Sister":
                c = 2;
                break;
            default:
                c = -1;
                break;
        }
        GameManager.SelectChara = c;
        GameManager.isFirstStart = false;
        PlayerDataManager.Save();
        CharaObjects.SetActive(false);
        tutorialWindow.Home();
    }
    IEnumerator Loader()
    {
        yield return new WaitUntil(() => MasterDataLoader.MasterDataLoadComplete);

        yield return new WaitUntil(() => PlayerDataManager.PlayerDataLoadComplete);

        if (GameManager.isFirstStart)
        {
            CharaObjects.SetActive(true);
        }
        else
        {
            CharaObjects.SetActive(false);
        }
    }
}
