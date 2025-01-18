using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RankIcon : MonoBehaviour
{
    [SerializeField] Image icon;

    [SerializeField] Sprite rankD;
    [SerializeField] Sprite rankC;
    [SerializeField] Sprite rankB;
    [SerializeField] Sprite rankA;
    [SerializeField] Sprite rankS;
    [SerializeField] Sprite rankSS;

    public void RankIconChange(string _rank)
    {
        switch(_rank)
        {
            case "D":
                icon.sprite = rankD;
                break;
            case "C":
                icon.sprite = rankC;
                break;
            case "B":
                icon.sprite = rankB;
                break;
            case "A":
                icon.sprite = rankA;
                break;
            case "S":
                icon.sprite = rankS;
                break;
            case "SS":
                icon.sprite = rankSS;
                break;
        }
    }

    public void RankIconChange(Rank _rank)
    {
        switch (_rank)
        {
            case Rank.D:
                icon.sprite = rankD;
                break;
            case Rank.C:
                icon.sprite = rankC;
                break;
            case Rank.B:
                icon.sprite = rankB;
                break;
            case Rank.A:
                icon.sprite = rankA;
                break;
            case Rank.S:
                icon.sprite = rankS;
                break;
            case Rank.SS:
                icon.sprite = rankSS;
                break;
        }
    }
}
