using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CharacterData", menuName = "Battle/CharacterData")]

public class CharacterData : MonoBehaviour
{
    public string characterName;
    public int maxHP;
    public int currentHP;
    public int attackPower;
    public int defensePower;
};