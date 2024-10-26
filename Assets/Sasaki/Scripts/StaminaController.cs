using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.UI;

public class StaminaController : MonoBehaviour
{
    [SerializeField,Header("�X���C�_�[�o�[")] private Slider slider;
    [SerializeField,Header("�X�^�~�i�\���̃e�L�X�g")] private Text staminaText;
    private StaminaManager staminaManager;

    private int stamina;
    private int staminaMax;

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
    }
}
