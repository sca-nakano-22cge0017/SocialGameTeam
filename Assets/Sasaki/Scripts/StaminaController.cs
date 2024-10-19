using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.UI;

public class StaminaController : MonoBehaviour
{
    [SerializeField] private Slider slider;
    [SerializeField] private Text LevelText;
    [SerializeField] private Text staminaText;
    private StaminaManager staminaManager;

    // Start is called before the first frame update
    void Start()
    {
        staminaManager = FindObjectOfType<StaminaManager>();
        if (staminaManager == null)
        {
            GameManager.GameManagerCreate();
            GameManager gameManager;
            gameManager = GameManager.Instance;
            staminaManager = gameManager.gameObject.GetComponent<StaminaManager>();
        }
    }

    // Update is called once per frame
    void Update()
    {
        staminaText.text = staminaManager.Stamina.ToString() + "/" + staminaManager.Stamina_Max.ToString();
    }
}
