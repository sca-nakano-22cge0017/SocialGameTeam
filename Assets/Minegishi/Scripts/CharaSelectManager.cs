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

    
    void Start()
    {
        SaveData player = loadPlayerData();
        Debug.Log(player.chara2);
    }

    void Update()
    {
        
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
    }

    public void CharaButton2()
    {
        WizardTrue();
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
            File.Delete("/savedata.json");
            Debug.Log("セーブデータを削除しました: ");
        }
    }
}
