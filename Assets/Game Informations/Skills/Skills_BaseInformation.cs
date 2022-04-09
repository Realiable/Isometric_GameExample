using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum SkillAttackType
{
    Melee = 0,
    Range = 1,
    Mobility = 2,
}

public enum CollisionType
{
    None = 0,
    FrontBox = 1,
    CircleAround = 2,
}

public enum SpecialBehavior
{
    None = 0,
    PassThroughCharacters = 1,
}

[CreateAssetMenu(fileName = "Skills_BaseInformation", menuName = "Realiable Script/Create a new Skill")]
public class Skills_BaseInformation : ScriptableObject
{
    [Header("General Setup")]
    public string SkillName = "";
    public SkillAttackType SkillType = SkillAttackType.Melee;
    public AnimationClip SkillAnimation;
    public float SkillSpeedMultiplier = 1.0f;
    public float SkillDamageMultiplier = 1.0f;
    public float PushForwardForce = 0.0f;
    public float KnockbackForce = 0.0f;
    public float KnockupForce = 0.0f;

    [Header("Collision Setup")]
    public CollisionType collisionType = CollisionType.FrontBox;
    public float collisionWidth = 1.0f;
    public float collisionHeight = 1.0f;
    public float collisionRange = 1.0f;

    [Header("Projectile Setup")]
    public GameObject spawnProjectileObject;
    public float projectileSpeedMultiplier = 1.0f;

    [Header("Visual Effect Setup")]
    public GameObject spawnVFXWhenHit;

    [Header("Special Behavior Setup")]
    public SpecialBehavior specialBehavior = SpecialBehavior.None;
    public bool applyAimAssist = false;
    public float aimAssistRange = 0.0f;
}
