using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// �v���C���[�̃Z�[�u�f�[�^�p�N���X
[System.Serializable]
public class SaveData
{
    public PlayerSaveData chara1 = new();
    public PlayerSaveData chara2 = new();
}

[System.Serializable]
public class PlayerSaveData
{
    /// <summary>
    /// �L�����N�^�[ID�@1�F���m�@2�F�V�X�^�[
    /// </summary>
    public int id = -1;

    /// <summary>
    /// ���݂̃X�e�[�^�X
    /// </summary>
    public int hp = 0;
    public int mp = 0;
    public int atk = 0;
    public int def = 0;
    public int spd = 0;
    public int dex = 0;

    /// <summary>
    /// ���݂̗ݐσ����NPt
    /// </summary>
    public int  hp_rankPt = 0;
    public int  mp_rankPt = 0;
    public int atk_rankPt = 0;
    public int def_rankPt = 0;
    public int spd_rankPt = 0;
    public int dex_rankPt = 0;

    /// <summary>
    /// �v���X�X�e�[�^�X
    /// </summary>
    public int  hp_plusStatus = 0;
    public int  mp_plusStatus = 0;
    public int atk_plusStatus = 0;
    public int def_plusStatus = 0;
    public int spd_plusStatus = 0;
    public int dex_plusStatus = 0;
}