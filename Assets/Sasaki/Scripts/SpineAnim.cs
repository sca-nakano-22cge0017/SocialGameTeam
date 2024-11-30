using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpineAnim : MonoBehaviour
{
    public delegate void CallBack();
    public CallBack callBack;
    [SerializeField] float buffTime;
    [SerializeField] float attackTime;


    [SerializeField] Animator anim;

    float animAttackSpeed;
    float animBuffSpeed;
    //アタック
    public void PlayAttackMotion()
    {
        anim.SetTrigger("attack");
        animAttackSpeed = anim.GetFloat("attackSpeed");
        StartCoroutine(attackCoroutine());
    }
    IEnumerator attackCoroutine()
    {
        yield return new WaitForSeconds(attackTime / animAttackSpeed);
        callBack();
    }
    //バフ
    public void PlaybuffMotion()
    {
        anim.SetTrigger("buff");
        animBuffSpeed = anim.GetFloat("buffSpeed");
        StartCoroutine(buffCoroutine());
    }
    IEnumerator buffCoroutine()
    {
        yield return new WaitForSeconds(buffTime / animBuffSpeed);
        callBack();
    }
}
