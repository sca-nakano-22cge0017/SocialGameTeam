using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StopWatch : MonoBehaviour
{
    [SerializeField] private Text timeText;
    float seconds;
    float t;
    private void Start()
    {
        seconds = 0.0f;
    }
    private void Update()
    {
        seconds += Time.deltaTime;

        timeText.text = ($"{seconds.ToString("00.0")}");
    }

    public void StopButton()
    {
        Time.timeScale = 0.0f;
    }
    public void StartButton()
    {
        Time.timeScale = 1.0f;
    }
}
