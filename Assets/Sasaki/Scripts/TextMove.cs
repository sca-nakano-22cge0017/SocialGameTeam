using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TextMove : MonoBehaviour
{
    [SerializeField, Header("フェードさせる時間")] private float fadeTime = 1f;
    private CanvasGroup img = null;

    private bool fadeCheck = false;
    // Start is called before the first frame update
    void Start()
    {
        img = GetComponent<CanvasGroup>();
        img.alpha = 0;
    }

    // Update is called once per frame
    void Update()
    {
        if (!fadeCheck)
        {
            img.alpha += fadeTime * Time.deltaTime;

            if (img.alpha == 1)
            {
                fadeCheck= true;
            }
        }
        else
        {
            img.alpha -= fadeTime * Time.deltaTime;
            if (img.alpha == 0)
            {
                fadeCheck = false;
            }
        }
    }
}
