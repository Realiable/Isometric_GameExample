using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyInformation : CharacterInformation
{
    [Header("Enemy Behavior Setting")]
    [Range(0.0f, 1.0f)]
    public float rotateToTargetSpeed = 1.0f;
    public float enemyAttackDelay = 1.0f;
}
