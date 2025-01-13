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
    [SerializeField] private GameObject menuButton;

    [SerializeField] CinemachineVirtualCamera defaultCamera;
    [SerializeField] CinemachineVirtualCamera bossUpCamera;

    // ボス　寄り演出
    [SerializeField] private GameObject warning;
    [SerializeField, Header("ボスにカメラ寄る長さ(秒)")] private float bossUpTime = 0.5f;
    bool isCompBossUp = false;
    [SerializeField, Header("UIがスライドイン完了するまでの時間(秒)")] private float UISlideInTime = 0.5f;
    Vector3 defaultPos_UpUI;
    Vector3 defaultPos_MenuButton;
    Vector3 defaultPos_Commands;
    [SerializeField] Vector3 initPos_UpUI;
    [SerializeField] Vector3 initPos_MenuButton;
    [SerializeField] Vector3 initPos_Commands;
    float moveSpeed_UpUI;
    float moveSpeed_MenuButton;
    float moveSpeed_Commands;

    /// <summary>
    /// バトル開始時の演出が全て完了しているかどうか
    /// </summary>
    [HideInInspector] public bool isCompleteStartDirection = false;

    // 画面揺れ
    [SerializeField] private CinemachineImpulseSource impulseSource;
    [SerializeField] private Vector3 normalDamage;
    [SerializeField] private Vector3 absolutelyDamage;

    // カットイン
    [SerializeField] private GameObject cutIn;

    // ゲーム終了
    [SerializeField] private GameObject winText;
    [SerializeField] private GameObject loseText;

    SoundController soundController;

    void Start()
    {
        soundController = FindObjectOfType<SoundController>();

        warning.SetActive(false);
        winText.SetActive(false);
        loseText.SetActive(false);
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
    /// ボス戦開始演出用意
    /// </summary>
    void ReadyBossStartDirection()
    {
        defaultPos_Commands = commandsObj.transform.localPosition;
        defaultPos_UpUI = upUI.transform.localPosition;
        defaultPos_MenuButton = menuButton.transform.localPosition;

        commandsObj.transform.localPosition = initPos_Commands;
        upUI.transform.localPosition = initPos_UpUI;
        menuButton.transform.localPosition = initPos_MenuButton;

        moveSpeed_Commands = (defaultPos_Commands.x - initPos_Commands.x) / UISlideInTime;
        moveSpeed_UpUI = (defaultPos_UpUI.y - initPos_UpUI.y) / UISlideInTime;
        moveSpeed_MenuButton = (defaultPos_MenuButton.y - initPos_MenuButton.y) / UISlideInTime;
    }

    /// <summary>
    /// ボス戦開始演出
    /// </summary>
    /// <returns></returns>
    public IEnumerator BossStart()
    {
        yield return new WaitForSecondsRealtime(0.5f);

        defaultCamera.Priority = 1;
        bossUpCamera.Priority = 100;

        warning.SetActive(true);

        yield return new WaitForSecondsRealtime(bossUpTime);

        defaultCamera.Priority = 100;
        bossUpCamera.Priority = 1;
        
        yield return new WaitForSecondsRealtime(2.0f);

        float t = 0;
        while (t < UISlideInTime)
        {
            t += Time.unscaledDeltaTime;

            var cPos = commandsObj.transform.localPosition;
            cPos.x += moveSpeed_Commands * Time.unscaledDeltaTime;
            commandsObj.transform.localPosition = cPos;

            var uiPos = upUI.transform.localPosition;
            uiPos.y += moveSpeed_UpUI * Time.unscaledDeltaTime;
            upUI.transform.localPosition = uiPos;

            var menuPos = menuButton.transform.localPosition;
            menuPos.y += moveSpeed_MenuButton * Time.unscaledDeltaTime;
            menuButton.transform.localPosition = menuPos;

            yield return null;
        }

        commandsObj.transform.localPosition = defaultPos_Commands;
        upUI.transform.localPosition = defaultPos_UpUI;
        menuButton.transform.localPosition = defaultPos_MenuButton;

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
        soundController.PlayGameOverJingle();
        loseText.SetActive(true);
    }

    public void Clear()
    {
        soundController.PlayClearJingle();
        winText.SetActive(true);
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
}