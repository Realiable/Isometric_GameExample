using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimationNotify : MonoBehaviour
{
    public PlayerInformation playerInformation;

    public void Push_CharacterForward()
    {
        GameObject mainCharacterObject = playerInformation.gameObject;
        playerInformation.physicsSystem.AddForce(mainCharacterObject.transform.forward * playerInformation.currentUseSkill.PushForwardForce);
    }

    public void End_Attacking()
    {
        playerInformation.onAttacking = false;
    }
}
