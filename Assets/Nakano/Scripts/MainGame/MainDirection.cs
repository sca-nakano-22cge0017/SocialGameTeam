using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using System;

public class MainDirection : MonoBehaviour
{
    [SerializeField] private Canvas commands;

    [SerializeField] private GameObject upUI;
    [SerializeField] private GameObject commandsObj;

    [SerializeField] CinemachineVirtualCamera defaultCamera;
    [SerializeField] CinemachineVirtualCamera bossUpCamera;

    // �{�X�@��艉�o
    [SerializeField] private GameObject warning;
    [SerializeField, Header("�{�X�ɃJ������钷��(�b)")] private float bossUpTime = 0.5f;
    bool isCompBossUp = false;
    [SerializeField, Header("UI���X���C�h�C����������܂ł̎���(�b)")] private float UISlideInTime = 0.5f;
    Vector3 defaultPos_UpUI;
    Vector3 defaultPos_Commands;
    [SerializeField] Vector3 initPos_UpUI;
    [SerializeField] Vector3 initPos_Commands;
    float moveSpeed_UpUI;
    float moveSpeed_Commands;

    /// <summary>
    /// �o�g���J�n���̉��o���S�Ċ������Ă��邩�ǂ���
    /// </summary>
    [HideInInspector] public bool isCompleteStartDirection = false;

    // ��ʗh��
    [SerializeField] private CinemachineImpulseSource impulseSource;
    [SerializeField] private Vector3 normalDamage;
    [SerializeField] private Vector3 absolutelyDamage;

    // �J�b�g�C��
    [SerializeField] private GameObject cutIn;

    // �Q�[���I��
    [SerializeField] private GameObject gameOverText;
    [SerializeField] private GameObject clearText;

    SoundController soundController;

    void Start()
    {
        soundController = FindObjectOfType<SoundController>();

        warning.SetActive(false);
        gameOverText.SetActive(false);
        clearText.SetActive(false);
        cutIn.SetActive(false);

        if (GameManager.SelectArea == 1)
        {
            isCompleteStartDirection = true;
        }
        if (GameManager.SelectArea == 2)
        {
            defaultCamera.Priority = 1;
            bossUpCamera.Priority = 100;

            ReadyBossStartDirection();
        }
    }

    void DirectionCompleteCheck()
    {
        if (!isCompBossUp) return;

        isCompleteStartDirection = true;
    }

    /// <summary>
    /// �{�X��J�n���o�p��
    /// </summary>
    void ReadyBossStartDirection()
    {
        defaultPos_Commands = commandsObj.transform.localPosition;
        defaultPos_UpUI = upUI.transform.localPosition;

        commandsObj.transform.localPosition = initPos_Commands;
        upUI.transform.localPosition = initPos_UpUI;

        moveSpeed_Commands = (defaultPos_Commands.x - initPos_Commands.x) / UISlideInTime;
        moveSpeed_UpUI = (defaultPos_UpUI.y - initPos_UpUI.y) / UISlideInTime;
    }

    /// <summary>
    /// �{�X��J�n���o
    /// </summary>
    /// <returns></returns>
    public IEnumerator BossStart()
    {
        yield return new WaitForSeconds(0.5f);

        defaultCamera.Priority = 1;
        bossUpCamera.Priority = 100;

        warning.SetActive(true);

        yield return new WaitForSeconds(bossUpTime);

        defaultCamera.Priority = 100;
        bossUpCamera.Priority = 1;
        
        yield return new WaitForSeconds(2.0f);

        float t = 0;
        while (t < UISlideInTime)
        {
            t += Time.deltaTime;

            var cPos = commandsObj.transform.localPosition;
            cPos.x += moveSpeed_Commands * Time.deltaTime;
            commandsObj.transform.localPosition = cPos;

            var uiPos = upUI.transform.localPosition;
            uiPos.y += moveSpeed_UpUI * Time.deltaTime;
            upUI.transform.localPosition = uiPos;

            yield return null;
        }

        commandsObj.transform.localPosition = defaultPos_Commands;
        upUI.transform.localPosition = defaultPos_UpUI;

        isCompBossUp = true;
        DirectionCompleteCheck();
    }

    public void DamageImpulse()
    {
        commands.renderMode = RenderMode.ScreenSpaceCamera;

        impulseSource.m_DefaultVelocity = normalDamage;
        impulseSource.GenerateImpulse();
    }

    public void AbsolutelyImpulse()
    {
        commands.renderMode = RenderMode.WorldSpace;

        impulseSource.m_DefaultVelocity = absolutelyDamage;
        impulseSource.GenerateImpulse();
    }

    public void GameOver()
    {
        gameOverText.SetActive(true);
    }

    public void Clear()
    {
        clearText.SetActive(true);
    }

    public IEnumerator CutIn(Action _action)
    {
        var animator = cutIn.GetComponent<Animator>();
        cutIn.SetActive(true);

        yield return null;

        yield return new WaitUntil(() => animator.GetCurrentAnimatorStateInfo(0).IsName("End"));

        yield return null;

        cutIn.SetActive(false);
        _action?.Invoke();
    }

    public void TapSE()
    {
        soundController.PlayTap1SE();
    }
}