using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonController : MonoBehaviour
{
    private SoundController soundController;

    void Start()
    {
        soundController = FindObjectOfType<SoundController>();
    }

    public void PlaySE()
    {
        if (!soundController) return;

        soundController.PlayTap2SE();
    }
}
