using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// �v���C���[�̃Z�[�u�f�[�^�p�N���X
[System.Serializable]
public class PlayerSaveData
{
    /// <summary>
    /// �L�����N�^�[ID�@1�F���m�@2�F�V�X�^�[
    /// </summary>
    public int id;

    /// <summary>
    /// ���݂̃X�e�[�^�X
    /// </summary>
    public Status status = new(0,0,0,0,0,0);

    /// <summary>
    /// ���݂̗ݐσ����NPt
    /// </summary>
    public Status rankPoint = new(0, 0, 0, 0, 0, 0);

    /// <summary>
    /// �v���X�X�e�[�^�X
    /// </summary>
    public Status plusStatus = new(0, 0, 0, 0, 0, 0);
}
