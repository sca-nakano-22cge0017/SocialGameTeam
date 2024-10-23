using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattleSystem : MonoBehaviour
{
    public GameObject player; // �v���C���[�I�u�W�F�N�g
    public GameObject enemy;  // �G�I�u�W�F�N�g

    public int playerMaxHP = 100;
    public int enemyMaxHP = 100;

    private int playerCurrentHP;
    private int enemyCurrentHP;

    public Text battleText;   // UI�e�L�X�g�i�퓬�̏󋵕\���j
    public Button attackButton;  // �v���C���[�̍U���{�^��
    public Button defenceButton;  // �v���C���[�̖h��{�^��

    private bool playerTurn = true; // �v���C���[�̃^�[�����ǂ���
    private bool battleOver = false; // �퓬���I�����Ă��邩�ǂ���

    [SerializeField] private WindowController windowController;
    [SerializeField] private DropController dropController;

    void Start()
    {
        playerCurrentHP = playerMaxHP;
        enemyCurrentHP = enemyMaxHP;

        attackButton.onClick.AddListener(OnAttackButton); // �{�^���̃N���b�N�C�x���g��ݒ�
        UpdateBattleText("�v���C���[�̃^�[��");
    }

    void OnAttackButton()
    {
        if (battleOver) return; // �퓬�I����̓{�^���𖳌���

        if (playerTurn)
        {
            // �v���C���[�̍U��
            int damage = Random.Range(10, 20);
            enemyCurrentHP -= damage;
            UpdateBattleText("�v���C���[�̍U���I �G�� " + damage + " �̃_���[�W�I");

            if (enemyCurrentHP <= 0)
            {
                enemyCurrentHP = 0;
                battleOver = true;
                UpdateBattleText("�G��|�����I �����I");

                dropController.DropLottery();
                windowController.Open();
            }
            else
            {
                playerTurn = false; // �G�̃^�[���Ɉڍs
                StartCoroutine(EnemyTurn());
            }
        }
    }

    //void onDefenceButton()
    //{
    //    if (battleOver) return; // �퓬�I����̓{�^���𖳌���

    //    if(playerTurn)
    //    {
    //        // �v���C���[�̖h��
    //        int decreasedamage = Random.Range(5,10);
    //        UpdateBattleText("�v���C���[�̖h��I �G�̍U���� " + decreasedamage + " �Ɍy�������I");
    //    }
    //}


    IEnumerator EnemyTurn()
    {
        Debug.Log("�G�̃^�[�����J�n����܂���");
        yield return new WaitForSeconds(1f); // �����ҋ@

        if (!battleOver)
        {
            int damage = Random.Range(15, 25);
            playerCurrentHP -= damage;
            UpdateBattleText("�G�̍U���I �v���C���[�� " + damage + " �̃_���[�W�I");
            Debug.Log("Player HP after attack: " + playerCurrentHP);

            yield return new WaitForSeconds(2f); // �����ҋ@

            if (playerCurrentHP <= 0)
            {
                playerCurrentHP = 0;
                battleOver = true;
                UpdateBattleText("�v���C���[���|���ꂽ... �s�k");

                windowController.Open();
            }
            else
            {
                playerTurn = true; // �Ăуv���C���[�̃^�[����
                UpdateBattleText("�v���C���[�̃^�[��");
                Debug.Log("Switched to player turn");
            }
        }
    }

    void UpdateBattleText(string message)
    {
        battleText.text = message; // �o�g���̏󋵂��e�L�X�g�ɔ��f
    }
}
