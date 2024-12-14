using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChangeSkin : MonoBehaviour
{
    [SerializeField] private WindowController confirmWindow;

    [SerializeField] private WindowController guageWindow;
    [SerializeField] private Image guage1;
    [SerializeField] private Image guage2;

    // 選択中の複合ランク
    CombiType selectType = CombiType.NORMAL;

    void Start()
    {
        
    }

    void Update()
    {
        
    }

    /// <summary>
    /// 差分解放済みかチェック
    /// </summary>
    /// <param name="_type"></param>
    public void ReleaseCheck(string _type)
    {
        var type = (CombiType)System.Enum.Parse(typeof(CombiType), _type);
        selectType = type;

        switch (type)
        {
            case CombiType.ATK:
                if (PlayerDataManager.player.atkTypeReleased)
                {
                    confirmWindow.Open();
                }
                else DispGuage();
                break;
            case CombiType.DEF:
                if (PlayerDataManager.player.defTypeReleased)
                {
                    confirmWindow.Open();
                }
                else DispGuage();
                break;
            case CombiType.TEC:
                if (PlayerDataManager.player.tecTypeReleased)
                {
                    confirmWindow.Open();
                }
                else DispGuage();
                break;
        }
    }

    /// <summary>
    /// スキン変更
    /// </summary>
    public void Change()
    {
        PlayerDataManager.player.SetSelectEvolutionType(selectType);
    }

    /// <summary>
    /// ゲージ表示
    /// </summary>
    void DispGuage()
    {
        float max = PlayerDataManager.player.GetCombiRankPtMax(selectType);

        switch (selectType)
        {
            case CombiType.ATK:
                float atk = (float)PlayerDataManager.player.GetRankPt(StatusType.ATK);
                float mp = (float)PlayerDataManager.player.GetRankPt(StatusType.MP);
                guage1.fillAmount = atk / max;
                guage2.fillAmount = (atk + mp) / max;

                Debug.Log($"test atk:{atk}, mp:{mp}, max:{max}, {atk / max}, {(atk + mp) / max}");
                break;
            case CombiType.DEF:
                float hp = (float)PlayerDataManager.player.GetRankPt(StatusType.HP);
                float def = (float)PlayerDataManager.player.GetRankPt(StatusType.DEF);
                guage1.fillAmount = hp / max;
                guage2.fillAmount = (hp + def) / max;

                Debug.Log($"test hp:{hp}, def:{def}, max:{max}, {hp / max}, {(hp + def) / max}");
                break;
            case CombiType.TEC:
                float dex = (float)PlayerDataManager.player.GetRankPt(StatusType.DEX);
                float agi = (float)PlayerDataManager.player.GetRankPt(StatusType.AGI);
                guage1.fillAmount = dex / max;
                guage2.fillAmount = (dex + agi) / max;

                Debug.Log($"test dex:{dex}, agi:{agi}, max:{max}, {dex/max}, {(dex + agi) / max}");
                break;
        }

        guageWindow.Open();
    }
}
