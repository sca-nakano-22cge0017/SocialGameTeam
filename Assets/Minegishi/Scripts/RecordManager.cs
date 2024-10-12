using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RecordManager : MonoBehaviour
{
    [SerializeField] GameObject saveWindow1;
    [SerializeField] GameObject saveWindow2;
    [SerializeField] GameObject saveWindow3;
    [SerializeField] GameObject saveWindow4;

    bool WindowClick = false;

    void Start()
    {
        
    }

    void Update()
    {
        if (WindowClick)
        {
            if(Input.GetMouseButtonDown(0))
            {
                saveWindow3.SetActive(true);
            }
        }
    }

    public void BuckButton()
    {
        SceneManager.LoadScene("HomeScene");
    }

    public void NoButton()
    {
        saveWindow1.SetActive(false);
        saveWindow2.SetActive(false);
        saveWindow3.SetActive(false);
        saveWindow4.SetActive(false);
    }

    public void YesButton1()
    {
        saveWindow2.SetActive(true);
        WindowClick = true;
    }

    public void YesButton2()
    {
        saveWindow1.SetActive(false);
        saveWindow2.SetActive(false);
        saveWindow3.SetActive(false);
        saveWindow4.SetActive(false);
    }
}
