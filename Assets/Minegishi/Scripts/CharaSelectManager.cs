using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.IO;
using UnityEngine.Events;
using Unity.VisualScripting;

public class CharaSelectManager : MonoBehaviour
{
    [SerializeField] Button[] buttonNum = new Button[8];

    [SerializeField] GameObject[] charaImage = new GameObject[2];

    [SerializeField] Text[] status = new Text[7];
    [SerializeField] Text charaName;
    [SerializeField] Text[] rank = new Text[6];
    [SerializeField] private RankIcon rankIconHP;
    [SerializeField] private RankIcon rankIconDEF;
    [SerializeField] private RankIcon rankIconATK;
    [SerializeField] private RankIcon rankIconMP;
    [SerializeField] private RankIcon rankIconAGI;
    [SerializeField] private RankIcon rankIconDEX;
    [SerializeField] Text[] plus = new Text[6];

    [SerializeField] GameObject plusStatusWindow;
    [SerializeField] Text plusStatusText;

    [SerializeField] GameObject SkillWindow;
    [SerializeField] GameObject skillButton;
    [SerializeField] GameObject skinButton;
    [SerializeField] Button[] skillButtons;
    SpecialTecniqueManager stm;

    void Start()
    {
        SaveData player = loadPlayerData();
        Debug.Log(player.chara2);
        stm = FindObjectOfType<SpecialTecniqueManager>();
        SkillRelease();
        if(GameManager.SelectChara == 1)
        {
            CharaButton1();
        }
        else if(GameManager.SelectChara == 2)
        {
            CharaButton2();
        }
    }

    void Update()
    {
        //Debug.Log(plusStatusText.text);
    }

    void SwordsManTrue()
    {
        skillButton.SetActive(true);
        skinButton.SetActive(true);
        charaImage[0].SetActive(true);
        charaImage[1].SetActive(false);
    }

    void WizardTrue()
    {
        skillButton.SetActive(true);
        skinButton.SetActive(true);
        charaImage[0].SetActive(false);
        charaImage[1].SetActive(true);
    }


    public void CharaButton1()
    {
        //SwordsManTrue();
        GameManager.SelectChara = 1;
        charaName.text = "ロゼッタ";
        status[0].text = PlayerDataManager.player.TotalPower.ToString(); 

        status[1].text = PlayerDataManager.player.GetStatus(StatusType.HP).ToString();
        status[2].text = PlayerDataManager.player.GetStatus(StatusType.DEF).ToString();
        status[3].text = PlayerDataManager.player.GetStatus(StatusType.ATK).ToString();
        status[4].text = PlayerDataManager.player.GetStatus(StatusType.MP).ToString();
        status[5].text = PlayerDataManager.player.GetStatus(StatusType.AGI).ToString();
        status[6].text = PlayerDataManager.player.GetStatus(StatusType.DEX).ToString();

        rankIconHP.RankIconChange(PlayerDataManager.player.GetRank(StatusType.HP).ToString());
        rankIconDEF.RankIconChange(PlayerDataManager.player.GetRank(StatusType.DEF).ToString());
        rankIconATK.RankIconChange(PlayerDataManager.player.GetRank(StatusType.ATK).ToString());
        rankIconMP.RankIconChange(PlayerDataManager.player.GetRank(StatusType.MP).ToString());
        rankIconAGI.RankIconChange(PlayerDataManager.player.GetRank(StatusType.AGI).ToString());
        rankIconDEX.RankIconChange(PlayerDataManager.player.GetRank(StatusType.DEX).ToString());
        //rank[0].text = PlayerDataManager.player.GetRank(StatusType.HP).ToString();
        //rank[1].text = PlayerDataManager.player.GetRank(StatusType.DEF).ToString();
        //rank[2].text = PlayerDataManager.player.GetRank(StatusType.ATK).ToString();
        //rank[3].text = PlayerDataManager.player.GetRank(StatusType.MP).ToString();
        //rank[4].text = PlayerDataManager.player.GetRank(StatusType.AGI).ToString();
        //rank[5].text = PlayerDataManager.player.GetRank(StatusType.DEX).ToString();


        if (PlayerDataManager.player.GetPlusStatus(StatusType.HP) > 0)
        {
            plus[0].text = "+" + PlayerDataManager.player.GetPlusStatus(StatusType.HP).ToString();
        }
        else plus[0].text = "";
        if (PlayerDataManager.player.GetPlusStatus(StatusType.DEF) > 0)
        {
            plus[1].text = "+" + PlayerDataManager.player.GetPlusStatus(StatusType.DEF).ToString();
        }
        else plus[1].text = "";
        if (PlayerDataManager.player.GetPlusStatus(StatusType.ATK) > 0)
        {
            plus[2].text = "+" + PlayerDataManager.player.GetPlusStatus(StatusType.ATK).ToString();
        }
        else plus[2].text = "";
        if (PlayerDataManager.player.GetPlusStatus(StatusType.MP) > 0)
        {
            plus[3].text = "+" + PlayerDataManager.player.GetPlusStatus(StatusType.MP).ToString();
        }
        else plus[3].text = "";
        if (PlayerDataManager.player.GetPlusStatus(StatusType.AGI) > 0)
        {
            plus[4].text = "+" + PlayerDataManager.player.GetPlusStatus(StatusType.AGI).ToString();
        }
        else plus[4].text = "";
        if (PlayerDataManager.player.GetPlusStatus(StatusType.DEX) > 0)
        {
            plus[5].text = "+" + PlayerDataManager.player.GetPlusStatus(StatusType.DEX).ToString();
        }
        else plus[5].text = "";

        SkillRelease();
    }

    public void CharaButton2()
    {
        //WizardTrue();
        GameManager.SelectChara = 2;
        charaName.text = "セレスティア";
        status[0].text = PlayerDataManager.player.TotalPower.ToString();

        status[1].text = PlayerDataManager.player.GetStatus(StatusType.HP).ToString();
        status[2].text = PlayerDataManager.player.GetStatus(StatusType.DEF).ToString();
        status[3].text = PlayerDataManager.player.GetStatus(StatusType.ATK).ToString();
        status[4].text = PlayerDataManager.player.GetStatus(StatusType.MP).ToString();
        status[5].text = PlayerDataManager.player.GetStatus(StatusType.AGI).ToString();
        status[6].text = PlayerDataManager.player.GetStatus(StatusType.DEX).ToString();

        rankIconHP.RankIconChange(PlayerDataManager.player.GetRank(StatusType.HP).ToString());
        rankIconDEF.RankIconChange(PlayerDataManager.player.GetRank(StatusType.DEF).ToString());
        rankIconATK.RankIconChange(PlayerDataManager.player.GetRank(StatusType.ATK).ToString());
        rankIconMP.RankIconChange(PlayerDataManager.player.GetRank(StatusType.MP).ToString());
        rankIconAGI.RankIconChange(PlayerDataManager.player.GetRank(StatusType.AGI).ToString());
        rankIconDEX.RankIconChange(PlayerDataManager.player.GetRank(StatusType.DEX).ToString());
        //rank[0].text = PlayerDataManager.player.GetRank(StatusType.HP).ToString();
        //rank[1].text = PlayerDataManager.player.GetRank(StatusType.DEF).ToString();
        //rank[2].text = PlayerDataManager.player.GetRank(StatusType.ATK).ToString();
        //rank[3].text = PlayerDataManager.player.GetRank(StatusType.MP).ToString();
        //rank[4].text = PlayerDataManager.player.GetRank(StatusType.AGI).ToString();
        //rank[5].text = PlayerDataManager.player.GetRank(StatusType.DEX).ToString();

        if (PlayerDataManager.player.GetPlusStatus(StatusType.HP) > 0)
        {
            plus[0].text = "+" + PlayerDataManager.player.GetPlusStatus(StatusType.HP).ToString();
        }
        else plus[0].text = "";
        if (PlayerDataManager.player.GetPlusStatus(StatusType.DEF) > 0)
        {
            plus[1].text = "+" + PlayerDataManager.player.GetPlusStatus(StatusType.DEF).ToString();
        }
        else plus[1].text = "";
        if (PlayerDataManager.player.GetPlusStatus(StatusType.ATK) > 0)
        {
            plus[2].text = "+" + PlayerDataManager.player.GetPlusStatus(StatusType.ATK).ToString();
        }
        else plus[2].text = "";
        if (PlayerDataManager.player.GetPlusStatus(StatusType.MP) > 0)
        {
            plus[3].text = "+" + PlayerDataManager.player.GetPlusStatus(StatusType.MP).ToString();
        }
        else plus[3].text = "";
        if (PlayerDataManager.player.GetPlusStatus(StatusType.AGI) > 0)
        {
            plus[4].text = "+" + PlayerDataManager.player.GetPlusStatus(StatusType.AGI).ToString();
        }
        else plus[4].text = "";
        if (PlayerDataManager.player.GetPlusStatus(StatusType.DEX) > 0)
        {
            plus[5].text = "+" + PlayerDataManager.player.GetPlusStatus(StatusType.DEX).ToString();
        }
        else plus[5].text = "";

        SkillRelease();
    }

    public void CharaStatus()
    {
        status[0].text = PlayerDataManager.player.TotalPower.ToString();

        status[1].text = PlayerDataManager.player.GetStatus(StatusType.HP).ToString();
        status[2].text = PlayerDataManager.player.GetStatus(StatusType.DEF).ToString();
        status[3].text = PlayerDataManager.player.GetStatus(StatusType.ATK).ToString();
        status[4].text = PlayerDataManager.player.GetStatus(StatusType.MP).ToString();
        status[5].text = PlayerDataManager.player.GetStatus(StatusType.AGI).ToString();
        status[6].text = PlayerDataManager.player.GetStatus(StatusType.DEX).ToString();

        rank[0].text = PlayerDataManager.player.GetRank(StatusType.HP).ToString();
        rank[1].text = PlayerDataManager.player.GetRank(StatusType.DEF).ToString();
        rank[2].text = PlayerDataManager.player.GetRank(StatusType.ATK).ToString();
        rank[3].text = PlayerDataManager.player.GetRank(StatusType.MP).ToString();
        rank[4].text = PlayerDataManager.player.GetRank(StatusType.AGI).ToString();
        rank[5].text = PlayerDataManager.player.GetRank(StatusType.DEX).ToString();


        if (PlayerDataManager.player.GetPlusStatus(StatusType.HP) < 0)
        {
            plus[0].text = "+" + PlayerDataManager.player.GetPlusStatus(StatusType.HP).ToString();
        }

        if (PlayerDataManager.player.GetPlusStatus(StatusType.DEF) < 0)
        {
            plus[1].text = "+" + PlayerDataManager.player.GetPlusStatus(StatusType.DEF).ToString();
        }

        if (PlayerDataManager.player.GetPlusStatus(StatusType.ATK) < 0)
        {
            plus[2].text = "+" + PlayerDataManager.player.GetPlusStatus(StatusType.ATK).ToString();
        }
        if (PlayerDataManager.player.GetPlusStatus(StatusType.MP) < 0)
        {
            plus[3].text = "+" + PlayerDataManager.player.GetPlusStatus(StatusType.MP).ToString();
        }
        if (PlayerDataManager.player.GetPlusStatus(StatusType.AGI) < 0)
        {
            plus[4].text = "+" + PlayerDataManager.player.GetPlusStatus(StatusType.AGI).ToString();
        }
        if (PlayerDataManager.player.GetPlusStatus(StatusType.DEX) < 0)
        {
            plus[5].text = "+" + PlayerDataManager.player.GetPlusStatus(StatusType.DEX).ToString();
        }

        SkillRelease();
    }

    public void PlusStatusHP()
    {
        if (PlayerDataManager.GetResetCurrentEffects(StatusType.HP) != "")
        {
            plusStatusText.text = PlayerDataManager.GetResetCurrentEffects(StatusType.HP).ToString();
            plusStatusWindow.SetActive(true);
        }
    }

    public void PlusStatusDEF()
    {
        if (PlayerDataManager.GetResetCurrentEffects(StatusType.DEF) != "")
        {
            plusStatusText.text = PlayerDataManager.GetResetCurrentEffects(StatusType.DEF).ToString();
            plusStatusWindow.SetActive(true);
        }
    }

    public void PlusStatusATK()
    {
        if (PlayerDataManager.GetResetCurrentEffects(StatusType.ATK) != "")
        {
            plusStatusText.text = PlayerDataManager.GetResetCurrentEffects(StatusType.ATK).ToString();
            plusStatusWindow.SetActive(true);
        }
    }

    public void PlusStatusMP()
    {
        if (PlayerDataManager.GetResetCurrentEffects(StatusType.MP) != "")
        {
            plusStatusText.text = PlayerDataManager.GetResetCurrentEffects(StatusType.MP).ToString();
            plusStatusWindow.SetActive(true);
        }
    }

    public void PlusStatusAGI()
    {
        if (PlayerDataManager.GetResetCurrentEffects(StatusType.AGI) != "")
        {
            plusStatusText.text = PlayerDataManager.GetResetCurrentEffects(StatusType.AGI).ToString();
            plusStatusWindow.SetActive(true);
        }
    }

    public void PlusStatusDEX()
    {
        if (PlayerDataManager.GetResetCurrentEffects(StatusType.DEX) != "")
        {
            plusStatusText.text = PlayerDataManager.GetResetCurrentEffects(StatusType.DEX).ToString();
            plusStatusWindow.SetActive(true);
        }
    }

    public void ButtonUp()
    {
        plusStatusWindow.SetActive(false);
    }

    public void BuckButton()
    {
        SceneLoader.LoadFade("HomeScene");
    }

    public static void savePlayerData(SaveData player)
    {
        StreamWriter writer;

        string jsonstr = JsonUtility.ToJson(player);

        // dataPathだとスマホでは保存不可なのでpersistentDataPathに変更
        writer = new StreamWriter(Application.persistentDataPath + "/savedata.json", false);
        writer.Write(jsonstr);
        writer.Flush();
        writer.Close();
    }

    public static SaveData loadPlayerData()
    {
        string datastr = "";
        StreamReader reader;

        // ファイルが無かったら作る
        while (!File.Exists(Application.persistentDataPath + "/savedata.json"))
            PlayerDataManager.CharacterInitialize();

        reader = new StreamReader(Application.persistentDataPath + "/savedata.json");
        datastr = reader.ReadToEnd();
        reader.Close();

        return JsonUtility.FromJson<SaveData>(datastr);
    }

    public static void DeleteSaveData()
    {
        // ファイルを削除
        if (File.Exists(Application.persistentDataPath + "/savedata.json"))
        {
            File.Delete(Application.persistentDataPath + "/savedata.json");
            Debug.Log("セーブデータを削除しました: ");

            GameManager.DataInitialize();
        }
    }

    void SkillRelease()
    {
        //　SpecialTecniqueManager stm;　StartでFindObjectObType使って取得
        //　[SerializeField] private Button[] skillButtons;

        // 全て非表示にする
        for (int j = 0; j < skillButtons.Length; j++)
        {
            skillButtons[j].gameObject.SetActive(false);
        }

        for (int i = 0; i < stm.specialTecniques.Length; i++)
        {
            for (int j = 0; j < skillButtons.Length; j++)
            {
                // ScriptableObjectとゲームオブジェクト(ボタン)の名前が同じなら
                // かつ解放済みなら
                if (stm.specialTecniques[i].name == skillButtons[j].name &&
                    stm.specialTecniques[i].m_released)
                {
                    skillButtons[j].gameObject.SetActive(true);
                }
            }
        }
    }
}
