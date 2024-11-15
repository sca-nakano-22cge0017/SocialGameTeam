using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.IO;
using UnityEngine.Events;

public class CharaSelectManager : MonoBehaviour
{
    [SerializeField] Button[] buttonNum = new Button[8];

    [SerializeField] GameObject[] charaImage = new GameObject[2];

    [SerializeField] Text[] status = new Text[7];
    [SerializeField] Text[] rank = new Text[6];
    
    void Start()
    {
        SaveData player = loadPlayerData();
        Debug.Log(player.chara2);
    }

    void Update()
    {
        //Debug.Log(PlayerDataManager.player.TotalPower);
        //Debug.Log(GameManager.SelectChara);
    }

    void SwordsManTrue()
    {
        charaImage[0].SetActive(true);
        charaImage[1].SetActive(false);
    }

    void WizardTrue()
    {
        charaImage[0].SetActive(false);
        charaImage[1].SetActive(true);
    }


    public void CharaButton1()
    {
        SwordsManTrue();
        GameManager.SelectChara = 1;
        //PlayerDataManager.Save();
        //PlayerDataManager.Load();
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
    }

    public void CharaButton2()
    {
        WizardTrue();
        GameManager.SelectChara = 2;
        //PlayerDataManager.Save();
        //PlayerDataManager.Load();
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
        if (!File.Exists(Application.persistentDataPath + "/savedata.json"))
            PlayerDataManager.Save();

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
        }
    }
}
