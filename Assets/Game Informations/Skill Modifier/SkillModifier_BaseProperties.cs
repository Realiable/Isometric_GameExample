using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ApplyEffectCondition
{
    Skill_Initialize = 0,
    Target_OnImpact = 1,
    Target_ReceiveDamage = 2,
    Projectile_Destroyed = 3,
    Target_Destroyed = 4,
}

public enum ApplyEffectType
{
    None = 0,
    BurnModifier = 1,
    ShockwaveModifier = 2,
    LifeLeechModifier = 3,
    StunModifier = 4,
    ProjectilePlus_2 = 5,
}

public enum ApplyEffectRestriction
{
    None = 0,
    MeleeSkillOnly = 1,
    RangeSkillOnly = 2,
    MobilitySkillOnly = 3,
}

[CreateAssetMenu(fileName = "SkillModifier_BaseProperties", menuName = "Realiable Script/Create a new Skill Modifier")]
public class SkillModifier_BaseProperties : ScriptableObject
{
    [Header("Modifier Information")]
    public string SkillMod_Name;
    public ApplyEffectCondition ApplyCondition = ApplyEffectCondition.Skill_Initialize;
    public ApplyEffectType ApplyEffectOnTarget = ApplyEffectType.None;
    public ApplyEffectRestriction ApplyRestriction = ApplyEffectRestriction.None;
    public int BaseDamage = 0;
    public float CharacterDamageMultiplier = 1.0f;
    public float ApplyEffectDuration = 0.0f;
    public float ApplyEffectRange = 0.0f;
    public float KnockbackForce = 0.0f;
    public float KnockupForce = 0.0f;

    [TextArea(3, 10)]
    public string SkillMod_Description;

    [Header("Modifier Asset")]
    public GameObject ApplyModifierObject;
    public GameObject ApplyModifierInitialVFX;
    public GameObject ApplyModifierAttachVFX;
}
