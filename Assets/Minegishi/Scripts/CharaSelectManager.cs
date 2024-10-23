using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class CharaSelectManager : MonoBehaviour
{
     [SerializeField] Button[] buttonNum = new Button[8];

    [SerializeField] GameObject[] charaImage = new GameObject[2];

    void Start()
    {

    }

    void Update()
    {
        
    }

    void SwordsManTrue()
    {
        charaImage[0].SetActive(true);
        charaImage[1].SetActive(false);
    }

    void WizardTrue()
    {
        charaImage[0].SetActive(false);
        charaImage[1].SetActive(true);
    }


    public void CharaButton1()
    {
        SwordsManTrue();
    }

    public void CharaButton2()
    {
        WizardTrue();
    }
}
