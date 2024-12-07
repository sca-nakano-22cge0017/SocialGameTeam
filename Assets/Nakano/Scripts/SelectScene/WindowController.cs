using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class WindowController : MonoBehaviour
{
    [SerializeField] private GameObject m_window;
    [SerializeField, Header("èâä˙èÛë‘")] private bool initialDisp = false;

    void Awake()
    {
        m_window.SetActive(initialDisp);
    }

    public void Open()
    {
        if (!m_window.activeSelf) m_window.SetActive(true);
    }

    public void Close()
    {
        if (m_window.activeSelf) m_window.SetActive(false);
    }

    public void ToHome()
    {
        SceneLoader.LoadFade("HomeScene");
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
    }

    public void ToMain()
    {
        //SceneLoader.LoadScene("Main");
        SceneLoader.LoadFade("MainTest");
    }
}
