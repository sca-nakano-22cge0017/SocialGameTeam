using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class MainDirection : MonoBehaviour
{
    [SerializeField] private Canvas commands;

    [SerializeField] private GameObject upUI;
    [SerializeField] private GameObject commandsObj;

    [SerializeField] CinemachineVirtualCamera defaultCamera;
    [SerializeField] CinemachineVirtualCamera bossUpCamera;

    // ボス　寄り演出
    [SerializeField, Header("ボスにカメラ寄る長さ(秒)")] private float bossUpTime = 0.5f;
    bool isCompBossUp = false;
    [SerializeField, Header("UIがスライドイン完了するまでの時間(秒)")] private float UISlideInTime = 0.5f;
    Vector3 defaultPos_UpUI;
    Vector3 defaultPos_Commands;
    [SerializeField] Vector3 initPos_UpUI;
    [SerializeField] Vector3 initPos_Commands;
    float moveSpeed_UpUI;
    float moveSpeed_Commands;

    /// <summary>
    /// バトル開始時の演出が全て完了しているかどうか
    /// </summary>
    [HideInInspector] public bool isCompleteStartDirection = false;

    // 画面揺れ
    [SerializeField] private CinemachineImpulseSource impulseSource;
    [SerializeField] private Vector3 normalDamage;
    [SerializeField] private Vector3 absolutelyDamage;

    void Start()
    {
        if (GameManager.SelectArea == 2)
        {
            defaultCamera.Priority = 1;
            bossUpCamera.Priority = 100;

            ReadyBossStartDirection();
            StartCoroutine(BossStart());
        }
    }

    void Update()
    {
        
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

        commandsObj.transform.localPosition = initPos_Commands;
        upUI.transform.localPosition = initPos_UpUI;

        moveSpeed_Commands = (defaultPos_Commands.x - initPos_Commands.x) / UISlideInTime;
        moveSpeed_UpUI = (defaultPos_UpUI.y - initPos_UpUI.y) / UISlideInTime;
    }

    /// <summary>
    /// ボス戦開始演出
    /// </summary>
    /// <returns></returns>
    public IEnumerator BossStart()
    {
        defaultCamera.Priority = 1;
        bossUpCamera.Priority = 100;

        yield return new WaitForSeconds(bossUpTime);

        bossUpCamera.Priority = 1;
        defaultCamera.Priority = 100;

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
}