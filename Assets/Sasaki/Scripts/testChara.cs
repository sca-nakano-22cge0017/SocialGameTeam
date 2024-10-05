using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class testChara : MonoBehaviour
{
    [SerializeField] private GameObject[] chara;
    CharaSelect charaSelect = null;
    int charaNumSelect;
    // Start is called before the first frame update
    void Start()
    {
        charaSelect = GetComponent<CharaSelect>();
        charaNumSelect = charaSelect.charaNum;
        for (int i = 0; i < chara.Length; i++)
        {
            chara[i].SetActive(false);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (charaNumSelect == 1)
        {
            chara[0].SetActive(true);
        }
        if (charaNumSelect == 2)
        {
            chara[1].SetActive(true);
        }
    }
}
