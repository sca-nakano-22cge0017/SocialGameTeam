using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStatus : MonoBehaviour
{
    public class Player
    {
        public int ID;
        public int hitpoint; //�̗�
        public int defense; //���
        public int attack; //�U��
        public int magicPawer; //����
        public int speed; //���x
        public int dexterity; //��p

        public Player(int ID, int hp, int def, int atk, int mgk, int spd, int dex)
        {
            hitpoint = hp;
            defense = def;
            attack = atk;
            magicPawer = spd;
            speed = spd;
            dexterity = dex;
        }
    }

    Player playerStatus = new Player(1, 100, 15, 15, 10, 30, 50);

    void Start()
    {
        Debug.Log(playerStatus.hitpoint);
    }

    void Update()
    {
        
    }
}
