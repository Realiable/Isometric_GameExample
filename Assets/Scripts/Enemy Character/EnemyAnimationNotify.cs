using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAnimationNotify : MonoBehaviour
{
    public EnemyInformation enemyInformation;

    public void Push_CharacterForward()
    {
        enemyInformation.Push_CharacterForward();
    }

    public void Open_AttackCollision()
    {
        enemyInformation.Open_AttackCollision();
    }

    public void Close_AttackCollision()
    {
        enemyInformation.Close_AttackCollision();
    }

    public void Spawn_Projectile()
    {
        enemyInformation.Spawn_Projectile();
    }

    public void Apply_SpecialBehavior()
    {
        enemyInformation.Apply_SpecialBehavior();
    }

    public void Deapply_SpecialBehavior()
    {
        enemyInformation.Deapply_SpecialBehavior();
    }

    public void Active_Invincible()
    {
        enemyInformation.Active_Invincible();
    }

    public void Deactive_Invincible()
    {
        enemyInformation.Deactive_Invincible();
    }

    public void StartReceive_HoldingInput()
    {
        enemyInformation.StartReceive_HoldingInput();
    }

    public void End_Attacking()
    {
        enemyInformation.End_Attacking();
    }
}
