using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Character_BaseProperties", menuName = "Realiable Script/Create a new Character Base Properties")]
public class Character_BaseProperties : ScriptableObject
{
    public int CharacterLevel = 1;
    public int CharacterHealthPoint = 100;
    public float MovementSpeed = 1.0f;
    public float AttackSpeed = 1.0f;
    public float AttackPower = 1.0f;
}
