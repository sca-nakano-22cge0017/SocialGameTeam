using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// PlayerDataManager.TraningReset()�Ńf�[�^������
/// </summary>
public class CharaStatus : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    //���Z�b�g�̃{�^���ݒ�
    public void ResetWindowScene(string str)
    {
        switch (str)
        {
            case "Reset": //�琬����(�^�C�g���ɖ߂�)
                //PlayerDataManager.TraningReset();
                SceneManager.LoadScene("TitleScene");
                break;
            default:
                SceneManager.LoadScene("TitleScene");
                break;
        }
    }
}
