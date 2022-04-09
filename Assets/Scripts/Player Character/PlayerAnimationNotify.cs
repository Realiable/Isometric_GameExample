using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimationNotify : MonoBehaviour
{
    public PlayerInformation playerInformation;

    public void Push_CharacterForward()
    {
        playerInformation.Push_CharacterForward();
    }

    public void Open_AttackCollision()
    {
        playerInformation.Open_AttackCollision();
    }

    public void Close_AttackCollision()
    {
        playerInformation.Close_AttackCollision();
    }

    public void Spawn_Projectile()
    {
        playerInformation.Spawn_Projectile();
    }

    public void Apply_SpecialBehavior()
    {
        playerInformation.Apply_SpecialBehavior();
    }

    public void Deapply_SpecialBehavior()
    {
        playerInformation.Deapply_SpecialBehavior();
    }
    public void Active_Invincible()
    {
        playerInformation.Active_Invincible();
    }

    public void Deactive_Invincible()
    {
        playerInformation.Deactive_Invincible();
    }

    public void End_Attacking()
    {
        playerInformation.End_Attacking();
    }
}
