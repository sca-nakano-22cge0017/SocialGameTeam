using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChanger : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
    }

    public void TitleSceneChange()
    {
        if (SceneManager.GetActiveScene().name == "TitleScene")
        {
            //SceneManager.LoadScene("HomeScene");
            SceneLoader.LoadScene("HomeScene", true);
        }
    }
    public void DebugReset()
    {
        Debug.Log("���Z�b�g");
        GameManager.isDelete = true;
        CharaSelectManager.DeleteSaveData();
    }
}
