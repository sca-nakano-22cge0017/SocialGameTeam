using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Example : MonoBehaviour
{
    [SerializeField,Header("�{�^��")] private Image[] image = null;
    private void Awake()
    {
        for (int i = 0; i < image.Length; i++)
        {
            image[i].alphaHitTestMinimumThreshold = 1;
        }
    }
}
