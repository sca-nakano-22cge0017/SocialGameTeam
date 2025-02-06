using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DebugMode : MonoBehaviour
{
    [SerializeField] Text text;

    private SpecialTecniqueManager specialTecniqueManager;

    private void Start()
    {
        specialTecniqueManager = FindObjectOfType<SpecialTecniqueManager>();

        TextChange();
    }

    public void HyperMode()
    {
        if (GameManager.isHyperTraningMode) GameManager.isHyperTraningMode = false;
        else GameManager.isHyperTraningMode = true;

        //if (GameManager.isHyperTraningMode) specialTecniqueManager.AllRelease();

        TextChange();
    }

    void TextChange()
    {
        if (text == null) return;
        if (GameManager.isHyperTraningMode)
        {
            text.text = "デバッグモード ON";
        }
        else text.text = "デバッグモード OFF";
    }
}
