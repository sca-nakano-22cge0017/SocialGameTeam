using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// プレイヤーのセーブデータ用クラス
[System.Serializable]
public class SaveData
{
    /// <summary>
    /// 初回起動かどうか
    /// </summary>
    public bool isFirstStart = true;

    /// <summary>
    /// クリアしたボスの最高難易度
    /// </summary>
    public int isCrearBossDifficulty = 0;

    /// <summary>
    /// 各ボスクリア状況
    /// </summary>
    public bool[] isBossClear = { false, false, false, false, false };

    /// <summary>
    /// 選択中キャラクター
    /// </summary>
    public int selectChara = -1;

    public PlayerSaveData chara1 = new();
    public PlayerSaveData chara2 = new();

    /// <summary>
    /// スタミナ
    /// </summary>
    public StaminaSaveData staminaData = new();

    /// <summary>
    /// チュートリアル達成状況
    /// </summary>
    public TutorialProgress tutorialData = new();
}

[System.Serializable]
public class PlayerSaveData
{
    /// <summary>
    /// キャラクターID　1：剣士　2：シスター
    /// </summary>
    public int id = -1;

    /// <summary>
    /// 現在の進化形態
    /// </summary>
    public string evolutionType = "NORMAL";

    /// <summary>
    /// 現在選択中の進化形態差分
    /// </summary>
    public string selectEvolutionType = "NORMAL";

    /// <summary>
    /// 各進化形態解放済みかどうか
    /// </summary>
    public bool atkTypeReleased = false;
    public bool defTypeReleased = false;
    public bool tecTypeReleased = false;

    /// <summary>
    /// 現在のステータス
    /// </summary>
    public int hp = 0;
    public int mp = 0;
    public int atk = 0;
    public int def = 0;
    public int agi = 0;
    public int dex = 0;

    /// <summary>
    /// 現在の累積ランクPt
    /// </summary>
    public int  hp_rankPt = 0;
    public int  mp_rankPt = 0;
    public int atk_rankPt = 0;
    public int def_rankPt = 0;
    public int agi_rankPt = 0;
    public int dex_rankPt = 0;

    /// <summary>
    /// プラスステータス
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
    /// 前回終了時のスタミナ
    /// </summary>
    public int lastStamina;

    /// <summary>
    /// 前回終了時の時間
    /// </summary>
    public string lastTime;

    /// <summary>
    /// 前回終了時の次にスタミナ回復するまでの時間
    /// </summary>
    public float lastRecoveryTime;

    /// <summary>
    /// 前回終了時のスタミナ全快するまでの時間
    /// </summary>
    public float lastCompleteRecoveryTime;
}

[System.Serializable]
public class TutorialProgress
{
    /// <summary>
    /// 初回起動時、タイトル画面をタップしたとき
    /// </summary>
    public bool checkedCharaSelect = false;

    /// <summary>
    /// ホーム画面に初遷移したとき
    /// </summary>
    public bool checkedHome = false;

    /// <summary>
    /// 育成ステージ選択画面に初遷移したとき
    /// </summary>
    public bool checkedStageSelect = false;

    /// <summary>
    /// バトル画面に初遷移したとき
    /// </summary>
    public bool checkedBattle = false;

    /// <summary>
    /// リザルト画面に初遷移したとき
    /// </summary>
    public bool checkedResult = false;

    /// <summary>
    /// ボスバトル画面に遷移したとき
    /// </summary>
    public bool checkedBossBattle = false;

    /// <summary>
    /// ボス撃破後育成ステージに初遷移したとき
    /// </summary>
    public bool checkedStageSelect_BossCleared = false;

    /// <summary>
    /// 初めてリセットボタンを押したとき
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