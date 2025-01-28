using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class WindowController : MonoBehaviour
{
    [SerializeField] private GameObject m_window;
    [SerializeField, Header("èâä˙èÛë‘")] private bool initialDisp = false;

    [SerializeField] private Animator windowDirection;

    void Awake()
    {
        m_window.SetActive(initialDisp);
        
        if (m_window.GetComponent<Animator>())
        {
            windowDirection = m_window.GetComponent<Animator>();
        }
    }

    private void Update()
    {
        if (windowDirection)
        {
            if (windowDirection.GetCurrentAnimatorStateInfo(0).IsName("End"))
            {
                if (m_window.activeSelf) m_window.SetActive(false);
            }
        }
    }

    public void Open()
    {
        if (!m_window.activeSelf) m_window.SetActive(true);
    }

    public void Close()
    {
        if (windowDirection) windowDirection.SetTrigger("Close");
        else
        {
            if (m_window.activeSelf) m_window.SetActive(false);
        }
    }

    public void ToHome()
    {
        SceneLoader.LoadFade("HomeScene");
    }
}
