using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// �Q�[���S�̂Ŏg�p����ϐ������Ǘ�
/// </summary>
public class GameManager : MonoBehaviour
{
    private static int charaID = -1;

    /// <summary>
    /// �I�𒆃L�����N�^�[ / 1:�V�X�^�[ , 2:���m , -1;�G���[
    /// </summary>
    public static int CharaID
    {
        get { return charaID; }
        set
        {
            if (value != 1 && value != 2) return;
            charaID = value;
        }
    }
}
