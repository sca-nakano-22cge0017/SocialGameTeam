using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

/// <summary>
/// �o�t�A�f�o�t�A����X�L���������Ȃǂ̓�����
/// </summary>
public class State
{
    /// <summary>
    /// �o�t���ǂ���
    /// </summary>
    public bool isBuff;

    /// <summary>
    /// �ǂ̓����Ԃ�
    /// �v���C���[�o�t�f�o�t���X�L���ԍ�
    /// �G�f�o�t�P��101�A�G�f�o�t�Q��102�A�G�o�t��103
    /// </summary>
    public int stateId;

    /// <summary>
    /// �p���^�[��
    /// </summary>
    public int continuationTurn;

    /// <summary>
    /// �o�߃^�[��
    /// </summary>
    public int elapsedTurn;

    /// <summary>
    /// ���ʗ�
    /// </summary>
    public float value;

    /// <summary>
    /// ���ʏI�����̏���
    /// </summary>
    public Action werasOffAction;

    /// <summary>
    /// �������ʁ@���ʏI���܂Ń^�[���I�����ɌĂяo��
    /// </summary>
    public Action lastingEffects;
}

/// <summary>
/// ���C���Q�[����œ����L�����N�^�[�@PlayerData��Enemy�Ɍp��
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

    [SerializeField, Header("�e�L�X�g�\������")] protected float textDispTime;

    // �X�e�[�^�X
    [HideInInspector] public int ATK; // �U��
    [HideInInspector] public int MP;  // ����
    [HideInInspector] public int HP;  // �̗�
    [HideInInspector] public int DEF; // �h��
    [HideInInspector] public int AGI; // ���x
    [HideInInspector] public int DEX; // ��p

    // �v�Z�p ���ݒl
    [HideInInspector] public int currentMp;
    [HideInInspector] public int currentHp;

    // �X�e�[�^�X�{��
    [HideInInspector] public float powerMp ;
    [HideInInspector] public float powerAtk;
    [HideInInspector] public float powerHp ;
    [HideInInspector] public float powerDef;
    [HideInInspector] public float powerDex;
    [HideInInspector] public float powerAgi;

    // �o�t���� ����
    [HideInInspector] public float buffMp ;
    [HideInInspector] public float buffAtk;
    [HideInInspector] public float buffHp ;
    [HideInInspector] public float buffDef;
    [HideInInspector] public float buffDex;
    [HideInInspector] public float buffAgi;

    // �f�o�t���� ����
    [HideInInspector] public float debuffMp ;
    [HideInInspector] public float debuffAtk;
    [HideInInspector] public float debuffHp ;
    [HideInInspector] public float debuffDef;
    [HideInInspector] public float debuffDex;
    [HideInInspector] public float debuffAgi;

    // �o�t�f�o�t
    public List<State> state = new();

    // ��S��
    public float criticalProbabilityInitial;  // �����l %
    public float _criticalProbability;        // �v�Z�p %

    public float power_CriticalInit;    // ��{��S���{�� �{
    public float buffCriticalPower;     // ��S���{���o�t ��
    public float critical;    // ��S���{���@�v�Z�p

    // ���[�V�����֌W
    public Animator motion;
    public SpineAnim spineAnim;

    private void Start()
    {
        specialTecniqueManager = FindObjectOfType<SpecialTecniqueManager>();
    }

    /// <summary>
    /// ������
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
    /// �s��
    /// </summary>
    public virtual void Move() { }

    /// <summary>
    /// �s���I��
    /// </summary>
    public virtual void MoveEnd() { }

    /// <summary>
    /// �^�[���I��
    /// </summary>
    public virtual void TurnEnd() { StateUpdate(); }

    /// <summary>
    /// �ʏ�U��
    /// </summary>
    public virtual void NormalAttack() { }

    /// <summary>
    /// �_���[�W
    /// </summary>
    /// <param name="_amount">�_���[�W��</param>
    /// <returns>�h��͕������������_���[�W��</returns>
    public virtual int Damage(float _amount)
    {
        // ��_�� - �h��� �����ۂ̔�_���[�W�ɂ���
        int damage = (int)Mathf.Ceil(_amount - (DEF * powerDef));

        if (this.GetComponent<Enemy>() != null) damage = damage < 0 ? 1 : damage; // �G�̏ꍇ��0�����Ȃ�1�ɂ���
        else damage = damage < 0 ? 0 : damage; // 0�����Ȃ�0�ɂ���

        currentHp -= damage;

        if (currentHp <= 0)
        {
            currentHp = 0;
            Dead();
        }

        // �Q�[�W�������o
        hpGuage.Sub(damage);

        // �_���[�W�\��
        StartCoroutine(DispText(damageText, damage.ToString()));

        return damage;
    }

    /// <summary>
    /// �_���[�W
    /// </summary>
    /// <param name="_amount">�_���[�W��</param>
    /// <param name="cantGuard">�h�䖳�����ǂ����@true�Ȃ�h�䖳��</param>
    /// <returns>�h��͕������������_���[�W��</returns>
    public virtual int Damage(float _amount, bool cantGuard)
    {
        // ��_�� - �h��� �����ۂ̔�_���[�W�ɂ���
        // �h�䖳���̂Ƃ��͔�_������h��͕����������Ȃ�
        int damage = cantGuard ? (int)Mathf.Ceil(_amount) : (int)Mathf.Ceil(_amount - (DEF * powerDef));

        if (this.GetComponent<Enemy>() != null) damage = damage < 0 ? 1 : damage; // �G�̏ꍇ��0�����Ȃ�1�ɂ���
        else damage = damage < 0 ? 0 : damage; // 0�����Ȃ�0�ɂ���

        currentHp -= damage;

        if (currentHp < 0)
        {
            currentHp = 0;
            Dead();
        }

        // �Q�[�W�������o
        hpGuage.Sub(damage);

        // �_���[�W�\��
        StartCoroutine(DispText(damageText, damage.ToString()));

        return damage;
    }

    /// <summary>
    /// �Œ�_���[�W
    /// </summary>
    /// <param name="_amount">�_���[�W��</param>
    /// <returns></returns>
    public virtual int Damage(int _amount)
    {
        int damage = _amount;

        if (this.GetComponent<Enemy>() != null) damage = damage < 0 ? 1 : damage; // �G�̏ꍇ��0�����Ȃ�1�ɂ���
        else damage = damage < 0 ? 0 : damage; // 0�����Ȃ�0�ɂ���

        currentHp -= damage;

        if (currentHp < 0)
        {
            currentHp = 0;
            Dead();
        }

        // �Q�[�W�������o
        hpGuage.Sub(damage);

        // �_���[�W�\��
        StartCoroutine(DispText(damageText, damage.ToString()));

        return damage;
    }

    /// <summary>
    /// HP��
    /// </summary>
    /// <param name="_amount">�񕜗�</param>
    public virtual void HealHP(int _amount)
    {
        currentHp += _amount;
        if (currentHp > HP) currentHp = HP;
        hpGuage.Add(_amount);
    }

    /// <summary>
    /// ���S
    /// </summary>
    public virtual void Dead() { }

    /// <summary>
    /// ��Ԓǉ�
    /// </summary>
    /// <param name="_stateNumber">�v���C���[�o�t�f�o�t���X�L���ԍ��A �G�f�o�t�P��101�A�G�f�o�t�Q��102�A�G�o�t��103</param>
    /// <param name="_isBuff">�o�t���ǂ���</param>
    /// <param name="_continuationTurn">���ʏI�����̏���</param>
    /// <param name="_isRestTurnUpdate">�d�ˊ|�����A���ʂ͏d�������A�c��^�[�����X�V���邩</param>
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
    /// ��Ԓǉ�
    /// </summary>
    /// <param name="_stateNumber">�v���C���[�o�t�f�o�t���X�L���ԍ��A �G�f�o�t�P��101�A�G�f�o�t�Q��102�A�G�o�t��103</param>
    /// <param name="_isBuff">�o�t���ǂ���</param>
    /// <param name="_continuationTurn">���ʏI�����̏���</param>
    /// <param name="_lastingEffects">�^�[��������������� �^�[���I�����ɌĂ΂��</param>
    /// <param name="_isRestTurnUpdate">�d�ˊ|�����A���ʂ͏d�������A�c��^�[�����X�V���邩</param>
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
    /// ��ԍX�V
    /// </summary>
    public void StateUpdate()
    {
        for (int i = 0; i < state.Count; ++i)
        {
            state[i].elapsedTurn++;
            //Debug.Log($"test ID:{state[i].stateId} elapsedTurn:{state[i].elapsedTurn} continuationTurn:{state[i].continuationTurn}");

            // ����
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
    /// ��ԏ�����
    /// </summary>
    public void ResetState()
    {
        state.Clear();
    }

    /// <summary>
    /// �o�t�ǉ�
    /// </summary>
    /// <param name="_type">�X�e�[�^�X�̎��</param>
    /// <param name="_amount">�o�t�ʁ@�����\�L</param>
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
    /// �o�t��ԃ��Z�b�g
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
    /// �f�o�t�ǉ�
    /// </summary>
    /// <param name="_type">�X�e�[�^�X�̎��</param>
    /// <param name="_amount">�f�o�t��</param>
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
    /// �f�o�t��ԃ��Z�b�g
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
    /// �X�e�[�^�X�{���v�Z
    /// </summary>
    void CalcPower()
    {
        powerHp = 1 + buffHp - debuffHp;
        powerMp = 1 + buffMp - debuffMp;
        powerAtk = 1 + buffAtk - debuffAtk;
        powerDef = 1 + buffDef - debuffDef;
        powerAgi = 1 + buffAgi - debuffAgi;
        powerDex = 1 + buffDex - debuffDex;

        Debug.Log($"�{�� HP:{powerHp}, MP:{powerMp}, ATK:{powerAtk}, DEF:{powerDef}, AGI:{powerAgi}, DEX:{powerDex}");
    }

    /// <summary>
    /// ��S���I
    /// </summary>
    public bool CriticalLottery()
    {
        int c = UnityEngine.Random.Range(0, 100);

        if (c < _criticalProbability)
        {
            critical = power_CriticalInit + buffCriticalPower;
            Debug.Log("��S����");
            return true;
        }
        else
        {
            critical = 1.0f;
            return false;
        }
    }

    /// <summary>
    /// ��S���I
    /// </summary>
    protected bool CriticalLottery(float criticalProbability)
    {
        int c = UnityEngine.Random.Range(0, 100);

        if (c < criticalProbability)
        {
            critical = power_CriticalInit + buffCriticalPower;
            Debug.Log("��S����");
            return true;
        }
        else
        {
            critical = 1.0f;
            return false;
        }
    }

    /// <summary>
    /// �uCritical!�v��\������
    /// </summary>
    public void CriticalDamage()
    {
        StartCoroutine(DispText(criticalText, "Critical!"));
    }

    [SerializeField] private float deadMotionTime = 1.0f;
    [SerializeField] private float deadMotionDist = 100;

    /// <summary>
    /// ���S���o�Đ�
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
    /// �e�L�X�g�\���@�_���[�W�ʓ��X
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
