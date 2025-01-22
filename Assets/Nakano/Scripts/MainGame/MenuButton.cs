using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuButton : MonoBehaviour
{
    [SerializeField] MainGameSystem mainGameSystem;

    private void OnEnable()
    {
        Time.timeScale = 0;
    }

    public void Back()
    {
        Time.timeScale = mainGameSystem.CurrentSpeedRatio;
    }

    public void ToSelect()
    {
        if (GameManager.SelectArea == 1)
        {
            SceneLoader.LoadFade("SelectScene_Traning");
        }
        if (GameManager.SelectArea == 2)
        {
            SceneLoader.LoadFade("SelectScene_Boss");
        }

        Time.timeScale = 1;
    }

    public void Cancel_ToSelect()
    {
        if (GameManager.SelectArea == 1)
        {
            var staminaManager = FindObjectOfType<StaminaManager>();
            staminaManager.TraningExit();
            SceneLoader.LoadFade("SelectScene_Traning");
        }
        if (GameManager.SelectArea == 2)
        {
            var staminaManager = FindObjectOfType<StaminaManager>();
            staminaManager.BossExit();
            SceneLoader.LoadFade("SelectScene_Boss");
        }

        Time.timeScale = 1;
    }

    public void ToMain()
    {
        SceneLoader.LoadFade("MainTest");

        Time.timeScale = 1;
    }
}
