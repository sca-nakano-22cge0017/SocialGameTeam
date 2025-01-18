using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChangeSkin : MonoBehaviour
{
    [SerializeField] private Image[] icons;
    [SerializeField] private Sprite[] iconCharaA;
    [SerializeField] private Sprite[] iconCharaB;

    [SerializeField] private WindowController confirmWindow;

    [SerializeField] private WindowController guageWindow;
    [SerializeField] private Image guage1;
    [SerializeField] private Image guage2;
    [SerializeField] private Text explain;
    [SerializeField] private Text restText;

    [SerializeField] private WindowController cantReleaseWindow;
    [SerializeField] private WindowController selectingWindow;

    // �I�𒆂̕��������N
    CombiType selectType = CombiType.NORMAL;

    private void OnEnable()
    {
        for (int i = 0; i < icons.Length; i++)
        {
            if (GameManager.SelectChara == 1)
            {
                icons[i].sprite = iconCharaA[i];
            }
            else if(GameManager.SelectChara == 2)
            {
                icons[i].sprite = iconCharaB[i];
            }
        }

        // ������̈ߑ��̓V���G�b�g�\��
        icons[0].color = PlayerDataManager.player.atkTypeReleased ? Color.white : Color.black;
        icons[1].color = PlayerDataManager.player.defTypeReleased ? Color.white : Color.black;
        icons[2].color = PlayerDataManager.player.tecTypeReleased ? Color.white : Color.black;
    }

    /// <summary>
    /// ��������ς݂��`�F�b�N
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
            case CombiType.NORMAL:
                confirmWindow.Open();
                break;
        }
    }

    /// <summary>
    /// �X�L���ύX
    /// </summary>
    public void Change()
    {
        if (PlayerDataManager.player.GetSelectEvolutionType() == selectType)
        {
            // �I�𒆂ł�
            selectingWindow.Open();
        }

        else
        {
            // �X�L���ύX
            PlayerDataManager.player.SetSelectEvolutionType(selectType);
        }
    }

    /// <summary>
    /// �Q�[�W�\��
    /// </summary>
    void DispGuage()
    {
        float max = PlayerDataManager.player.GetCombiRankPtMax(selectType);
        int rest = 0;

        if (PlayerDataManager.player.GetEvolutionType() != CombiType.NORMAL && selectType != CombiType.NORMAL)
        {
            cantReleaseWindow.Open();
            return;
        }

        switch (selectType)
        {
            case CombiType.ATK:
                float atk = (float)PlayerDataManager.player.GetRankPt(StatusType.ATK);
                float mp = (float)PlayerDataManager.player.GetRankPt(StatusType.MP);
                guage1.fillAmount = atk / max;
                guage2.fillAmount = (atk + mp) / max;
                explain.text = "Attack�l\n�U���|�C���g + ���̓|�C���g";

                rest = PlayerDataManager.player.StatusData.rankPoint.atkRankPt_NextUp[Rank.SS] - PlayerDataManager.player.GetCombiRankPt(selectType);
                restText.text = "����܂Ŏc��" + rest + "�|�C���g";
                break;
            case CombiType.DEF:
                float hp = (float)PlayerDataManager.player.GetRankPt(StatusType.HP);
                float def = (float)PlayerDataManager.player.GetRankPt(StatusType.DEF);
                guage1.fillAmount = hp / max;
                guage2.fillAmount = (hp + def) / max;
                explain.text = "Defense�l\n�̗̓|�C���g + ����|�C���g";

                rest = PlayerDataManager.player.StatusData.rankPoint.defRankPt_NextUp[Rank.SS] - PlayerDataManager.player.GetCombiRankPt(selectType);
                restText.text = "����܂Ŏc��" + rest + "�|�C���g";
                break;
            case CombiType.TEC:
                float agi = (float)PlayerDataManager.player.GetRankPt(StatusType.AGI);
                float dex = (float)PlayerDataManager.player.GetRankPt(StatusType.DEX);
                guage1.fillAmount = agi / max;
                guage2.fillAmount = (agi + dex) / max;
                explain.text = "Tecnique�l\n���x�|�C���g + ��p�|�C���g";

                rest = PlayerDataManager.player.StatusData.rankPoint.tecRankPt_NextUp[Rank.SS] - PlayerDataManager.player.GetCombiRankPt(selectType);
                restText.text = "����܂Ŏc��" + rest + "�|�C���g";
                break;
        }

        guageWindow.Open();
    }
}
