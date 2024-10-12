using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AbilityManager : MonoBehaviour
{
    [SerializeField] GameObject abilityText;

    void Start()
    {
        
    }

    void Update()
    {
        
    }

    public void BuckButton()
    {
        SceneManager.LoadScene("HomeScene");
    }

    public void ButtonDown()
    {
        abilityText.SetActive(true);
    }

    public void ButtonUp()
    {
        abilityText.SetActive(false);
    }
}
