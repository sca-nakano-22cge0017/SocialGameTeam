using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

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
        SceneManager.LoadScene("HomeScene");
    }
}
