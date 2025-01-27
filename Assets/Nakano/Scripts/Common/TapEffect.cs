using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TapEffect : MonoBehaviour
{
    [SerializeField] GameObject parent;
    [SerializeField] ParticleSystem tapEffect;
    List<ParticleSystem> effects = new();

    void Start()
    {
        effects.Clear();
    }

    void Update()
    {
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);

            if (touch.phase == TouchPhase.Began)
            {
                Vector3 worldPos = Camera.main.ScreenToWorldPoint(touch.position);
                worldPos.z = 10;

                GameObject eff = null;

                int canPlay = CheckPlayableEffects();
                if (canPlay >= 0)
                {
                    eff = effects[canPlay].gameObject;
                    eff.transform.position = worldPos;
                }
                else
                {
                    eff = Instantiate(tapEffect.gameObject, worldPos, Quaternion.identity, parent.transform);
                    effects.Add(eff.GetComponent<ParticleSystem>());
                }

                // カメラのサイズに応じて大きさ調整
                if (SceneManager.GetActiveScene().name == "MainTest")
                {
                    var defaultScale = 1.5f;
                    var currentCameraSize = Camera.main.orthographicSize;
                    float defaultCameraSize = 6.2f;
                    eff.transform.localScale = new Vector3(defaultScale * (currentCameraSize / defaultCameraSize), defaultScale * (currentCameraSize / defaultCameraSize), defaultScale);
                }

                eff.GetComponent<ParticleSystem>().Play();
            }
        }
    }

    int CheckPlayableEffects()
    {
        // オブジェクト自体が一つもない
        if (effects.Count == 0) return -1;

        for (int i = 0; i < effects.Count; i++)
        {
            // 再生中でないものがある
            if (!effects[i].isPlaying) return i;
        }

        return -1;
    }
}
