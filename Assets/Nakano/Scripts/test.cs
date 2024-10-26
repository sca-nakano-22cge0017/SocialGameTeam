using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class test : MonoBehaviour
{
    [SerializeField] MainGameGuage one;
    [SerializeField] MainGameGuage two;

    void Start()
    {
        one.Initialize(300);
        two.Initialize(500);
    }

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.P))
        {
            one.Sub(100);
        }
        if (Input.GetKeyDown(KeyCode.O))
        {
            two.Sub(150);
        }
    }
}
