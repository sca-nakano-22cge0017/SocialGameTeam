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

    // �{�X�@��艉�o
    [SerializeField, Header("�{�X�ɃJ������钷��(�b)")] private float bossUpTime = 0.5f;
    bool isCompBossUp = false;
    [SerializeField, Header("UI���X���C�h�C����������܂ł̎���(�b)")] private float UISlideInTime = 0.5f;

    /// <summary>
    /// �o�g���J�n���̉��o���S�Ċ������Ă��邩�ǂ���
    /// </summary>
    [HideInInspector] public bool isCompleteStartDirection = false;

    // ��ʗh��
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