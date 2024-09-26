using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WindowController : MonoBehaviour
{
    [SerializeField] private GameObject m_window;
    [SerializeField, Header("èâä˙èÛë‘")] private bool initialDisp = false;

    void Start()
    {
        m_window.SetActive(initialDisp);
    }

    void Update()
    {
        
    }

    public void Open()
    {
        if (!m_window.activeSelf) m_window.SetActive(true);
    }

    public void Close()
    {
        if (m_window.activeSelf) m_window.SetActive(false);
    }
}
