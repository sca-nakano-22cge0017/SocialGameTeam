using Spine.Unity.Examples;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class SpineAnimation : MonoBehaviour
{
    float time;
    bool t = false;
    [SerializeField] private Animator anim;
    private AnimatorStateInfo stateInfo;

    // Update is called once per frame
    void Update()
    {
        time += Time.time;
        if (anim)
        {
            time = 0;
            t = true;

        }
        if (t)
        {
            Debug.Log(time);
        }
    }
}
