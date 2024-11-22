using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.UI;

public class StaminaController : MonoBehaviour
{
    [SerializeField,Header("�X���C�_�[�o�[")] private Image slider;
    [SerializeField,Header("�X�^�~�i�\���̃e�L�X�g")] private Text staminaText;
    [SerializeField,Header("�X�^�~�i�̎��ԕ\���E�B���h�E")] private GameObject staminaWindow;
    [SerializeField,Header("�X�^�~�i�̎��ԃe�L�X�g")] private Text staminaTimeText;
    private StaminaManager staminaManager;

    private int stamina;
    private int staminaMax;
    private string staminaTime; //1�񕜂̎���

    int tapNum = 0; //��������

    // Start is called before the first frame update
    void Start()
    {
        staminaWindow.SetActive(false);
        staminaManager = FindObjectOfType<StaminaManager>();
        //staminaManager��null����������
        if (staminaManager == null)
        {
            GameManager.GameManagerCreate();
            GameManager gameManager;
            gameManager = GameManager.Instance;
            staminaManager = gameManager.gameObject.GetComponent<StaminaManager>();
        }

        staminaMax = staminaManager.Stamina_Max;

        slider.fillAmount = 1; //�X���C�_�[���^��
    }

    // Update is called once per frame
    void Update()
    {
        stamina = staminaManager.Stamina;
        slider.fillAmount = (float)stamina/(float)staminaMax;//�o�[�\�����f
        //�e�L�X�g�\��
        staminaText.text = stamina.ToString() + "/" + staminaMax.ToString();

        //�^�C���\��
        if (staminaWindow.activeSelf)
        {
            staminaTime = staminaManager.RecoveryTimeText;
            staminaTimeText.text = "���̉񕜂܂Ŏc��" + staminaTime.ToString();
        }

        if (Input.GetMouseButtonDown(0) && staminaWindow.activeSelf) 
        {
            StaminaHidden();
        }
    }

    //�X�^�~�i�̎��Ԃ�\�����邽�߂̃{�^��
    public void OnStaminaButton()
    {
        staminaWindow.SetActive(true);
    }

    //���ԕ\���̃v���O����
    void StaminaHidden()
    {
        staminaWindow.SetActive(false);
    }
}
