using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStatus : MonoBehaviour
{
    public class Player
    {
        public int ID;
        public int hitpoint; //‘Ì—Í
        public int defense; //ç”õ
        public int attack; //UŒ‚
        public int magicPawer; //–‚—Í
        public int speed; //‘¬“x
        public int dexterity; //Ší—p

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
