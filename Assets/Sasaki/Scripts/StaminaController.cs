using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.UI;

public class StaminaController : MonoBehaviour
{
    [SerializeField,Header("�X���C�_�[�o�[")] private Slider slider;
    [SerializeField,Header("�X�^�~�i�\���̃e�L�X�g")] private Text staminaText;
    [SerializeField,Header("�X�^�~�i�̎��ԕ\���E�B���h�E")] private GameObject staminaWindow;
    [SerializeField,Header("�X�^�~�i�̎��ԃe�L�X�g")] private Text staminaTimeText;
    [SerializeField,Header("�X�^�~�i�̎��ԕ\�������b�\�����邩")] private float stopTime = 3.0f;
    private StaminaManager staminaManager;

    private int stamina;
    private int staminaMax;
    private string staminaTime; //i�񕜂̎���

    // Start is called before the first frame update
    void Start()
    {
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

        slider.value = 1; //�X���C�_�[���^��
    }

    // Update is called once per frame
    void Update()
    {
        stamina = staminaManager.Stamina;
        slider.value = (float)stamina/(float)staminaMax;//�o�[�\�����f
        //�e�L�X�g�\��
        staminaText.text = stamina.ToString() + "/" + staminaMax.ToString();

        //�^�C���\��
        if (staminaWindow.activeSelf)
        {
            staminaTime = staminaManager.RecoveryTimeText;
            staminaTimeText.text = "�c��" + staminaTime.ToString();
        }
    }

    //�X�^�~�i�̎��Ԃ�\�����邽�߂̃{�^��
    public void OnStaminaButton()
    {
        staminaWindow.SetActive(true);
        StartCoroutine(StaminaTimeImage()); //���ԕ\��
    }

    //���ԕ\���̃v���O����
    private IEnumerator StaminaTimeImage()
    {
        yield return new WaitForSeconds(stopTime);
        staminaWindow.SetActive(false);
    }
}
