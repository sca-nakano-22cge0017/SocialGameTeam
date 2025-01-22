using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// �|�����Ă���o�t�A�f�o�t��\��
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
    /// ���X�V
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
    /// �v���C���[�̃o�t�f�o�t�\��
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
            
            //������Ȃ���Βǉ�����
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

            turn.text = "�c��" + (s.continuationTurn - s.elapsedTurn + 1) + "�^�[��";
            if (s.stateId == 4 || s.stateId == 12)
            {
                // �s�|�̍\���A�w���̐w�ɂ��o�t�̓^�[���\���Ȃ�
                turn.enabled = false;
            }
            else turn.enabled = true;

            // �G����̃f�o�t
            if (s.stateId == 101 || s.stateId == 102 || s.stateId == 103)
            {
                switch(s.stateId)
                {
                    case 101:
                        icon.sprite = debuff_DEF;
                        name.text = "�h��̓_�E��";
                        explain.text = "�h���" + s.value + "%�_�E��";
                        break;
                    case 102:
                        icon.sprite = debuff_ATK;
                        name.text = "�U���̓_�E��";
                        explain.text = "�U����" + s.value + "%�_�E��";
                        break;
                    default:
                        break;
                }
            }

            // �X�L���ȊO�̎��ȃo�t
            // 201���K�[�h
            else if (s.stateId == 201)
            {
                icon.sprite = buff_DEF;
                name.text = "�h��̓A�b�v";
                explain.text = "�h���" + s.value + "%�A�b�v";
            }

            // ���g�̃o�t
            else
            {
                var info = GetSkillInformation(s.stateId);

                icon.sprite = BuffIcon(info);
                name.text = info.m_name;
                explain.text = TextEdit(info.m_effects, false, s.stateId);

                // �w���̐w
                if (s.stateId == 12)
                {
                    explain.text = "�U����" + s.value + "%�A�b�v";
                }
                // �I�[��
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
    /// ����ς݃p�b�V�u�X�L���̌��ʕ\��
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
    /// �G�̃o�t�f�o�t�\��
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
            // ������Ȃ���Βǉ�����
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

            turn.text = "�c��" + (s.continuationTurn - s.elapsedTurn + 1) + "�^�[��";

            // �G�o�t�f�o�t
            if (s.stateId == 101 || s.stateId == 102 || s.stateId == 103)
            {
                switch (s.stateId)
                {
                    case 101:
                        icon.sprite = debuff_DEF;
                        name.text = "�h��̓_�E��";
                        explain.text = "�h���" + s.value + "%�_�E��";
                        break;
                    case 102:
                        icon.sprite = debuff_ATK;
                        name.text = "�U���̓_�E��";
                        explain.text = "�U����" + s.value + "%�_�E��";
                        break;
                    case 103:
                        icon.sprite = buff_ATK;
                        name.text = "�U���̓A�b�v";
                        explain.text = "�U����" + s.value + "%�A�b�v";
                        break;
                    default:
                        break;
                }
            }

            // �v���C���[�o�t�f�o�t
            else
            {
                var info = GetSkillInformation(s.stateId);
                
                icon.sprite = BuffIcon(info);
                name.text = info.m_name;

                var str = TextEdit(info.m_effects, true, s.stateId);
                explain.text = str;

                // �K�[�h�u���C�J�[
                if (s.stateId == 13)
                {
                    explain.text = "�h���" + s.value + "%�_�E��";
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
    /// �������̒���
    /// </summary>
    string TextEdit(string _text, bool _isEnemy, int _id)
    {
        string str = _text;
        var info = GetSkillInformation(_id);

        // �un�^�[���̊ԁA�v���ȗ�����
        if (str.Contains("�^�[���̊ԁA"))
        {
            string str1 = str.Remove(0, 1);
            string str2 = str1.Replace("�^�[���̊ԁA", "");
            str = str2;
        }

        if (_isEnemy)
        {
            // �u�G�́v���ȗ�����
            if (str.Contains("�G��"))
            {
                string str1 = str.Replace("�G��", "");
                str = str1;
            }
        }

        // �K�[�h�N���b�V��
        if (_id == 26)
        {
            str = "�h���" + info.m_value2 + "%�_�E��";
        }

        // ��
        if (_id == 19)
        {
            var value = info.m_value2;
            str = "���^�[��" + value + "%HP������������";
        }

        return str;
    }

    Sprite BuffIcon(SpecialTecnique _info)
    {
        Sprite s = _info.m_illust;
        int id = _info.m_id;

        // �U���A�b�v
        if (id == 4 || id == 11 || id == 12 || id == 14)
        {
            // �s�|�̍\���A�s�A�X�A�w���̐w�A�S�g�S��
            s = buff_ATK;
        }
        // �h��A�b�v
        if (id == 8 || id == 10)
        {
            // ���G�A���_�̌��\
            s = buff_DEF;
        }
        // ���x�A�b�v
        if (id == 21)
        {
            // ����
            s = buff_AGI;
        }

        // �h��_�E��
        if (id == 13 || id == 26)
        {
            // �K�[�h�u���C�J�[�A�K�[�h�N���b�V��
            s = debuff_DEF;
        }
        // ���x�_�E��
        if (id == 22)
        {
            // �X���E
            s = debuff_AGI;
        }

        // �A�C�R������
        if (id == 19)
        {
            // ��
            s = empty;
        }

        return s;
    }
}
