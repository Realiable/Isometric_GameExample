using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInformation : MonoBehaviour
{
    [Header("Player Share Component")]
    public Rigidbody physicsSystem;
    public CapsuleCollider capsuleCollider;
    public GameObject cameraObject;
    public Animator characterAnimator;
    public GameObject WeaponSlot_Right;
    public AnimatorOverrideController basedCharacterAnimatorController;

    [Header("Player Skill Information")]
    public Skills_BaseInformation meleeSkill;
    public Skills_BaseInformation rangeSkill;
    public Skills_BaseInformation mobilitySkill;

    [HideInInspector]
    public bool onAttacking = false;
    [HideInInspector]
    public Vector3 movingDirection = new Vector3(0, 0, 0);
    [HideInInspector]
    public Skills_BaseInformation currentUseSkill;
}
