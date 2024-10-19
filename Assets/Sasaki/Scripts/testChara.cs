using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class testChara : MonoBehaviour
{
    [SerializeField] private GameObject[] chara;

    int charaNumSelect;
    // Start is called before the first frame update
    void Start()
    {
        charaNumSelect = GameManager.SelectChara;
    }

    // Update is called once per frame
    void Update()
    {
        if (charaNumSelect == 1)
        {
            chara[0].SetActive(true);
            chara[1].SetActive(false);
        }
        if (charaNumSelect == 2)
        {
            chara[1].SetActive(true);
            chara[0].SetActive(false);
        }
    }
}
