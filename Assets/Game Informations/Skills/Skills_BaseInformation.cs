using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Skills_BaseInformation", menuName = "Realiable Script/Create a new Skill")]
public class Skills_BaseInformation : ScriptableObject
{
    public string SkillName = "";
    public AnimationClip SkillAnimation;
    public float SkillDamageMultiplier = 1.0f;
    public float PushForwardForce = 0.0f;
}
