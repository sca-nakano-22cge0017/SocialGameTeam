using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 掛かっているバフ、デバフを表示
/// </summary>
public class BuffDisplay : MonoBehaviour
{
    [SerializeField] WindowController buffWindow_Player;

    [SerializeField] GameObject prefab_Active;
    [SerializeField] GameObject parent_Active;
    private List<GameObject> explains_Active = new();

    [SerializeField] GameObject prefab_Passive;
    [SerializeField] GameObject parent_Passive;
    private int effectsAmount_Passive = 0;

    [SerializeField] WindowController buffWindow_Enemy;
    [SerializeField] GameObject parent_Enemy;
    private List<GameObject> explains_Enemy = new();

    SpecialTecniqueManager specialTecniqueManager;

    [SerializeField] Sprite buff_ATK;
    [SerializeField] Sprite buff_DEF;
    [SerializeField] Sprite buff_AGI;
    [SerializeField] Sprite debuff_ATK;
    [SerializeField] Sprite debuff_DEF;
    [SerializeField] Sprite debuff_AGI;
    [SerializeField] Sprite empty;

    [SerializeField] private float longTapTime = 0.5f;
    private bool isTapping = false;
    private float tapTime = 0;

    [SerializeField] private float blinkingCoolTime = 1.0f;

    private bool isPlayer = false;
    private GameObject tappingChara = null;
    private Character selectChara = null;

    void Start()
    {
        specialTecniqueManager = GameObject.FindObjectOfType<SpecialTecniqueManager>();
        PassiveSkill();

        tappingChara = null;

        explains_Active.Clear();
        explains_Enemy.Clear();
    }

    void Update()
    {
        if (isTapping)
        {
            tapTime += Time.deltaTime;
        }

        if (tapTime >= longTapTime)
        {
            if (isPlayer)
            {
                PlayerState();
                buffWindow_Player.Open();
            }
            else
            {
                EnemyState();
                buffWindow_Enemy.Open();
            }

            tapTime = 0;
            isTapping = false;
        }
    }

    /// <summary>
    /// 情報更新
    /// </summary>
    public void UpdateInformation()
    {
        if (isPlayer)
        {
            PlayerState();
        }
        else
        {
            EnemyState();
        }
    }

    /// <summary>
    /// プレイヤーのバフデバフ表示
    /// </summary>
    void PlayerState()
    {
        for (int i = 0; i < explains_Active.Count; i++)
        {
            explains_Active[i].SetActive(false);
        }

        if (selectChara == null) return;

        var rect = parent_Active.gameObject.GetComponent<RectTransform>();
        var size = rect.sizeDelta;
        size.y = selectChara.state.Count * 140.0f;
        rect.sizeDelta = size;

        for (int i = 0; i < selectChara.state.Count; i++)
        {
            State s = selectChara.state[i];

            GameObject obj;
            
            //数足りなければ追加生成
            if (explains_Active.Count - 1 < i)
            {
                obj = Instantiate(prefab_Active, parent_Active.transform);
                explains_Active.Add(obj);
            }
            else
            {
                obj = explains_Active[i];
                obj.SetActive(true);
            }

            var icon = obj.transform.GetChild(0).gameObject.GetComponent<Image>();
            var name = obj.transform.GetChild(1).gameObject.GetComponent<Text>();
            var turn = obj.transform.GetChild(2).gameObject.GetComponent<Text>();
            var explain = obj.transform.GetChild(3).gameObject.GetComponent<Text>();

            turn.text = "残り" + (s.continuationTurn - s.elapsedTurn + 1) + "ターン";
            if (s.stateId == 4 || s.stateId == 12)
            {
                // 不倒の構え、背水の陣によるバフはターン表示なし
                turn.enabled = false;
            }
            else turn.enabled = true;

            // 敵からのデバフ
            if (s.stateId == 101 || s.stateId == 102 || s.stateId == 103)
            {
                switch(s.stateId)
                {
                    case 101:
                        icon.sprite = debuff_DEF;
                        name.text = "防御力ダウン";
                        explain.text = "防御力" + s.value + "%ダウン";
                        break;
                    case 102:
                        icon.sprite = debuff_ATK;
                        name.text = "攻撃力ダウン";
                        explain.text = "攻撃力" + s.value + "%ダウン";
                        break;
                    default:
                        break;
                }
            }

            // スキル以外の自己バフ
            // 201→ガード
            else if (s.stateId == 201)
            {
                icon.sprite = buff_DEF;
                name.text = "防御力アップ";
                explain.text = "防御力" + s.value + "%アップ";
            }

            // 自身のバフ
            else
            {
                var info = GetSkillInformation(s.stateId);

                icon.sprite = BuffIcon(info);
                name.text = info.m_name;
                explain.text = TextEdit(info.m_effects, false, s.stateId);

                // 背水の陣
                if (s.stateId == 12)
                {
                    explain.text = "攻撃力" + s.value + "%アップ";
                }
                // オーラ
                if (s.stateId == 16)
                {
                    icon.sprite = buff_ATK;
                    var buffObj = obj.GetComponent<BuffObject>();
                    List<Sprite> sprites = new();
                    sprites.Add(buff_ATK);
                    sprites.Add(buff_DEF);

                    buffObj.IconChangeAlternately(sprites, blinkingCoolTime);
                }
                else
                {
                    var buffObj = obj.GetComponent<BuffObject>();
                    buffObj.isIconChangeAlternately = false;
                }
            }
        }
    }

    /// <summary>
    /// 解放済みパッシブスキルの効果表示
    /// </summary>
    void PassiveSkill()
    {
        for (int i = 0; i < specialTecniqueManager.specialTecniques.Length; i++)
        {
            var skill = specialTecniqueManager.specialTecniques[i];
            
            if (skill.m_released && skill.m_skillType == 2)
            {
                WindowSizeChange_Passive();
                var obj = Instantiate(prefab_Passive, parent_Passive.transform);

                var name = obj.transform.GetChild(0).gameObject.GetComponent<Text>();
                var explain = obj.transform.GetChild(1).gameObject.GetComponent<Text>();

                name.text = skill.m_name;
                explain.text = skill.m_effects;
            }
        }
    }

    /// <summary>
    /// 敵のバフデバフ表示
    /// </summary>
    void EnemyState()
    {
        for (int i = 0; i < explains_Enemy.Count; i++)
        {
            explains_Enemy[i].SetActive(false);
        }

        if (selectChara == null) return;

        var rect = parent_Enemy.gameObject.GetComponent<RectTransform>();
        var size = rect.sizeDelta;
        size.y = selectChara.state.Count * 140.0f;
        rect.sizeDelta = size;

        for (int i = 0; i < selectChara.state.Count; i++)
        {
            State s = selectChara.state[i];

            GameObject obj;
            // 数足りなければ追加生成
            if (explains_Enemy.Count - 1 < i)
            {
                obj = Instantiate(prefab_Active, parent_Enemy.transform);
                explains_Enemy.Add(obj);
            }
            else
            {
                obj = explains_Enemy[i];
                obj.SetActive(true);
            }

            var icon = obj.transform.GetChild(0).gameObject.GetComponent<Image>();
            var name = obj.transform.GetChild(1).gameObject.GetComponent<Text>();
            var turn = obj.transform.GetChild(2).gameObject.GetComponent<Text>();
            var explain = obj.transform.GetChild(3).gameObject.GetComponent<Text>();

            turn.text = "残り" + (s.continuationTurn - s.elapsedTurn + 1) + "ターン";

            // 敵バフデバフ
            if (s.stateId == 101 || s.stateId == 102 || s.stateId == 103)
            {
                switch (s.stateId)
                {
                    case 101:
                        icon.sprite = debuff_DEF;
                        name.text = "防御力ダウン";
                        explain.text = "防御力" + s.value + "%ダウン";
                        break;
                    case 102:
                        icon.sprite = debuff_ATK;
                        name.text = "攻撃力ダウン";
                        explain.text = "攻撃力" + s.value + "%ダウン";
                        break;
                    case 103:
                        icon.sprite = buff_ATK;
                        name.text = "攻撃力アップ";
                        explain.text = "攻撃力" + s.value + "%アップ";
                        break;
                    default:
                        break;
                }
            }

            // プレイヤーバフデバフ
            else
            {
                var info = GetSkillInformation(s.stateId);
                
                icon.sprite = BuffIcon(info);
                name.text = info.m_name;

                var str = TextEdit(info.m_effects, true, s.stateId);
                explain.text = str;

                // ガードブレイカー
                if (s.stateId == 13)
                {
                    explain.text = "防御力" + s.value + "%ダウン";
                }
            }
        }
    }

    void WindowSizeChange_Passive()
    {
        effectsAmount_Passive++;
        var rect = parent_Passive.gameObject.GetComponent<RectTransform>();
        var size = rect.sizeDelta;
        size.y = effectsAmount_Passive * 140.0f;
        rect.sizeDelta = size;
    }

    public void PointerDown(GameObject _obj)
    {
        if (isTapping) return;

        isTapping = true;
        tapTime = 0;
        tappingChara = _obj;
        selectChara = _obj.GetComponent<Character>();

        if (_obj.GetComponent<PlayerData>()) isPlayer = true;
        else isPlayer = false;
    }

    public void PointerUp(GameObject _obj)
    {
        if (tappingChara == null || _obj != tappingChara) return;
        isTapping = false;
        tapTime = 0;
        tappingChara = null;
    }

    public void Close()
    {
        buffWindow_Player.Close();
        buffWindow_Enemy.Close();

        isTapping = false;
        tapTime = 0;
        tappingChara = null;
    }

    SpecialTecnique GetSkillInformation(int _id)
    {
        for (int i = 0; i < specialTecniqueManager.specialTecniques.Length; i++)
        {
            var skill = specialTecniqueManager.specialTecniques[i];
            if (skill.m_id == _id)
            {
                return skill;
            }
        }

        return null;
    }

    /// <summary>
    /// 説明文の調整
    /// </summary>
    string TextEdit(string _text, bool _isEnemy, int _id)
    {
        string str = _text;
        var info = GetSkillInformation(_id);

        // 「nターンの間、」を省略する
        if (str.Contains("ターンの間、"))
        {
            string str1 = str.Remove(0, 1);
            string str2 = str1.Replace("ターンの間、", "");
            str = str2;
        }

        if (_isEnemy)
        {
            // 「敵の」を省略する
            if (str.Contains("敵の"))
            {
                string str1 = str.Replace("敵の", "");
                str = str1;
            }
        }

        // ガードクラッシュ
        if (_id == 26)
        {
            str = "防御力" + info.m_value2 + "%ダウン";
        }

        // 呪い
        if (_id == 19)
        {
            var value = info.m_value2;
            str = "毎ターン" + value + "%HPを減少させる";
        }

        return str;
    }

    Sprite BuffIcon(SpecialTecnique _info)
    {
        Sprite s = _info.m_illust;
        int id = _info.m_id;

        // 攻撃アップ
        if (id == 4 || id == 11 || id == 12 || id == 14)
        {
            // 不倒の構え、ピアス、背水の陣、全身全霊
            s = buff_ATK;
        }
        // 防御アップ
        if (id == 8 || id == 10)
        {
            // 無敵、守護神の権能
            s = buff_DEF;
        }
        // 速度アップ
        if (id == 21)
        {
            // 加速
            s = buff_AGI;
        }

        // 防御ダウン
        if (id == 13 || id == 26)
        {
            // ガードブレイカー、ガードクラッシュ
            s = debuff_DEF;
        }
        // 速度ダウン
        if (id == 22)
        {
            // スロウ
            s = debuff_AGI;
        }

        // アイコン無し
        if (id == 19)
        {
            // 呪い
            s = empty;
        }

        return s;
    }
}
