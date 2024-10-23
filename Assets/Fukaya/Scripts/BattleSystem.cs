using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattleSystem : MonoBehaviour
{
    public GameObject player; // プレイヤーオブジェクト
    public GameObject enemy;  // 敵オブジェクト

    public int playerMaxHP = 100;
    public int enemyMaxHP = 100;

    private int playerCurrentHP;
    private int enemyCurrentHP;

    public Text battleText;   // UIテキスト（戦闘の状況表示）
    public Button attackButton;  // プレイヤーの攻撃ボタン
    public Button defenceButton;  // プレイヤーの防御ボタン

    private bool playerTurn = true; // プレイヤーのターンかどうか
    private bool battleOver = false; // 戦闘が終了しているかどうか

    [SerializeField] private WindowController windowController;
    [SerializeField] private DropController dropController;

    void Start()
    {
        playerCurrentHP = playerMaxHP;
        enemyCurrentHP = enemyMaxHP;

        attackButton.onClick.AddListener(OnAttackButton); // ボタンのクリックイベントを設定
        UpdateBattleText("プレイヤーのターン");
    }

    void OnAttackButton()
    {
        if (battleOver) return; // 戦闘終了後はボタンを無効に

        if (playerTurn)
        {
            // プレイヤーの攻撃
            int damage = Random.Range(10, 20);
            enemyCurrentHP -= damage;
            UpdateBattleText("プレイヤーの攻撃！ 敵に " + damage + " のダメージ！");

            if (enemyCurrentHP <= 0)
            {
                enemyCurrentHP = 0;
                battleOver = true;
                UpdateBattleText("敵を倒した！ 勝利！");

                dropController.DropLottery();
                windowController.Open();
            }
            else
            {
                playerTurn = false; // 敵のターンに移行
                StartCoroutine(EnemyTurn());
            }
        }
    }

    //void onDefenceButton()
    //{
    //    if (battleOver) return; // 戦闘終了後はボタンを無効に

    //    if(playerTurn)
    //    {
    //        // プレイヤーの防御
    //        int decreasedamage = Random.Range(5,10);
    //        UpdateBattleText("プレイヤーの防御！ 敵の攻撃を " + decreasedamage + " に軽減した！");
    //    }
    //}


    IEnumerator EnemyTurn()
    {
        Debug.Log("敵のターンが開始されました");
        yield return new WaitForSeconds(1f); // 少し待機

        if (!battleOver)
        {
            int damage = Random.Range(15, 25);
            playerCurrentHP -= damage;
            UpdateBattleText("敵の攻撃！ プレイヤーに " + damage + " のダメージ！");
            Debug.Log("Player HP after attack: " + playerCurrentHP);

            yield return new WaitForSeconds(2f); // 少し待機

            if (playerCurrentHP <= 0)
            {
                playerCurrentHP = 0;
                battleOver = true;
                UpdateBattleText("プレイヤーが倒された... 敗北");

                windowController.Open();
            }
            else
            {
                playerTurn = true; // 再びプレイヤーのターンに
                UpdateBattleText("プレイヤーのターン");
                Debug.Log("Switched to player turn");
            }
        }
    }

    void UpdateBattleText(string message)
    {
        battleText.text = message; // バトルの状況をテキストに反映
    }
}
