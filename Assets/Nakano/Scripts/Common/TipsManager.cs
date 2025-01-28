using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TipsManager : MonoBehaviour
{
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private Text tipsText1;
    [SerializeField] private RectTransform rt1;
    [SerializeField] private Text tipsText2;
    [SerializeField] private RectTransform rt2;
    [SerializeField] private Animator anim;

    [SerializeField, TextArea(1, 6)] private string[] tips;

    [SerializeField, Header("次のTipsに自動で切り替わるまでの時間")] private float coolTime;
    [SerializeField, Header("タップのクールタイム")] private float tapCoolTime;

    private bool isSlide = false;
    private bool isSlideComplete = true;
    private float elapsedTime = 0;
    private int currentIndex = 0;

    private void OnEnable()
    {
        elapsedTime = 0;
        isSlide = false;
        isSlideComplete = true;

        TextChangeFirst();
    }

    void Update()
    {
        if (canvasGroup.alpha < 1) return;

        if (isSlideComplete)
        {
            if (Input.touchCount > 0 && elapsedTime >= tapCoolTime)
            {
                Touch touch = Input.GetTouch(0);

                if (touch.phase == TouchPhase.Began)
                {
                    isSlide = true;
                    isSlideComplete = false;
                    elapsedTime = 0;
                }
            }

            elapsedTime += Time.unscaledDeltaTime;
        }

        if (elapsedTime >= coolTime)
        {
            isSlide = true;
            elapsedTime = 0;
        }

        if (isSlide)
        {
            if (rt1.localPosition.x > 0)
            {
                tipsText1.text = TextChange();
            }
            if (rt2.localPosition.x > 0)
            {
                tipsText2.text = TextChange();
            }

            isSlide = false;
            isSlideComplete = false;
            elapsedTime = 0;

            StartCoroutine(SlideComplete());
        }
    }

    IEnumerator SlideComplete()
    {
        yield return new WaitForEndOfFrame();
        anim.SetTrigger("Next");

        yield return new WaitForSecondsRealtime(0.5f);
        isSlideComplete = true;
    }

    void TextChangeFirst()
    {
        List<int> tipsIndex = new();
        for (int i = 0; i < tips.Length; i++)
        {
            tipsIndex.Add(i);
        }

        int rand1 = Random.Range(0, tipsIndex.Count);
        int index1 = tipsIndex[rand1];
        tipsText1.text = tips[index1];
        currentIndex = tipsIndex[rand1];

        tipsIndex.Remove(currentIndex);

        tipsIndex.Clear();
    }

    string TextChange()
    {
        List<int> tipsIndex = new();
        for (int i = 0; i < tips.Length; i++)
        {
            tipsIndex.Add(i);
        }

        tipsIndex.Remove(currentIndex);

        int rand = Random.Range(0, tipsIndex.Count);
        int index = tipsIndex[rand];
        var str = tips[index];
        currentIndex = tipsIndex[rand];

        tipsIndex.Clear();
        return str;
    }
}
