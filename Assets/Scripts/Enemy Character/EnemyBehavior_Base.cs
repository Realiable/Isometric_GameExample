using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum AI_BehaviorState
{
    Idle = 0,
    Patrol = 1,
    MoveToTarget = 2,
    Attacking = 3,
    Death = 4,
}

public class EnemyBehavior_Base : MonoBehaviour
{
    [HideInInspector]
    public GameObject targetToAttack;
}
