using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInformation : CharacterInformation
{
    [Header("Player Share Component")]
    public GameObject cameraObject;
    public GameObject WeaponSlot_Right;
    public AnimatorOverrideController basedCharacterAnimatorController;
}
