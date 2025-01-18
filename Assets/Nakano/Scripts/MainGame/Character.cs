using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

/// <summary>
/// バフ、デバフ、特殊スキル発動中などの特殊状態
/// </summary>
public class State
{
    /// <summary>
    /// バフかどうか
    /// </summary>
    public bool isBuff;

    /// <summary>
    /// どの特殊状態か
    /// プレイヤーバフデバフ→スキル番号
    /// 敵デバフ１→101、敵デバフ２→102、敵バフ→103
    /// </summary>
    public int stateId;

    /// <summary>
    /// 継続ターン
    /// </summary>
    public int continuationTurn;

    /// <summary>
    /// 経過ターン
    /// </summary>
    public int elapsedTurn;

    /// <summary>
    /// 効果量
    /// </summary>
    public float value;

    /// <summary>
    /// 効果終了時の処理
    /// </summary>
    public Action werasOffAction;

    /// <summary>
    /// 持続効果　効果終了までターン終了時に呼び出す
    /// </summary>
    public Action lastingEffects;
}

/// <summary>
/// メインゲーム上で動くキャラクター　PlayerDataとEnemyに継承
/// </summary>
public class Character : MonoBehaviour
{
    protected SpecialTecniqueManager specialTecniqueManager;
    protected SoundController soundController;
    [SerializeField] protected MainGameSystem mainGameSystem;
    [SerializeField] protected MainDirection mainDirection;

    public MeshRenderer meshRenderer;
    public Image image;

    [SerializeField] public MainGameGuage hpGuage;
    [SerializeField] protected Text criticalText;
    [SerializeField] protected Text damageText;
    [SerializeField] protected Text healText;
    [SerializeField] protected Text buffText;

    [SerializeField, Header("テキスト表示時間")] protected float textDispTime;

    // ステータス
    [HideInInspector] public int ATK; // 攻撃
    [HideInInspector] public int MP;  // 魔力
    [HideInInspector] public int HP;  // 体力
    [HideInInspector] public int DEF; // 防御
    [HideInInspector] public int AGI; // 速度
    [HideInInspector] public int DEX; // 器用

    // 計算用 現在値
    [HideInInspector] public int currentMp;
    [HideInInspector] public int currentHp;

    // ステータス倍率
    [HideInInspector] public float powerMp ;
    [HideInInspector] public float powerAtk;
    [HideInInspector] public float powerHp ;
    [HideInInspector] public float powerDef;
    [HideInInspector] public float powerDex;
    [HideInInspector] public float powerAgi;

    // バフ総量 割合
    [HideInInspector] public float buffMp ;
    [HideInInspector] public float buffAtk;
    [HideInInspector] public float buffHp ;
    [HideInInspector] public float buffDef;
    [HideInInspector] public float buffDex;
    [HideInInspector] public float buffAgi;

    // デバフ総量 割合
    [HideInInspector] public float debuffMp ;
    [HideInInspector] public float debuffAtk;
    [HideInInspector] public float debuffHp ;
    [HideInInspector] public float debuffDef;
    [HideInInspector] public float debuffDex;
    [HideInInspector] public float debuffAgi;

    // バフデバフ
    public List<State> state = new();

    // 会心率
    public float criticalProbabilityInitial;  // 初期値 %
    public float _criticalProbability;        // 計算用 %

    public float power_CriticalInit;    // 基本会心時倍率 倍
    public float buffCriticalPower;     // 会心時倍率バフ 割
    public float critical;    // 会心時倍率　計算用

    // モーション関係
    public Animator motion;
    public SpineAnim spineAnim;

    private void Start()
    {
        specialTecniqueManager = FindObjectOfType<SpecialTecniqueManager>();
    }

    /// <summary>
    /// 初期化
    /// </summary>
    public virtual void Initialize()
    {
        critical = 1.0f;

        currentMp = MP;
        currentHp = HP;

        powerMp = 1;
        powerAtk = 1;
        powerHp = 1;
        powerDef = 1;
        powerDex = 1;
        powerAgi = 1;

        buffMp = 0;
        buffAtk = 0;
        buffHp = 0;
        buffDef = 0;
        buffDex = 0;
        buffAgi = 0;

        debuffMp = 0;
        debuffAtk = 0;
        debuffHp = 0;
        debuffDef = 0;
        debuffDex = 0;
        debuffAgi = 0;

        hpGuage.Initialize(HP);
        if (damageText) damageText.gameObject.SetActive(false);
        if (healText) healText.gameObject.SetActive(false);
        if (buffText) buffText.gameObject.SetActive(false);
        if (criticalText) criticalText.gameObject.SetActive(false);

        _criticalProbability = criticalProbabilityInitial;

        state.Clear();
    }

    /// <summary>
    /// 行動
    /// </summary>
    public virtual void Move() { }

    /// <summary>
    /// 行動終了
    /// </summary>
    public virtual void MoveEnd() { }

    /// <summary>
    /// ターン終了
    /// </summary>
    public virtual void TurnEnd() { StateUpdate(); }

    /// <summary>
    /// 通常攻撃
    /// </summary>
    public virtual void NormalAttack() { }

    /// <summary>
    /// ダメージ
    /// </summary>
    /// <param name="_amount">ダメージ量</param>
    /// <returns>防御力分減少させたダメージ量</returns>
    public virtual int Damage(float _amount)
    {
        // 被ダメ - 防御力 を実際の被ダメージにする
        int damage = (int)Mathf.Ceil(_amount - (DEF * powerDef));

        if (this.GetComponent<Enemy>() != null) damage = damage < 0 ? 1 : damage; // 敵の場合は0未満なら1にする
        else damage = damage < 0 ? 0 : damage; // 0未満なら0にする

        currentHp -= damage;

        if (currentHp <= 0)
        {
            currentHp = 0;
            Dead();
        }

        // ゲージ減少演出
        hpGuage.Sub(damage);

        // ダメージ表示
        StartCoroutine(DispText(damageText, damage.ToString()));

        return damage;
    }

    /// <summary>
    /// ダメージ
    /// </summary>
    /// <param name="_amount">ダメージ量</param>
    /// <param name="cantGuard">防御無視かどうか　trueなら防御無視</param>
    /// <returns>防御力分減少させたダメージ量</returns>
    public virtual int Damage(float _amount, bool cantGuard)
    {
        // 被ダメ - 防御力 を実際の被ダメージにする
        // 防御無視のときは被ダメから防御力分減少させない
        int damage = cantGuard ? (int)Mathf.Ceil(_amount) : (int)Mathf.Ceil(_amount - (DEF * powerDef));

        if (this.GetComponent<Enemy>() != null) damage = damage < 0 ? 1 : damage; // 敵の場合は0未満なら1にする
        else damage = damage < 0 ? 0 : damage; // 0未満なら0にする

        currentHp -= damage;

        if (currentHp < 0)
        {
            currentHp = 0;
            Dead();
        }

        // ゲージ減少演出
        hpGuage.Sub(damage);

        // ダメージ表示
        StartCoroutine(DispText(damageText, damage.ToString()));

        return damage;
    }

    /// <summary>
    /// 固定ダメージ
    /// </summary>
    /// <param name="_amount">ダメージ量</param>
    /// <returns></returns>
    public virtual int Damage(int _amount)
    {
        int damage = _amount;

        if (this.GetComponent<Enemy>() != null) damage = damage < 0 ? 1 : damage; // 敵の場合は0未満なら1にする
        else damage = damage < 0 ? 0 : damage; // 0未満なら0にする

        currentHp -= damage;

        if (currentHp < 0)
        {
            currentHp = 0;
            Dead();
        }

        // ゲージ減少演出
        hpGuage.Sub(damage);

        // ダメージ表示
        StartCoroutine(DispText(damageText, damage.ToString()));

        return damage;
    }

    /// <summary>
    /// HP回復
    /// </summary>
    /// <param name="_amount">回復量</param>
    public virtual void HealHP(int _amount)
    {
        currentHp += _amount;
        if (currentHp > HP) currentHp = HP;
        hpGuage.Add(_amount);
    }

    /// <summary>
    /// 死亡
    /// </summary>
    public virtual void Dead() { }

    /// <summary>
    /// 状態追加
    /// </summary>
    /// <param name="_stateNumber">プレイヤーバフデバフ→スキル番号、 敵デバフ１→101、敵デバフ２→102、敵バフ→103</param>
    /// <param name="_isBuff">バフかどうか</param>
    /// <param name="_continuationTurn">効果終了時の処理</param>
    /// <param name="_isRestTurnUpdate">重ね掛け時、効果は重複せず、残りターンを更新するか</param>
    public void AddState(bool _isBuff, int _stateNumber, int _continuationTurn, float _value, Action _wearsOffAction, bool _isRestTurnUpdate)
    {
        State s = new();
        s.isBuff = _isBuff;
        s.stateId = _stateNumber;
        s.elapsedTurn = 1;
        s.continuationTurn = _continuationTurn;
        s.value = _value;
        s.werasOffAction = _wearsOffAction;
        s.lastingEffects = null;

        if (!_isRestTurnUpdate)
        {
            state.Add(s);
        }
        else
        {
            for (int i = 0; i < state.Count; i++)
            {
                if (state[i].stateId == _stateNumber)
                {
                    state[i].elapsedTurn = 1;

                    return;
                }
            }

            state.Add(s);
        }
    }

    /// <summary>
    /// 状態追加
    /// </summary>
    /// <param name="_stateNumber">プレイヤーバフデバフ→スキル番号、 敵デバフ１→101、敵デバフ２→102、敵バフ→103</param>
    /// <param name="_isBuff">バフかどうか</param>
    /// <param name="_continuationTurn">効果終了時の処理</param>
    /// <param name="_lastingEffects">ターン中持続する効果 ターン終了時に呼ばれる</param>
    /// <param name="_isRestTurnUpdate">重ね掛け時、効果は重複せず、残りターンを更新するか</param>
    public void AddState(bool _isBuff, int _stateNumber, int _continuationTurn, float _value, Action _wearsOffAction, Action _lastingEffects, bool _isRestTurnUpdate)
    {
        State s = new();
        s.isBuff = _isBuff;
        s.stateId = _stateNumber;
        s.elapsedTurn = 1;
        s.continuationTurn = _continuationTurn;
        s.value = _value;
        s.werasOffAction = _wearsOffAction;
        s.lastingEffects = _lastingEffects;

        if (!_isRestTurnUpdate)
        {
            state.Add(s);
        }
        else
        {
            for (int i = 0; i < state.Count; i++)
            {
                if (state[i].stateId == _stateNumber)
                {
                    state[i].elapsedTurn = 1;

                    return;
                }
                else continue;
            }

            state.Add(s);
        }
    }

    /// <summary>
    /// 状態更新
    /// </summary>
    public void StateUpdate()
    {
        for (int i = 0; i < state.Count; ++i)
        {
            state[i].elapsedTurn++;
            //Debug.Log($"test ID:{state[i].stateId} elapsedTurn:{state[i].elapsedTurn} continuationTurn:{state[i].continuationTurn}");

            // 解除
            if (state[i].elapsedTurn > state[i].continuationTurn)
            {
                state[i].werasOffAction?.Invoke();
                state.Remove(state[i]);

                StateUpdate();
            }

            else
            {
                if (state[i].lastingEffects != null)
                {
                    state[i].lastingEffects?.Invoke();
                }
            }
        }
    }

    /// <summary>
    /// 状態初期化
    /// </summary>
    public void ResetState()
    {
        state.Clear();
    }

    /// <summary>
    /// バフ追加
    /// </summary>
    /// <param name="_type">ステータスの種類</param>
    /// <param name="_amount">バフ量　割合表記</param>
    public virtual void AddBuff(StatusType _type, float _amount)
    {
        switch(_type)
        {
            case StatusType.HP:
                buffHp += _amount;
                break;
            case StatusType.MP:
                buffMp += _amount;
                break;
            case StatusType.ATK:
                buffAtk += _amount;
                break;
            case StatusType.DEF:
                buffDef += _amount;
                break;
            case StatusType.AGI:
                buffAgi += _amount;
                break;
            case StatusType.DEX:
                buffDex += _amount;
                break;
        }

        CalcPower();

        if (_amount > 0)
        {
            soundController.PlayBuffSE();
            string str = _type.ToString() + " " + (int)(_amount * 100) + " %UP";
            Color orange = new Color(1.0f, 0.56f, 0.0f, 1.0f);
            StartCoroutine(DispBuffText(buffText, str, orange, true));
        }
    }

    /// <summary>
    /// バフ状態リセット
    /// </summary>
    public void ResetBuff()
    {
        for (int i = 0; i < state.Count; i++)
        {
            if (state[i].isBuff)
            {
                state.Remove(state[i]);
            }
        }

        buffMp = 0;
        buffAtk = 0;
        buffHp = 0;
        buffDef = 0;
        buffDex = 0;
        buffAgi = 0;

        CalcPower();
    }

    /// <summary>
    /// デバフ追加
    /// </summary>
    /// <param name="_type">ステータスの種類</param>
    /// <param name="_amount">デバフ量</param>
    public virtual void AddDebuff(StatusType _type, float _amount)
    {
        switch (_type)
        {
            case StatusType.HP:
                debuffHp += _amount;
                break;
            case StatusType.MP:
                debuffMp += _amount;
                break;
            case StatusType.ATK:
                debuffAtk += _amount;
                break;
            case StatusType.DEF:
                debuffDef += _amount;
                break;
            case StatusType.AGI:
                debuffAgi += _amount;
                break;
            case StatusType.DEX:
                debuffDex += _amount;
                break;
        }

        CalcPower();

        if (_amount > 0)
        {
            soundController.PlayDebuffSE();
            string str = _type.ToString() + " " + (int)(_amount * 100) + " %DOWN";
            StartCoroutine(DispBuffText(buffText, str, Color.blue, false));
        }
    }

    /// <summary>
    /// デバフ状態リセット
    /// </summary>
    public void ResetDebuff()
    {
        for (int i = 0; i < state.Count; i++)
        {
            if (!state[i].isBuff)
            {
                state.Remove(state[i]);
            }
        }

        debuffMp = 0;
        debuffAtk = 0;
        debuffHp = 0;
        debuffDef = 0;
        debuffDex = 0;
        debuffAgi = 0;

        CalcPower();
    }

    /// <summary>
    /// ステータス倍率計算
    /// </summary>
    void CalcPower()
    {
        powerHp = 1 + buffHp - debuffHp;
        powerMp = 1 + buffMp - debuffMp;
        powerAtk = 1 + buffAtk - debuffAtk;
        powerDef = 1 + buffDef - debuffDef;
        powerAgi = 1 + buffAgi - debuffAgi;
        powerDex = 1 + buffDex - debuffDex;

        Debug.Log($"倍率 HP:{powerHp}, MP:{powerMp}, ATK:{powerAtk}, DEF:{powerDef}, AGI:{powerAgi}, DEX:{powerDex}");
    }

    /// <summary>
    /// 会心抽選
    /// </summary>
    public bool CriticalLottery()
    {
        int c = UnityEngine.Random.Range(0, 100);

        if (c < _criticalProbability)
        {
            critical = power_CriticalInit + buffCriticalPower;
            Debug.Log("会心発動");
            return true;
        }
        else
        {
            critical = 1.0f;
            return false;
        }
    }

    /// <summary>
    /// 会心抽選
    /// </summary>
    protected bool CriticalLottery(float criticalProbability)
    {
        int c = UnityEngine.Random.Range(0, 100);

        if (c < criticalProbability)
        {
            critical = power_CriticalInit + buffCriticalPower;
            Debug.Log("会心発動");
            return true;
        }
        else
        {
            critical = 1.0f;
            return false;
        }
    }

    /// <summary>
    /// 「Critical!」を表示する
    /// </summary>
    public void CriticalDamage()
    {
        StartCoroutine(DispText(criticalText, "Critical!"));
    }

    [SerializeField] private float deadMotionTime = 1.0f;
    [SerializeField] private float deadMotionDist = 100;

    /// <summary>
    /// 死亡演出再生
    /// </summary>
    /// <param name="_action"></param>
    /// <returns></returns>
    protected IEnumerator DeadMotion(System.Action _action)
    {
        yield return new WaitForSeconds(0.5f);

        float time = 0;
        float alpha = 1;

        while (time < deadMotionTime)
        {
            Vector3 pos = image.rectTransform.localPosition;
            pos.y -= (deadMotionDist / deadMotionTime * Time.deltaTime);
            image.rectTransform.localPosition = pos;

            alpha -= 1 / deadMotionTime * Time.deltaTime;
            image.color = new Color(1, 1, 1, alpha);

            time += Time.deltaTime;

            yield return null;
        }

        _action?.Invoke();
    }

    /// <summary>
    /// テキスト表示　ダメージ量等々
    /// </summary>
    /// <returns></returns>
    protected IEnumerator DispText(Text _text, string _str)
    {
        _text.text = _str;
        _text.gameObject.SetActive(true);

        yield return new WaitForSeconds(textDispTime);

        _text.gameObject.SetActive(false);
    }

    protected IEnumerator DispBuffText(Text _text, string _str, Color _color, bool _isBuff)
    {
        _text.text = _str;
        _text.gameObject.SetActive(true);
        _text.color = _color;

        var anim = _text.gameObject.GetComponent<Animator>();
        if (_isBuff)
        {
            anim.SetTrigger("Buff");
        }
        else
        {
            anim.SetTrigger("Debuff");
        }

        yield return new WaitForSeconds(textDispTime);

        _text.gameObject.SetActive(false);
    }

    protected IEnumerator HPGuageDirectionCompleteWait(Action _action)
    {
        yield return new WaitUntil(() => hpGuage.isDirectionCompleted);

        _action?.Invoke();
    }
}
