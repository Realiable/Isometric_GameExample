using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterAnimationNotify : MonoBehaviour
{
    public CharacterInformation characterInformation;

    public void Push_CharacterForward()
    {
        characterInformation.Push_CharacterForward();
    }

    public void Open_AttackCollision()
    {
        characterInformation.Open_AttackCollision();
    }

    public void Close_AttackCollision()
    {
        characterInformation.Close_AttackCollision();
    }

    public void Spawn_Projectile()
    {
        characterInformation.Spawn_Projectile();
    }

    public void Apply_SpecialBehavior()
    {
        characterInformation.Apply_SpecialBehavior();
    }

    public void Deapply_SpecialBehavior()
    {
        characterInformation.Deapply_SpecialBehavior();
    }

    public void Active_Invincible()
    {
        characterInformation.Active_Invincible();
    }

    public void Deactive_Invincible()
    {
        characterInformation.Deactive_Invincible();
    }

    public void StartReceive_HoldingInput()
    {
        characterInformation.StartReceive_HoldingInput();
    }

    public void End_Attacking()
    {
        characterInformation.End_Attacking();
    }
}
