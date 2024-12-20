using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DebugMode : MonoBehaviour
{
    [SerializeField] Text text;

    private void Start()
    {
        TextChange();
    }

    public void HyperMode()
    {
        if (GameManager.isHyperTraningMode) GameManager.isHyperTraningMode = false;
        else GameManager.isHyperTraningMode = true;

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
