using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class MainDirection : MonoBehaviour
{
    [SerializeField] private Canvas commands;

    [SerializeField] private GameObject elapsedTurnObj;
    [SerializeField] private GameObject menuButtonObj;
    [SerializeField] private GameObject commandsObj;

    [SerializeField] CinemachineVirtualCamera defaultCamera;
    [SerializeField] CinemachineVirtualCamera bossUpCamera;

    // ボス　寄り演出
    [SerializeField, Header("ボスにカメラ寄る長さ(秒)")] private float bossUpTime = 0.5f;
    bool isCompBossUp = false;
    [SerializeField, Header("UIがスライドイン完了するまでの時間(秒)")] private float UISlideInTime = 0.5f;

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

    public IEnumerator BossStart()
    {
        defaultCamera.Priority = 1;
        bossUpCamera.Priority = 100;

        yield return new WaitForSeconds(bossUpTime);

        bossUpCamera.Priority = 1;
        defaultCamera.Priority = 100;

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