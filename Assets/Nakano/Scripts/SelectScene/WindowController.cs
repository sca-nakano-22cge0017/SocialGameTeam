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
        SceneLoader.LoadScene("HomeScene");
    }

    public void ToSelect()
    {
        if (GameManager.SelectArea == 1)
        {
            SceneLoader.LoadScene("SelectScene_Traning");
        }
        if (GameManager.SelectArea == 2)
        {
            SceneLoader.LoadScene("SelectScene_Boss");
        }
    }

    public void ToMain()
    {
        //SceneLoader.LoadScene("Main");
        SceneLoader.LoadScene("MainTest");
    }
}
