using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class Result
{
    public int getPoint;
    public Image guage;
    public string rank;
}

public class ResultManager : MonoBehaviour
{
    [SerializeField] private Result results;

    void Start()
    {
        
    }

    void Update()
    {
        
    }
}
