using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Character_BaseProperties", menuName = "Realiable Script/Create a new Character Base Properties")]
public class Character_BaseProperties : ScriptableObject
{
    public int characterLevel = 1;
    public int characterHealthPoint = 100;
    public float movementSpeed = 1.0f;
    public float attackSpeed = 1.0f;
    public float attackPower = 1.0f;
}
