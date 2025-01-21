using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BuffObject : MonoBehaviour
{
    /// <summary>
    /// アイコンを複数表示するか
    /// </summary>
    public bool isIconChangeAlternately = false;
    [SerializeField] private Image image;
    private List<Sprite> iconSprites = new();
    private float blinkingCoolTime = 0.0f;
    private float elapsedTime = 0.0f;

    private int num = 0;

    void Start()
    {
        isIconChangeAlternately = false;
        elapsedTime = 0.0f;
    }

    private void OnDisable()
    {
        isIconChangeAlternately = false;
        elapsedTime = 0.0f;
    }

    void Update()
    {
        if (isIconChangeAlternately)
        {
            elapsedTime += Time.unscaledDeltaTime;

            if (elapsedTime >= blinkingCoolTime)
            {
                num++;
                if (num >= iconSprites.Count)
                {
                    num = 0;
                }

                image.sprite = iconSprites[num];
                elapsedTime = 0.0f;
            }
        }
    }

    /// <summary>
    /// アイコンを複数種使う場合交互に表示
    /// </summary>
    /// <param name="_sprites">使用アイコン</param>
    /// <param name="_blinkingCoolTime">次に表示アイコンを変えるまでのクールタイム</param>
    /// <param name="_blinkingFadeTime">フェードする時間</param>
    public void IconChangeAlternately(List<Sprite> _sprites, float _blinkingCoolTime)
    {
        if (isIconChangeAlternately) return;

        for (int i = 0; i < _sprites.Count; i++)
        {
            iconSprites.Add(_sprites[i]);
        }

        isIconChangeAlternately = true;
        blinkingCoolTime = _blinkingCoolTime;
        elapsedTime = 0.0f;

        num = 0;
        image.sprite = iconSprites[num];
    }
}