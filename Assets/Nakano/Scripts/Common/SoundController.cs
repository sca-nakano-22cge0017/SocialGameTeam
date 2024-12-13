using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class SoundController : MonoBehaviour
{
    [SerializeField] AudioMixer audioMixer;

    [SerializeField] Slider BGMSlider;
    [SerializeField] Slider SESlider;

    [Header("BGM")]
    [SerializeField, Header("メインテーマ")] AudioSource mainTheme;
    [SerializeField, Header("バトルテーマ")] AudioSource battle;
    [SerializeField, Header("ボス戦テーマ")] AudioSource boss;

    [Header("ジングル")]
    [SerializeField, Header("ジングル")] AudioSource jingle;
    [SerializeField, Header("ステージクリア")] AudioClip clear;
    [SerializeField, Header("育成リセット")] AudioClip reset;
    [SerializeField, Header("ゲームオーバー")] AudioClip gameover;
    [SerializeField, Header("キャラ差分解放")] AudioClip charaRelease;

    [Header("SE")]
    [SerializeField, Header("SE")] AudioSource SE;
    [SerializeField, Header("画面タップ音")] AudioClip tap1;
    [SerializeField, Header("ボタンタップ音")] AudioClip tap2;

    [SerializeField, Header("通常攻撃　剣士")] AudioClip attack_Sord;
    [SerializeField, Header("通常攻撃　シスター")] AudioClip attack_Sister;

    [SerializeField, Header("スキル攻撃")] AudioClip skill;
    [SerializeField, Header("スキル攻撃 爆発")] AudioClip skillExplode;
    [SerializeField, Header("バフ")] AudioClip buff;
    [SerializeField, Header("デバフ")] AudioClip debuff;

    [SerializeField, Header("必殺技予備動作　剣士")] AudioClip ultReady_Sord;
    [SerializeField, Header("必殺技予備動作　シスター")] AudioClip ultReady_Sister;
    [SerializeField, Header("必殺技カットイン")] AudioClip ultCutIn;
    [SerializeField, Header("必殺技攻撃　剣士")] AudioClip ultAttack_Sord;
    [SerializeField, Header("必殺技攻撃　シスター")] AudioClip ultAttack_Sister;

    [SerializeField, Header("ダメージ")] AudioClip damage;
    [SerializeField, Header("敵撃破")] AudioClip defeat;
    [SerializeField, Header("ドロップ")] AudioClip drop;

    [SerializeField, Header("ゲージ上昇　短")] AudioClip guageUp1;
    [SerializeField, Header("ゲージ上昇　長")] AudioClip guageUp2;
    [SerializeField, Header("ランクアップ")] AudioClip rankUp;
    [SerializeField, Header("技能解放")] AudioClip skillRelease;

    bool isCrossFadeOut = false;
    bool isCrossFadeIn = false;
    AudioSource crossFadeBefore;
    AudioSource crossFadeAfter;
    [SerializeField] private float fadeTime = 0.5f;

    private void Start()
    {
        PlayMainTheme();
        boss.volume = 0;
        battle.volume = 0;

        //BGM
        //audioMixer.GetFloat("BGM", out float bgmVolume);
        //BGMSlider.value = bgmVolume;

        //SE
        //audioMixer.GetFloat("SE", out float seVolume);
        //SESlider.value = seVolume;
    }

    private void Update()
    {
        if (isCrossFadeOut)
        {
            crossFadeBefore.volume -= 1 / fadeTime * Time.deltaTime;

            if (crossFadeBefore.volume <= 0)
            {
                crossFadeBefore.volume = 0;
                crossFadeBefore.Stop();
                
                crossFadeAfter.volume = 0;
                crossFadeAfter.Play();

                isCrossFadeOut = false;
                isCrossFadeIn = true;
            }
        }

        if (isCrossFadeIn)
        {
            crossFadeAfter.volume += 1 / fadeTime * Time.deltaTime;

            if (crossFadeAfter.volume >= 1)
            {
                crossFadeAfter.volume = 1;
                isCrossFadeIn = false;
            }
        }
    }

    /// <summary>
    /// BGMクロスフェード
    /// </summary>
    /// <param name="_beforeSound">フェードアウトする音声</param>
    /// <param name="_afterSound">フェードインする音声</param>
    public void CrossFade(string _beforeSound, string _afterSound)
    {
        switch (_beforeSound)
        {
            case "Main":
                crossFadeBefore = mainTheme;
                break;
            case "Battle":
                crossFadeBefore = battle;
                break;
            case "Boss":
                crossFadeBefore = boss;
                break;
        }

        switch (_afterSound)
        {
            case "Main":
                crossFadeAfter = mainTheme;
                break;
            case "Battle":
                crossFadeAfter = battle;
                break;
            case "Boss":
                crossFadeAfter = boss;
                break;
        }
        
        isCrossFadeOut = true;
    }

    public void MainToBattle()
    {
        if (GameManager.SelectArea == 1)
        {
            CrossFade("Main", "Battle");
        }
        
        if (GameManager.SelectArea == 2)
        {
            CrossFade("Main", "Boss");
        }
    }

    public void BattleToMain()
    {
        if (battle.isPlaying)
        {
            CrossFade("Battle", "Main");
        }

        if (boss.isPlaying)
        {
            CrossFade("Boss", "Main");
        }
    }

    public void SetBGM(float volume)
    {
        audioMixer.SetFloat("BGM", volume);
    }

    public void SetSE(float volume)
    {
        audioMixer.SetFloat("SE", volume);
    }

    public void PlayMainTheme()
    {
        mainTheme.Play();
    }

    public void StopMainTheme()
    {
        mainTheme.Stop();
    }

    public void PlayBattleTheme()
    {
        battle.Play();
    }

    public void StopBattleTheme()
    {
        battle.Stop();
    }

    public void PlayBossTheme()
    {
        boss.Play();
    }

    public void StopBossTheme()
    {
        boss.Stop();
    }

    public void PlayClearJingle()
    {
        jingle.PlayOneShot(clear);
    }

    public void PlayGameOverJingle()
    {
        jingle.PlayOneShot(gameover);
    }

    public void PlayResetJingle()
    {
        jingle.PlayOneShot(reset);
    }

    public void PlayCharaReleaseJingle()
    {
        jingle.PlayOneShot(charaRelease);
    }

    public void PlayTap1SE()
    {
        SE.PlayOneShot(tap1);
    }

    public void PlayTap2SE()
    {
        SE.PlayOneShot(tap2);
    }

    public void PlayAttackSE(int _id)
    {
        if (attack_Sister == null || attack_Sord == null) return;

        if (_id == 1) SE.PlayOneShot(attack_Sord);
        if (_id == 2) SE.PlayOneShot(attack_Sister);
    }

    public void PlaySkillSE()
    {
        SE.PlayOneShot(skill);
    }

    public void PlaySkillExplodeSE()
    {
        SE.PlayOneShot(skillExplode);
    }

    public void PlayBuffSE()
    {
        SE.PlayOneShot(buff);
    }

    public void PlayDebuffSE()
    {
        SE.PlayOneShot(debuff);
    }

    public void PlayUltReadySE(int _id)
    {
        if (_id == 1) SE.PlayOneShot(ultReady_Sord);
        if (_id == 2) SE.PlayOneShot(ultReady_Sister);
    }

    public void PlayUltCutInSE()
    {
        SE.PlayOneShot(ultCutIn);
    }

    public void PlayUltAttackSE(int _id)
    {
        if (_id == 1) SE.PlayOneShot(ultAttack_Sord);
        if (_id == 2) SE.PlayOneShot(ultAttack_Sister);
    }

    public void PlayDamageSE()
    {
        if (damage == null) return;
        SE.PlayOneShot(damage);
    }

    public void PlayDefeatSE()
    {
        SE.PlayOneShot(defeat);
    }

    public void PlayDropSE()
    {
        SE.PlayOneShot(drop);
    }

    public void PlayGuageUp1SE()
    {
        SE.PlayOneShot(guageUp1);
    }

    public void PlayGuageUp2SE()
    {
        SE.PlayOneShot(guageUp2);
    }

    public void PlayRankUpSE()
    {
        SE.PlayOneShot(rankUp);
    }

    public void PlaySkillReleaseSE()
    {
        SE.PlayOneShot(skillRelease);
    }
}
