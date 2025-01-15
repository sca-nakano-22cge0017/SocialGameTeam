using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SpecialFade : MonoBehaviour
{
    [SerializeField, Header("フェードさせる時間")] private float fadeTime;
    [SerializeField, Header("初期サイズ")] private Vector3 defaultSize;

    [SerializeField] GameObject fadeImage;
    [SerializeField] GameObject unmask;
    [SerializeField] GameObject bg;

    private bool isClose = false;
    private bool isOpen = false;
    private bool isFadeCompleted = false;
    /// <summary>
    /// フェード完了したか
    /// </summary>
    public bool IsFadeCompleted { get => isFadeCompleted; }

    void Start()
    {
        isClose = false;
        isOpen = false;
        isFadeCompleted = false;

        unmask.transform.localScale = defaultSize;
        bg.transform.localScale = defaultSize;

        fadeImage.SetActive(false);
    }

    void Update()
    {
        if (isClose)
        {
            var unmaskTF = unmask.transform.localScale;
            var bgTF = bg.transform.localScale;

            unmaskTF.y -= Time.deltaTime / fadeTime;
            bgTF.y -= Time.deltaTime / fadeTime;

            unmask.transform.localScale = unmaskTF;
            bg.transform.localScale = bgTF;

            if (unmaskTF.y <= 0)
            {
                unmask.transform.localScale = Vector3.zero;
                bg.transform.localScale = Vector3.zero;

                isClose = false;
                isFadeCompleted = true;
            }
        }

        if (isOpen)
        {
            var unmaskTF = unmask.transform.localScale;
            var bgTF = bg.transform.localScale;

            unmaskTF.y += Time.deltaTime / fadeTime;
            bgTF.y += Time.deltaTime / fadeTime;

            unmask.transform.localScale = unmaskTF;
            bg.transform.localScale = bgTF;

            if (unmaskTF.y >= defaultSize.y)
            {
                unmask.transform.localScale = defaultSize;
                bg.transform.localScale = defaultSize;

                isOpen = false;
                isFadeCompleted = true;
                fadeImage.SetActive(false);
            }
        }
    }

    public void EyeClose()
    {
        isClose = true;

        unmask.transform.localScale = defaultSize;
        bg.transform.localScale = defaultSize;

        fadeImage.SetActive(true);
    }

    public void EyeOpen()
    {
        isOpen = true;

        unmask.transform.localScale = new Vector3(defaultSize.x, 0, 1);
        bg.transform.localScale = new Vector3(defaultSize.x, 0, 1);

        fadeImage.SetActive(true);
    }
}
