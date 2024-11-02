using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{

    public GameObject enemy; // エネミーオブジェクト

    public int DIFFICULTY; // 難易度
    public int AREA; // 育成orボス
    public int STAGEID; // ステージID

    public int TOTAL; // 敵の数
    public int POSITION; // 敵の位置
    public int e_ID; // 敵ID

    public int ATK; // 攻撃力
    public int MP; // 魔力
    public int HP; // 体力
    public int DEF; // 防御力
    public int SPEED; // 速度
    public int DEX; // 器用

    public int ATTACKPATTERN; // 攻撃方法ID


    void Start()
    {
        
    }
    void Update()
    {
        
    }
}
