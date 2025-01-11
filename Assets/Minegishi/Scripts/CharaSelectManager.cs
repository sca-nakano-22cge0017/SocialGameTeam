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
    [SerializeField] Text[] rank = new Text[6];
    [SerializeField] Text[] plus = new Text[6];

    [SerializeField] GameObject plusStatusWindow;
    [SerializeField] Text plusStatusText;

    [SerializeField] GameObject SkillWindow;
    [SerializeField] GameObject skillButton;
    [SerializeField] GameObject skinButton;
    [SerializeField] Button[] skillButtons;
    private bool trueWindow = false;
    SpecialTecniqueManager stm;

    void Start()
    {
        SaveData player = loadPlayerData();
        Debug.Log(player.chara2);
        stm = FindObjectOfType<SpecialTecniqueManager>();
        SkillRelease();
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
        SwordsManTrue();
        GameManager.SelectChara = 1;
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


        if(PlayerDataManager.player.GetPlusStatus(StatusType.HP) < 0)
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

    public void CharaButton2()
    {
        WizardTrue();
        GameManager.SelectChara = 2;
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
        plusStatusText.text = PlayerDataManager.GetResetCurrentEffects(StatusType.HP).ToString();
        plusStatusWindow.SetActive(true);
    }

    public void PlusStatusDEF()
    {
        plusStatusText.text = PlayerDataManager.GetResetCurrentEffects(StatusType.DEF).ToString();
        plusStatusWindow.SetActive(true);
    }

    public void PlusStatusATK()
    {
        plusStatusText.text = PlayerDataManager.GetResetCurrentEffects(StatusType.ATK).ToString();
        plusStatusWindow.SetActive(true);
    }

    public void PlusStatusMP()
    {
        plusStatusText.text = PlayerDataManager.GetResetCurrentEffects(StatusType.MP).ToString();
        plusStatusWindow.SetActive(true);
    }

    public void PlusStatusAGI()
    {
        plusStatusText.text = PlayerDataManager.GetResetCurrentEffects(StatusType.AGI).ToString();
        plusStatusWindow.SetActive(true);
    }

    public void PlusStatusDEX()
    {
        plusStatusText.text = PlayerDataManager.GetResetCurrentEffects(StatusType.DEX).ToString();
        plusStatusWindow.SetActive(true);
    }

    public void ButtonUp()
    {
        plusStatusWindow.SetActive(false);
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
