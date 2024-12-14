using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// �v���C���[�̃Z�[�u�f�[�^�p�N���X
[System.Serializable]
public class SaveData
{
    /// <summary>
    /// ����N�����ǂ���
    /// </summary>
    public bool isFirstStart = true;

    /// <summary>
    /// �N���A�����{�X�̍ō���Փx
    /// </summary>
    public int isCrearBossDifficulty = 0;

    /// <summary>
    /// �e�{�X�N���A��
    /// </summary>
    public bool[] isBossClear = { false, false, false, false, false };

    /// <summary>
    /// �I�𒆃L�����N�^�[
    /// </summary>
    public int selectChara = -1;

    public PlayerSaveData chara1 = new();
    public PlayerSaveData chara2 = new();

    /// <summary>
    /// �X�^�~�i
    /// </summary>
    public StaminaSaveData staminaData = new();

    /// <summary>
    /// �`���[�g���A���B����
    /// </summary>
    public TutorialProgress tutorialData = new();
}

[System.Serializable]
public class PlayerSaveData
{
    /// <summary>
    /// �L�����N�^�[ID�@1�F���m�@2�F�V�X�^�[
    /// </summary>
    public int id = -1;

    /// <summary>
    /// ���݂̐i���`��
    /// </summary>
    public string evolutionType = "NORMAL";

    /// <summary>
    /// ���ݑI�𒆂̐i���`�ԍ���
    /// </summary>
    public string selectEvolutionType = "NORMAL";

    /// <summary>
    /// �e�i���`�ԉ���ς݂��ǂ���
    /// </summary>
    public bool atkTypeReleased = false;
    public bool defTypeReleased = false;
    public bool tecTypeReleased = false;

    /// <summary>
    /// ���݂̃X�e�[�^�X
    /// </summary>
    public int hp = 0;
    public int mp = 0;
    public int atk = 0;
    public int def = 0;
    public int agi = 0;
    public int dex = 0;

    /// <summary>
    /// ���݂̗ݐσ����NPt
    /// </summary>
    public int  hp_rankPt = 0;
    public int  mp_rankPt = 0;
    public int atk_rankPt = 0;
    public int def_rankPt = 0;
    public int agi_rankPt = 0;
    public int dex_rankPt = 0;

    /// <summary>
    /// �v���X�X�e�[�^�X
    /// </summary>
    public int  hp_plusStatus = 0;
    public int  mp_plusStatus = 0;
    public int atk_plusStatus = 0;
    public int def_plusStatus = 0;
    public int agi_plusStatus = 0;
    public int dex_plusStatus = 0;
}

[System.Serializable]
public class StaminaSaveData
{
    /// <summary>
    /// �O��I�����̃X�^�~�i
    /// </summary>
    public int lastStamina;

    /// <summary>
    /// �O��I�����̎���
    /// </summary>
    public string lastTime;

    /// <summary>
    /// �O��I�����̎��ɃX�^�~�i�񕜂���܂ł̎���
    /// </summary>
    public float lastRecoveryTime;

    /// <summary>
    /// �O��I�����̃X�^�~�i�S������܂ł̎���
    /// </summary>
    public float lastCompleteRecoveryTime;
}

[System.Serializable]
public class TutorialProgress
{
    /// <summary>
    /// ����N�����A�^�C�g����ʂ��^�b�v�����Ƃ�
    /// </summary>
    public bool checkedCharaSelect = false;

    /// <summary>
    /// �z�[����ʂɏ��J�ڂ����Ƃ�
    /// </summary>
    public bool checkedHome = false;

    /// <summary>
    /// �琬�X�e�[�W�I����ʂɏ��J�ڂ����Ƃ�
    /// </summary>
    public bool checkedStageSelect = false;

    /// <summary>
    /// �o�g����ʂɏ��J�ڂ����Ƃ�
    /// </summary>
    public bool checkedBattle = false;

    /// <summary>
    /// ���U���g��ʂɏ��J�ڂ����Ƃ�
    /// </summary>
    public bool checkedResult = false;

    /// <summary>
    /// �{�X�o�g����ʂɑJ�ڂ����Ƃ�
    /// </summary>
    public bool checkedBossBattle = false;

    /// <summary>
    /// �{�X���j��琬�X�e�[�W�ɏ��J�ڂ����Ƃ�
    /// </summary>
    public bool checkedStageSelect_BossCleared = false;

    /// <summary>
    /// ���߂ă��Z�b�g�{�^�����������Ƃ�
    /// </summary>
    public bool checkedReset = false;

    public TutorialProgress() { }

    public TutorialProgress(TutorialProgress _data)
    {
        if (_data == null) _data = new();

        checkedCharaSelect = _data.checkedCharaSelect;
        checkedHome = _data.checkedHome;
        checkedStageSelect = _data.checkedStageSelect;
        checkedBattle = _data.checkedBattle;
        checkedResult = _data.checkedResult;
        checkedBossBattle = _data.checkedBossBattle;
        checkedStageSelect_BossCleared = _data.checkedStageSelect_BossCleared;
        checkedReset = _data.checkedReset;
    }
}